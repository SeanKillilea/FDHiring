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

        public async Task<IEnumerable<WorkflowStep>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<WorkflowStep>("GetAllWorkflowSteps", commandType: CommandType.StoredProcedure);
        }

        public async Task<WorkflowStep?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<WorkflowStep>("GetWorkflowStepById",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }
    }
}