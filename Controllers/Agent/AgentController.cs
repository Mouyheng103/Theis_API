using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers.Agent
{
    [Route("api/agent")]
    [ApiController]
    [AllowAnonymous]
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
        [HttpGet]
        [SwaggerOperation(Summary = "retrive all active agents", Description = "active agent only")]
        public async Task<IActionResult> GetAgent() 
        {
            try
            {
                var data = from agent in _dataContext.tblO_Agent where agent.Active==true
                             join position in _dataContext.tblO_Position on agent.PositionId equals position.Id
                             join branch in _dataContext.tblO_Branch on agent.BranchId equals branch.Id
                             join address in _dataContext.ViewO_Address on agent.VillageCode equals address.VillageCode
                             select new
                             {
                                 Id=agent.Id,
                                 Name=agent.Name,
                                 Gender=agent.Gender,
                                 DOB=agent.DOB,
                                 Tel=agent.Tel_1,
                                 Tel1=agent.Tel_2,
                                 Tel2=agent.Tel_3,
                                 Commision=agent.Commission,
                                 Active = agent.Active,
                                 Created_At=agent.Created_At,
                                 Created_By=agent.Created_By,
                                 Updated_At=agent.Updated_At,
                                 Updated_by=agent.Updated_By,
                                 position = position,
                                 branch = branch,
                                 address = address,
                             };
                var dataList = await data.ToListAsync();

                return dataList.Any() ? Ok(new { Message = "success!", Data = dataList }) : NotFound(new { Message = "No agents found." });

            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }
        [HttpGet("inactive")]
        [SwaggerOperation(Summary = "retrive all InActive agents", Description = "InActive agent only")]
        public async Task<IActionResult> GetNotActiveAgent()
        {
            try
            {
                var data = from agent in _dataContext.tblO_Agent
                           where agent.Active == false
                           join position in _dataContext.tblO_Position on agent.PositionId equals position.Id
                           join branch in _dataContext.tblO_Branch on agent.BranchId equals branch.Id
                           join address in _dataContext.ViewO_Address on agent.VillageCode equals address.VillageCode
                           select new
                           {
                               Id = agent.Id,
                               Name = agent.Name,
                               Gender = agent.Gender,
                               DOB = agent.DOB,
                               Tel = agent.Tel_1,
                               Tel1 = agent.Tel_2,
                               Tel2 = agent.Tel_3,
                               Commision = agent.Commission,
                               Active = agent.Active,
                               Created_At = agent.Created_At,
                               Created_By = agent.Created_By,
                               Updated_At = agent.Updated_At,
                               Updated_by = agent.Updated_By,
                               position = position,
                               branch = branch,
                               address = address,
                           };
                var dataList = await data.ToListAsync();

                return dataList.Any() ? Ok(new { Message = "success!", Data = dataList }) : NotFound(new { Message = "No agents found." });

            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "retrive a single agent", Description = "active agent only")]
        public async Task<IActionResult> GetAgent(string id)
        {
            try
            {
                var data = from agent in _dataContext.tblO_Agent where agent.Active == true && agent.Id == id
                           join position in _dataContext.tblO_Position on agent.PositionId equals position.Id
                           join branch in _dataContext.tblO_Branch on agent.BranchId equals branch.Id
                           join address in _dataContext.ViewO_Address on agent.VillageCode equals address.VillageCode
                           select new
                           {
                               Id = agent.Id,
                               Name = agent.Name,
                               Gender = agent.Gender,
                               DOB = agent.DOB,
                               Tel = agent.Tel_1,
                               Tel1 = agent.Tel_2,
                               Tel2 = agent.Tel_3,
                               Commision = agent.Commission,
                               Active = agent.Active,
                               Created_At = agent.Created_At,
                               Created_By = agent.Created_By,
                               Updated_At = agent.Updated_At,
                               Updated_by = agent.Updated_By,
                               position = position,
                               branch = branch,
                               address = address,
                           };
                var dataList = await data.ToListAsync();

                return dataList.Any() ? Ok(new { Message = "success!", Data = dataList }) : NotFound(new { Message = "No agents found." });

            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "add a single agent", Description = "active agent only")]

        public async Task<IActionResult> AddAgent(Agents agentDTO)
        {
            if (agentDTO == null) return BadRequest(new { Message = "Model is empty" });
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var newAgent = new Agents
                {
                    Id = GetNewAgentId(agentDTO.VillageCode),
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

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "replace a single agent", Description = "user can update only in create date!")]
        public async Task<IActionResult> UpdateAgent(string id, Agents agentDTO)
        {
            if (agentDTO == null) return BadRequest("Model is empty");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingAgent = await _dataContext.tblO_Agent.FindAsync(id);
            if (existingAgent == null) return NotFound(new { Message = "Agent not found!" });
            if (existingAgent.Created_At.Date != DateTime.Now.Date)
            {
                var checkRole = await _dataContext.ViewAuth_UserRole.FindAsync(agentDTO.Updated_By);
                if (checkRole.Name != "admin" && checkRole.Name !="master admin")
                    return BadRequest(new { Message = "You Don't have permission to delete. Please contact to admin" });
            }
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
        [HttpPatch("{id}")]
        [SwaggerOperation(Summary = "update a single agent", Description = "user can update only in create date!")]
        public async Task<IActionResult> UpdateStatus(string id, bool active,string UserId)
        {
            try
            {
                var agent = await _dataContext.tblO_Agent.FindAsync(id);
                if (agent == null) return NotFound(new { Message = "Agent not found!" });
                if (agent.Created_At.Date != DateTime.Now.Date)
                {
                    var checkRole = await _dataContext.ViewAuth_UserRole.FindAsync(UserId);
                    if (checkRole.Name != "admin" && checkRole.Name != "master admin")
                        return BadRequest(new { Message = "You Don't have permission to delete. Please contact to admin" });
                }
                agent.Active = active;
                agent.Updated_At = DateTime.Now;
                agent.Updated_By = UserId;
                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = "Agent Deleted successfully!" });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data in the database.");
            }
        }
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "delete a single agent from DB", Description = "admin can delete only in create date!")]
        public async Task<IActionResult> DeleteAgent(string id, string UserId)
        {
            try
            {
                var agent = await _dataContext.tblO_Agent.FindAsync(id);
                if (agent == null) return NotFound(new { Message = "Agent not found!" });
                var checkRole = await _dataContext.ViewAuth_UserRole.FindAsync(UserId);
                if (agent.Created_At.Date != DateTime.Now.Date)
                {
                    if (checkRole.Name == "master admin")
                    {
                        _dataContext.tblO_Agent.Remove(agent);
                        await _dataContext.SaveChangesAsync();
                        return Ok(new { result = "Agent has been deleted successfully" });
                    }
                    return BadRequest(new { Message = "You Don't have permission to delete. Please contact to Master admin" });
                }
                else
                {
                    if (checkRole.Name == "admin")
                    {
                        _dataContext.tblO_Agent.Remove(agent);
                        await _dataContext.SaveChangesAsync();
                        return Ok(new { result = "Agent has been deleted successfully" });
                    }
                    return BadRequest(new { Message = "You Don't have permission to delete. Please contact to admin" });
                }
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
