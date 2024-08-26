using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;

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
        [SwaggerOperation(Summary = "get all customer", Description = "")]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {

                var data = from customer in _dataContext.tblO_Customer
                           join agent in _dataContext.tblO_Agent on customer.AgentID equals agent.Id
                           join address in _dataContext.ViewO_Address on agent.VillageCode equals address.VillageCode
                           select new
                           {
                               Id= customer.Id,
                               Wife_Name=customer.Wife_Name,
                               Husband_Name=customer.Husband_Name,
                               Tel=customer.Tel,
                               DOB=customer.DOB,
                               BlackList=customer.BlackList,
                               Active=customer.Active,
                               Created_By=customer.Created_By,
                               Created_At=customer.Created_At,
                               Updated_By=customer.Updated_By,
                               Updated_At=customer.Updated_At,
                               agent=agent,
                               address = address,
                           };
                var dataList = await data.ToListAsync();

                return dataList.Any() ? Ok(new { Message = "success!", Data = dataList }) : NotFound(new { Message = "No csutomer found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "get a single customer by id", Description = "")]
        public async Task<IActionResult> GetCustomers(string id)
        {
            try
            {

                var data = from customer in _dataContext.tblO_Customer where customer.Id == id
                           join agent in _dataContext.tblO_Agent on customer.AgentID equals agent.Id
                           join address in _dataContext.ViewO_Address on agent.VillageCode equals address.VillageCode
                           select new
                           {
                               Id = customer.Id,
                               Wife_Name = customer.Wife_Name,
                               Husband_Name = customer.Husband_Name,
                               Tel = customer.Tel,
                               DOB = customer.DOB,
                               BlackList = customer.BlackList,
                               Active = customer.Active,
                               Created_By = customer.Created_By,
                               Created_At = customer.Created_At,
                               Updated_By = customer.Updated_By,
                               Updated_At = customer.Updated_At,
                               agent = agent,
                               address = address,
                           };
                var dataList = await data.ToListAsync();

                return dataList.Any() ? Ok(new { Message = "success!", Data = dataList }) : NotFound(new { Message = "No csutomer found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }
        [HttpGet("inBrand/{BranchId}")]
        [SwaggerOperation(Summary = "get customers in a branch", Description = "")]
        public async Task<IActionResult> GetCustomersByBranch(int BranchId)
        {
            try
            {
                var data = from customer in _dataContext.tblO_Customer
                           join agent in _dataContext.tblO_Agent on customer.AgentID equals agent.Id
                           join branch in _dataContext.tblO_Branch on agent.BranchId equals branch.Id
                           join address in _dataContext.ViewO_Address on agent.VillageCode equals address.VillageCode
                           where agent.BranchId == BranchId
                           select new
                           {
                               Id = customer.Id,
                               Wife_Name = customer.Wife_Name,
                               Husband_Name = customer.Husband_Name,
                               Tel = customer.Tel,
                               DOB = customer.DOB,
                               BlackList = customer.BlackList,
                               Active = customer.Active,
                               Created_By = customer.Created_By,
                               Created_At = customer.Created_At,
                               Updated_By = customer.Updated_By,
                               Updated_At = customer.Updated_At,
                               agent = agent,
                               address = address,
                               branch = branch,
                           };
                var dataList = await data.ToListAsync();

                return dataList.Any() ? Ok(new { Message = "success!", Data = dataList }) : NotFound(new { Message = "No csutomer found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }
        [HttpPost]
        [SwaggerOperation(Summary = "add a single customer", Description = "")]
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
                    Wife_Name = customerDTO.Wife_Name,
                    Husband_Name = customerDTO.Husband_Name,
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
                var data = new 
                {
                    Id = newCustomer.Id,
                    Wife_Name = newCustomer.Wife_Name,
                    Husband_Name = newCustomer.Husband_Name,
                    Tel = newCustomer.Tel,
                    DOB = newCustomer.DOB,
                    BlackList = newCustomer.BlackList,
                    Active = newCustomer.Active,
                    Created_By = newCustomer.Created_By,
                    Created_At = newCustomer.Created_At,
                    Updated_By = newCustomer.Updated_By,
                    Updated_At = newCustomer.Updated_At,
                    agent = agent,
                    address = address
                };
                return Ok(new { Message = "Customer added successfully!", data = data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "replace a single customer", Description = "")]
        public async Task<IActionResult> UpdateCustomer(string id, Customers customerDTO)
        {
            if (customerDTO == null) return BadRequest("Model is empty");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingCustomer = await _dataContext.tblO_Customer.FindAsync(id);
            if (existingCustomer == null) return NotFound(new { Message = "Customer not found!" });
            if (existingCustomer.Created_At.Date != DateTime.Now.Date)
            {
                var checkRole = await _dataContext.ViewAuth_UserRole.FindAsync(customerDTO.Updated_By);
                if (checkRole.Name != "admin" && checkRole.Name != "master admin")
                    return BadRequest(new { Message = "You Don't have permission to delete. Please contact to admin" });
            }
            try
            {
                existingCustomer.AgentID = customerDTO.AgentID;
                existingCustomer.Wife_Name = customerDTO.Wife_Name;
                existingCustomer.Husband_Name = customerDTO.Husband_Name;
                existingCustomer.Tel = customerDTO.Tel;
                existingCustomer.VillageCode = customerDTO.VillageCode;
                existingCustomer.DOB = customerDTO.DOB;
                existingCustomer.BlackList = customerDTO.BlackList;
                existingCustomer.Active = customerDTO.Active;
                existingCustomer.Updated_By = customerDTO.Updated_By;
                existingCustomer.Updated_At = DateTime.Now;
                var address = await _dataContext.ViewO_Address.FindAsync(customerDTO.VillageCode);
                var agent = await _dataContext.tblO_Agent.FindAsync(customerDTO.AgentID);
                await _dataContext.SaveChangesAsync();
                var data = new
                {
                    Id = existingCustomer.Id,
                    Wife_Name = existingCustomer.Wife_Name,
                    Husband_Name = existingCustomer.Husband_Name,
                    Tel = existingCustomer.Tel,
                    DOB = existingCustomer.DOB,
                    BlackList = existingCustomer.BlackList,
                    Active = existingCustomer.Active,
                    Created_By = existingCustomer.Created_By,
                    Created_At = existingCustomer.Created_At,
                    Updated_By = existingCustomer.Updated_By,
                    Updated_At = existingCustomer.Updated_At,
                    agent = agent,
                    address = address
                };
                return Ok(new { Message = "Customer updated successfully!", data = data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data in the database.");
            }
        }
        [HttpPatch("{id}")]
        [SwaggerOperation(Summary = "update a single customer", Description = "user can update only in create date!")]
        public async Task<IActionResult> UpdateStatus(string id, bool active, string UserId)
        {
            try
            {
                var customer = await _dataContext.tblO_Customer.FindAsync(id);
                if (customer == null) return NotFound(new { Message = "customer not found!" });
                if (customer.Created_At.Date != DateTime.Now.Date)
                {
                    var checkRole = await _dataContext.ViewAuth_UserRole.FindAsync(UserId);
                    if (checkRole.Name != "admin" && checkRole.Name != "master admin")
                        return BadRequest(new { Message = "You Don't have permission to delete. Please contact to admin" });
                }
                customer.Active = active;
                customer.Updated_At = DateTime.Now;
                customer.Updated_By = UserId;
                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = "Customer Deleted successfully!" });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data in the database.");
            }
        }
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "delete a single customer from DB", Description = "admin can delete only in create date!")]
        public async Task<IActionResult> Delete(string id, string UserId)
        {
            try
            {
                var customer = await _dataContext.tblO_Customer.FindAsync(id);
                if (customer == null) return NotFound(new { Message = "Customer not found!" });
                var checkRole = await _dataContext.ViewAuth_UserRole.FindAsync(UserId);
                if (customer.Created_At.Date != DateTime.Now.Date)
                {
                    if (checkRole.Name == "master admin")
                    {
                        _dataContext.tblO_Customer.Remove(customer);
                        await _dataContext.SaveChangesAsync();
                        return Ok(new { result = "Customer has been deleted successfully" });
                    }
                    return BadRequest(new { Message = "You Don't have permission to delete. Please contact to Master admin" });
                }
                else
                {
                    if (checkRole.Name == "admin")
                    {
                        _dataContext.tblO_Customer.Remove(customer);
                        await _dataContext.SaveChangesAsync();
                        return Ok(new { result = "Customer has been deleted successfully" });
                    }
                    return BadRequest(new { Message = "You Don't have permission to delete. Please contact to admin" });
                }
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
