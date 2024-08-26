using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Miller
{
    [Route("api/RicePurchase")]
    [ApiController]
    public class RicePurchaseController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public RicePurchaseController(DataContext dataContext)
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
        [SwaggerOperation(Summary = "Retrieve all rice purchases", Description = "")]
        public async Task<IActionResult> GetRicePurchases()
        {
            try
            {
                var data = await _dataContext.tblS_RicePurchase.ToListAsync();
                return data.Any() ? Ok(new { Message = "Success!", Data = data }) : NotFound(new { Message = "No rice purchases found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Retrieve a single rice purchase by ID", Description = "")]
        public async Task<IActionResult> GetRicePurchase(int id)
        {
            try
            {
                var ricePurchase = await _dataContext.tblS_RicePurchase.FindAsync(id);
                return ricePurchase != null ? Ok(new { Message = "Success!", Data = ricePurchase }) : NotFound(new { Message = "Rice purchase not found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Add a new rice purchase", Description = "")]
        public async Task<IActionResult> AddRicePurchase([FromBody] RicePurchase ricePurchase)
        {
            if (ricePurchase == null) return BadRequest(new { Message = "Model is empty" });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                ricePurchase.Created_At = DateTime.Now;
                _dataContext.tblS_RicePurchase.Add(ricePurchase);
                await _dataContext.SaveChangesAsync();

                return Ok(new { Message = "Rice purchase added successfully!", Data = ricePurchase });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update an existing rice purchase", Description = "")]
        public async Task<IActionResult> UpdateRicePurchase(int id, [FromBody] RicePurchase ricePurchase)
        {
            if (ricePurchase == null) return BadRequest(new { Message = "Model is empty" });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingRicePurchase = await _dataContext.tblS_RicePurchase.FindAsync(id);
            if (existingRicePurchase == null) return NotFound(new { Message = "Rice purchase not found!" });

            try
            {
                existingRicePurchase.YearID = ricePurchase.YearID;
                existingRicePurchase.MillerID = ricePurchase.MillerID;
                existingRicePurchase.Section = ricePurchase.Section;
                existingRicePurchase.PurchaseDate = ricePurchase.PurchaseDate;
                existingRicePurchase.Cost = ricePurchase.Cost;
                existingRicePurchase.Weight = ricePurchase.Weight;
                existingRicePurchase.Quantity = ricePurchase.Quantity;
                existingRicePurchase.TotalCost = ricePurchase.TotalCost;
                existingRicePurchase.PaymentStatus = ricePurchase.PaymentStatus;
                existingRicePurchase.Updated_By = ricePurchase.Updated_By;
                existingRicePurchase.Updated_At = DateTime.Now;

                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = "Rice purchase updated successfully!", Data = existingRicePurchase });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data in the database.");
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a rice purchase", Description = "")]
        public async Task<IActionResult> DeleteRicePurchase(int id)
        {
            try
            {
                var ricePurchase = await _dataContext.tblS_RicePurchase.FindAsync(id);
                if (ricePurchase == null) return NotFound(new { Message = "Rice purchase not found!" });

                _dataContext.tblS_RicePurchase.Remove(ricePurchase);
                await _dataContext.SaveChangesAsync();

                return Ok(new { Message = "Rice purchase deleted successfully!" });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while deleting data from the database.");
            }
        }
    }
}

