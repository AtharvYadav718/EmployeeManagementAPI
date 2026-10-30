using EmployeeManagementApi.DTOs;

namespace EmployeeManagementApi.Repositories.Interface
{
    public interface IAuthRepository
    {
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
