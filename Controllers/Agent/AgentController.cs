using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet("getagent")]
        public IActionResult GetAgents()
        {
            try
            {
                var result = from agent in _dataContext.tblO_Agent
                             join address in _dataContext.ViewO_Address on agent.VillageCode equals address.VillageCode
                             join position in _dataContext.tblO_Position on agent.PositionId equals position.Id
                             join staff in _dataContext.tblO_Staff on agent.StaffId equals staff.StaffId
                             join branch in _dataContext.tblO_Branch on agent.BranchId equals branch.Id
                             select new
                             {
                                 AgentId = agent.Id,
                                 Name = agent.Name,
                                 Gender = agent.Gender,
                                 DOB = agent.DOB,
                                 Tel_1 = agent.Tel_1,
                                 Tel_2 = agent.Tel_2,
                                 Tel_3= agent.Tel_3,
                                 Position= position.Kh_Name,
                                 Commision=agent.Commission,
                                 staffName=staff.Kh_Name,
                                 branch=branch.Name,
                                 address= address.VillageName+" "+address.CommuneName+" "+address.DistrictName+" "+address.ProvinceName,
                                 Active = agent.Active,
                                 Updated_At = agent.Updated_At,
                                 Updated_By = agent.Updated_By,
                             };
                var resultList= result.AsNoTracking().ToList();
                if (resultList is null) { return NotFound("No agents found!"); }
                return Ok(resultList);
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while retrieving data from the database.");
            }
        }

        [HttpGet("findagent/{agentId}")]
        public async Task<IActionResult> FindAgent(int agentId)
        {
            try
            {
                var agents = from agent in _dataContext.tblO_Agent
                             join address in _dataContext.ViewO_Address on agent.VillageCode equals address.VillageCode
                             join position in _dataContext.tblO_Position on agent.PositionId equals position.Id
                             join staff in _dataContext.tblO_Staff on agent.StaffId equals staff.StaffId
                             join branch in _dataContext.tblO_Branch on agent.BranchId equals branch.Id
                             select new
                             {
                                 AgentId = agent.Id,
                                 Name = agent.Name,
                                 Gender = agent.Gender,
                                 DOB = agent.DOB,
                                 Tel_1 = agent.Tel_1,
                                 Tel_2 = agent.Tel_2,
                                 Tel_3 = agent.Tel_3,
                                 Position = position.Kh_Name,
                                 Commision = agent.Commission,
                                 staffName = staff.Kh_Name,
                                 branch = branch.Name,
                                 address = address.VillageName + " " + address.CommuneName + " " + address.DistrictName + " " + address.ProvinceName,
                                 Active = agent.Active,
                                 Updated_At = agent.Updated_At,
                                 Updated_By = agent.Updated_By,
                             };
                var agentList = agents.AsNoTracking()
                    .Where(a => a.AgentId == agentId)
                    .ToList();
                if (agentList is null) { return NotFound("No agents found!"); }
                return Ok(agentList);
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
            int getlastid;
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                getlastid = _dataContext.tblO_Agent.OrderByDescending(e => e.Id).FirstOrDefault()?.Id ?? 0;

                var newAgent = new Agents
                {
                    Id = getlastid + 1,
                    Name = agentDTO.Name,
                    Gender = agentDTO.Gender,
                    DOB = agentDTO.DOB,
                    Tel_1 = agentDTO.Tel_1,
                    Tel_2 = agentDTO.Tel_2,
                    Tel_3 = agentDTO.Tel_3,
                    PositionId = agentDTO.PositionId,
                    Commission = agentDTO.Commission,
                    StaffId = agentDTO.StaffId,
                    VillageCode = agentDTO.VillageCode,
                    Active = true,
                    Created_By = agentDTO.Created_By,
                    Created_At = DateTime.Now,
                    Updated_By = agentDTO.Created_By,
                    Updated_At = DateTime.Now
                };
                var address= await _dataContext.ViewO_Address.FindAsync(agentDTO.VillageCode);
                var position = await _dataContext.tblO_Position.FindAsync(agentDTO.PositionId);
                var staff= await _dataContext.tblO_Staff.FindAsync(agentDTO.StaffId);
                var data = new { newAgent, address, position, staff };
                await _dataContext.tblO_Agent.AddAsync(newAgent);
                await _dataContext.SaveChangesAsync();
                return Ok(new { Message = "Agent added successfully!",Data=data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while adding data to the database.");
            }
        }

        [HttpPut("agent/update/{id}")]
        public async Task<IActionResult> UpdateAgent(int id, Agents agentDTO)
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
                existingAgent.Commission = agentDTO.Commission;
                existingAgent.StaffId = agentDTO.StaffId;
                existingAgent.VillageCode = agentDTO.VillageCode;
                existingAgent.Active = agentDTO.Active;
                existingAgent.Updated_By = agentDTO.Updated_By;
                existingAgent.Updated_At = DateTime.Now;

                await _dataContext.SaveChangesAsync();
                var address = await _dataContext.ViewO_Address.FindAsync(agentDTO.VillageCode);
                var position = await _dataContext.tblO_Position.FindAsync(agentDTO.PositionId);
                var staff = await _dataContext.tblO_Staff.FindAsync(agentDTO.StaffId);
                var data = new { existingAgent, address, position, staff };
                return Ok(new { Message = "Agent updated successfully!",data= data });
            }
            catch (Exception ex)
            {
                return HandleException(ex, "An error occurred while updating data in the database.");
            }
        }
    }

}
