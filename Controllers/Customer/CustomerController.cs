using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.Customer
{
    [Route("api/customer/")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public CustomerController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private IActionResult HandleException(Exception ex, string customMessage)
        {
            if (ex is SqlException)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"{customMessage} Error: {ex.Message}" });
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred.", Error = ex.Message });
        }

        [HttpGet]
        public IActionResult GetCustomers()
        {
            try
            {
                var customers = _dataContext.ViewO_Customers.AsNoTracking().ToList();
                return customers.Any() ? Ok(new { Message = "success!", Data = customers }) : NotFound(new { Message = "No customers found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }

        [HttpGet("find")]
        public IActionResult FindCustomer(string? Id, string? AgentId,string? VillageCode,int? BranchId)
        {
            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var dataById =  _dataContext.ViewO_Customers.Where(a => a.Id == Id).AsNoTracking().ToList();
                    return dataById.Any() ? Ok(new { Message = "success!", Data = dataById }) : NotFound(new { Message = "No customer found." });
                }
                else if (!string.IsNullOrEmpty(VillageCode))
                {
                    var dataByVillageCode = _dataContext.ViewO_Customers.Where(a => a.VillageCode == VillageCode).AsNoTracking().ToList();
                    return dataByVillageCode.Any() ? Ok(new { Message = "success!", Data = dataByVillageCode }) : NotFound(new { Message = "No customer found." });
                }
                else if (!string.IsNullOrEmpty(AgentId))
                {
                    var dataByAgent = _dataContext.ViewO_Customers.Where(a => a.AgentID == AgentId).AsNoTracking().ToList();
                    return dataByAgent.Any() ? Ok(new { Message = "success!", Data = dataByAgent }) : NotFound(new { Message = "No customer found." });

                }
                else if (BranchId != null || BranchId != 0)
                {
                    var dataByBranch = _dataContext.ViewO_Customers.Where(a => a.BranchId == BranchId).AsNoTracking().ToList();
                    return dataByBranch.Any() ? Ok(new { Message = "success!", Data = dataByBranch }) : NotFound(new { Message = "No customer found." });
                }
                else
                {
                    return BadRequest("Please provide Id or VillageCode or BranchId");
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customers customerDTO)
        {
            if (customerDTO == null) return BadRequest(new { Message = "Model is empty" });
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var newCustomer = new Customers
                {
                    Id = GetNewCusId(),
                    AgentID = customerDTO.AgentID,
                    FirstNameM = customerDTO.FirstNameM,
                    LastNameM = customerDTO.LastNameM,
                    FirstName = customerDTO.FirstName,
                    LastName = customerDTO.LastName,  
                    Tel = customerDTO.Tel,
                    VillageCode = customerDTO.VillageCode,
                    DOB = customerDTO.DOB,
                    BlackList = false,
                    Active = true,
                    Created_By = customerDTO.Created_By,
                    Created_At = DateTime.Now,
                    Updated_By = customerDTO.Created_By,
                    Updated_At = DateTime.Now
                };
                var address = await _dataContext.ViewO_Address.FindAsync(customerDTO.VillageCode);
                var agent = await _dataContext.tblO_Agent.FindAsync(customerDTO.AgentID);
                
                await _dataContext.tblO_Customer.AddAsync(newCustomer);
                await _dataContext.SaveChangesAsync();
                var data = new { newCustomer, address, agent };
                return Ok(new { Message = "Customer added successfully!", data = data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(string id, Customers customerDTO)
        {
            if (customerDTO == null) return BadRequest("Model is empty");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingCustomer = await _dataContext.tblO_Customer.FindAsync(id);
            var address = await _dataContext.ViewO_Address.FindAsync(customerDTO.VillageCode);
            if (existingCustomer == null) return NotFound(new { Message = "Customer not found!" });

            try
            {
                existingCustomer.AgentID = customerDTO.AgentID;
                existingCustomer.FirstNameM = customerDTO.FirstNameM;
                existingCustomer.LastNameM = customerDTO.LastNameM;
                existingCustomer.FirstName = customerDTO.FirstName;
                existingCustomer.LastName = customerDTO.LastName;
                existingCustomer.Tel = customerDTO.Tel;
                existingCustomer.VillageCode = customerDTO.VillageCode;
                existingCustomer.DOB = customerDTO.DOB;
                existingCustomer.BlackList = customerDTO.BlackList;
                existingCustomer.Active = customerDTO.Active;
                existingCustomer.Updated_By = customerDTO.Updated_By;
                existingCustomer.Updated_At = DateTime.Now;

                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = "Customer updated successfully!", data = existingCustomer, address = address });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data in the database.");
            }
        }
        private string GetNewCusId()
        {
            string NewId = "";
            var getlastid = _dataContext.tblO_Customer.OrderByDescending(e => e.Id).FirstOrDefault();
            var currentYear = DateTime.Now.Year.ToString();

            if (getlastid == null )
            {
                return NewId = currentYear + "00001";
            }

            var lastIdString = getlastid.Id.ToString();
            var yearid = lastIdString.Substring(0, 4);
            int lastid = int.Parse(lastIdString.Substring(4)) + 1;

            if (currentYear == yearid)
            {
                NewId = currentYear + lastid.ToString("D5");
            }
            else
            {
                NewId = currentYear + "00001";
            }

            return NewId;
        }
    }
}
