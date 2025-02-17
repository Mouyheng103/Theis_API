﻿using API.Data;
using API.Data.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Core
{
    [Route("api/core/branch")]
    [ApiController]
    [AllowAnonymous]
    public class BranchController : ControllerBase
    {
       
        private readonly DataContext _dataContext;
        public BranchController(DataContext dataContext) {
            _dataContext = dataContext;
           
        }
        private IActionResult HandleException(Exception ex, string customMessage)
        {
            if (ex is SqlException)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"{customMessage} Error: {ex.Message}" });
            else if(ex is DbUpdateConcurrencyException)
                 return StatusCode(StatusCodes.Status409Conflict, new { Message = "A concurrency error occurred while updating the data.", Error = ex.Message });
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred.", Error = ex.Message });
        }
        [HttpGet]
        [SwaggerOperation(Summary = "retrive all branches", Description = "")]
        public async Task<IActionResult> GetBranch()
        {
            try
            {
                var data = from branch in _dataContext.tblO_Branch
                           join address in _dataContext.ViewO_Address on branch.VillageCode equals address.VillageCode
                           join manager in _dataContext.tblO_Staff on branch.BranchMangerId equals manager.Id
                           select new
                           {
                               Id = branch.Id,
                               Name = branch.Name,
                               Email = branch.Email,
                               Password = branch.Password,
                               PhoneNumber = branch.PhoneNumber,
                               InternetNumber = branch.InternetNumber,
                               Location = branch.Location,
                               Created_At = branch.Created_At,
                               Created_by = branch.Created_By,
                               IsActive = branch.IsActive,
                               Address = address,
                               BranchMangerId = manager,
                           };
                var dataList = await data.ToListAsync();
                return data.Any() ? Ok(new { Message = "success!", Data = data }) : NotFound(new { Message = "No branch found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data to the database.");
            }
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "retrive a single branch", Description = "")]
        public async Task<IActionResult> GetBranch(int id)
        {
            try
            {
                var data = from branch in _dataContext.tblO_Branch where branch.Id == id
                           join address in _dataContext.ViewO_Address on branch.VillageCode equals address.VillageCode
                           join manager in _dataContext.tblO_Staff on branch.BranchMangerId equals manager.Id
                           select new
                           {
                               Id = branch.Id,
                               Name = branch.Name,
                               Email = branch.Email,
                               Password = branch.Password,
                               PhoneNumber = branch.PhoneNumber,
                               InternetNumber = branch.InternetNumber,
                               Location = branch.Location,
                               Created_At = branch.Created_At,
                               Created_by = branch.Created_By,
                               IsActive = branch.IsActive,
                               Address = address,
                               BranchMangerId = manager,
                           };
                var dataList = await data.ToListAsync();
                return data.Any() ? Ok(new { Message = "success!", Data = data }) : NotFound(new { Message = "No branch found." });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data to the database.");
            }
        }
        [HttpPost]
        [SwaggerOperation(Summary = "add a single branch", Description = "")]
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
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data to the database.");
            }
            try
            {
                var newBranch = new Branch()
                {
                    Id = getlastid+1,
                    Name = branchDTO.Name,
                    VillageCode = branchDTO.VillageCode,
                    Email = branchDTO.Email,
                    Password = branchDTO.Password,
                    PhoneNumber = branchDTO.PhoneNumber,
                    InternetNumber = branchDTO.InternetNumber,
                    Location = branchDTO.Location,
                    BranchMangerId = branchDTO.BranchMangerId,
                    Created_At = DateTime.Now,
                    Created_By = branchDTO.Created_By, 
                    IsActive = true
                };
                var addBranch = _dataContext.Add(newBranch);
                var BM = await _dataContext.tblO_Staff.FindAsync(branchDTO.BranchMangerId);
                var address = await _dataContext.ViewO_Address.FindAsync(branchDTO.VillageCode);
                await _dataContext.SaveChangesAsync();
                var data = new
                {
                    Id = newBranch.Id,
                    Name = branchDTO.Name,
                    Address = address,
                    Email = branchDTO.Email,
                    Password = branchDTO.Password,
                    PhoneNumber = branchDTO.PhoneNumber,
                    InternetNumber = branchDTO.InternetNumber,
                    Location = branchDTO.Location,
                    BranchMangerId =BM,
                    Created_at = DateTime.Now,
                    Created_By = branchDTO.Created_By,
                    IsActive = true
                };
                return Ok(new { Message = $"Branch {branchDTO.Name} add successfully !", data= data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "update a single branch", Description = "")]
        public async Task<IActionResult> UpdateBranch(int id,Branch branchDTO)
        {
            if (branchDTO == null) { return BadRequest("Model is Empty"); }
            var findBranchById = await _dataContext.tblO_Branch.FindAsync(id);
            if (findBranchById == null) { return NotFound(new { Message = $"Branch Not Found !" }); }
            var finddup=_dataContext.tblO_Branch.Where(b=>b.Name == branchDTO.Name && b.Id!=id).FirstOrDefault();
           if (finddup != null) { return BadRequest(new { Message = "Branch Already Exsist !!" }); }
            try
            {

                findBranchById.Name = branchDTO.Name;
                findBranchById.VillageCode = branchDTO.VillageCode;
                findBranchById.Email = branchDTO.Email;
                findBranchById.Password = branchDTO.Password;
                findBranchById.PhoneNumber = branchDTO.PhoneNumber;
                findBranchById.InternetNumber = branchDTO.InternetNumber;
                findBranchById.Location = branchDTO.Location;
                findBranchById.BranchMangerId = branchDTO.BranchMangerId;
                findBranchById.Created_At = DateTime.Now;
                findBranchById.Created_By = branchDTO.Created_By;
                
                var BM = await _dataContext.tblO_Staff.FindAsync(branchDTO.BranchMangerId);
                var address = await _dataContext.ViewO_Address.FindAsync(branchDTO.VillageCode);
                await _dataContext.SaveChangesAsync();
                var data = new
                {
                    Id = id,
                    Name = branchDTO.Name,
                    address = address,
                    Email = branchDTO.Email,
                    Password = branchDTO.Password,
                    PhoneNumber = branchDTO.PhoneNumber,
                    InternetNumber = branchDTO.InternetNumber,
                    Location = branchDTO.Location,
                    BranchMangerId = BM,
                    Created_At = DateTime.Now,
                    Created_By = branchDTO.Created_By,
                    IsActive = true
                };
                return Ok(new { Message = $"Branch {branchDTO.Name} updated successfully !", data = data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data to the database.");
            }
        }
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "delete a single branch", Description = "")]
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
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while deleting data to the database.");
            }
        }
    }
    
}