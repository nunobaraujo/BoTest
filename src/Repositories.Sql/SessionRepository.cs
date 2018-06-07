using Core.Repositories;
using Core.Repositories.Commands.SessionRepository;
using NBsoft.Logs.Interfaces;
using Repositories.Common;
using System.Data.SqlClient;

namespace Repositories.Sql
{
    public class SessionRepository : ISessionRepository
    {
        private readonly string _connString;
        private readonly ILogger _log;

        public ISessionCommands Session { get; }

        public SessionRepository(string connString, ILogger log)
        {
            _connString = connString;
            _log = log;
            var sessionRepositoryFactory = new SessionRepositoryFactory(() => new SqlConnection(_connString), _log);
            Session = sessionRepositoryFactory.CreateSessionCommands();
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
