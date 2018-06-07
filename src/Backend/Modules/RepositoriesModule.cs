using System;
using Autofac;
using Core.Repositories;
using Core.Settings;
using NBsoft.Logs.Interfaces;
using Repositories.Sql;

namespace Backend.Modules
{
    public class RepositoriesModule: Module
    {
        private readonly BackendSetting _backendSetting;
        private readonly ILogger _log;
        public RepositoriesModule(BackendSetting backendSetting, ILogger log)
        {
            _backendSetting = backendSetting;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<ISessionRepository>(c => new SessionRepository(_backendSetting.Db.SessionConnString, _log))
                .SingleInstance();

            IUserRepository userRepo = new UserRepository(_backendSetting.Db.UserConnString, _log);
            builder.RegisterInstance(userRepo);

            builder.Register<ICompanyRepositoryResolver>(c => new CompanyRepositoryResolver(_backendSetting.Db.CompanyConnString, userRepo, _log))
                .SingleInstance();

            
        }
    }
}
