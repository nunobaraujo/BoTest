﻿using Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Repositories.Commands.CompanyRepository
{
    public interface IProductCommands : ICrudBaseCommands<IProduct>
    {
        Task<IList<IProduct>> GetByProductCategory(string productCategoryId);
    }
}
