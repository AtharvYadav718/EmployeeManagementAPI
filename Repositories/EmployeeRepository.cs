using Dapper;
using EmployeeManagementApi.DTOs;
using EmployeeManagementApi.Models;
using EmployeeManagementApi.Repositories.Interface;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EmployeeManagementApi.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string? _connectionString;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            string procedure = "sp_GetAllEmployees";

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var employees = await db.QueryAsync<Employee>(procedure, commandType: CommandType.StoredProcedure);
                return employees;
            }
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            string procedure = "sp_GetEmployeeId";

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<Employee?>(procedure, new { Id = id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<Employee> InsertEmployeeAsync(CreateEmployeeRequest employee)
        {
            string procedure = "sp_InsertEmployee";

            var parameters = new { employee.FirstName, employee.LastName, employee.Email, employee.DepartmentId, employee.Salary };

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var createdEmployee = await db.QuerySingleAsync<Employee>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return createdEmployee;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeRequest employee)
        {
            string procedure = "sp_UpdateEmployee";

            var parameters = new { Id = id, employee.FirstName, employee.LastName, employee.Email, employee.Salary, employee.DepartmentId };

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                await db.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure);

                return true;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            string procedure = "sp_DeleteEmployee";

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                await db.ExecuteAsync(procedure, new { Id = id }, commandType: CommandType.StoredProcedure);
                return true;
            }

        }

        public async Task<(List<Employee> EmployeeData, List<Department> DepartmentData)> GetEmployeeAndDepartmentDataAsync()
        {
            string procedure = "sp_EmployeeDepartmentData";

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                using (var Multi = await db.QueryMultipleAsync(procedure, commandType: CommandType.StoredProcedure))
                {
                    var employee = (await Multi.ReadAsync<Employee>()).ToList();

                    var department = (await Multi.ReadAsync<Department>()).ToList();

                    return (employee, department);
                }
            }
        }

        public async Task<IEnumerable<EmployeeDetailsDto>> GetEmployeeDetailsWithJoinAsync()
        {
            string procedure = "sp_EmployeeAndDepartmentName";

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var result = await db.QueryAsync<EmployeeDetailsDto>(procedure, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentNameAsync(string departmentName)
        {
            string procedure = "sp_GetEmployeesByDepartmentName";

            var parameters = new
            {
                DepartmentName = departmentName
            };

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryAsync<Employee>(procedure,parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<string?> GetDepartmentNameByIdAsync(int departmentId)
        {
            string procedure = "sp_GetDepartmentNameById";

            var parameters = new
            {
                DepartmentId = departmentId
            };

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryFirstOrDefaultAsync<string>(procedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<EmployeeSearchResponseDto>> SearchEmployeesByNameAsync(string name)
        {
            string procedure = "SP_SearchEmployeeByName";

            var parameters = new
            {
                EmployeeName = name
            };

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryAsync<EmployeeSearchResponseDto>(procedure, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<SearchByDeparmentIDResponceDto>> SearchByDeparmentIDAsync(int id)
        {
            string procedure = "SP_SearchByDeparmentID";

            var parameters = new
            {
                Department_Id = id
            };
            
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                return await db.QueryAsync<SearchByDeparmentIDResponceDto>(procedure, parameters,commandType: CommandType.StoredProcedure);
            }
        }
    }
}
