using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using static System.Collections.Specialized.BitVector32;

namespace API.Controllers.Staff
{
    [Route("api/staff/salary/Change")]
    [ApiController]
    [AllowAnonymous]
    public class SalaryNewController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public SalaryNewController(DataContext dataContext)
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
        [SwaggerOperation(Summary = "retrive all staff salary increase", Description = "")]
        public async Task<IActionResult> Get()
        {
            try
            {

                var data = from salary in _dataContext.tblO_Staff_SalaryChange
                           join staff in _dataContext.tblO_Staff on salary.StaffId equals staff.Id
                           join branch in _dataContext.tblO_Branch on staff.BranchId equals branch.Id
                           join position in _dataContext.tblO_Position on staff.PositionId equals position.Id
                           join address in _dataContext.ViewO_Address on staff.VillageCode equals address.VillageCode
                           select new
                           {
                               Id = salary.Id,
                               Previous_Salary= salary.Previous_Salary,
                               Increase_Salary=salary.Increase_Salary,
                               New_Salary=salary.New_Salary,
                               Reason=salary.Reason,
                               Section=salary.Section,
                               Created_By = salary.Created_By,
                               Created_At = salary.Created_At,
                               Updated_By = salary.Updated_By,
                               Updated_At = salary.Updated_At,
                               staff = new
                               {
                                   Id = staff.Id,
                                   Name = staff.Name,
                                   En_Name = staff.En_Name,
                                   Gender = staff.Gender,
                                   Tel_Cellcard = staff.Tel_Cellcard,
                                   Tel_Smart = staff.Tel_Smart,
                                   Tel_Metfone = staff.Tel_Metfone,
                                   UserId = staff.UserId,
                                   HireDate = staff.HireDate,
                                   address = address,
                                   branch = branch,
                                   position = position,
                               }
                           };
                var dataList = await data.ToListAsync();

                return dataList.Any() ? Ok(new { Message = "success!", Data = dataList }) : NotFound(new { Message = "No salary found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "retrive a single staff salary", Description = "staff id")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {

                var data = from salary in _dataContext.tblO_Staff_SalaryChange where salary.StaffId == id   
                           join staff in _dataContext.tblO_Staff on salary.StaffId equals staff.Id
                           join branch in _dataContext.tblO_Branch on staff.BranchId equals branch.Id
                           join position in _dataContext.tblO_Position on staff.PositionId equals position.Id
                           join address in _dataContext.ViewO_Address on staff.VillageCode equals address.VillageCode
                           select new
                           {
                               Id = salary.Id,
                               Previous_Salary = salary.Previous_Salary,
                               Increase_Salary = salary.Increase_Salary,
                               New_Salary = salary.New_Salary,
                               Reason = salary.Reason,
                               Section = salary.Section,
                               Created_By = salary.Created_By,
                               Created_At = salary.Created_At,
                               Updated_By = salary.Updated_By,
                               Updated_At = salary.Updated_At,
                               staff = new
                               {
                                   Id = staff.Id,
                                   Name = staff.Name,
                                   En_Name = staff.En_Name,
                                   Gender = staff.Gender,
                                   Tel_Cellcard = staff.Tel_Cellcard,
                                   Tel_Smart = staff.Tel_Smart,
                                   Tel_Metfone = staff.Tel_Metfone,
                                   UserId = staff.UserId,
                                   HireDate = staff.HireDate,
                                   address = address,
                                   branch = branch,
                                   position = position,
                               }
                           };
                var dataList = await data.ToListAsync();

                return dataList.Any() ? Ok(new { Message = "success!", Data = dataList }) : NotFound(new { Message = "No salary found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }
        [HttpPost]
        [SwaggerOperation(Summary = "add a single salary of staff", Description = "")]
        public async Task<IActionResult> AddSalaryBonuses(SalaryChange salary)
        {
            if (salary == null) return BadRequest(new { Message = "Model is empty" });
            try
            {
                var staff = _dataContext.tblO_Staff.Find(salary.StaffId);
                var countSection=_dataContext.tblO_Staff_SalaryChange.Where(x=>x.StaffId == salary.StaffId).Count();

                if (staff.BaseSalary != salary.Previous_Salary)
                    return BadRequest(new { Message = "Base Salary is Wrong !!!" });

                salary.Section = countSection+1;
                salary.Created_At = DateTime.Now;
                salary.Updated_At = DateTime.Now;
                salary.New_Salary = salary.Previous_Salary + salary.Increase_Salary;
                _dataContext.tblO_Staff_SalaryChange.Add(salary);

                staff.BaseSalary = salary.New_Salary;
                _dataContext.tblO_Staff.Update(staff);

                await _dataContext.SaveChangesAsync();
                var branch = _dataContext.tblO_Branch.Find(staff.BranchId);
                var position = _dataContext.tblO_Position.Find(staff.PositionId);
                var address = _dataContext.ViewO_Address.Find(staff.VillageCode);
                var data = new
                {
                    Id = salary.Id,
                    Previous_Salary = salary.Previous_Salary,
                    Increase_Salary = salary.Increase_Salary,
                    New_Salary = salary.New_Salary,
                    Reason = salary.Reason,
                    Section = salary.Section,
                    Created_By = salary.Created_By,
                    Created_At = salary.Created_At,
                    Updated_By = salary.Updated_By,
                    Updated_At = salary.Updated_At,
                    staff = new
                    {
                        Id = staff.Id,
                        Name = staff.Name,
                        En_Name = staff.En_Name,
                        Gender = staff.Gender,
                        Tel_Cellcard = staff.Tel_Cellcard,
                        Tel_Smart = staff.Tel_Smart,
                        Tel_Metfone = staff.Tel_Metfone,
                        UserId = staff.UserId,
                        HireDate = staff.HireDate,
                        BaseSalary=staff.BaseSalary,
                        address = address,
                        branch = branch,
                        position = position,
                    }
                };
                return Ok(new { Message = "New Salary Added successfully!", data = data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }
        [HttpPut]
        [SwaggerOperation(Summary = "replace a single salary of staff", Description = "")]
        public async Task<IActionResult> UpdateSalaryBonuses(SalaryChange salary)
        {
            if (salary == null) return BadRequest(new { Message = "Model is empty" });
            if (salary.Created_At.Date != DateTime.Now.Date)
            {
                var checkRole = await _dataContext.ViewAuth_UserRole.FindAsync(salary.Updated_By);
                if (checkRole.Name != "master admin")
                {
                    return BadRequest(new { Message = "You Don't have permission to update. Please contact to Master admin" });
                }
            }
            try
            {
                var staff = _dataContext.tblO_Staff.Find(salary.StaffId);

                salary.Created_At = DateTime.Now;
                salary.Updated_At = DateTime.Now;
                salary.New_Salary = salary.Previous_Salary + salary.Increase_Salary;

                _dataContext.tblO_Staff_SalaryChange.Update(salary);


                staff.BaseSalary = salary.New_Salary;
                _dataContext.tblO_Staff.Update(staff);

                await _dataContext.SaveChangesAsync();
                var branch = _dataContext.tblO_Branch.Find(staff.BranchId);
                var position = _dataContext.tblO_Position.Find(staff.PositionId);
                var address = _dataContext.ViewO_Address.Find(staff.VillageCode);
                var data = new
                {
                    Id = salary.Id,
                    Previous_Salary = salary.Previous_Salary,
                    Increase_Salary = salary.Increase_Salary,
                    New_Salary = salary.New_Salary,
                    Reason = salary.Reason,
                    Section = salary.Section,
                    Created_By = salary.Created_By,
                    Created_At = salary.Created_At,
                    Updated_By = salary.Updated_By,
                    Updated_At = salary.Updated_At,
                    staff = new
                    {
                        Id = staff.Id,
                        Name = staff.Name,
                        En_Name = staff.En_Name,
                        Gender = staff.Gender,
                        Tel_Cellcard = staff.Tel_Cellcard,
                        Tel_Smart = staff.Tel_Smart,
                        Tel_Metfone = staff.Tel_Metfone,
                        UserId = staff.UserId,
                        HireDate = staff.HireDate,
                        address = address,
                        branch = branch,
                        position = position,
                    }
                };
                return Ok(new { Message = "New Salary Added successfully!", data = data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }
        [HttpDelete]
        [SwaggerOperation(Summary = "replace a single salary of staff", Description = "")]
        public async Task<IActionResult> DeleteNewSalary(int id, string UserId)
        {
            var salary = _dataContext.tblO_Staff_SalaryChange.Find(id);
            if (salary == null)
                return BadRequest(new { Message = "id not correct!", });

            if (salary.Created_At.Date != DateTime.Now.Date)
            {
                var checkRole = await _dataContext.ViewAuth_UserRole.FindAsync(UserId);
                if (checkRole.Name != "master admin")
                {
                    return BadRequest(new { Message = "You Don't have permission to delete. Please contact to Master admin" });
                }
            }
            try
            {
                _dataContext.tblO_Staff_SalaryChange.Remove(salary);
                await _dataContext.SaveChangesAsync();

                return Ok(new { Message = "New Salary Deleted successfully!", });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }
    }
}
