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
using System.Threading.Tasks;

namespace Repositories.Common.Commands.CompanyRepository
{
    internal class JobOptionsJobCommands : IJobOptionsJobCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public JobOptionsJobCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(IJobOptionsJob model)
        {
            return (await AddBatch(new List<IJobOptionsJob>(new IJobOptionsJob[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> AddBatch(IList<IJobOptionsJob> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    foreach (var jobOptionsJob in models)
                    {
                        // Validate Ids
                        if (jobOptionsJob.Id == null)
                            throw new ArgumentNullException(nameof(jobOptionsJob.Id));
                        var id = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [JobOptionsJob] WHERE Id=@Id", new { jobOptionsJob.Id }, transaction);
                        if (id != null)
                            throw new InvalidConstraintException($"Id already exists: {jobOptionsJob.Id}");
                        var nid = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [JobOptionsJob] WHERE JobId=@JobId AND JobOptionsId=@JobOptionsId", new { jobOptionsJob.JobId, jobOptionsJob.JobOptionsId }, transaction);
                        if (nid != null)
                            throw new InvalidConstraintException($"Id already exists: JobId={jobOptionsJob.JobId} JobOptionsId={jobOptionsJob.JobOptionsId}");
                    }
                    string query =
                        $"INSERT INTO [JobOptionsJob] ({Common.GetColumnNames<IJobOptionsJob>()}) VALUES ({Common.GetFieldNames<IJobOptionsJob>()});";
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
                _log?.WriteError(nameof(JobOptionsJobCommands), nameof(AddBatch), models?.ToJson(), ex);
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
                        $"DELETE FROM [JobOptionsJob] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobOptionsJobCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }

        public async Task<IJobOptionsJob> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<IJobOptionsJob>()} FROM [JobOptionsJob] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<JobOptionsJob>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobOptionsJobCommands), nameof(Get), id, ex);
                throw;
            }
        }

        public async Task<IList<IJobOptionsJob>> GetByJob(string jobId)
        {
            try
            {
                if (string.IsNullOrEmpty(jobId))
                    throw new ArgumentNullException(nameof(jobId));

                var query = $"SELECT {Common.GetColumnNames<IJobOptionsJob>()} FROM [JobOptionsJob] WHERE JobId = @JobId";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<JobOptionsJob>(query, new { JobId = jobId }))
                        .Cast<IJobOptionsJob>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobOptionsJobCommands), nameof(GetByJob), jobId, ex);
                throw;
            }
        }

        public async Task<string> Update(IJobOptionsJob model)
        {
            return (await UpdateBatch(new List<IJobOptionsJob>(new IJobOptionsJob[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> UpdateBatch(IList<IJobOptionsJob> models)
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
                        $"UPDATE [JobOptionsJob] SET {Common.GetUpdateQueryFields<IJobOptionsJob>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobOptionsJobCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
        }
    }
}
