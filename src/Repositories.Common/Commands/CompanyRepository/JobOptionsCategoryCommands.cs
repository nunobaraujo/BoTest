﻿using Contracts;
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
    internal class JobOptionsCategoryCommands : IJobOptionsCategoryCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public JobOptionsCategoryCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(IJobOptionsCategory model)
        {
            return (await AddBatch(new List<IJobOptionsCategory>(new IJobOptionsCategory[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> AddBatch(IList<IJobOptionsCategory> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    foreach (var jobOptionsCategory in models)
                    {
                        // Validate Ids
                        if (jobOptionsCategory.Id == null)
                            throw new ArgumentNullException(nameof(jobOptionsCategory.Id));
                        var id = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [JobOptionsCategory] WHERE Id=@Id", new { jobOptionsCategory.Id }, transaction);
                        if (id != null)
                            throw new InvalidConstraintException($"Id already exists: {jobOptionsCategory.Id}");
                    }
                    string query =
                        $"INSERT INTO [JobOptionsCategory] ({Common.GetColumnNames<IJobOptionsCategory>()}) VALUES ({Common.GetFieldNames<IJobOptionsCategory>()});";
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
                _log?.WriteError(nameof(JobOptionsCategoryCommands), nameof(AddBatch), models?.ToJson(), ex);
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
                        $"DELETE FROM [JobOptionsCategory] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobOptionsCategoryCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }

        public async Task<IJobOptionsCategory> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<IJobOptionsCategory>()} FROM [JobOptionsCategory] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<JobOptionsCategory>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobOptionsCategoryCommands), nameof(Get), id, ex);
                throw;
            }
        }

        public async Task<string> Update(IJobOptionsCategory model)
        {
            return (await UpdateBatch(new List<IJobOptionsCategory>(new IJobOptionsCategory[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> UpdateBatch(IList<IJobOptionsCategory> models)
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
                        $"UPDATE [JobOptionsCategory] SET {Common.GetUpdateQueryFields<IJobOptionsCategory>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobOptionsCategoryCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
        }
    }
}
