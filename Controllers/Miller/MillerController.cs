using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace API.Controllers
{
    [Route("api/miller/")]
    [ApiController]
    public class MillerController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public MillerController(DataContext dataContext)
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
        [SwaggerOperation(Summary = "get all miller", Description = "")]
        public async Task<IActionResult> GetMillers()
        {
            try
            {
                var data = from miller in _dataContext.tblO_Miller
                           join address in _dataContext.ViewO_Address on miller.VillageCode equals address.VillageCode
                           select new
                           {
                               miller = miller,
                               address = address,
                           };
                var dataList = await data.ToListAsync();
                return dataList.Any() ? Ok(new { Message = "success!", Data = dataList }) : NotFound(new { Message = "No miller found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "get a single miller", Description = "")]
        public async Task<IActionResult> GetMillers(int id)
        {
            try
            {
                var data = from miller in _dataContext.tblO_Miller.Where(x => x.Id == id)
                           join address in _dataContext.ViewO_Address on miller.VillageCode equals address.VillageCode
                           select new
                           {
                               miller = miller,
                               address = address,
                           };
                var dataList = await data.ToListAsync();
                return dataList.Any() ? Ok(new { Message = "success!", Data = dataList }) : NotFound(new { Message = "No miller found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "add a single miller", Description = "")]
        public async Task<IActionResult> AddMiller(Millers millerDTO)
        {
            if (millerDTO == null) return BadRequest(new { Message = "Model is empty" });
            int getlastid;
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                getlastid = _dataContext.tblO_Miller.OrderByDescending(e => e.Id).FirstOrDefault()?.Id ?? 0;

                var newMiller = new Millers
                {
                    Id = getlastid + 1,
                    Name = millerDTO.Name,
                    Description = millerDTO.Description,
                    Tel_1 = millerDTO.Tel_1,
                    Tel_2 = millerDTO.Tel_2,
                    Tel_3 = millerDTO.Tel_3,
                    VillageCode = millerDTO.VillageCode,
                    Active = true,
                    Created_By = millerDTO.Created_By,
                    Created_At = DateTime.Now,
                    Updated_By = millerDTO.Created_By,
                    Updated_At = DateTime.Now
                };

                await _dataContext.tblO_Miller.AddAsync(newMiller);
                await _dataContext.SaveChangesAsync();

                var address = await _dataContext.ViewO_Address.FindAsync(millerDTO.VillageCode);
                var data=new {newMiller, address};

                return Ok(new { Message = "Miller added successfully!",data=data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "replace a single miller", Description = "")]
        public async Task<IActionResult> UpdateMiller(int id, Millers millerDTO)
        {
            if (millerDTO == null) return BadRequest("Model is empty");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingMiller = await _dataContext.tblO_Miller.FindAsync(id);
            if (existingMiller == null) return NotFound(new { Message = "Miller not found!" });
            try
            {
                existingMiller.Name = millerDTO.Name;
                existingMiller.Description = millerDTO.Description;
                existingMiller.Tel_1 = millerDTO.Tel_1;
                existingMiller.Tel_2 = millerDTO.Tel_2;
                existingMiller.Tel_3 = millerDTO.Tel_3; 
                existingMiller.VillageCode = millerDTO.VillageCode;
                existingMiller.Active = millerDTO.Active;
                existingMiller.Updated_By = millerDTO.Updated_By;
                existingMiller.Updated_At = DateTime.Now;
                await _dataContext.SaveChangesAsync();

                var address = await _dataContext.ViewO_Address.FindAsync(millerDTO.VillageCode);
                var data = new { existingMiller, address };
                return Ok(new { Message = "Miller updated successfully!",data=data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data in the database.");
            }
        }
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "delete a single miller from DB", Description = "")]
        public async Task<IActionResult> Delete(string id, string UserId)
        {
            try
            {
                var miller = await _dataContext.tblO_Miller.FindAsync(id);
                if (miller == null) return NotFound(new { Message = "Miller not found!" });
               
                _dataContext.tblO_Miller.Remove(miller);
                await _dataContext.SaveChangesAsync();
                return Ok(new { result = "Miller has been deleted successfully" });
                   
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data in the database.");
            }
        }
    }


}