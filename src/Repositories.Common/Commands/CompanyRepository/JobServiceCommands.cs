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
    internal class JobServiceCommands: IJobServiceCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public JobServiceCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(IJobService model)
        {
            return (await AddBatch(new List<IJobService>(new IJobService[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> AddBatch(IList<IJobService> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    foreach (var jobService in models)
                    {
                        // Validate Ids
                        if (jobService.Id == null)
                            throw new ArgumentNullException(nameof(jobService.Id));
                        var id = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [JobService] WHERE Id=@Id", new { jobService.Id }, transaction);
                        if (id != null)
                            throw new InvalidConstraintException($"Id already exists: {jobService.Id}");
                    }
                    string query =
                        $"INSERT INTO [JobService] ({Common.GetColumnNames<IJobService>()}) VALUES ({Common.GetFieldNames<IJobService>()});";
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
                _log?.WriteError(nameof(JobServiceCommands), nameof(AddBatch), models?.ToJson(), ex);
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
                        $"DELETE FROM [JobService] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobServiceCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }

        public async Task<IJobService> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<IJobService>()} FROM [JobService] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<JobService>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobServiceCommands), nameof(Get), id, ex);
                throw;
            }
        }

        public async Task<IList<IJobService>> GetByJob(string jobId)
        {
            try
            {
                if (string.IsNullOrEmpty(jobId))
                    throw new ArgumentNullException(nameof(jobId));

                var query = $"SELECT {Common.GetColumnNames<IJobService>()} FROM [JobService] WHERE JobId = @JobId";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<JobService>(query, new { JobId = jobId }))
                        .Cast<IJobService>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobServiceCommands), nameof(GetByJob), jobId, ex);
                throw;
            }
        }

        public async Task<IList<IJobService>> GetOngoing()
        {
            try
            {
                var query = $"SELECT {Common.GetColumnNames<IJobService>()} FROM [JobService] WHERE FinishDate IS NULL";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<JobService>(query))
                        .Cast<IJobService>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobServiceCommands), nameof(GetOngoing), null, ex);
                throw;
            }
        }

        public async Task<string> Update(IJobService model)
        {
            return (await UpdateBatch(new List<IJobService>(new IJobService[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> UpdateBatch(IList<IJobService> models)
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
                        $"UPDATE [JobService] SET {Common.GetUpdateQueryFields<IJobService>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobServiceCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
        }
    }
}
