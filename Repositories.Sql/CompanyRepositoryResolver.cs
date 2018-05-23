using Contracts;
using Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositories.Sql
{
    public class CompanyRepositoryResolver: ICompanyRepositoryResolver
    {
        private readonly string _serverConnectionString;
        private readonly IUserRepository _userRepository;

        private List<ICompany> _companyList;

        public CompanyRepositoryResolver(string serverConnectionString, IUserRepository userRepository)
        {
            _serverConnectionString = serverConnectionString;
            _userRepository = userRepository;
        }

        public ICompanyRepository Resolve(string companyId)
        {
            if (_companyList == null)
                _companyList = _userRepository.Company.List().Result.ToList();

            var company = _companyList.FirstOrDefault(c => c.Id == companyId);
            if (company == null)
            {
                // Reload companies
                _companyList = _companyList = _userRepository.Company.List().Result.ToList();
                company = _companyList.FirstOrDefault(c => c.Id == companyId);
            }

            if (company == null)
                return null;

            return new CompanyRepository($"{_serverConnectionString};Database={company.Reference}");
        }

        
    }
}
