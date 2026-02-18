using System.Data;
using Dapper;
using FDHiring.Core.Models;

namespace FDHiring.Data.Repositories
{
    public class WorkflowHistoryRepository
    {
        private readonly DbConnectionFactory _db;

        public WorkflowHistoryRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<WorkflowHistory>> GetByWorkflowIdAsync(int workflowId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<WorkflowHistory>("GetWorkflowHistoryByWorkflowId",
                new { WorkflowId = workflowId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(WorkflowHistory wh)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertWorkflowHistory", new
            {
                wh.WorkflowId,
                wh.StepId,
                wh.CompletedByUserId,
                wh.Notes
            }, commandType: CommandType.StoredProcedure);
        }
    }
}