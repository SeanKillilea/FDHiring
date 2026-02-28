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

        public async Task<IEnumerable<Interview>> GetByCandidateAndPositionAsync(int candidateId, int positionId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Interview>("GetInterviewsByCandidateAndPosition",
                new { CandidateId = candidateId, PositionId = positionId },
                commandType: CommandType.StoredProcedure);
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
                i.PositionId,
                i.InterviewTypeId,
                i.Owner,
                i.ScheduledDate,
                i.CandidateGo,
                i.Notes
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(Interview i)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateInterview", new
            {
                i.Id,
                i.InterviewTypeId,
                i.Owner,
                i.ScheduledDate,
                i.CandidateGo,
                i.Notes
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task ToggleGoAsync(int id, bool candidateGo)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("ToggleInterviewGo",
                new { Id = id, CandidateGo = candidateGo },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("DeleteInterview",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteByCandidateAndPositionAsync(int candidateId, int positionId)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("DeleteInterviewsByCandidateAndPosition",
                new { CandidateId = candidateId, PositionId = positionId },
                commandType: CommandType.StoredProcedure);
        }
    }
}