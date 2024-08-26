using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Staff
{
    [Route("api/staff")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public StaffController(DataContext dataContext)
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
        [SwaggerOperation(Summary = "retrive all staff", Description = "")]
        public async Task<IActionResult> GetStaff()
        {
            try
            {

                var data = from staff in _dataContext.tblO_Staff
                           join branch in _dataContext.tblO_Branch on staff.BranchId equals branch.Id
                           join position in _dataContext.tblO_Position on staff.PositionId equals position.Id
                           join address in _dataContext.ViewO_Address on staff.VillageCode equals address.VillageCode
                           join user in _dataContext.ViewO_Users on staff.UserId equals user.Id
                           select new
                           {
                               staff = staff,
                               branch = branch,
                               position = position,
                               address = address,
                               user = user,
                           };
                var dataList = await data.ToListAsync();

                return dataList.Any() ? Ok(new { Message = "success!", Data = dataList }) : NotFound(new { Message = "No staff found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "retrive a single staff", Description = "")]
        public async Task<IActionResult> FindStaff(int id)
        {
            try
            {

                var data = from staff in _dataContext.tblO_Staff
                           where staff.Id == id
                           join branch in _dataContext.tblO_Branch on staff.BranchId equals branch.Id
                           join position in _dataContext.tblO_Position on staff.BranchId equals position.Id
                           join address in _dataContext.ViewO_Address on staff.VillageCode equals address.VillageCode
                           join user in _dataContext.ViewO_Users on staff.UserId equals user.Id
                           select new
                           {
                               staff = staff,
                               branch = branch,
                               position = position,
                               address = address,
                               user = user,
                           };
                var dataList = await data.ToListAsync();

                return dataList.Any() ? Ok(new { Message = "success!", Data = dataList }) : NotFound(new { Message = "No staff found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "add a single staff", Description = "")]
        public async Task<IActionResult> AddStaff(Staffs staffDTO)
        {
            if (staffDTO == null) return BadRequest(new { Message = "Model is empty" });

            if (!ModelState.IsValid) return BadRequest(ModelState);
            int getlastid;
            try
            {
                getlastid = _dataContext.tblO_Staff.OrderByDescending(e => e.Id).FirstOrDefault()?.Id ?? 0;

                var newStaff = new Staffs
                {
                    Id = getlastid + 1,
                    BranchId = staffDTO.BranchId,
                    Name = staffDTO.Name,
                    En_Name = staffDTO.En_Name,
                    Gender = staffDTO.Gender,
                    Tel_Cellcard = staffDTO.Tel_Cellcard,
                    Tel_Smart = staffDTO.Tel_Smart,
                    Tel_Metfone = staffDTO.Tel_Metfone,
                    PositionId = staffDTO.PositionId,
                    UserId = staffDTO.UserId,
                    VillageCode = staffDTO.VillageCode,
                    HireDate = staffDTO.HireDate,
                    Active = true,
                    BlackList = false,
                    Created_By = staffDTO.Created_By,
                    Created_At = DateTime.Now,
                    Updated_By = staffDTO.Created_By,
                    Updated_At = DateTime.Now
                };
                await _dataContext.tblO_Staff.AddAsync(newStaff);
                var branch = _dataContext.tblO_Branch.Find(staffDTO.BranchId);
                var address = _dataContext.ViewO_Address.Find(staffDTO.VillageCode);
                var position = _dataContext.tblO_Position.Find(staffDTO.PositionId);
                var user = _dataContext.ViewO_Users.Find(staffDTO.UserId);
                await _dataContext.SaveChangesAsync();

                var data = new { newStaff, branch, position, address, user };
                return Ok(new { Message = "Staff Added successfully!", data = data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "replace a single staff", Description = "")]
        public async Task<IActionResult> UpdateStaff(Guid id, Staffs staffDTO)
        {
            if (staffDTO == null) return BadRequest("Model is empty");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingStaff = await _dataContext.tblO_Staff.FindAsync(id);
            if (existingStaff == null) return NotFound(new { Message = "Staff not found!" });
            try
            {
                existingStaff.BranchId = staffDTO.BranchId;
                existingStaff.Name = staffDTO.Name;
                existingStaff.En_Name = staffDTO.En_Name;
                existingStaff.Gender = staffDTO.Gender;
                existingStaff.Tel_Cellcard = staffDTO.Tel_Cellcard;
                existingStaff.Tel_Smart = staffDTO.Tel_Smart;
                existingStaff.Tel_Metfone = staffDTO.Tel_Metfone;
                existingStaff.PositionId = staffDTO.PositionId;
                existingStaff.UserId = staffDTO.UserId;
                existingStaff.VillageCode = staffDTO.VillageCode;
                existingStaff.HireDate = staffDTO.HireDate;
                existingStaff.Active = staffDTO.Active;
                existingStaff.BlackList = staffDTO.BlackList;
                existingStaff.Updated_By = staffDTO.Updated_By;
                existingStaff.Updated_At = DateTime.Now;

                var branch = _dataContext.tblO_Branch.Find(staffDTO.BranchId);
                var address = _dataContext.ViewO_Address.Find(staffDTO.VillageCode);
                var position = _dataContext.tblO_Position.Find(staffDTO.PositionId);
                var user = _dataContext.ViewO_Users.Find(staffDTO.UserId);
                await _dataContext.SaveChangesAsync();
                var data = new { existingStaff, branch, position, address, user };
                return Ok(new { Message = $"Staff updated successfully!", data = data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data in the database.");
            }
        }
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "delete a single staff", Description = "Admin allow only in create date and master admin allow")]
        public async Task<IActionResult> Delete(string id, string UserId)
        {
            try
            {
                var staff = await _dataContext.tblO_Staff.FindAsync(id);
                if (staff == null) return NotFound(new { Message = "Customer not found!" });
                var checkRole = await _dataContext.ViewAuth_UserRole.FindAsync(UserId);
                if (staff.Created_At.Date != DateTime.Now.Date)
                {
                    if (checkRole.Name == "master admin")
                    {
                        _dataContext.tblO_Staff.Remove(staff);
                        await _dataContext.SaveChangesAsync();
                        return Ok(new { result = "Customer has been deleted successfully" });
                    }
                    return BadRequest(new { Message = "You Don't have permission to delete. Please contact to Master admin" });
                }
                else
                {
                    if (checkRole.Name == "admin")
                    {
                        _dataContext.tblO_Staff.Remove(staff);
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
    }
}