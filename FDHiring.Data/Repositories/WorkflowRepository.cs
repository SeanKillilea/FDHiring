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

        public async Task<Workflow?> GetByCandidateIdAsync(int candidateId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Workflow>("GetWorkflowByCandidateId",
                new { CandidateId = candidateId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<Workflow?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Workflow>("GetWorkflowById",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Workflow>> GetActiveAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Workflow>("GetActiveWorkflows", commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(Workflow w)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertWorkflow", new
            {
                w.CandidateId,
                w.PositionId,
                w.CurrentStepId
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateStepAsync(int id, int currentStepId)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateWorkflowStep",
                new { Id = id, CurrentStepId = currentStepId }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeactivateAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("DeactivateWorkflow",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }
    }
}