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
    internal class DocumentSeriesCommands : IDocumentSeriesCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public DocumentSeriesCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(IDocumentSeries model)
        {
            return (await AddBatch(new List<IDocumentSeries>(new IDocumentSeries[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> AddBatch(IList<IDocumentSeries> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    foreach (var documentSeries in models)
                    {
                        // Validate Ids
                        if (documentSeries.Id == null)
                            throw new ArgumentNullException(nameof(documentSeries.Id));
                        var id = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [DocumentSeries] WHERE Id=@Id", new { documentSeries.Id }, transaction);
                        if (id != null)
                            throw new InvalidConstraintException($"Id already exists: {documentSeries.Id}");
                    }
                    string query =
                        $"INSERT INTO [DocumentSeries] ({Common.GetColumnNames<IDocumentSeries>()}) VALUES ({Common.GetFieldNames<IDocumentSeries>()});";
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
                _log?.WriteError(nameof(DocumentSeriesCommands), nameof(AddBatch), models?.ToJson(), ex);
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
                        $"DELETE FROM [DocumentSeries] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentSeriesCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }

        public async Task<IDocumentSeries> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<IDocumentSeries>()} FROM [DocumentSeries] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<DocumentSeries>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentSeriesCommands), nameof(Get), id, ex);
                throw;
            }
        }

        public async Task<IList<IDocumentSeries>> GetByDocumentType(string documentTypeId)
        {
            if (string.IsNullOrEmpty(documentTypeId))
                throw new ArgumentNullException(nameof(documentTypeId));
            var query = $"SELECT {Common.GetColumnNames<IDocumentSeries>()} FROM [DocumentSeries] WHERE DocumentTypeId = @DocumentTypeId";
            try
            {
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<DocumentSeries>(query, new { DocumentTypeId = documentTypeId }))
                        .Cast<IDocumentSeries>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentSeriesCommands), nameof(GetByDocumentType), query, ex);
                throw;
            }
        }

        public async Task<string> Update(IDocumentSeries model)
        {
            return (await UpdateBatch(new List<IDocumentSeries>(new IDocumentSeries[] { model })))
                 .FirstOrDefault();
        }

        public async Task<IList<string>> UpdateBatch(IList<IDocumentSeries> models)
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
                        $"UPDATE [DocumentSeries] SET {Common.GetUpdateQueryFields<IDocumentSeries>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentSeriesCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
        }
    }
}
