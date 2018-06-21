using Core.Json;
using Core.Repositories.Commands.SessionRepository;
using Core.Services.Session;
using Core.Services.Session.Models;
using Dapper;
using NBsoft.Logs.Interfaces;
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
        private readonly ILogger _log;

        public SessionCommands(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }


        public async Task<IUserSession> Get(string token)
        {            
            try
            {                
                var res = new List<IUserSession>();
                var query = $"SELECT {Common.GetColumnNames<IUserSession>()} FROM UserSession";
                using (var cnn = _createdDbConnection())
                {
                    return await cnn.QueryFirstOrDefaultAsync<UserSession>(query + " WHERE SessionToken = @SessionToken", new { SessionToken = token });
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(SessionCommands), nameof(Get), token, ex);
                throw;
            }
        }

        public async Task New(IUserSession session)
        {            
            try
            {
                if (session == null)
                    throw new ArgumentException("Session cannot be null", nameof(session));
                var query = $"INSERT INTO UserSession ({Common.GetColumnNames<IUserSession>()}) VALUES ({Common.GetFieldNames<IUserSession>()})";
                using (var cnn = _createdDbConnection())
                {
                    var res = await cnn.ExecuteAsync(query, session);
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(SessionCommands), nameof(New), session?.ToJson(), ex);
                throw;
            }
        }

        public async Task Update(IUserSession session)
        {
            
            try
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
            catch (Exception ex)
            {
                _log?.WriteError(nameof(SessionCommands), nameof(Update), session?.ToJson(), ex);
                throw;
            }
        }

        public async Task Remove(string token)
        {            
            try
            {
                var query = $"DELETE FROM UserSession WHERE SessionToken=@SessionToken";
                using (var cnn = _createdDbConnection())
                {
                    var res = await cnn.ExecuteAsync(query, new { SessionToken = token });
                }
            }
            catch (Exception ex)
            {
                _log?.WriteError(nameof(SessionCommands), nameof(Remove), token, ex);
                throw;
            }
        }
    }
}
