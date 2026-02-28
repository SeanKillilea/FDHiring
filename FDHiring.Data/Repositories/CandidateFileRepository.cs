using System.Data;
using Dapper;
using FDHiring.Core.Models;

namespace FDHiring.Data.Repositories
{
    public class CandidateFileRepository
    {
        private readonly DbConnectionFactory _db;

        public CandidateFileRepository(DbConnectionFactory db)
        {
            _db = db;
        }

        public async Task<IEnumerable<CandidateFile>> GetByCandidateIdAsync(int candidateId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryAsync<CandidateFile>("GetFilesByCandidateId",
                new { CandidateId = candidateId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<CandidateFile?> GetByIdAsync(int id)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<CandidateFile>("GetCandidateFileById",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<CandidateFile?> GetProfilePictureAsync(int candidateId)
        {
            using var conn = _db.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<CandidateFile>("GetCandidateProfilePicture",
                new { CandidateId = candidateId }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(CandidateFile f)
        {
            using var conn = _db.CreateConnection();
            return await conn.ExecuteScalarAsync<int>("InsertCandidateFile", new
            {
                f.CandidateId,
                f.FileName,
                f.FileDescription,
                f.FilePath,
                f.FileSize,
                f.IsUserPicture,
                f.UploadedByUserId
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("DeleteCandidateFile",
                new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(CandidateFile f)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("UpdateCandidateFile", new
            {
                f.Id,
                f.FileDescription,
                f.IsUserPicture
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task SetProfilePictureAsync(int id, int candidateId)
        {
            using var conn = _db.CreateConnection();
            await conn.ExecuteAsync("SetCandidateProfilePicture",
                new { Id = id, CandidateId = candidateId },
                commandType: CommandType.StoredProcedure);
        }
    }
}