using Autofac;
using Backend.Managers;
using Backend.Middleware.Validator;
using Backend.Services;
using Core.Managers;
using Core.Services;
using Core.Services.Session;
using Core.Settings;
using NBsoft.Logs.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Modules
{
    public class ServicesModule:Module
    {
        private readonly BackendSetting _backendSetting;
        private readonly ILogger _log;
        public ServicesModule(BackendSetting backendSetting, ILogger log)
        {
            _backendSetting = backendSetting;
            _log = log;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApiKeyValidator>()
                .As<IApiKeyValidator>()
                .WithParameter("apiKey", _backendSetting.ApiKey)
                .SingleInstance();

            builder.RegisterType<SessionService>()
                .As<ISessionService>()
                .SingleInstance();

            builder.RegisterType<UserManager>()
                .As<IUserManager>()
                .SingleInstance();
        }
    }
}
