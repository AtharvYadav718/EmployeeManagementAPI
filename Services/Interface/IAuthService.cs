using EmployeeManagementApi.DTOs;

namespace EmployeeManagementApi.Services.Interface
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
