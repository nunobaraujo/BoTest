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
    internal class DocumentLineCommands : IDocumentLineCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public DocumentLineCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(IDocumentLine model)
        {
            return (await AddBatch(new List<IDocumentLine>(new IDocumentLine[] { model })))
                .FirstOrDefault();
        }

        public async  Task<IList<string>> AddBatch(IList<IDocumentLine> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    foreach (var documentLine in models)
                    {
                        // Validate Ids
                        if (documentLine.Id == null)
                            throw new ArgumentNullException(nameof(documentLine.Id));
                        var id = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [DocumentLines] WHERE Id=@Id", new { documentLine.Id }, transaction);
                        if (id != null)
                            throw new InvalidConstraintException($"Id already exists: {documentLine.Id}");
                    }
                    string query =
                        $"INSERT INTO [DocumentLines] ({Common.GetColumnNames<IDocumentLine>()}) VALUES ({Common.GetFieldNames<IDocumentLine>()});";
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
                _log?.WriteError(nameof(DocumentLineCommands), nameof(AddBatch), models?.ToJson(), ex);
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
                    foreach (var documentLineId in ids)
                    {
                        if (documentLineId == null)
                            throw new ArgumentNullException(nameof(documentLineId));
                    }

                    await cnn.ExecuteAsync(
                        $"DELETE FROM [DocumentLines] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentLineCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }

        public async Task<IDocumentLine> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<IDocumentLine>()} FROM [DocumentLines] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<DocumentLine>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentLineCommands), nameof(Get), id, ex);
                throw;
            }
        }

        public async Task<IList<IDocumentLine>> GetByDocument(string docId)
        {
            if (string.IsNullOrEmpty(docId))
                throw new ArgumentNullException(nameof(docId));
            var query = $"SELECT {Common.GetColumnNames<IDocumentLine>()} FROM [DocumentLines] WHERE DocumentId = @DocumentId";
            try
            {
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<DocumentLine>(query, new { DocumentId = docId }))
                        .Cast<IDocumentLine>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentLineCommands), nameof(GetByDocument), query, ex);
                throw;
            }
        }

        public async Task<string> Update(IDocumentLine model)
        {
            return (await UpdateBatch(new List<IDocumentLine>(new IDocumentLine[] { model })))
                 .FirstOrDefault();
        }

        public async Task<IList<string>> UpdateBatch(IList<IDocumentLine> models)
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
                        $"UPDATE [DocumentLines] SET {Common.GetUpdateQueryFields<IDocumentLine>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentLineCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
        }
    }
}
