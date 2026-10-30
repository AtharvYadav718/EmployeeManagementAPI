using EmployeeManagementApi.Services.Interface;
using EmployeeManagementApi.DTOs;
using EmployeeManagementApi.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService service, ILogger<AuthController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromQuery] LoginRequest request)
        {

            var response = await _service.LoginAsync(request);
            if (response == null)
            {
                _logger.LogWarning("Login failed for user: {UserName}", request.UserName);

                var errorResponse = new BaseResponse<LoginResponse>
                {
                    Status = new BaseStatus
                    {
                        Success = false,
                        StatusCode = 401,
                        Message = "Username or Password is required."
                    },
                    Data = new BaseData<LoginResponse>
                    {
                        Result = null,
                        //TotalCount = 0
                    }
                };
                return StatusCode(401, errorResponse);
            }

            _logger.LogInformation("User {UserName} logged in successfully.", request.UserName);
            var successResponse = new BaseResponse<LoginResponse>
            {
                Status = new BaseStatus
                {
                    Success = true,
                    StatusCode = 200,
                    Message = "Login successful"
                },
                Data = new BaseData<LoginResponse>
                {
                    Result = response,
                }
            };
            return Ok(successResponse);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromQuery] RegisterRequest request)
        {
            var result = await _service.RegisterAsync(request);

            if (!result)
            {
                _logger.LogWarning("User registration failed for {UserName}.", request.UserName);
                var errorResponse = new BaseStatus
                {
                    Success = false,
                    StatusCode = 400,
                    Message = "User registration failed."
                };

                return BadRequest(errorResponse);
            }

            _logger.LogInformation("User {UserName} registered successfully.", request.UserName);
            var successResponse = new BaseStatus
            {
                Success = true,
                StatusCode = 201,
                Message = "User registered successfully."
            };

            return StatusCode(201, successResponse);

        }

    }
}
