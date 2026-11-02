
using EmployeeManagementApi.DTOs;
using EmployeeManagementApi.Models;
using EmployeeManagementApi.Repositories.Interface;
using EmployeeManagementApi.Services.Interface;

namespace EmployeeManagementApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeService(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _repository.GetAllEmployeesAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _repository.GetEmployeeByIdAsync(id);
        }

        public async Task<Employee> InsertEmployeeAsync(CreateEmployeeRequest employee)
        {
            return await _repository.InsertEmployeeAsync(employee);
        }

        public async Task<bool> UpdateEmployeeAsync(int id, UpdateEmployeeRequest employee)
        {
            return await _repository.UpdateEmployeeAsync(id, employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            return await _repository.DeleteEmployeeAsync(id);
        }

        public async Task<(List<Employee> EmployeeData,List<Department> DepartmentData)> GetEmployeeAndDepartmentDataAsync()
        {
            return await _repository.GetEmployeeAndDepartmentDataAsync();
        }

        public async Task<IEnumerable<EmployeeDetailsDto>> GetEmployeeDetailsWithJoinAsync()
        {
            return await _repository.GetEmployeeDetailsWithJoinAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentNameAsync(string departmentName)
        {
            return await _repository.GetEmployeesByDepartmentNameAsync(departmentName);

        }

        public async Task<string?> GetDepartmentNameByIdAsync(int departmentId)
        {
            return await _repository.GetDepartmentNameByIdAsync(departmentId);
        }

        public async Task<IEnumerable<EmployeeSearchResponseDto>>SearchEmployeesByNameAsync(string name)
        {
            return await _repository.SearchEmployeesByNameAsync(name);
        }

        public async Task<IEnumerable<SearchByDeparmentIDResponceDto>> SearchByDeparmentIDAsync(int id)
        {
            return await _repository.SearchByDeparmentIDAsync(id);
        }
    }
}
