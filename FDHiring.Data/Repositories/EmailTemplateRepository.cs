using System.Data;
using Dapper;
using FDHiring.Core.Models;

namespace FDHiring.Data.Repositories
{
    public class EmailTemplateRepository
    {
        private readonly DbConnectionFactory _db;

        public EmailTemplateRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<EmailTemplate>> GetAllAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<EmailTemplate>("GetAllEmailTemplates", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<EmailTemplate>> GetActiveAsync()
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<EmailTemplate>("GetActiveEmailTemplates", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<EmailTemplate>> GetByPositionAsync(int positionId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<EmailTemplate>("GetEmailTemplatesByPosition",
                new { PositionId = positionId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<EmailTemplate?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<EmailTemplate>("GetEmailTemplateById",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(EmailTemplate et)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertEmailTemplate", new
            {
                et.Name,
                et.Subject,
                et.Body,
                et.PositionId,
                et.Active,
                et.CreatedByUserId
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(EmailTemplate et)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateEmailTemplate", new
            {
                et.Id,
                et.Name,
                et.Subject,
                et.Body,
                et.PositionId,
                et.Active
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("DeleteEmailTemplate",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }
    }
}