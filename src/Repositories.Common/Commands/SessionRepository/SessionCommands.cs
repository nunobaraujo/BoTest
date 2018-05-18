using Core.Repositories.Commands.SessionRepository;
using Core.Services.Session;
using Core.Services.Session.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Common.Commands.SessionRepository
{
    internal class SessionCommands : ISessionCommands
    {
        private readonly Func<IDbConnection> _createdDbConnection;

        public SessionCommands(Func<IDbConnection> createdDbConnection)
        {
            _createdDbConnection = createdDbConnection;
        }


        public async Task<IUserSession> Get(string token)
        {
            var query = $"SELECT {Common.GetColumnNames<IUserSession>()} FROM UserSession";
            var res = new List<IUserSession>();
            using (var cnn = _createdDbConnection())
            {
                return await cnn.QueryFirstOrDefaultAsync<UserSession>(query + " WHERE SessionToken = @SessionToken", new { SessionToken = token });
            }
        }

        public async Task New(IUserSession session)
        {
            if (session == null)
                throw new ArgumentException("Session cannot be null", nameof(session));

            var query = $"INSERT INTO UserSession ({Common.GetColumnNames<IUserSession>()}) VALUES ({Common.GetFieldNames<IUserSession>()})";
            using (var cnn = _createdDbConnection())
            {
                var res = await cnn.ExecuteAsync(query, session);
            }
        }

        public async Task Update(IUserSession session)
        {
            if (session == null)
                throw new ArgumentException("UserId cannot be null", nameof(session));

            var fields = string.Join(",", typeof(IUserSession).GetProperties()
                .Select(x => x.Name + "=@" + x.Name));
            fields = fields.Replace("SessionToken=@SessionToken,", "");

            var query = $"UPDATE UserSession SET {fields} WHERE SessionToken=@SessionToken";
            using (var cnn = _createdDbConnection())
            {
                var res = await cnn.ExecuteAsync(query, session);
            }
        }

        public async Task Remove(string token)
        {
            var query = $"DELETE FROM UserSession WHERE SessionToken=@SessionToken";
            using (var cnn = _createdDbConnection())
            {
                var res = await cnn.ExecuteAsync(query, new { SessionToken = token });
            }
        }
    }
}
