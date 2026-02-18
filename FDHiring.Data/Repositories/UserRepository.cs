using System.Data;
using Dapper;
using FDHiring.Core.Models;

namespace FDHiring.Data.Repositories
{
    public class UserRepository
    {
        private readonly DbConnectionFactory _db;

        public UserRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<User>("GetAllUsers", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<User>> GetActiveAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<User>("GetActiveUsers", commandType: CommandType.StoredProcedure);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<User>("GetUserById",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(User u)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertUser", new
            {
                u.FirstName,
                u.LastName,
                u.Email,
                u.Active
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(User u)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateUser", new
            {
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email,
                u.Active
            }, commandType: CommandType.StoredProcedure);
        }
    }
}