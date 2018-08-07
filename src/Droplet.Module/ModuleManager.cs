using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Droplet.Module.AssemblySelector;

namespace Droplet.Module
{
    public class ModuleManager : IModuleFinder, IModuleInitor
    {
        private List<Assembly> _moduleAssemblies = new List<Assembly>();
        private string _moduleKeyword;
        private List<IAssemblySelector> _selectors = new List<IAssemblySelector>();

        public ModuleManager()
        {
        }

        public ModuleManager(string moduleKeyword)
        {
            _moduleKeyword = moduleKeyword;
        }

        public List<Assembly> GetModuleAssemblies()
        {
            return _moduleAssemblies;
        }

        public IModuleFinder AddFinder(IAssemblySelector finder)
        {
            _selectors.Add(finder);
            return this;
        }

        public void Init()
        {
            initSelectors();
            excuteSelector();
        }

        private void initSelectors()
        {
            if (!string.IsNullOrEmpty(_moduleKeyword))
                _selectors.Add(new KeywordAssemblySelector(_moduleKeyword));
            else
                _selectors.Add(new SimiliarAssemblySelector());

            _selectors.Add(new InitModuleAssemblySelector());
        }

        private void excuteSelector()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            _selectors.ForEach(p => _moduleAssemblies.AddRange(p.SelectModuleAssembly(allAssemblies)));
        }
    }

}
