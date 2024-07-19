using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace API.Controllers.Core
{
    [Route("api/core/location")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public LocationController(DataContext dataContext)
        {
            _dataContext = dataContext;

        }

        [HttpGet("getprovince")]
        public IActionResult GetProvince()
        {
            try
            {
                var provinces = _dataContext.tblOL_Provinces.ToList();
                if (provinces is null) { return NotFound("No Provinces !"); }
                return Ok(provinces);
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
        [HttpGet("getdistrict/{Province_Code}")]
        public IActionResult GetDistrict(string Province_Code)
        {
            return Ok(GetEntries("tblOL_Districts", "ProvinceCode", Province_Code));
        }
        [HttpGet("getcommune/{district_Code}")]
        public IActionResult getCommune(string district_Code)
        {
            return Ok(GetEntries("tblOL_Communes", "DistrictCode",district_Code));
        }
        [HttpGet("getvillage/{commune_Code}")]
        public IActionResult getVillage(string commune_Code)
        {
            return Ok(GetEntries("tblOL_Villages", "CommuneCode", commune_Code));
        }
        [HttpPut("province/update")]
        public async Task<IActionResult> updateProvince(Province province)
        {
            try
            {
                var findProvince = _dataContext.tblOL_Provinces.Find(province.ProvinceCode);
                if (findProvince == null) return BadRequest(new { Message = "Province Invalid!" });
                findProvince.ProvinceName = province.ProvinceName;
                findProvince.Active = province.Active;
                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = "Update Success!" });
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
        [HttpPut("district/{id}")]
        public IActionResult updateDistrict(string id, string newName)
        {
            return Ok(UpdateName("tblOL_Districts", "DistrictCode", "DistrictName", id, newName));
        }
        [HttpPut("commune/{id}")]
        public IActionResult updateCommune(string id, string newName)
        {
            return Ok(UpdateName("tblOL_Communes", "CommuneCode", "CommuneName", id, newName));
        }
        [HttpPut("village/{id}")]
        public IActionResult updateVillage(string id,string newName)
        {
            return Ok(UpdateName("tblOL_Villages", "VillageCode", "VillageName",id,newName));
        }
        private IActionResult GetEntries(string tblName, string tblCol, string value)
        {
            try
            {
                // Get the table from the data context using reflection
                var table = _dataContext.GetType().GetProperty(tblName).GetValue(_dataContext);

                // Cast the table to IQueryable and filter it using dynamic LINQ
                var result = ((IQueryable<object>)table).Where($"{tblCol} == @0", value).ToList();

                if (result == null || !result.Any())
                {
                    return NotFound("No Data!");
                }
                return Ok(result);
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
       
        private IActionResult UpdateName(string tblName, string idCol, string nameCol, string id, string newName)
        {
            try
            {
                // Get the table from the data context using reflection
                var tableProperty = _dataContext.GetType().GetProperty(tblName);
                if (tableProperty == null)
                {
                    return BadRequest("Invalid table name!");
                }

                var table = tableProperty.GetValue(_dataContext) as IQueryable<object>;
                if (table == null)
                {
                    return BadRequest("Invalid table type!");
                }

                // Find the record by ID
                var record = table.FirstOrDefault(x => EF.Property<string>(x, idCol) == id);
                if (record == null)
                {
                    return NotFound("Record not found!");
                }

                // Use reflection to set the new name value
                var propertyInfo = record.GetType().GetProperty(nameCol);
                if (propertyInfo == null)
                {
                    return BadRequest("Invalid column name!");
                }
                propertyInfo.SetValue(record, newName);

                // Mark the entity as modified
                _dataContext.Entry(record).State = EntityState.Modified;

                // Save changes to the database
                _dataContext.SaveChanges();

                return Ok("Name updated successfully!");
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while updating data in the database.", Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }

    }
}
