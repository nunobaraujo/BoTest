using Contracts.User;
using Contracts.User.Models;
using Core.Repositories;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Common
{
    public abstract class UserRepositoryBase:IUserRepository
    {
        protected abstract IDbConnection CreateConnection();        
        protected abstract string GetLastInsertedId();

        #region IUser
        public async Task<string> AuthUser(string userName, string userPassword)
        {
            var query = $"SELECT Id FROM [User] WHERE UserName = @UserName AND PasswordHash = @PasswordHash";
            using (var cnn = CreateConnection())
            {
                var res = (await cnn.ExecuteScalarAsync(query, new { UserName = userName, PasswordHash = userPassword }));
                return res?.ToString();
            }
        }
        public async Task<IUser> GetUser(string userId)
        {
            var query = $"SELECT {Common.GetColumnNames<IUser>()} FROM [User] WHERE Id = @Id";
            using (var cnn = CreateConnection())
            {
                return (await cnn.QueryAsync<User>(query, new { Id = userId }))
                    .FirstOrDefault();
            }
        }

        public async Task<IUser> GetUserByUserName(string userName)
        {
            var query = $"SELECT {Common.GetColumnNames<IUser>()} FROM [User] WHERE UserName = @UserName";
            using (var cnn = CreateConnection())
            {
                return (await cnn.QueryAsync<User>(query, new { UserName = userName }))
                    .FirstOrDefault();
            }
        }

        public Task AddUser(IUser user)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUser(IUser user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUser(IUser user)
        {
            throw new NotImplementedException();
        }
        #endregion

        public Task<ICompany> GetCompany(string companyId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ICompanyUser>> GetCompanyByUser(string userId)
        {
            throw new NotImplementedException();
        }

        

        

        public Task AddCompany(ICompany company)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCompany(ICompany company)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCompany(ICompany company)
        {
            throw new NotImplementedException();
        }

        public Task AddCompanyUser(ICompanyUser companyUser)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCompanyUser(ICompanyUser companyUser)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCompanyUser(ICompanyUser companyUser)
        {
            throw new NotImplementedException();
        }

       

        
    }
}
