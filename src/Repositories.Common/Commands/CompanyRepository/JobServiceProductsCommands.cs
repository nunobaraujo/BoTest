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
    internal class JobServiceProductsCommands : IJobServiceProductsCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public JobServiceProductsCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(IJobServiceProducts model)
        {
            return (await AddBatch(new List<IJobServiceProducts>(new IJobServiceProducts[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> AddBatch(IList<IJobServiceProducts> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    foreach (var jobServiceProducts in models)
                    {
                        // Validate Ids
                        if (jobServiceProducts.Id == null)
                            throw new ArgumentNullException(nameof(jobServiceProducts.Id));
                        var id = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [JobServiceProducts] WHERE Id=@Id", new { jobServiceProducts.Id }, transaction);
                        if (id != null)
                            throw new InvalidConstraintException($"Id already exists: {jobServiceProducts.Id}");
                    }
                    string query =
                        $"INSERT INTO [JobServiceProducts] ({Common.GetColumnNames<IJobServiceProducts>()}) VALUES ({Common.GetFieldNames<IJobServiceProducts>()});";
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
                _log?.WriteError(nameof(JobServiceProductsCommands), nameof(AddBatch), models?.ToJson(), ex);
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
                        $"DELETE FROM [JobServiceProducts] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobServiceProductsCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }

        public async Task<IJobServiceProducts> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<IJobServiceProducts>()} FROM [JobServiceProducts] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<JobServiceProducts>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobServiceProductsCommands), nameof(Get), id, ex);
                throw;
            }
        }

        public async Task<IList<IJobServiceProducts>> GetByJob(string jobId)
        {
            try
            {
                if (string.IsNullOrEmpty(jobId))
                    throw new ArgumentNullException(nameof(jobId));

                var query = $"SELECT {Common.GetColumnNamesWithTable<IJobServiceProducts>("JobServiceProducts")} FROM [JobServiceProducts] " +
                    " INNER JOIN [JobService] " +
                    " ON [JobServiceProducts].JobServiceId=[JobService].Id " +
                    " WHERE [JobService].JobId = @JobId";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<JobServiceProducts>(query, new { JobId = jobId }))
                        .Cast<IJobServiceProducts>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobServiceProductsCommands), nameof(GetByJob), jobId, ex);
                throw;
            }
        }

        public async Task<string> Update(IJobServiceProducts model)
        {
            return (await UpdateBatch(new List<IJobServiceProducts>(new IJobServiceProducts[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> UpdateBatch(IList<IJobServiceProducts> models)
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
                        $"UPDATE [JobServiceProducts] SET {Common.GetUpdateQueryFields<IJobServiceProducts>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobServiceProductsCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
        }
    }
}
