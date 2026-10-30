namespace EmployeeManagementApi.Models
{
    public class LoginUser
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string? UserName { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public string? RoleName { get; set; }
    }
}
