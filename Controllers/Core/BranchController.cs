using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.Core
{
    [Route("api/core/")]
    [ApiController]
    public class BranchController : ControllerBase
    {
       
        private readonly DataContext _dataContext;
        public BranchController(DataContext dataContext) {
            _dataContext = dataContext;
           
        }

        [HttpGet("getbranch")]
        public IActionResult GetBranch()
        {
            try
            {
                var branches = _dataContext.tblO_Branch.ToList();
                if(branches is null) { return NotFound("No Branch !"); }
                return Ok(branches);
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving data from the database.", Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }
        [HttpGet("findbranch/{branchName}")]
        public async Task<IActionResult> FindBranch(string branchName)
        {
            try
            {
                if (branchName == null) { return BadRequest("please provide branch name!!"); }
                var branch = _dataContext.tblO_Branch.Where(b => b.Name.Contains(branchName)).ToList();
                if (branch == null || !branch.Any())
                {
                    return NotFound(new { Message = $"No branches found with the name '{branchName}'." });
                }
                return Ok(branch);
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving data from the database.", Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }
        [HttpPost("branch/add")]
        public async Task<IActionResult> AddBranch(Branch branchDTO)
        {
            if (branchDTO == null) { return BadRequest( new { Message = "Model is Empty"}); }
            int getlastid;
            try
            {
                 getlastid = _dataContext.tblO_Branch.OrderByDescending(e => e.Id).FirstOrDefault()?.Id ?? 0;
                 var findBranch=_dataContext.tblO_Branch.Where(b => b.Name == branchDTO.Name).FirstOrDefault();
                 if (findBranch is not null) { return Ok(new { Message = "Name already Exsist !" }); }
            }
            catch (SqlException ex){return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving data from the database.", Error = ex.Message });}
            catch (Exception ex) { return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred.", Error = ex.Message }); }
            
            try
            {
                var newBranch = new Branch()
                {
                    Id = getlastid+1,
                    Name = branchDTO.Name,
                    ProvinceId = branchDTO.ProvinceId,
                    Email = branchDTO.Email,
                    Password = branchDTO.Password,
                    PhoneNumber = branchDTO.PhoneNumber,
                    InternetNumber = branchDTO.InternetNumber,
                    Location = branchDTO.Location,
                    BranchMangerId = branchDTO.BranchMangerId,
                    Created_at = DateTime.UtcNow,
                    created_by = branchDTO.created_by, //will config
                    IsActive = true
                };
                var addBranch = _dataContext.Add(newBranch);
                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = $"Branch {branchDTO.Name} add successfully !" });
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving data from the database.", Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }
        [HttpPut("branch/update/{id}")]
        public async Task<IActionResult> UpdateBranch(int id,Branch branchDTO)
        {
            if (branchDTO == null) { return BadRequest("Model is Empty"); }
            var findBranchById = await _dataContext.tblO_Branch.FindAsync(id);
            if (findBranchById == null) { return NotFound(new { Message = $"Branch Not Found !" }); }
            if(findBranchById.Name==branchDTO.Name) { return Ok(new { Message = "Name already Exsist !" }); }
            try
            {

                findBranchById.Name = branchDTO.Name;
                findBranchById.ProvinceId = branchDTO.ProvinceId;
                findBranchById.Email = branchDTO.Email;
                findBranchById.Password = branchDTO.Password;
                findBranchById.PhoneNumber = branchDTO.PhoneNumber;
                findBranchById.InternetNumber = branchDTO.InternetNumber;
                findBranchById.Location = branchDTO.Location;
                findBranchById.BranchMangerId = branchDTO.BranchMangerId;
                findBranchById.Created_at = DateTime.UtcNow;
                findBranchById.created_by = branchDTO.created_by;//will config
                
                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = $"Branch {branchDTO.Name} updated successfully !" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(StatusCodes.Status409Conflict, new { Message = "A concurrency error occurred while updating the data.", Error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "A database error occurred while updating the data.", Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }
        [HttpDelete("branch/delete/{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var findBranchById = await _dataContext.tblO_Branch.FindAsync(id);
            if (findBranchById == null) { return NotFound(new { Message = $"Branch Not Found !" }); }
            try
            {
                _dataContext.tblO_Branch.Remove(findBranchById);
                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = $"Branch deleted successfully !" });
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "A database error occurred while updating the data.", Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }
    }
    
}