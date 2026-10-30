namespace EmployeeManagementApi.Response
{
    public class BaseData<T>
    {
        public T? Result { get; set; }
        //public int TotalCount { get; set; }
    }
}