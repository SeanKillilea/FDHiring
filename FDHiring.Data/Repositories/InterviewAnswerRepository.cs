using System.Data;
using Dapper;
using FDHiring.Core.Models;

namespace FDHiring.Data.Repositories
{
    public class InterviewAnswerRepository
    {
        private readonly DbConnectionFactory _db;

        public InterviewAnswerRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<InterviewAnswer>> GetByInterviewIdAsync(int interviewId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<InterviewAnswer>("GetAnswersByInterviewId",
                new { InterviewId = interviewId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(InterviewAnswer a)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertInterviewAnswer", new
            {
                a.InterviewId,
                a.InterviewQuestionId,
                a.Answer,
                a.AnsweredByUserId
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(InterviewAnswer a)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateInterviewAnswer", new
            {
                a.Id,
                a.Answer,
                a.AnsweredByUserId
            }, commandType: CommandType.StoredProcedure);
        }
    }
}