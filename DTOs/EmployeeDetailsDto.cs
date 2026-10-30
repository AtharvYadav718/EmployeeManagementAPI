namespace EmployeeManagementApi.DTOs
{
    public class EmployeeDetailsDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? DepartmentName { get; set; }
        public bool IsActive { get; set; }
    }
}
