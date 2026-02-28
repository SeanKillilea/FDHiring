using System.Data;
using Dapper;
using FDHiring.Core.Models;

namespace FDHiring.Data.Repositories
{
    public class WorkflowRepository
    {
        private readonly DbConnectionFactory _db;

        public WorkflowRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        // Get all workflow steps for a candidate+position combo
        public async Task<IEnumerable<Workflow>> GetByCandidateAndPositionAsync(int candidateId, int positionId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Workflow>("GetWorkflowByCandidateAndPosition",
                new { CandidateId = candidateId, PositionId = positionId },
                commandType: CommandType.StoredProcedure);
        }

        // Get a single workflow step by Id
        public async Task<Workflow?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Workflow>("GetWorkflowById",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        // Insert a single workflow step
        public async Task<int> InsertAsync(Workflow w)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertWorkflow", new
            {
                w.CandidateId,
                w.PositionId,
                w.StartDate,
                w.Owner,
                w.StepName,
                w.StepOrder,
                w.Complete
            }, commandType: CommandType.StoredProcedure);
        }

        // Add all template steps from WorkflowSteps for a position
        public async Task InsertAllStepsForPositionAsync(int candidateId, int positionId)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("InsertAllWorkflowStepsForPosition",
                new { CandidateId = candidateId, PositionId = positionId },
                commandType: CommandType.StoredProcedure);
        }

        // Toggle complete flag on a step
        public async Task ToggleCompleteAsync(int id, bool complete)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("ToggleWorkflowComplete",
                new { Id = id, Complete = complete },
                commandType: CommandType.StoredProcedure);
        }

        // Update a workflow step (owner, start date, step name, order)
        public async Task UpdateAsync(Workflow w)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateWorkflow", new
            {
                w.Id,
                w.StartDate,
                w.Owner,
                w.StepName,
                w.StepOrder,
                w.Complete
            }, commandType: CommandType.StoredProcedure);
        }

        // Delete a single workflow step
        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("DeleteWorkflow",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        // Delete all workflow steps for a candidate+position
        public async Task DeleteByCandidateAndPositionAsync(int candidateId, int positionId)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("DeleteWorkflowByCandidateAndPosition",
                new { CandidateId = candidateId, PositionId = positionId },
                commandType: CommandType.StoredProcedure);
        }


        public async Task UpdateStepOrderAsync(int id, int stepOrder)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateWorkflowStepOrder",
                new { Id = id, StepOrder = stepOrder },
                commandType: CommandType.StoredProcedure);
        }
    }
}