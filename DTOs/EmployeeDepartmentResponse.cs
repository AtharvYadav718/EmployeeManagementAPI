using EmployeeManagementApi.Models;

namespace EmployeeManagementApi.DTOs
{
    public class EmployeeDepartmentResponse
    {
        public IEnumerable<Employee> ?EmployeeList { get; set; }
        public IEnumerable<Department> ?DepartmentList { get; set; }
    }
}
