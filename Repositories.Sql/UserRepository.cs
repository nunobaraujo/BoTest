using Dapper;
using Repositories.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Repositories.Sql
{
    public class UserRepository : UserRepositoryBase
    {
        private readonly string _connString;

        public UserRepository(string connString)
        {
            _connString = connString;
        }

        protected override IDbConnection CreateConnection()
        {
            return new SqlConnection(_connString);
        }
        protected override string GetLastInsertedId()
        {
            throw new NotImplementedException();
        }
    }
}
