using Core.Repositories;
using Core.Services.Session;
using Core.Services.Session.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Common
{
    public abstract class SessionRepositoryBase : ISessionRepository
    {
        protected abstract IDbConnection CreateConnection();
        
        public async Task<IUserSession> GetSession(string token)
        {
            var query = $"SELECT {Common.GetColumnNames<IUserSession>()} FROM UserSession";
            var res = new List<IUserSession>();
            using (var cnn = CreateConnection())
            {   
                return await cnn.QueryFirstOrDefaultAsync<UserSession>(query + " WHERE SessionToken = @SessionToken", new { SessionToken = token });
            }
        }

        public async Task NewSession(IUserSession session)
        {
            if (session == null)
                throw new ArgumentException("Session cannot be null", nameof(session));
            
            var query = $"INSERT INTO UserSession ({Common.GetColumnNames<IUserSession>()}) VALUES ({Common.GetFieldNames<IUserSession>()})";            
            using (var cnn = CreateConnection())
            {
                var res = await cnn.ExecuteAsync(query, session);                
            }
        }

        public async Task UpdateSession(IUserSession session)
        {
            if (session == null)
                throw new ArgumentException("UserId cannot be null", nameof(session));

            var fields = string.Join(",", typeof(IUserSession).GetProperties()
                .Select(x => x.Name + "=@" + x.Name));
            fields = fields.Replace("SessionToken=@SessionToken,", "");

            var query = $"UPDATE UserSession SET {fields} WHERE SessionToken=@SessionToken";
            using (var cnn = CreateConnection())
            {
                var res = await cnn.ExecuteAsync(query, session);
            }
        }

        public async Task RemoveSession(string token)
        {
            var query = $"DELETE FROM UserSession WHERE SessionToken=@SessionToken";
            using (var cnn = CreateConnection())
            {
                var res = await cnn.ExecuteAsync(query, new { SessionToken = token });
            }
        }
    }
}
