
using EmployeeManagementApi.DTOs;
using EmployeeManagementApi.Models;

namespace EmployeeManagementApi.Services.Interface
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();

        Task<Employee?> GetEmployeeByIdAsync(int id);

        Task<Employee> InsertEmployeeAsync(CreateEmployeeRequest employee);

        Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeRequest employee);

        Task<bool> DeleteEmployeeAsync(int id);

        Task<(List<Employee> EmployeeData, List<Department> DepartmentData)> GetEmployeeAndDepartmentDataAsync();

        Task<IEnumerable<EmployeeDetailsDto>> GetEmployeeDetailsWithJoinAsync();

        Task<IEnumerable<Employee>> GetEmployeesByDepartmentNameAsync(string departmentName);

        Task<string?> GetDepartmentNameByIdAsync(int departmentId);

        Task<IEnumerable<EmployeeSearchResponseDto>> SearchEmployeesByNameAsync(string name);

        Task<IEnumerable<SearchByDeparmentIDResponceDto>> SearchByDeparmentIDAsync(int id);


    }
}
