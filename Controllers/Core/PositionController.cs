using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Core
{
    [Route("api/core/position")]
    [ApiController]
    public class PositionController : Controller
    {

        private readonly DataContext _dataContext;
        public PositionController(DataContext dataContext)
        {
            _dataContext = dataContext;

        }
        private IActionResult HandleException(Exception ex, string customMessage)
        {
            if (ex is SqlException)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"{customMessage} Error: {ex.Message}" });
            else if (ex is DbUpdateConcurrencyException)
                return StatusCode(StatusCodes.Status409Conflict, new { Message = "A concurrency error occurred while updating the data.", Error = ex.Message });
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred.", Error = ex.Message });
        }
        [HttpGet]
        [SwaggerOperation(Summary = "retrive all positions", Description = "")]
        public async Task<IActionResult> GetPosition()
        {
            try
            {
                var data = await _dataContext.tblO_Position.ToListAsync();
                return data.Any() ? Ok(new { Message = "success!", Data = data }) : NotFound(new { Message = "No position found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data to the database.");
            }
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "retrive a single position", Description = "")]
        public async Task<IActionResult> GetPosition(int id)
        {
            try
            {
                var data = await _dataContext.tblO_Position.Where(e=>e.Id==id).ToListAsync();
                return data.Any() ? Ok(new { Message = "success!", Data = data }) : NotFound(new { Message = "No position found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data to the database.");
            }
        }
        [HttpPost]
        [SwaggerOperation(Summary = "add a single position", Description = "")]
        public async Task<IActionResult> AddPosition(Position positionDTO)
        {
            if (positionDTO == null)
            {
                return BadRequest(new { Message = "Model is Empty" });
            }

            int getLastId;
            try
            {
                getLastId = _dataContext.tblO_Position.OrderByDescending(e => e.Id).FirstOrDefault()?.Id ?? 0;
                var findPosition = _dataContext.tblO_Position.Where(p => p.Name == positionDTO.Name).FirstOrDefault();
                if (findPosition is not null)
                {
                    return BadRequest(new { Message = "Name already exists!" });
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data to the database.");
            }

            try
            {
                var newPosition = new Position()
                {
                    Id = getLastId + 1,
                    Name = positionDTO.Name,
                    Description = positionDTO.Description,
                    Created_At = DateTime.Now,
                    Created_By = positionDTO.Created_By,
                    Updated_At = DateTime.Now,
                    Updated_By = positionDTO.Created_By,
                    Active = true
                };
                var addPosition = _dataContext.Add(newPosition);
                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = $"Position added successfully!", data = newPosition });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "replace a single position", Description = "")]
        public async Task<IActionResult> UpdatePosition(int id, Position positionDTO)
        {
            if (positionDTO == null)
            {
                return BadRequest("Model is Empty");
            }

            var findPositionById = await _dataContext.tblO_Position.FindAsync(id);
            if (findPositionById == null)
            {
                return NotFound(new { Message = $"Position Not Found!" });
            }
            var findDup = _dataContext.tblO_Position.Where(p => p.Name == positionDTO.Name && p.Id != id).FirstOrDefault();
            if (findDup is not null)
            {
                return Ok(new { Message = "Name already exists!" });
            }
            try
            {
                findPositionById.Name = positionDTO.Name;
                findPositionById.Description = positionDTO.Description;
                findPositionById.Active = positionDTO.Active;
                findPositionById.Updated_By = positionDTO.Updated_By;
                findPositionById.Updated_At = DateTime.Now;

                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = $"Position updated successfully!",data=findPositionById });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data to the database.");
            }
        }
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "delete a single position", Description = "delete from db")]
        public async Task<IActionResult> DeletePosition(int id)
        {
            var findpos = await _dataContext.tblO_Position.FindAsync(id);
            if (findpos == null) { return NotFound(new { Message = $"Branch Not Found !" }); }
            try
            {
                _dataContext.tblO_Position.Remove(findpos);
                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = $"Position deleted successfully !" });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while deleting data to the database.");
            }
        }

    }
}
