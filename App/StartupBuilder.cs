﻿namespace Easy.Net.Starter.App
{
    public class StartupBuilder
    {
        private readonly Dictionary<string, string> _scopedWithInterfaces = new();
        private readonly Dictionary<string, string> _singletonWithInterfaces = new();
        private readonly Dictionary<string, string> _transcientWithInterfaces = new();
        private readonly List<string> _singletonServices = new();
        private readonly List<string> _scopedServices = new();
        private readonly List<string> _transientServices = new();
        private bool _useDatabase = false;

        public StartupBuilder AddDatabase()
        {
            _useDatabase = true;
            return this;
        }

        public StartupBuilder AddScoped<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _scopedWithInterfaces[typeof(TInterface).Name] = typeof(TImplementation).Name;
            return this;
        }

        public StartupBuilder AddSingleton<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _singletonWithInterfaces[typeof(TInterface).Name] = typeof(TImplementation).Name;
            return this;
        }

        public StartupBuilder AddTransient<TInterface, TImplementation>() where TImplementation : TInterface
        {
            _transcientWithInterfaces[typeof(TInterface).Name] = typeof(TImplementation).Name;
            return this;
        }

        public StartupBuilder AddSingleton<TImplementation>()
        {
            _singletonServices.Add(typeof(TImplementation).Name);
            return this;
        }

        public StartupBuilder AddScoped<TImplementation>()
        {
            _scopedServices.Add(typeof(TImplementation).Name);
            return this;
        }

        public StartupBuilder AddTransient<TImplementation>()
        {
            _transientServices.Add(typeof(TImplementation).Name);
            return this;
        }

        public StartupOptions Build()
        {
            return new StartupOptions
            {
                WithDatabase = _useDatabase,
                SingletonServices = _singletonServices.ToArray(),
                ScopedServices = _scopedServices.ToArray(),
                TransientServices = _transientServices.ToArray(),
                ScopedWithInterfaces = new Dictionary<string, string>(_scopedWithInterfaces),
                SingletonsWithInterfaces = new Dictionary<string, string>(_singletonWithInterfaces),
                TransientsWithInterfaces = new Dictionary<string, string>(_transcientWithInterfaces)
            };
        }
    }
}