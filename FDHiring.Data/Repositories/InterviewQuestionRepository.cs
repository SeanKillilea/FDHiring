using System.Data;
using Dapper;
using FDHiring.Core.Models;

namespace FDHiring.Data.Repositories
{
    public class InterviewQuestionRepository
    {
        private readonly DbConnectionFactory _db;

        public InterviewQuestionRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<InterviewQuestion>> GetByPositionAndTypeAsync(int positionId, int interviewTypeId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<InterviewQuestion>("GetInterviewQuestionsByPositionAndType",
                new { PositionId = positionId, InterviewTypeId = interviewTypeId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<InterviewQuestion?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<InterviewQuestion>("GetInterviewQuestionById",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(InterviewQuestion q)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertInterviewQuestion", new
            {
                q.PositionId,
                q.InterviewTypeId,
                q.Question,
                q.SortOrder
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(InterviewQuestion q)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateInterviewQuestion", new
            {
                q.Id,
                q.Question,
                q.SortOrder
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("DeleteInterviewQuestion",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }
    }
}