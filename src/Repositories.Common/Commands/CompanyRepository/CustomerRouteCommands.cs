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
    internal class CustomerRouteCommands : ICustomerRouteCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public CustomerRouteCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(ICustomerRoute model)
        {
            return (await AddBatch(new List<ICustomerRoute>(new ICustomerRoute[] { model })))
                .FirstOrDefault();
        }
        public async Task Delete(string id)
        {
            await DeleteBatch(new List<string>(new string[] { id }));
        }
        public async Task<ICustomerRoute> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<ICustomerRoute>()} FROM [CustomerRoute] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<CustomerRoute>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CustomerRouteCommands), nameof(Get), id, ex);
                throw;
            }
        }
        public async Task<IList<ICustomerRoute>> GetByCustomer(string customerId)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                    throw new ArgumentNullException(nameof(customerId));

                var query = $"SELECT {Common.GetColumnNames<ICustomerRoute>()} FROM [CustomerRoute] WHERE CustomerId = @CustomerId";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<CustomerRoute>(query, new { CustomerId = customerId }))
                        .Cast<ICustomerRoute>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CustomerRouteCommands), nameof(GetByCustomer), customerId, ex);
                throw;
            }
        }
        public async Task<string> Update(ICustomerRoute model)
        {
            return (await UpdateBatch(new List<ICustomerRoute>(new ICustomerRoute[] { model })))
                 .FirstOrDefault();
        }

        public async Task<IList<string>> AddBatch(IList<ICustomerRoute> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    
                    foreach (var customerRoute in models)
                    {
                        // Validate Ids
                        if (customerRoute.Id == null)
                            throw new ArgumentNullException(nameof(customerRoute.Id));
                        var id = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [CustomerRoute] WHERE Id=@Id", new { customerRoute.Id }, transaction);
                        if (id != null)
                            throw new InvalidConstraintException($"Id already exists: {customerRoute.Id}");
                    }
                    string query =
                        $"INSERT INTO [CustomerRoute] ({Common.GetColumnNames<ICustomerRoute>()}) VALUES ({Common.GetFieldNames<ICustomerRoute>()});";
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
                _log?.WriteError(nameof(CustomerRouteCommands), nameof(AddBatch), models?.ToJson(), ex);
                throw;
            }
        }
        public async Task<IList<string>> UpdateBatch(IList<ICustomerRoute> models)
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
                        $"UPDATE [CustomerRoute] SET {Common.GetUpdateQueryFields<ICustomerRoute>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CustomerRouteCommands), nameof(UpdateBatch), models?.ToJson(), ex);
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
                    foreach (var customerRouteId in ids)
                    {
                        if (customerRouteId == null)
                            throw new ArgumentNullException(nameof(customerRouteId));
                    }
                    await cnn.ExecuteAsync(
                        $"DELETE FROM [CustomerRoute] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(CustomerRouteCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }
    }
}
