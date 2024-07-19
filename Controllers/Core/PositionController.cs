using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.Core
{
    [Route("api/core/")]
    [ApiController]
    public class PositionController : Controller
    {

        private readonly DataContext _dataContext;
        public PositionController(DataContext dataContext)
        {
            _dataContext = dataContext;

        }

        [HttpGet("getposition")]
        public IActionResult GetPosition()
        {
            try
            {
                var positions = _dataContext.tblO_Position.ToList();
                if (positions is null) { return NotFound("No Position !"); }
                return Ok(positions);
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
        [HttpGet("findposition/{position}")]
        public IActionResult FindPosition(string position)
        {
            try
            {
                if(position==null) { return BadRequest("please provide position name!!"); }
                var positions = _dataContext.tblO_Position.Where(p=>p.En_Name.Contains(position)).ToList();
                if (positions is null || !positions.Any()) { return NotFound("No Position !"); }
                return Ok(positions);
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
        [HttpPost("position/add")]
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
                var findPosition = _dataContext.tblO_Position.Where(p => p.En_Name == positionDTO.En_Name).FirstOrDefault();
                if (findPosition is not null)
                {
                    return Ok(new { Message = "Name already exists!" });
                }
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving data from the database.", Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }

            try
            {
                var newPosition = new Position()
                {
                    Id = getLastId + 1,
                    Kh_Name = positionDTO.Kh_Name,
                    En_Name = positionDTO.En_Name,
                    Description = positionDTO.Description,
                    Created_At = DateTime.Now,
                    Created_By = positionDTO.Created_By,
                    Updated_At = DateTime.Now,
                    Updated_By = positionDTO.Created_By,
                    Active = true
                };
                var addPosition = _dataContext.Add(newPosition);
                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = $"Position added successfully!" });
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
        [HttpPut("position/update/{id}")]
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
            var findDup = _dataContext.tblO_Position.Where(p => p.En_Name == positionDTO.En_Name && p.Id !=id).FirstOrDefault();
            if (findDup is not null)
            {
                return Ok(new { Message = "Name already exists!" });
            }
            try
            {
                findPositionById.En_Name = positionDTO.En_Name;
                findPositionById.Kh_Name = positionDTO.Kh_Name;
                findPositionById.Description = positionDTO.Description;
                findPositionById.Active = positionDTO.Active;
                findPositionById.Updated_By = positionDTO.Updated_By;
                findPositionById.Updated_At = DateTime.Now;

                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = $"Position updated successfully!" });
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

    }
}
