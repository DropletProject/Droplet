using Droplet.AutoDI;
using Droplet.AutoDI.Dotnet;
using Droplet.Module;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Droplet.Bootstrapper
{
    /// <summary>
    /// It is uesed to init droplet component
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Droplet to project
        /// </summary>
        /// <param name="this"></param>
        /// <param name="keyWord"></param>
        public static void BootDroplet(this IServiceCollection @this, BootOption opt = null)
        {
            if (opt == null)
                opt = new BootOption();

            var moduleManger = new ModuleManager(opt.KeyWord, opt.EntryAssembly);
            moduleManger.Init();
            @this.AddSingleton<IModuleFinder>(moduleManger);

            InitAutoDI(@this, moduleManger);
        }

        private static void InitAutoDI(IServiceCollection services,IModuleFinder moduleFinder)
        {
            var dotnetRegister = new DotnetRegister(services);
            var registrar = new ComponentRegistrar(dotnetRegister);
            registrar.RegisterAssembly(moduleFinder.GetModuleAssemblies().ToArray());
        }
    }
}
