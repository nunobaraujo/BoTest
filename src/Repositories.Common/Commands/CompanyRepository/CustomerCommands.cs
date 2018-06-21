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
    internal class CustomerCommands: ICustomerCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public CustomerCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<IList<ICustomer>> List()
        {
            try
            {
                var query = $"SELECT {Common.GetColumnNames<ICustomer>()} FROM [Customer]";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Customer>(query))
                        .Cast<ICustomer>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CustomerCommands), nameof(List), null, ex);
                throw;
            }
        }
        public async Task<ICustomer> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<ICustomer>()} FROM [Customer] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Customer>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CustomerCommands), nameof(Get), id, ex);
                throw;
            }
        }
        public async Task<string> Add(ICustomer model)
        {
            return (await AddBatch(new List<ICustomer>(new ICustomer[] { model })))
                .FirstOrDefault();
        }
        public async Task Delete(string id)
        {
            await DeleteBatch(new List<string>(new string[] { id }));
        }
        public async Task<string> Update(ICustomer model)
        {
            return (await UpdateBatch(new List<ICustomer>(new ICustomer[] { model })))
                 .FirstOrDefault();
        }

        public async Task<IList<string>> AddBatch(IList<ICustomer> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    
                    foreach (var customer in models)
                    {
                        // Validate Ids
                        if (customer.Id == null)
                            throw new ArgumentNullException(nameof(customer.Id));
                        if (customer.TaxIdNumber == null)
                            throw new ArgumentNullException(nameof(customer.TaxIdNumber));
                        var customerId = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [Customer] WHERE Id=@Id", new { customer.Id }, transaction);
                        if (customerId != null)
                            throw new InvalidConstraintException($"Id already exists: {customer.Id}");
                        var customerTIN = await cnn.ExecuteScalarAsync(
                            "SELECT TaxIdNumber FROM [Customer] WHERE TaxIdNumber=@TaxIdNumber", new { customer.TaxIdNumber }, transaction);
                        if (customerTIN != null)
                            throw new InvalidConstraintException($"TaxIdNumber already exists: {customer.TaxIdNumber}");
                    }
                    string query =
                        $"INSERT INTO [Customer] ({Common.GetColumnNames<ICustomer>()}) VALUES ({Common.GetFieldNames<ICustomer>()});";
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
                _log?.WriteError(nameof(CustomerCommands), nameof(AddBatch), models?.ToJson(), ex);
                throw;
            }
        }
        public async Task<IList<string>> UpdateBatch(IList<ICustomer> models)
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
                        $"UPDATE [Customer] SET {Common.GetUpdateQueryFields<ICustomer>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CustomerCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
        }
        public async Task DeleteBatch(IList<string> modelIds)
        {
            try
            {
                if (modelIds == null || modelIds.Count <= 0)
                    throw new ArgumentNullException(nameof(modelIds));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    foreach (var customerId in modelIds)
                    {
                        if (customerId == null)
                            throw new ArgumentNullException(nameof(customerId));
                        // Validate if there are jobs cannot be deleted
                        var res = await cnn.ExecuteScalarAsync(
                            $"SELECT Count(Id) FROM [Job] WHERE CustomerId = @CustomerId", new { CustomerId = customerId }, transaction);
                        if (res == null || res.ToString() != "0")
                            throw new InvalidOperationException("Customer cannot be deleted.");
                        // Validate if there are documents cannot be deleted
                        res = await cnn.ExecuteScalarAsync(
                            $"SELECT Count(Id) FROM [Document] WHERE CustomerId = @CustomerId", new { CustomerId = customerId }, transaction);
                        if (res == null || res.ToString() != "0")
                            throw new InvalidOperationException("Customer cannot be deleted.");
                    }

                    await cnn.ExecuteAsync(
                        $"DELETE FROM [Customer] WHERE Id = @Id", new { Id = modelIds }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CustomerCommands), nameof(DeleteBatch), modelIds.ToJson(), ex);
                throw;
            }
        }

    }
}
