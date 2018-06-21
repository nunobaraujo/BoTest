using Contracts;
using Contracts.Models;
using Core.Json;
using Core.Repositories.Commands.CompanyRepository;
using Dapper;
using NBsoft.Logs.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Common.Commands.CompanyRepository
{
    internal class JobHistoryCommands : IJobHistoryCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public JobHistoryCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(IJobHistory model)
        {
            return (await AddBatch(new List<IJobHistory>(new IJobHistory[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> AddBatch(IList<IJobHistory> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    foreach (var jobHistory in models)
                    {
                        // Validate Ids
                        if (jobHistory.Id == null)
                            throw new ArgumentNullException(nameof(jobHistory.Id));
                        var id = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [JobHistory] WHERE Id=@Id", new { jobHistory.Id }, transaction);
                        if (id != null)
                            throw new InvalidConstraintException($"Id already exists: {jobHistory.Id}");
                    }
                    string query =
                        $"INSERT INTO [JobHistory] ({Common.GetColumnNames<IJobHistory>()}) VALUES ({Common.GetFieldNames<IJobHistory>()});";
                    var res = await cnn.ExecuteAsync(query, models, transaction);
                    if (res != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();

                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobHistoryCommands), nameof(AddBatch), models?.ToJson(), ex);
                throw;
            }
        }

        public async Task Delete(string id)
        {
            await DeleteBatch(new List<string>(new string[] { id }));
        }

        public async Task DeleteBatch(IList<string> ids)
        {
            try
            {
                if (ids == null || ids.Count <= 0)
                    throw new ArgumentNullException(nameof(ids));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    foreach (var id in ids)
                    {
                        if (id == null)
                            throw new ArgumentNullException(nameof(id));
                    }

                    await cnn.ExecuteAsync(
                        $"DELETE FROM [JobHistory] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobHistoryCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }

        public async Task<IJobHistory> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<IJobHistory>()} FROM [JobHistory] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<JobHistory>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobHistoryCommands), nameof(Get), id, ex);
                throw;
            }
        }

        public async Task<IList<IJobHistory>> GetByJob(string jobId)
        {
            try
            {
                if (string.IsNullOrEmpty(jobId))
                    throw new ArgumentNullException(nameof(jobId));

                var query = $"SELECT {Common.GetColumnNames<IJobHistory>()} FROM [JobHistory] WHERE JobId = @JobId";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<JobHistory>(query, new { JobId = jobId }))
                        .Cast<IJobHistory>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobHistoryCommands), nameof(Get), jobId, ex);
                throw;
            }
        }

        public async Task<string> Update(IJobHistory model)
        {
            return (await UpdateBatch(new List<IJobHistory>(new IJobHistory[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> UpdateBatch(IList<IJobHistory> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    string query =
                        $"UPDATE [JobHistory] SET {Common.GetUpdateQueryFields<IJobHistory>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobHistoryCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
        }
    }
}
