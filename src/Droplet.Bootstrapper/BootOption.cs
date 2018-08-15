using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Droplet.Bootstrapper
{
    public class BootOption
    {
        /// <summary>
        /// If you set Keyword, bootstrapper will not set similar assembly finder
        /// </summary>
        public string KeyWord { get; set; }

        /// <summary>
        /// You should rarely set entry assembly, it usually uesd in test
        /// </summary>
        public Assembly EntryAssembly { get; set; }


    }
}
