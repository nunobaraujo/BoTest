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
    internal class ProductCommands : IProductCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public ProductCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(IProduct model)
        {
            return (await AddBatch(new List<IProduct>(new IProduct[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> AddBatch(IList<IProduct> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    foreach (var product in models)
                    {
                        // Validate Ids
                        if (product.Id == null)
                            throw new ArgumentNullException(nameof(product.Id));
                        var id = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [Product] WHERE Id=@Id", new { product.Id }, transaction);
                        if (id != null)
                            throw new InvalidConstraintException($"Id already exists: {product.Id}");
                    }
                    string query =
                        $"INSERT INTO [Product] ({Common.GetColumnNames<IProduct>()}) VALUES ({Common.GetFieldNames<IProduct>()});";
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
                _log?.WriteError(nameof(ProductCommands), nameof(AddBatch), models?.ToJson(), ex);
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
                        $"DELETE FROM [Product] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(ProductCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }

        public async Task<IProduct> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<IProduct>()} FROM [Product] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Product>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(ProductCommands), nameof(Get), id, ex);
                throw;
            }
        }

        public async Task<IList<IProduct>> GetByProductCategory(string productCategoryId)
        {
            try
            {
                if (string.IsNullOrEmpty(productCategoryId))
                    throw new ArgumentNullException(nameof(productCategoryId));

                var query = $"SELECT {Common.GetColumnNames<IProduct>()} FROM [Product] WHERE ProductCategoryId = @ProductCategoryId";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<Product>(query, new { ProductCategoryId = productCategoryId }))
                        .Cast<IProduct>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(ProductCommands), nameof(GetByProductCategory), productCategoryId, ex);
                throw;
            }
        }

        public async Task<string> Update(IProduct model)
        {
            return (await UpdateBatch(new List<IProduct>(new IProduct[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> UpdateBatch(IList<IProduct> models)
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
                        $"UPDATE [Product] SET {Common.GetUpdateQueryFields<IProduct>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(ProductCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
        }
    }
}
