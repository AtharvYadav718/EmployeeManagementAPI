namespace EmployeeManagementApi.Response
{
    public class BaseStatus
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
    }
}