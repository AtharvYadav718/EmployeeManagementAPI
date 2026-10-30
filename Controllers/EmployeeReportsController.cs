using EmployeeManagementApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeReportsController : ControllerBase
    {
        private readonly IEmployeeService _service;
        private readonly ILogger<EmployeeReportsController> _logger;
        private readonly IMemoryCache _cache;


        public EmployeeReportsController(IEmployeeService service, ILogger<EmployeeReportsController> logger, IMemoryCache cache)
        {
            _service = service;
            _logger = logger;
            _cache = cache;
        }
    }
}
