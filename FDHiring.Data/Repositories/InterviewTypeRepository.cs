using System.Data;
using Dapper;
using FDHiring.Core.Models;

namespace FDHiring.Data.Repositories
{
    public class InterviewTypeRepository
    {
        private readonly DbConnectionFactory _db;

        public InterviewTypeRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<InterviewType>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<InterviewType>("GetAllInterviewTypes",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<InterviewType?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<InterviewType>("GetInterviewTypeById",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }
    }
}