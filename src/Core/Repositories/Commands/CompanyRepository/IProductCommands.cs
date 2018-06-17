using Contracts;
using System.Collections.Generic;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IProductCommands : ICrudBaseCommands<IProduct>
    {
        IList<IProduct> GetByProductCategory(string productCategoryId);
    }
}
