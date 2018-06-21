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
    internal class DocumentCommands : IDocumentCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public DocumentCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(IDocument model)
        {
            return (await AddBatch(new List<IDocument>(new IDocument[] { model })))
                .FirstOrDefault();
        }        
        public async Task Delete(string id)
        {
            await DeleteBatch(new List<string>(new string[] { id }));
        }        
        public async Task<IDocument> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<IDocument>()} FROM [Document] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Document>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentCommands), nameof(Get), id, ex);
                throw;
            }
        }
        public Task<IDocument> Get(string seriesId, long docNumber)
        {
            throw new NotImplementedException();
        }
        public async Task<string> Update(IDocument model)
        {
            return (await UpdateBatch(new List<IDocument>(new IDocument[] { model })))
                 .FirstOrDefault();
        }


        public async Task<IList<string>> AddBatch(IList<IDocument> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    
                    foreach (var document in models)
                    {
                        // Validate Ids
                        if (document.Id == null)
                            throw new ArgumentNullException(nameof(document.Id));
                        var id = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [Document] WHERE Id=@Id", new { document.Id }, transaction);
                        if (id != null)
                            throw new InvalidConstraintException($"Id already exists: {document.Id}");

                        //Validate if document already exists
                        var check = await cnn.ExecuteScalarAsync(
                            $"SELECT Count(Id) FROM [Document] WHERE DocumentSeriesId = @DocumentSeriesId AND Number = @Number",
                            new { document.DocumentSeriesId, document.Number }, transaction);
                        if (check != null)
                            throw new InvalidOperationException($"Document series {document.DocumentSeriesId} number {document.Number} already exists ");
                    }
                    string query =
                        $"INSERT INTO [Document] ({Common.GetColumnNames<IDocument>()}) VALUES ({Common.GetFieldNames<IDocument>()});";
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
                _log?.WriteError(nameof(DocumentCommands), nameof(AddBatch), models?.ToJson(), ex);
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
                    var transaction = cnn.BeginTransaction();
                    foreach (var documentId in ids)
                    {
                        if (documentId == null)
                            throw new ArgumentNullException(nameof(documentId));
                        // Validate if there are jobs cannot be deleted
                        var res = await cnn.ExecuteScalarAsync(
                            $"SELECT IntegrationDate FROM [Document] WHERE Id = @Id", new { Id = documentId }, transaction);
                        if (res != null)
                            throw new InvalidOperationException("Document cannot be deleted.");
                    }
                    // Delete document lines
                    await cnn.ExecuteAsync(
                        $"DELETE FROM [DocumentLines] WHERE DocumentId = @DocumentId", new { DocumentId = ids }, transaction);
                    // Delete document
                    await cnn.ExecuteAsync(
                        $"DELETE FROM [Document] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }
        public async Task<IList<string>> UpdateBatch(IList<IDocument> models)
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
                        $"UPDATE [Document] SET {Common.GetUpdateQueryFields<IDocument>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
        }

        public async Task<IList<IDocument>> GetByCustomer(string customerId)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                    throw new ArgumentNullException(nameof(customerId));

                var query = $"SELECT {Common.GetColumnNames<IDocument>()} FROM [Document] WHERE CustomerId = @CustomerId";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Document>(query, new { CustomerId = customerId }))
                        .Cast<IDocument>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentCommands), nameof(GetByCustomer), customerId, ex);
                throw;
            }
        }
        public async Task<IList<IDocument>> GetByDate(DateTime dateFrom, DateTime dateTo)
        {
            try
            {   
                var query = $"SELECT {Common.GetColumnNames<IDocument>()} FROM [Document] WHERE DocumentDate >= @DateFrom AND DocumentDate <= @DateTo";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Document>(query, new { DateFrom = dateFrom, DateTo = dateTo }))
                        .Cast<IDocument>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentCommands), nameof(GetByDate), $"{dateFrom.ToString("u")} => {dateTo.ToString("u")}", ex);
                throw;
            }
        }
        public async Task<IList<IDocument>> GetBySeries(string seriesId)
        {
            try
            {
                if (string.IsNullOrEmpty(seriesId))
                    throw new ArgumentNullException(nameof(seriesId));

                var query = $"SELECT {Common.GetColumnNames<IDocument>()} FROM [Document] WHERE DocumentSeriesId = @DocumentSeriesId";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Document>(query, new { DocumentSeriesId = seriesId }))
                        .Cast<IDocument>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentCommands), nameof(GetBySeries), seriesId, ex);
                throw;
            }
        }

        public async Task<string> GetName(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var document = await Get(id);
                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    object sdata = (await cnn.QueryAsync(
                            $"SELECT Name, DocumentTypeId FROM [DocumentSeries] WHERE Id = @DocumentSeriesId", new { document.DocumentSeriesId }, transaction))
                            .First();

                    var seriesData = (IDictionary<string, object>)sdata;

                    var documentSeriesRef = seriesData["Name"];
                    var documentTypeId = seriesData["DocumentTypeId"];
                    var documentTypeRef = await cnn.ExecuteScalarAsync(
                            $"SELECT DocRef FROM [DocumentType] WHERE Id = @Id", new { Id = documentTypeId }, transaction);

                    string docName = $"{documentTypeRef} {documentSeriesRef}/{document.Number}";
                    return docName;

                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentCommands), nameof(GetName), id, ex);
                throw;
            }
        }

        public async Task<string> GetDescription(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var document = await Get(id);
                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();
                    object sdata = (await cnn.QueryAsync(
                            $"SELECT Description, DocumentTypeId FROM [DocumentSeries] WHERE Id = @DocumentSeriesId", new { document.DocumentSeriesId }, transaction))
                            .First();

                    var seriesData = (IDictionary<string, object>)sdata;

                    var documentSeriesDescription = seriesData["Description"];
                    var documentTypeId = seriesData["DocumentTypeId"];
                    var documentTypeDescription = await cnn.ExecuteScalarAsync(
                            $"SELECT Name FROM [DocumentType] WHERE Id = @Id", new { Id = documentTypeId }, transaction);

                    string docName = $"{documentTypeDescription} {documentSeriesDescription}/{document.Number}";
                    return docName;

                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentCommands), nameof(GetName), id, ex);
                throw;
            }
        }

        
    }
}
