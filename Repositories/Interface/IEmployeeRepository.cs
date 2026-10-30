using EmployeeManagementApi.Models;
using EmployeeManagementApi.DTOs;

namespace EmployeeManagementApi.Repositories.Interface
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee> InsertEmployeeAsync(CreateEmployeeRequest employee);
        Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeRequest employee);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<(List<Employee> EmployeeData, List<Department>DepartmentData)> GetEmployeeAndDepartmentDataAsync();
        Task<IEnumerable<EmployeeDetailsDto>> GetEmployeeDetailsWithJoinAsync();
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentNameAsync(string departmentName);
        Task<string?> GetDepartmentNameByIdAsync(int departmentId);

        Task<IEnumerable<Employee>> SearchEmployeesByNameAsync(string name);
    }
}
