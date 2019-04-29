using Droplet.Module;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Droplet.Bootstrapper
{
    /// <summary>
    /// It is uesed to init droplet component
    /// </summary>
    public static  class ServiceCollectionExtensions
    {

        /// <summary>
        /// Add Droplet to project
        /// </summary>
        /// <param name="this"></param>
        /// <param name="keyWord"></param>
        public static DropletBuilder BootDroplet(this IServiceCollection @this, BootOption opt = null)
        {
            if (opt == null)
                opt = new BootOption();

            var moduleManger = new ModuleManager(opt.KeyWord, opt.EntryAssembly);
            moduleManger.Init();
            @this.AddSingleton<IModuleFinder>(moduleManger);
            var registerAssembly = moduleManger.GetModuleAssemblies();

            return new DropletBuilder(@this, registerAssembly);
        }
    }

  
}
