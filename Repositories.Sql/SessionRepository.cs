using Dapper;
using Repositories.Common;
using System.Data;
using System.Data.SqlClient;

namespace Repositories.Sql
{
    public class SessionRepository : SessionRepositoryBase
    {
        private readonly string _connString;

        public SessionRepository(string connString)
        {
            _connString = connString;
            //CreateDataBaseIfDoesntExist();
        }
        protected override IDbConnection CreateConnection()
        {
            return new SqlConnection(_connString);
        }

        //private void CreateDataBaseIfDoesntExist()
        //{
        //    string query = @"CREATE TABLE [UserSession](
	       //     [SessionToken]  [nvarchar](255) NOT NULL,
	       //     [UserId]        [nvarchar](128) NOT NULL,
        //        [UserInfo]      [text] NOT NULL, 
        //        [Registered]    [datetime] NOT NULL,
        //        [LastAction]    [datetime] NOT NULL,
        //        CONSTRAINT [PK_UserSession] PRIMARY KEY CLUSTERED ([SessionToken] ASC));";
        //    using (var conn = CreateConnection())
        //    {
        //        conn.Open();
        //        try
        //        {
        //            // Check if table exists
        //            conn.ExecuteScalar($"select top 1 * from UserSession");
        //        }
        //        catch (SqlException)
        //        {   
        //            conn.Execute(query);
        //        }
        //        finally
        //        { conn.Close(); }
        //    }
        //}
    }
}
