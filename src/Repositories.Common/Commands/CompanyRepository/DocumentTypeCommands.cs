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
    internal class DocumentTypeCommands: IDocumentTypeCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public DocumentTypeCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(IDocumentType model)
        {
            return (await AddBatch(new List<IDocumentType>(new IDocumentType[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> AddBatch(IList<IDocumentType> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    foreach (var documentType in models)
                    {
                        // Validate Ids
                        if (documentType.Id == null)
                            throw new ArgumentNullException(nameof(documentType.Id));
                        var id = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [DocumentType] WHERE Id=@Id", new { documentType.Id }, transaction);
                        if (id != null)
                            throw new InvalidConstraintException($"Id already exists: {documentType.Id}");
                    }
                    string query =
                        $"INSERT INTO [DocumentType] ({Common.GetColumnNames<IDocumentType>()}) VALUES ({Common.GetFieldNames<IDocumentType>()});";
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
                _log?.WriteError(nameof(DocumentTypeCommands), nameof(AddBatch), models?.ToJson(), ex);
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
                        $"DELETE FROM [DocumentType] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentTypeCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }

        public async Task<IDocumentType> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<IDocumentType>()} FROM [DocumentType] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<DocumentType>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentTypeCommands), nameof(Get), id, ex);
                throw;
            }
        }

        public async Task<IList<IDocumentType>> List()
        {
            try
            {
                var query = $"SELECT {Common.GetColumnNames<IDocumentType>()} FROM [DocumentType]";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<DocumentType>(query))
                        .Cast<IDocumentType>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(DocumentTypeCommands), nameof(Get), id, ex);
                throw;
            }
        }

        public async Task<string> Update(IDocumentType model)
        {
            return (await UpdateBatch(new List<IDocumentType>(new IDocumentType[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> UpdateBatch(IList<IDocumentType> models)
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
                        $"UPDATE [DocumentType] SET {Common.GetUpdateQueryFields<IDocumentType>("Id")} WHERE Id=@Id";

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
