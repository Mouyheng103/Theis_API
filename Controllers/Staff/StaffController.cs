using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

[Route("api/")]
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

    [HttpGet("getstaff")]
    public IActionResult GetStaff()
    {
        try
        {
            var staffs = _dataContext.tblO_Staff.AsNoTracking().ToList();
            if (staffs is null) { return NotFound("No staffs !"); }
            return Ok(staffs);
        }
        catch (Exception ex)
        {
            return HandleException(ex, "An error occurred while retrieving data from the database.");
        }
    }

    [HttpGet("findstaff/{staffCode}")]
    public async Task<IActionResult> FindStaff(int staffCode)
    {

        try
        {
            var staff = await _dataContext.tblO_Staff.AsNoTracking()
                .Where(s => s.StaffId ==staffCode)
                .ToListAsync();
            return staff.Any() ? Ok(staff) : NotFound(new { Message = $"No staff found with the code '{staffCode}'." });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "An error occurred while retrieving data from the database.");
        }
    }

    [HttpPost("staff/add")]
    public async Task<IActionResult> AddStaff( Staffs staffDTO)
    {
        if (staffDTO == null) return BadRequest(new { Message = "Model is empty" });

        if (!ModelState.IsValid) return BadRequest(ModelState);
        int getlastid;
        try
        {
            getlastid = _dataContext.tblO_Staff.OrderByDescending(e => e.StaffId).FirstOrDefault()?.StaffId ?? 0;
            var branch=_dataContext.tblO_Branch.Find(staffDTO.BranchId);
            var address=_dataContext.ViewO_Address.Find(staffDTO.VillageCode);
            var newStaff = new Staffs
            {
                StaffId = getlastid+1,
                BranchId = staffDTO.BranchId,
                Kh_Name = staffDTO.Kh_Name,
                En_Name = staffDTO.En_Name,
                Gender = staffDTO.Gender,
                Tel_Cellcard = staffDTO.Tel_Cellcard,
                Tel_Smart = staffDTO.Tel_Smart,
                Tel_Metfone = staffDTO.Tel_Metfone,
                PositionId = staffDTO.PositionId,
                UserId = staffDTO.UserId,
                VillageCode = staffDTO.VillageCode,
                Active = true,
                BlackList = false,
                Created_By = staffDTO.Created_By,
                Created_At = DateTime.Now,
                Updated_By = staffDTO.Created_By,
                Updated_At = DateTime.Now
            };
            await _dataContext.tblO_Staff.AddAsync(newStaff);
            await _dataContext.SaveChangesAsync();
            return Ok(new { Message = "Staff Added successfully!",data=newStaff,branchData=branch,address=address });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "An error occurred while adding data to the database.");
        }
    }

    [HttpPut("staff/update/{id}")]
    public async Task<IActionResult> UpdateStaff(Guid id, Staffs staffDTO)
    {
        if (staffDTO == null) return BadRequest("Model is empty");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingStaff = await _dataContext.tblO_Staff.FindAsync(id);
        if (existingStaff == null) return NotFound(new { Message = "Staff not found!" });
        var branch = _dataContext.tblO_Branch.Find(staffDTO.BranchId);
        var address = _dataContext.ViewO_Address.Find(staffDTO.VillageCode);
        try
        {
            existingStaff.BranchId = staffDTO.BranchId;
            existingStaff.Kh_Name = staffDTO.Kh_Name;
            existingStaff.En_Name = staffDTO.En_Name;
            existingStaff.Gender = staffDTO.Gender;
            existingStaff.Tel_Cellcard = staffDTO.Tel_Cellcard;
            existingStaff.Tel_Smart = staffDTO.Tel_Smart;
            existingStaff.Tel_Metfone = staffDTO.Tel_Metfone;
            existingStaff.PositionId = staffDTO.PositionId;
            existingStaff.UserId = staffDTO.UserId;
            existingStaff.VillageCode = staffDTO.VillageCode;
            existingStaff.Active = staffDTO.Active;
            existingStaff.BlackList = staffDTO.BlackList;
            existingStaff.Updated_By = staffDTO.Updated_By;
            existingStaff.Updated_At = DateTime.Now;

            await _dataContext.SaveChangesAsync();
            return Ok(new { Message = $"Staff updated successfully!",data= existingStaff,branchData = branch, address = address });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "An error occurred while updating data in the database.");
        }
    }
}
