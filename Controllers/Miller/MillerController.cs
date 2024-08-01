using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        [HttpGet("get")]
        public IActionResult GetMillers()
        {
            try
            {
                var millers = _dataContext.tblO_Miller.AsNoTracking().ToList();
                return millers.Any() ? Ok(new { Message = "success!", Data = millers }) : NotFound(new { Message = "No millers found." });

            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }

        [HttpGet("find")]
        public IActionResult FindMiller(int? Id, string? Name)
        {
            try
            {
                if (!string.IsNullOrEmpty(Name)) {
                    var dataByName = (from miller in _dataContext.tblO_Miller.Where(m=>m.Name==Name).AsNoTracking()
                                        join address in _dataContext.ViewO_Address.AsNoTracking()
                                        on miller.VillageCode equals address.VillageCode
                                        select new
                                        {
                                            Miller = miller,
                                            Address = "ភូមិ" + address.VillageName + " ឃុំ" + address.CommuneName + " ឃុំ" + address.DistrictName + " ឃុំ" + address.ProvinceName
                                        }).ToList();
                    return dataByName.Any() ? Ok(new { Message = "success!", Data = dataByName }) : NotFound(new { Message = "No millers found." });
                }
                else if(Id !=0 || Id != null)
                {
                    var dataById = (from miller in _dataContext.tblO_Miller.Where(m => m.Id == Id).AsNoTracking()
                                      join address in _dataContext.ViewO_Address.AsNoTracking()
                                      on miller.VillageCode equals address.VillageCode
                                      select new
                                      {
                                          Miller = miller,
                                          Address = "ភូមិ" + address.VillageName + " ឃុំ" + address.CommuneName + " ឃុំ" + address.DistrictName + " ឃុំ" + address.ProvinceName
                                      }).ToList();
                    return dataById.Any() ? Ok(new { Message = "success!", Data = dataById }) : NotFound(new { Message = "No millers found." });
                }
                else
                {
                    return BadRequest(new { Message = "Please provide Id or Province or Name" });
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }

        [HttpPost("add")]
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

        [HttpPut("update/{id}")]
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
    }


}