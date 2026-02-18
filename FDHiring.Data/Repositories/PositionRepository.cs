using System.Data;
using Dapper;
using FDHiring.Core.Models;

namespace FDHiring.Data.Repositories
{
    public class PositionRepository
    {
        private readonly DbConnectionFactory _db;

        public PositionRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Position>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<Position>("GetAllPositions", commandType: CommandType.StoredProcedure);
        }

        public async Task<Position?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Position>("GetPositionById",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(Position p)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertPosition",
                new { p.Name }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(Position p)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdatePosition",
                new { p.Id, p.Name }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("DeletePosition",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }
    }
}