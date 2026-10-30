using Dapper;
using EmployeeManagementApi.DTOs;
using EmployeeManagementApi.Models;
using EmployeeManagementApi.Repositories.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace EmployeeManagementApi.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;


        public AuthRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;

        }
        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            string procedure = "sp_Registration";

            // Convert plain password into BCrypt hash
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var parameters = new
            {
                EmployeeId = request.EmployeeId,
                UserName = request.UserName,
                PasswordHash = passwordHash,
                RoleId = request.RoleId
            };

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                await db.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);
            }

            return true;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            string procedure = "sp_Login";

            var parameters = new { request.UserName };
            
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var user = await db.QueryFirstOrDefaultAsync<LoginUser>(procedure, parameters, commandType: CommandType.StoredProcedure);

                if (user == null)
                {
                    return null;
                }

                bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

                if (!passwordValid)
                {

                    return null;
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Role, user.RoleName!)

                };

                var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

                var credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: credentials
                );

                return new LoginResponse
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                };
            }

        }

    }
}
