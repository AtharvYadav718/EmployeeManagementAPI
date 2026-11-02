using EmployeeManagementApi.DTOs;
using EmployeeManagementApi.Models;
using EmployeeManagementApi.Response;
using EmployeeManagementApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Xml.Linq;

namespace EmployeeManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeSearchController : ControllerBase
    {
        private readonly IEmployeeService _service;
        private readonly ILogger<EmployeeSearchController> _logger;
        private readonly IMemoryCache _cache;

        public EmployeeSearchController(IEmployeeService service, ILogger<EmployeeSearchController> logger, IMemoryCache cache)
        {
            _service = service;
            _logger = logger;
            _cache = cache;
        }


        [HttpGet("EmployeeAndDepartmentData")]
        public async Task<IActionResult> GetEmployeeAndDepartmentData()
        {
            var (employee, department) = await _service.GetEmployeeAndDepartmentDataAsync();

            if (!employee.Any() && !department.Any())
            {
                _logger.LogWarning("No employees or department found in the database.");
                var errorResponse = new BaseResponse<EmployeeDepartmentResponse>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "No employee or department data found."
                    }
                };
                return NotFound(errorResponse);
            }

            var successResponse = new BaseResponse<EmployeeDepartmentResponse>
            {
                Status = new BaseStatus
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Employee and department data fetched successfully."
                },
                Data = new BaseData<EmployeeDepartmentResponse>
                {
                    Result = new EmployeeDepartmentResponse
                    {
                        EmployeeList = employee,
                        DepartmentList = department
                    }
                }
            };
            return Ok(successResponse);
        }


        [HttpGet("SearchByDeparmentID")]
        public async Task<IActionResult> SearchEmployeesByName([FromQuery] string name)
        {

            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new BaseResponse<IEnumerable<EmployeeSearchResponseDto>>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Employee name is required."
                    }
                });
            }

            var employees = await _service.SearchEmployeesByNameAsync(name);

            var successResponse = new BaseResponse<IEnumerable<EmployeeSearchResponseDto>>
            {
                Status = new BaseStatus
                {
                    Success = true,
                    StatusCode = 200,
                    Message = employees.Any()
                        ? "Employees fetched successfully."
                        : "No employees found matching the given name."
                },
                Data = new BaseData<IEnumerable<EmployeeSearchResponseDto>>
                {
                    Result = employees
                }
            };

            return Ok(successResponse);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> SearchByDeparmentID(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid Department ID provided: {DepartmentId}", id);
                var errorResponse = new BaseResponse<IEnumerable<SearchByDeparmentIDResponceDto>>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Invalid Department ID."
                    }
                };
                return BadRequest(errorResponse);
            }

            var employee = await _service.SearchByDeparmentIDAsync(id);

            if (!employee.Any())
            {
                _logger.LogWarning("Employee record not found for ID: {EmployeeId}", id);
                var errorResponse = new BaseResponse<IEnumerable<SearchByDeparmentIDResponceDto>>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 404,
                        Message = "No employeeDepartment data found."
                    }
                };
                return NotFound(errorResponse);
            }

            var successResponse = new BaseResponse<IEnumerable<SearchByDeparmentIDResponceDto>>
            {
                Status = new BaseStatus
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "EmployeeDepartment data fetched successfully."
                },
                Data = new BaseData<IEnumerable<SearchByDeparmentIDResponceDto>>
                {
                    Result = employee
                }
            };
            return Ok(successResponse);
        }
    }      
}
