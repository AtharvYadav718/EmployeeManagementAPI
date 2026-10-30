using EmployeeManagementApi.Models;
using EmployeeManagementApi.Response;
using EmployeeManagementApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IEmployeeService _service;
        private readonly ILogger<DepartmentsController> _logger;
        private readonly IMemoryCache _cache;

        public DepartmentsController(IEmployeeService service, ILogger<DepartmentsController> logger, IMemoryCache cache)
        {
            _service = service;
            _logger = logger;
            _cache = cache;
        }

        [HttpGet("DepartmentWiseData")]
        public async Task<IActionResult> GetEmployeesByDepartment([FromQuery] string departmentName)
        {
            if (string.IsNullOrWhiteSpace(departmentName))
            {
                _logger.LogWarning(" Department name was empty or null.");
                var errorResponse = new BaseResponse<IEnumerable<Employee>>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 400,
                        Message = "Department name is required."
                    }
                };
                return BadRequest(errorResponse);
            }

            var cacheKey = $"employees_department_{departmentName.ToLower()}";

            if (_cache.TryGetValue(cacheKey, out List<Employee>? employees))
            {
                var cachedResponse = new BaseResponse<IEnumerable<Employee>>
                {
                    Status = new BaseStatus
                    {
                        Success = true,
                        StatusCode = 200,
                        Message = "Employee details fetched successfully from cache."
                    },
                    Data = new BaseData<IEnumerable<Employee>>
                    {
                        Result = employees
                    }
                };
                return Ok(cachedResponse);
            }

            employees = (await _service.GetEmployeesByDepartmentNameAsync(departmentName)).ToList();

            if (!employees.Any())
            {
                _logger.LogWarning("No employees found for department: {DepartmentName}", departmentName);
                var errorResponse = new BaseResponse<IEnumerable<Employee>>
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

            _cache.Set(cacheKey, employees, TimeSpan.FromMinutes(30));

            var successResponse = new BaseResponse<IEnumerable<Employee>>
            {
                Status = new BaseStatus
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Employee details fetched successfully."
                },
                Data = new BaseData<IEnumerable<Employee>>
                {
                    Result = employees
                }
            };
            return Ok(successResponse);
        }
    }
}
