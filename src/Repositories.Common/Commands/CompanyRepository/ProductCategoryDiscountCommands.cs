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
    internal class ProductCategoryDiscountCommands : IProductCategoryDiscountCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public ProductCategoryDiscountCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public async Task<string> Add(IProductCategoryDiscount model)
        {
            return (await AddBatch(new List<IProductCategoryDiscount>(new IProductCategoryDiscount[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> AddBatch(IList<IProductCategoryDiscount> models)
        {
            try
            {
                if (models == null || models.Count <= 0)
                    throw new ArgumentNullException(nameof(models));

                using (var cnn = _createdDbConnection())
                {
                    cnn.Open();
                    var transaction = cnn.BeginTransaction();

                    foreach (var productCategoryDiscount in models)
                    {
                        // Validate Ids
                        if (productCategoryDiscount.Id == null)
                            throw new ArgumentNullException(nameof(productCategoryDiscount.Id));
                        var id = await cnn.ExecuteScalarAsync(
                            "SELECT Id FROM [ProductCategoryDiscount] WHERE Id=@Id", new { productCategoryDiscount.Id }, transaction);
                        if (id != null)
                            throw new InvalidConstraintException($"Id already exists: {productCategoryDiscount.Id}");
                    }
                    string query =
                        $"INSERT INTO [ProductCategoryDiscount] ({Common.GetColumnNames<IProductCategoryDiscount>()}) VALUES ({Common.GetFieldNames<IProductCategoryDiscount>()});";
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
                _log?.WriteError(nameof(ProductCategoryDiscountCommands), nameof(AddBatch), models?.ToJson(), ex);
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
                        $"DELETE FROM [ProductCategoryDiscount] WHERE Id = @Id", new { Id = ids }, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(ProductCategoryDiscountCommands), nameof(DeleteBatch), ids.ToJson(), ex);
                throw;
            }
        }

        public async Task<IProductCategoryDiscount> Get(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException(nameof(id));

                var query = $"SELECT {Common.GetColumnNames<IProductCategoryDiscount>()} FROM [ProductCategoryDiscount] WHERE Id = @Id";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<ProductCategoryDiscount>(query, new { Id = id }))
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(ProductCategoryDiscountCommands), nameof(Get), id, ex);
                throw;
            }
        }

        public async Task<IList<IProductCategoryDiscount>> GetByCustomer(string customerId)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                    throw new ArgumentNullException(nameof(customerId));

                var query = $"SELECT {Common.GetColumnNames<IProductCategoryDiscount>()} FROM [ProductCategoryDiscount] WHERE CustomerId = @CustomerId";
                using (var cnn = _createdDbConnection())
                {
                    return (await cnn.QueryAsync<ProductCategoryDiscount>(query, new { CustomerId = customerId }))
                        .Cast<IProductCategoryDiscount>()
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(ProductCategoryDiscountCommands), nameof(GetByCustomer), customerId, ex);
                throw;
            }
        }

        public async  Task<string> Update(IProductCategoryDiscount model)
        {
            return (await UpdateBatch(new List<IProductCategoryDiscount>(new IProductCategoryDiscount[] { model })))
                .FirstOrDefault();
        }

        public async Task<IList<string>> UpdateBatch(IList<IProductCategoryDiscount> models)
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
                        $"UPDATE [ProductCategoryDiscount] SET {Common.GetUpdateQueryFields<IProductCategoryDiscount>("Id")} WHERE Id=@Id";

                    if (await cnn.ExecuteAsync(query, models, transaction) != models.Count)
                        throw new Exception($"ExecuteAsync failed: {query}");
                    transaction.Commit();
                    return models.Select(m => m.Id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(ProductCategoryDiscountCommands), nameof(UpdateBatch), models?.ToJson(), ex);
                throw;
            }
        }
    }
}
