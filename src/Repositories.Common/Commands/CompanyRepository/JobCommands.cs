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
    internal class JobCommands: IJobCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;        
        private readonly ILogger _log;

        public JobCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<IList<IJob>> List()
        {
            try
            {
                var query = $"SELECT {Common.GetColumnNames<IJob>()} FROM [Job]";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Job>(query))
                        .Cast<IJob>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobCommands), nameof(List), null, ex);
                throw;
            }
        }
        public async Task<IJob> Get(string id)
        {            
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<IJob>()} FROM [Job] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Job>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobCommands), nameof(Get), id, ex);
                throw;
            }
        }
        public async Task<string> Add(IJob model)
        {
            return (await AddBatch(new List<IJob>(new IJob[] { model })))
                .FirstOrDefault();
        }
        public async Task Delete(string id)
        {
            await DeleteBatch(new List<string>(new string[] { id }));
        }
        public async Task<string> Update(IJob model)
        {
            return (await UpdateBatch(new List<IJob>(new IJob[] { model })))
                 .FirstOrDefault();
        }

        public async Task<IList<string>> AddBatch(IList<IJob> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    
                    foreach (var job in models)
                    {
                        // Validate Ids
                        if (job.Id == null)
                            throw new ArgumentNullException(nameof(job.Id));
                        var jobId = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [Job] WHERE Id=@Id", new { job.Id }, transaction);
                        if (jobId != null)
                            throw new InvalidConstraintException($"Id already exists: {job.Id}");
                    }
                    string query =
                        $"INSERT INTO [Job] ({Common.GetColumnNames<IJob>()}) VALUES ({Common.GetFieldNames<IJob>()});";
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
                _log?.WriteError(nameof(JobCommands), nameof(AddBatch), models?.ToJson(), ex);
                throw;
            }
        }
        public async Task<IList<string>> UpdateBatch(IList<IJob> models)
        {
            try
            {
                if (models == null || models.Count<=0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    string query =
                        $"UPDATE [Job] SET {Common.GetUpdateQueryFields<IJob>("Id")} WHERE Id=@Id";
                    
                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
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
                    foreach (var jobId in ids)
                    {
                        if (jobId == null)
                            throw new ArgumentNullException(nameof(jobId));
                        // Validate if there is history job cannot be deleted
                        var res = await cnn.ExecuteScalarAsync(
                            $"SELECT Count(Id) FROM [JobHistory] WHERE JobId = @JobId", new { JobId = jobId }, transaction);
                        if (res == null || res.ToString() != "0")
                            throw new InvalidOperationException("Job cannot be deleted.");
                    }

                    //cnn.Open();
                    await cnn.ExecuteAsync(
                        $"DELETE FROM [Job] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }

        public async Task<IList<IJob>> GetByDate(DateTime from, DateTime to)
        {
            var query = $"SELECT {Common.GetColumnNames<IJob>()} FROM [Job] WHERE CreationDate >= @DateFrom AND CreationDate <= @DateTo";
            try
            {
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Job>(query, new { DateFrom = from, DateTo = to }))
                        .Cast<IJob>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobCommands), nameof(GetByDate), query, ex);
                throw;
            }
        }
        public async Task<IList<IJob>> GetByCustomer(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
                throw new ArgumentNullException(nameof(customerId));
            var query = $"SELECT {Common.GetColumnNames<IJob>()} FROM [Job] WHERE CustomerId = @CustomerId ";
            try
            {
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Job>(query, new { CustomerId = customerId }))
                        .Cast<IJob>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobCommands), nameof(GetByCustomer), query, ex);
                throw;
            }
        }                
        public async Task<IList<IJob>> GetByCustomerRoute(string customerRouteId)
        {
            if (string.IsNullOrEmpty(customerRouteId))
                throw new ArgumentNullException(nameof(customerRouteId));

            var query = $"SELECT {Common.GetColumnNames<IJob>()} FROM [Job] WHERE CustomerRouteId = @CustomerRouteId";
            try
            {
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Job>(query, new { CustomerRouteId = customerRouteId }))
                        .Cast<IJob>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(JobCommands), nameof(GetByCustomerRoute), query, ex);
                throw;
            }
        }
                
    }
}
