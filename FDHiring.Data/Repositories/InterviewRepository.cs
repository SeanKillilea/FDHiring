using System.Data;
using Dapper;
using FDHiring.Core.Models;

namespace FDHiring.Data.Repositories
{
    public class InterviewRepository
    {
        private readonly DbConnectionFactory _db;

        public InterviewRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Interview>> GetByCandidateIdAsync(int candidateId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Interview>("GetInterviewsByCandidateId",
                new { CandidateId = candidateId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<Interview?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Interview>("GetInterviewById",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(Interview i)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertInterview", new
            {
                i.CandidateId,
                i.WorkflowId,
                i.InterviewNumber,
                i.InterviewedByUserId,
                i.ScheduledDate,
                i.Status,
                i.Notes
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(Interview i)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateInterview", new
            {
                i.Id,
                i.InterviewedByUserId,
                i.ScheduledDate,
                i.CompletedDate,
                i.Status,
                i.Notes
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("DeleteInterview",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }
    }
}