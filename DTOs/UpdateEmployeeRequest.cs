namespace EmployeeManagementApi.DTOs
{
    public class UpdateEmployeeRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public decimal Salary { get; set; }
        public int DepartmentId { get; set; }
    }
}
