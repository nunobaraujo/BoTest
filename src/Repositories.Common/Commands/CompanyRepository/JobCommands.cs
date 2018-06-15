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
                await _log.WriteErrorAsync(nameof(JobCommands), nameof(GetByCustomer), null, ex);
                throw;
            }
        }
        public async Task<IJob> Get(string jobId)
        {            
            try
            {
                if (string.IsNullOrEmpty(jobId))
                    throw new ArgumentNullException(nameof(jobId));

                var query = $"SELECT {Common.GetColumnNames<IJob>()} FROM [Job] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Job>(query, new { Id = jobId }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(JobCommands), nameof(Get), jobId, ex);
                throw;
            }
        }
        public async Task<string> Add(IJob job)
        {
            try
            {
                if (job == null || job.Id == null)
                    throw new ArgumentNullException(nameof(job));
                
                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    // Validate Id
                    var jobId = await cnn.ExecuteScalarAsync(
                        "SELECT Id FROM [Job] WHERE Id=@Id", new { job.Id }, transaction);
                    if (jobId != null)
                        throw new InvalidConstraintException($"Reference already exists: {job.Id}");
                    
                    string query =
                        $"INSERT INTO [Job] ({Common.GetColumnNames<IJob>()}) VALUES ({Common.GetFieldNames<IJob>()});";
                    var res = await cnn.ExecuteAsync(query, job, transaction);
                    if (res != 1)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return job.Id;
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(JobCommands), nameof(Add), job?.ToJson(), ex);
                throw;
            }
        }
        public async Task Delete(string jobId)
        {
            try
            {
                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();                    
                    // Validate if there is history job cannot be deleted
                    var res = await cnn.ExecuteScalarAsync(
                        $"SELECT Count(Id) FROM [JobHistory] WHERE JobId = @JobId", new { JobId = jobId }, transaction);
                    if (res == null || res.ToString() != "0")
                        throw new InvalidOperationException("Job cannot be deleted.");
                    //cnn.Open();
                    await cnn.ExecuteAsync(
                        $"DELETE FROM [Job] WHERE Id = @Id", new { Id = jobId}, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(JobCommands), nameof(Delete), jobId, ex);
                throw;
            }
        }
        public async Task<string> Update(IJob job)
        {
            try
            {
                if (job == null || job.Id == null)
                    throw new ArgumentNullException(nameof(job));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    
                    string query =
                        $"UPDATE [Job] SET {Common.GetUpdateQueryFields<IJob>("Id")} WHERE Id=@Id";
                    if (await cnn.ExecuteAsync(query, job, transaction) != 1)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return job.Id;
                }
            }
            catch (Exception ex)
            {
                await _log.WriteErrorAsync(nameof(JobCommands), nameof(Update), job?.ToJson(), ex);
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
                await _log.WriteErrorAsync(nameof(JobCommands), nameof(GetByDate), query, ex);
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
                await _log.WriteErrorAsync(nameof(JobCommands), nameof(GetByCustomer), query, ex);
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
                await _log.WriteErrorAsync(nameof(JobCommands), nameof(GetByCustomerRoute), query, ex);
                throw;
            }
        }
    }
}
