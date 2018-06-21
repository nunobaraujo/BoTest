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
    internal class FrontTerminalCommands: IFrontTerminalCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public FrontTerminalCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(IFrontTerminal model)
        {
            return (await AddBatch(new List<IFrontTerminal>(new IFrontTerminal[] { model })))
                .FirstOrDefault();
        }

        public async  Task<IList<string>> AddBatch(IList<IFrontTerminal> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    foreach (var frontTerminal in models)
                    {
                        // Validate Ids
                        if (frontTerminal.Id == null)
                            throw new ArgumentNullException(nameof(frontTerminal.Id));
                        var id = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [FrontTerminal] WHERE Id=@Id", new { frontTerminal.Id }, transaction);
                        if (id != null)
                            throw new InvalidConstraintException($"Id already exists: {frontTerminal.Id}");
                        var tid = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [FrontTerminal] WHERE TerminalId=@TerminalId", new { frontTerminal.TerminalId}, transaction);
                        if (tid != null)
                            throw new InvalidConstraintException($"TerminalId already exists: {frontTerminal.TerminalId}");
                    }
                    string query =
                        $"INSERT INTO [FrontTerminal] ({Common.GetColumnNames<IFrontTerminal>()}) VALUES ({Common.GetFieldNames<IFrontTerminal>()});";
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
                _log?.WriteError(nameof(FrontTerminalCommands), nameof(AddBatch), models?.ToJson(), ex);
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
                        $"DELETE FROM [FrontTerminal] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(FrontTerminalCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }

        public async Task<IFrontTerminal> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<IFrontTerminal>()} FROM [FrontTerminal] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<FrontTerminal>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(FrontTerminalCommands), nameof(Get), id, ex);
                throw;
            }
        }

        public async Task<string> Update(IFrontTerminal model)
        {
            return (await UpdateBatch(new List<IFrontTerminal>(new IFrontTerminal[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> UpdateBatch(IList<IFrontTerminal> models)
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
                        $"UPDATE [FrontTerminal] SET {Common.GetUpdateQueryFields<IFrontTerminal>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(FrontTerminalCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
        }
    }
}
