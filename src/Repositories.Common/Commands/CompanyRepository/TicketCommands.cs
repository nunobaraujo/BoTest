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
    internal class TicketCommands : ITicketCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public TicketCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(ITicket model)
        {
            return (await AddBatch(new List<ITicket>(new ITicket[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> AddBatch(IList<ITicket> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    foreach (var ticket in models)
                    {
                        // Validate Ids
                        if (ticket.Id == null)
                            throw new ArgumentNullException(nameof(ticket.Id));
                        var id = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [Ticket] WHERE Id=@Id", new { ticket.Id }, transaction);
                        if (id != null)
                            throw new InvalidConstraintException($"Id already exists: {ticket.Id}");
                    }
                    string query =
                        $"INSERT INTO [Ticket] ({Common.GetColumnNames<ITicket>()}) VALUES ({Common.GetFieldNames<ITicket>()});";
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
                _log?.WriteError(nameof(TicketCommands), nameof(AddBatch), models?.ToJson(), ex);
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
                        $"DELETE FROM [Ticket] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(TicketCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }

        public async Task<ITicket> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<ITicket>()} FROM [Ticket] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Ticket>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(TicketCommands), nameof(Get), id, ex);
                throw;
            }
        }

        public async Task<IList<ITicket>> GetByCustomer(string customerId)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                    throw new ArgumentNullException(nameof(customerId));

                var query = $"SELECT {Common.GetColumnNames<ITicket>()} FROM [Ticket] WHERE CustomerId = @CustomerId";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Ticket>(query, new { CustomerId = customerId }))
                        .Cast<ITicket>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(TicketCommands), nameof(GetByCustomer), customerId, ex);
                throw;
            }
        }

        public async Task<IList<ITicket>> GetByCustomerRoute(string customerRouteId)
        {
            try
            {
                if (string.IsNullOrEmpty(customerRouteId))
                    throw new ArgumentNullException(nameof(customerRouteId));

                var query = $"SELECT {Common.GetColumnNames<ITicket>()} FROM [Ticket] WHERE CustomerRouteId = @CustomerRouteId";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Ticket>(query, new { CustomerRouteId = customerRouteId }))
                        .Cast<ITicket>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(TicketCommands), nameof(GetByCustomerRoute), customerRouteId, ex);
                throw;
            }
        }

        public async Task<IList<ITicket>> GetByJob(string jobId)
        {
            try
            {
                if (string.IsNullOrEmpty(jobId))
                    throw new ArgumentNullException(nameof(jobId));

                var query = $"SELECT {Common.GetColumnNames<ITicket>()} FROM [Ticket] WHERE JobId = @JobId";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Ticket>(query, new { JobId = jobId }))
                        .Cast<ITicket>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(TicketCommands), nameof(GetByJob), jobId, ex);
                throw;
            }
        }

        public async Task<string> Update(ITicket model)
        {
            return (await UpdateBatch(new List<ITicket>(new ITicket[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> UpdateBatch(IList<ITicket> models)
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
                        $"UPDATE [Ticket] SET {Common.GetUpdateQueryFields<ITicket>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(TicketCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
        }
    }
}
