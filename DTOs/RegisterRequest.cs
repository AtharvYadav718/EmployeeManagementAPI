namespace EmployeeManagementApi.DTOs
{
    public class RegisterRequest
    {
        public int EmployeeId { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public int RoleId { get; set; }
    }
}