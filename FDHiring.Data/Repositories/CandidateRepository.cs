using System.Data;
using Dapper;
using FDHiring.Core.Models;

namespace FDHiring.Data.Repositories
{
    public class CandidateRepository
    {
        private readonly DbConnectionFactory _db;

        public CandidateRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Candidate>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Candidate>("GetAllCandidates", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Candidate>> GetActiveAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Candidate>("GetActiveCandidates", commandType: CommandType.StoredProcedure);
        }

        public async Task<Candidate?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Candidate>("GetCandidateById",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Candidate>> GetByPositionAsync(int positionId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Candidate>("GetCandidatesByPosition",
                new { PositionId = positionId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Candidate>> GetByAgencyAsync(int agencyId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Candidate>("GetCandidatesByAgency",
                new { AgencyId = agencyId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Candidate>> SearchAsync(string searchTerm)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Candidate>("SearchCandidates",
                new { SearchTerm = searchTerm }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(Candidate c)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertCandidate", new
            {
                c.FirstName,
                c.LastName,
                c.Email,
                c.Phone,
                c.LinkedIn,
                c.ImagePath,
                c.PositionId,
                c.AgencyId,
                c.DateFound,
                c.WouldHire,
                c.Active,
                c.IsCurrent,
                c.Notes,
                c.LastUpdatedByUserId
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(Candidate c)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateCandidate", new
            {
                c.Id,
                c.FirstName,
                c.LastName,
                c.Email,
                c.Phone,
                c.LinkedIn,
                c.ImagePath,
                c.PositionId,
                c.AgencyId,
                c.DateFound,
                c.WouldHire,
                c.Active,
                c.IsCurrent,
                c.Notes,
                c.LastUpdatedByUserId
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("DeleteCandidate",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }
    }
}