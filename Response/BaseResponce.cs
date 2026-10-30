namespace EmployeeManagementApi.Response
{
    public class BaseResponse<T>
    {
        public BaseStatus Status { get; set; } = new();
        public BaseData<T> Data { get; set; } = new();
    }
}