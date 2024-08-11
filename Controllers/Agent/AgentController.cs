﻿using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Controllers.Agent
{
    [Route("api/")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public AgentController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private IActionResult HandleException(Exception ex, string customMessage)
        {
            if (ex is SqlException)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = $"{customMessage} Error: {ex.Message}" });
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred.", Error = ex.Message });
        }
        [HttpGet("agent/get")]
        public  IActionResult GetAgent() 
        {
            try
            {
                var data = _dataContext.ViewO_Agents.AsNoTracking().ToList();
                return data.Any() ? Ok(new { Message = "success!", Data = data }) : NotFound(new { Message = "No agents found." });

            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }
        [HttpGet("agent/find")]
        public IActionResult FindAgent( string? Id, string? VillageCode, int? BranchId)
        {
            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    var dataById = _dataContext.ViewO_Agents.Where(a => a.Id == Id).AsNoTracking().ToList();
                    return dataById.Any() ? Ok(new { Message = "success!", Data = dataById }) : NotFound(new { Message = "No agents found." });
                }
                else if (!string.IsNullOrEmpty(VillageCode))
                {
                    var dataByVillageCode = _dataContext.ViewO_Agents.Where(a => a.VillageCode == VillageCode).AsNoTracking().ToList();
                    return dataByVillageCode.Any() ? Ok(new { Message = "success!", Data = dataByVillageCode }) : NotFound(new { Message = "No agents found." });
                }
                else if (BranchId!=null || BranchId !=0)
                {
                    var dataByBranch = _dataContext.ViewO_Agents.Where(a => a.BranchId == BranchId).AsNoTracking().ToList();
                    return dataByBranch.Any() ? Ok(new { Message = "success!", Data = dataByBranch }) : NotFound(new { Message = "No agents found." });
                }
                else
                {
                    return BadRequest(new { Message = "Please provide Id or VillageCode or BranchId" });
                }
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }

        [HttpPost("agent/add")]
        public async Task<IActionResult> AddAgent(Agents agentDTO)
        {
            if (agentDTO == null) return BadRequest(new { Message = "Model is empty" });
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var newAgent = new Agents
                {
                    Id = GetNewAgentId(agentDTO.VillageCode ?? throw new ArgumentNullException(nameof(agentDTO.VillageCode))),
                    Name = agentDTO.Name,
                    Gender = agentDTO.Gender,
                    DOB = agentDTO.DOB,
                    Tel_1 = agentDTO.Tel_1,
                    Tel_2 = agentDTO.Tel_2,
                    Tel_3 = agentDTO.Tel_3,
                    PositionId = agentDTO.PositionId,
                    Commission = agentDTO.Commission,
                    BranchId= agentDTO.BranchId,
                    VillageCode = agentDTO.VillageCode,
                    Active = true,
                    Created_By = agentDTO.Created_By,
                    Created_At = DateTime.Now,
                    Updated_By = agentDTO.Created_By,
                    Updated_At = DateTime.Now
                };
                var findAgent= await _dataContext.tblO_Agent.Where(a=>a.Name ==agentDTO.Name && a.VillageCode==agentDTO.VillageCode ).FirstOrDefaultAsync();
                if (findAgent != null)
                    return BadRequest(new { Message = "Agent already exsist!" });
                await _dataContext.tblO_Agent.AddAsync(newAgent);
                await _dataContext.SaveChangesAsync();
                var address = await _dataContext.ViewO_Address.FindAsync(agentDTO.VillageCode);
                var position = await _dataContext.tblO_Position.FindAsync(agentDTO.PositionId);
                var branch = await _dataContext.tblO_Branch.FindAsync(agentDTO.BranchId);
                var data =new {newAgent,address,position,branch};
                return Ok(new { Message = "Agent added successfully!", Data = data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }

        [HttpPut("agent/update/{id}")]
        public async Task<IActionResult> UpdateAgent(string id, Agents agentDTO)
        {
            if (agentDTO == null) return BadRequest("Model is empty");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingAgent = await _dataContext.tblO_Agent.FindAsync(id);
            if (existingAgent == null) return NotFound(new { Message = "Agent not found!" });

            try
            {
                existingAgent.Name = agentDTO.Name;
                existingAgent.Gender = agentDTO.Gender;
                existingAgent.DOB = agentDTO.DOB;
                existingAgent.Tel_1 = agentDTO.Tel_1;
                existingAgent.Tel_2 = agentDTO.Tel_2;
                existingAgent.Tel_3 = agentDTO.Tel_3;
                existingAgent.PositionId = agentDTO.PositionId;
                existingAgent.BranchId = agentDTO.BranchId;
                existingAgent.Commission = agentDTO.Commission;
                existingAgent.Active = agentDTO.Active;
                existingAgent.Updated_By = agentDTO.Updated_By;
                existingAgent.Updated_At = DateTime.Now;

                var checkdata= await _dataContext.tblO_Agent.Where(a=>a.Id == existingAgent.Id && a.Name==existingAgent.Name).FirstOrDefaultAsync();
                if (checkdata == null)
                {
                    var findAgent = await _dataContext.tblO_Agent.Where(a => a.Name == agentDTO.Name && a.VillageCode == agentDTO.VillageCode).FirstOrDefaultAsync();
                    if (findAgent != null)
                        return BadRequest(new { Message = "Agent already exsist!" });
                }
                await _dataContext.SaveChangesAsync();
                var address = await _dataContext.ViewO_Address.FindAsync(agentDTO.VillageCode);
                var position = await _dataContext.tblO_Position.FindAsync(agentDTO.PositionId);
                var branch = await _dataContext.tblO_Branch.FindAsync(agentDTO.BranchId);
                var data = new { existingAgent, address, position, branch };
                return Ok(new { Message = "Agent updated successfully!", data = data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data in the database.");
            }
        }
        private string GetNewAgentId(string VillageCode)
        { 
            var countData = _dataContext.tblO_Agent.Where(a => a.VillageCode == VillageCode).Count();
            string id = VillageCode + (countData + 1).ToString("D2");
            return id;
        }
    }

}
