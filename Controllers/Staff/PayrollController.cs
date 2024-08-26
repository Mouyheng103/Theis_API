using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API.Controllers.Staff
{
    [ApiController]
    [Route("api/staff/payroll")]
    [AllowAnonymous]
    public class PayrollController : ControllerBase
    {
        
    }
}
