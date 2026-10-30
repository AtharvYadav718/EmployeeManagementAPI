using EmployeeManagementApi.DTOs;
using EmployeeManagementApi.Response;
using EmployeeManagementApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

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
    }
}
