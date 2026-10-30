namespace EmployeeManagementApi.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; } 

        public bool IsActive { get; set; }

        public DateTime Createddate { get; set; }
    }
}
