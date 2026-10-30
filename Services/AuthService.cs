using EmployeeManagementApi.DTOs;
using EmployeeManagementApi.Repositories.Interface;
using EmployeeManagementApi.Services.Interface;

namespace EmployeeManagementApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;

        public AuthService(IAuthRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            return await _repository.RegisterAsync(request);
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            return await _repository.LoginAsync(request);
        }
    }
}
