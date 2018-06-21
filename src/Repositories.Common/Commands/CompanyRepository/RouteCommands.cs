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
    internal class RouteCommands : IRouteCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public RouteCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(IRoute model)
        {
            return (await AddBatch(new List<IRoute>(new IRoute[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> AddBatch(IList<IRoute> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    foreach (var route in models)
                    {
                        // Validate Ids
                        if (route.Id == null)
                            throw new ArgumentNullException(nameof(route.Id));
                    }
                    string query =
                        $"INSERT INTO [Route] ({Common.GetColumnNames<IRoute>()}) VALUES ({Common.GetFieldNames<IRoute>()});";
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
                _log?.WriteError(nameof(RouteCommands), nameof(AddBatch), models?.ToJson(), ex);
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
                    foreach (var routeId in ids)
                    {
                        if (routeId == null)
                            throw new ArgumentNullException(nameof(routeId));
                        
                        var res = await cnn.ExecuteScalarAsync(
                            $"SELECT Count(Id) FROM [CustomerRoute] WHERE RouteId = @RouteId", new { RouteId = routeId}, transaction);
                        if (res == null || res.ToString() != "0")
                            throw new InvalidOperationException("Route cannot be deleted.");
                        
                    }

                    await cnn.ExecuteAsync(
                        $"DELETE FROM [Route] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(RouteCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }

        public async Task<IRoute> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<IRoute>()} FROM [Route] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Route>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(RouteCommands), nameof(Get), id, ex);
                throw;
            }
        }

        public async Task<IList<IRoute>> List()
        {
            try
            {
                var query = $"SELECT {Common.GetColumnNames<IRoute>()} FROM [Route]";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Route>(query))
                        .Cast<IRoute>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(RouteCommands), nameof(List), null, ex);
                throw;
            }
        }

        public async Task<string> Update(IRoute model)
        {
            return (await UpdateBatch(new List<IRoute>(new IRoute[] { model })))
                 .FirstOrDefault();
        }

        public async Task<IList<string>> UpdateBatch(IList<IRoute> models)
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
                        $"UPDATE [Route] SET {Common.GetUpdateQueryFields<IRoute>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(RouteCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
        }
    }
}
