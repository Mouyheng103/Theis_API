using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    [Route("api/")]
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

        [HttpGet("getmiller")]
        public IActionResult GetMillers()
        {
            try
            {
                var millers = _dataContext.tblO_Miller.AsNoTracking().ToList();
                if (millers is null) { return NotFound("No millers found!"); }
                return Ok(millers);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }

        [HttpGet("findmiller/{millerId}")]
        public async Task<IActionResult> FindMiller(int millerId)
        {
            try
            {
                var miller = await _dataContext.tblO_Miller.AsNoTracking()
                    .Where(m => m.AutoId == millerId)
                    .ToListAsync();
                return miller.Any() ? Ok(miller) : NotFound(new { Message = "No miller founded." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }

        [HttpPost("miller/add")]
        public async Task<IActionResult> AddMiller(Millers millerDTO)
        {
            if (millerDTO == null) return BadRequest(new { Message = "Model is empty" });
            int getlastid;
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                getlastid = _dataContext.tblO_Miller.OrderByDescending(e => e.AutoId).FirstOrDefault()?.AutoId ?? 0;

                var newMiller = new Millers
                {
                    AutoId = getlastid + 1,
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
                var address = await _dataContext.ViewO_Address.FindAsync(millerDTO.VillageCode);

                await _dataContext.tblO_Miller.AddAsync(newMiller);
                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = "Miller added successfully!",data=newMiller, address = address });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }

        [HttpPut("miller/update/{id}")]
        public async Task<IActionResult> UpdateMiller(int id, Millers millerDTO)
        {
            if (millerDTO == null) return BadRequest("Model is empty");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingMiller = await _dataContext.tblO_Miller.FindAsync(id);
            var address= await _dataContext.ViewO_Address.FindAsync(millerDTO.VillageCode);
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
                return Ok(new { Message = "Miller updated successfully!",data=existingMiller, address=address });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data in the database.");
            }
        }
    }


}