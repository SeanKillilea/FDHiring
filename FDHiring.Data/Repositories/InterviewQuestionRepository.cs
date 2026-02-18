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

        public async Task<IEnumerable<InterviewQuestion>> GetByPositionAndNumberAsync(int positionId, int interviewNumber)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<InterviewQuestion>("GetInterviewQuestionsByPosition",
                new { PositionId = positionId, InterviewNumber = interviewNumber }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<InterviewQuestion>> GetAllByPositionAsync(int positionId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<InterviewQuestion>("GetAllInterviewQuestionsByPosition",
                new { PositionId = positionId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(InterviewQuestion q)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertInterviewQuestion", new
            {
                q.PositionId,
                q.InterviewNumber,
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