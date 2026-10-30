namespace EmployeeManagementApi.DTOs
{
    public class EmployeeSearchResponseDto
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Email { get; set; }

        public decimal? Salary { get; set; }

        public DateTime CreatedDate { get; set; }

        public int DepartmentId { get; set; }
    }
}
