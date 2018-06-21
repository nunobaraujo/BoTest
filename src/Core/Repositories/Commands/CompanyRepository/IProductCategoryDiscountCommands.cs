using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IProductCategoryDiscountCommands: ICrudBaseCommands<IProductCategoryDiscount>
    {
        Task<IList<IProductCategoryDiscount>> GetByCustomer(string customerId);
    }
}
