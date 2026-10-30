using EmployeeManagementApi.DTOs;
using EmployeeManagementApi.Models;
using EmployeeManagementApi.Response;
using EmployeeManagementApi.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;


namespace EmployeeManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;
        private readonly ILogger<EmployeesController> _logger;
        private readonly IMemoryCache _cache;


        public EmployeesController(IEmployeeService service, ILogger<EmployeesController> logger, IMemoryCache cache)
        {
            _service = service;
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            const string EmployeeCacheKey = "all_employees";

            if (_cache.TryGetValue(EmployeeCacheKey, out IEnumerable<Employee>? employees))
            {
                _logger.LogInformation("Employees fetched from cache.");

                var cachedResponse = new BaseResponse<IEnumerable<Employee>>
                {
                    Status = new BaseStatus
                    {
                        Success = true,
                        StatusCode = 200,
                        Message = "Employees fetched successfully from cache."
                    },
                    Data = new BaseData<IEnumerable<Employee>>
                    {
                        Result = employees
                    }
                };
                return Ok(cachedResponse);
            }

            employees = await _service.GetAllEmployeesAsync();

            if (employees == null || !employees.Any())
            {
                _logger.LogWarning("No employees found in the database.");
                var errorResponse = new BaseResponse<IEnumerable<Employee>>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "No employees found."
                    }
                };
                return NotFound(errorResponse);
            }

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            _cache.Set(EmployeeCacheKey, employees, cacheOptions);

            _logger.LogInformation("Employees fetched from database and stored in cache.");

            var successResponse = new BaseResponse<IEnumerable<Employee>>
            {
                Status = new BaseStatus
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Employees fetched successfully."
                },
                Data = new BaseData<IEnumerable<Employee>>
                {
                    Result = employees
                }
            };

            return Ok(successResponse);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid employee ID provided: {EmployeeId}", id);
                var errorResponse = new BaseResponse<Employee>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Invalid employee ID."
                    }
                };
                return BadRequest(errorResponse);
            }

            var employee = await _service.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                _logger.LogWarning("Employee record not found for ID: {EmployeeId}", id);
                var errorResponse = new BaseResponse<Employee>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "No employee data found."
                    }
                };
                return NotFound(errorResponse);
            }

            var successResponse = new BaseResponse<Employee>
            {
                Status = new BaseStatus
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Employee fetched successfully."
                },
                Data = new BaseData<Employee>
                {
                    Result = employee
                }
            };
            return Ok(successResponse);
        }


        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest employee)
        {
            if (employee == null)
            {
                _logger.LogWarning("Employee data received as null.");
                var errorResponse = new BaseResponse<Employee>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Employee data is required."
                    }
                };
                return BadRequest(errorResponse);
            }

            if (employee.Salary < 0)
            {
                _logger.LogWarning("Salary cannot be negative.");
                return BadRequest("Salary cannot be negative.");
            }

            Employee createdEmployee = await _service.InsertEmployeeAsync(employee);

            if (createdEmployee == null || createdEmployee.Id <= 0)
            {
                _logger.LogWarning("Employee was not added.");

                return BadRequest("Employee not added.");
            }

            var departmentName = await _service.GetDepartmentNameByIdAsync(createdEmployee.DepartmentId);

            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                var cacheKey = $"employees_department_{departmentName.ToLower()}";

                if (_cache.TryGetValue(cacheKey, out List<Employee>? employees) && employees != null)
                {
                    employees.Add(createdEmployee);
                    _cache.Set(cacheKey, employees, TimeSpan.FromMinutes(30));
                }
            }

            var successResponse = new BaseResponse<Employee>
            {
                Status = new BaseStatus
                {
                    Success = true,
                    StatusCode = 201,
                    Message = "Employee created successfully."
                },
                Data = new BaseData<Employee>
                {
                    Result = createdEmployee
                }
            };
            return CreatedAtAction(nameof(GetEmployeeById), new { id = createdEmployee.Id }, successResponse);
        }


        [Authorize(Roles = "Admin,HR")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeRequest employee)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Update failed: Invalid employee ID {EmployeeId}.", id);
                var errorResponse = new BaseResponse<Employee>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Invalid employee ID."
                    }
                };

                return BadRequest(errorResponse);
            }

            if (employee == null)
            {
                _logger.LogWarning("Update failed: Received employee body data is null.");
                var errorResponse = new BaseResponse<Employee>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Employee data is required."
                    }
                };
                return BadRequest(errorResponse);
            }

            if (employee.Salary < 0)
            {
                _logger.LogWarning("Salary cannot be negative.");
                return BadRequest("Salary cannot be negative.");
            }

            await _service.UpdateEmployeeAsync(id, employee);

            var updatedEmployee = await _service.GetEmployeeByIdAsync(id);

            if (updatedEmployee == null)
            {
                return NotFound($"Employee with Id {id} not found.");
            }

            var departmentName = await _service.GetDepartmentNameByIdAsync(updatedEmployee.DepartmentId);

            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                var cacheKey = $"employees_department_{departmentName.ToLower()}";

                if (_cache.TryGetValue(cacheKey, out List<Employee>? employees) && employees != null)
                {
                    var existingEmployee = employees.FirstOrDefault(x => x.Id == id);

                    if (existingEmployee != null)
                    {                   
                        var index = employees.IndexOf(existingEmployee);

                        employees[index] = updatedEmployee;
                
                        _cache.Set(cacheKey, employees, TimeSpan.FromMinutes(30));
                    }
                }
            }

            var successResponse = new BaseResponse<Employee>
            {
                Status = new BaseStatus
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Employee updated successfully."
                },
                Data = new BaseData<Employee>
                {
                    Result = updatedEmployee
                }
            };
            return Ok(successResponse);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Delete failed: Invalid ID {EmployeeId} provided.", id);
                var errorResponse = new BaseResponse<Employee>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Invalid employee ID."
                    }
                };
                return BadRequest(errorResponse);
            }

            var employee = await _service.GetEmployeeByIdAsync(id);

            if (employee == null)
            {
                _logger.LogWarning("Delete failed: Employee ID {EmployeeId} not found.", id);
                var errorResponse = new BaseResponse<Employee>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "Employee ID not found."
                    }
                };
                return NotFound(errorResponse);
            }

            var departmentName = await _service.GetDepartmentNameByIdAsync(employee.DepartmentId);

            await _service.DeleteEmployeeAsync(id);

            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                var cacheKey = $"employees_department_{departmentName.ToLower()}";

                if (_cache.TryGetValue(cacheKey, out List<Employee>? employees) && employees != null)
                {
 
                    var existingEmployee = employees.FirstOrDefault(x => x.Id == id);

                    if (existingEmployee != null)
                    {                  
                        employees.Remove(existingEmployee);

                        _cache.Set(cacheKey, employees, TimeSpan.FromMinutes(30));
                    }
                }
            }

            var successResponse = new BaseResponse<Employee>
            {
                Status = new BaseStatus
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Employee deleted successfully."
                },
                Data = new BaseData<Employee>
                {
                    Result = employee
                }
            };
            return Ok(successResponse);
        }



        [HttpGet("EmployeeDetailswithJoin")]
        public async Task<IActionResult> GetEmployeeDetailsWithJoin()
        {
            var result = await _service.GetEmployeeDetailsWithJoinAsync();

            if (!result.Any())
            {
                _logger.LogWarning("No joined employee records found in the database.");
                var errorResponse = new BaseResponse<IEnumerable<EmployeeDetailsDto>>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "No employee details found."
                    }
                };
                return NotFound(errorResponse);
            }

            var successResponse = new BaseResponse<IEnumerable<EmployeeDetailsDto>>
            {
                Status = new BaseStatus
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Employee details fetched successfully."
                },
                Data = new BaseData<IEnumerable<EmployeeDetailsDto>>
                {
                    Result = result
                }
            };
            return Ok(successResponse);
        }


        
    }
}