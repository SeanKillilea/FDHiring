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
                new { InterviewId = interviewId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<InterviewAnswer?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<InterviewAnswer>("GetAnswerById",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(InterviewAnswer a)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertAnswer", new
            {
                a.InterviewId,
                a.InterviewQuestionId,
                a.Answer,
                a.AnsweredByUser
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(InterviewAnswer a)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateAnswer", new
            {
                a.Id,
                a.Answer
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("DeleteAnswer",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task LoadQuestionsForInterviewAsync(int interviewId, int positionId, int interviewTypeId, string answeredByUser)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("LoadQuestionsForInterview", new
            {
                InterviewId = interviewId,
                PositionId = positionId,
                InterviewTypeId = interviewTypeId,
                AnsweredByUser = answeredByUser
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateSortOrderAsync(int id, int sortOrder)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateAnswerSortOrder",
                new { Id = id, SortOrder = sortOrder },
                commandType: CommandType.StoredProcedure);
        }


    }
}