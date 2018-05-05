using Autofac;
using Core.Settings;

namespace Backend.Modules
{
    public class SettingsModule : Module
    {
        private readonly BackendSetting _backendSettings;

        public SettingsModule(BackendSetting backendSettings)
        {
            _backendSettings = backendSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_backendSettings).SingleInstance();
        }

    }
}
