using Core.Repositories.Commands.SessionRepository;
using Repositories.Common.Commands.SessionRepository;
using System;
using System.Data;

namespace Repositories.Common
{
    public class SessionRepositoryFactory
    {
        private readonly Func<IDbConnection> _createdDbConnection;

        public SessionRepositoryFactory(Func<IDbConnection> createdDbConnection)
        {
            _createdDbConnection = createdDbConnection;
        }

        public ISessionCommands CreateSessionCommands()
        {
            return new SessionCommands(_createdDbConnection);
        }
    }
}
