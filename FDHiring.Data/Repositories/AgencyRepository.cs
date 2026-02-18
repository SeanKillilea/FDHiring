using System.Data;
using Dapper;
using FDHiring.Core.Models;

namespace FDHiring.Data.Repositories
{
    public class AgencyRepository
    {
        private readonly DbConnectionFactory _db;

        public AgencyRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Agency>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Agency>("GetAllAgencies", commandType: CommandType.StoredProcedure);
        }

        public async Task<Agency?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Agency>("GetAgencyById",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(Agency a)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertAgency",
                new { a.Name }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(Agency a)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateAgency",
                new { a.Id, a.Name }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("DeleteAgency",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }
    }
}