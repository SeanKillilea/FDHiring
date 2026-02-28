using System.Data;
using Dapper;
using FDHiring.Core.Models;

namespace FDHiring.Data.Repositories
{
    public class WorkflowStepRepository
    {
        private readonly DbConnectionFactory _db;

        public WorkflowStepRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        // Get all template steps (across all positions)
        public async Task<IEnumerable<WorkflowStep>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<WorkflowStep>("GetAllWorkflowSteps",
                commandType: CommandType.StoredProcedure);
        }

        // Get template steps for a specific position
        public async Task<IEnumerable<WorkflowStep>> GetByPositionIdAsync(int positionId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<WorkflowStep>("GetWorkflowStepsByPositionId",
                new { PositionId = positionId },
                commandType: CommandType.StoredProcedure);
        }

        // Get a single template step by Id
        public async Task<WorkflowStep?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<WorkflowStep>("GetWorkflowStepById",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        // Insert a new template step
        public async Task<int> InsertAsync(WorkflowStep step)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertWorkflowStep", new
            {
                step.PositionId,
                step.StepName,
                step.StepOrder
            }, commandType: CommandType.StoredProcedure);
        }

        // Update a template step
        public async Task UpdateAsync(WorkflowStep step)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateWorkflowStep", new
            {
                step.Id,
                step.StepName,
                step.StepOrder
            }, commandType: CommandType.StoredProcedure);
        }

        // Delete a template step
        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("DeleteWorkflowStep",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }
    }
}