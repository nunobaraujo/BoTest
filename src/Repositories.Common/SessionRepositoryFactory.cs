using Core.Repositories.Commands.SessionRepository;
using NBsoft.Logs.Interfaces;
using Repositories.Common.Commands.SessionRepository;
using System;
using System.Data;

namespace Repositories.Common
{
    public class SessionRepositoryFactory
    {
        private readonly Func<IDbConnection> _createdDbConnection;
        private readonly ILogger _log;

        public SessionRepositoryFactory(Func<IDbConnection> createdDbConnection, ILogger log)
        {
            _createdDbConnection = createdDbConnection;
            _log = log;
        }

        public ISessionCommands CreateSessionCommands()
        {
            return new SessionCommands(_createdDbConnection, _log);
        }
    }
}
