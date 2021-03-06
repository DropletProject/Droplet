﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Droplet.Module.AssemblySelector;
using System.IO;

namespace Droplet.Module
{
    public class ModuleManager : IModuleFinder, IModuleInitor
    {
        private List<Assembly> _moduleAssemblies = new List<Assembly>();
        private string _moduleKeyword;
        private Assembly _entryAssembly;
        private List<IAssemblySelector> _selectors = new List<IAssemblySelector>();
        private bool _isInited = false;

        public ModuleManager(string moduleKeyword = "", Assembly entryAssembly = null)
        {
            _moduleKeyword = moduleKeyword;
            _entryAssembly = entryAssembly;
        }

        public List<Assembly> GetModuleAssemblies()
        {
            if (!_isInited) {
                Init();
            }

            return _moduleAssemblies;
        }

        public IModuleFinder AddFinder(IAssemblySelector finder)
        {
            _selectors.Add(finder);
            return this;
        }

        public void Init()
        {
            if (_isInited) {
                return;
            }

            InitSelectors();
            ExcuteSelector();
            _isInited = true;
        }

        private void InitSelectors()
        {
            if (!string.IsNullOrEmpty(_moduleKeyword))
                _selectors.Add(new KeywordAssemblySelector(_moduleKeyword));
            else
                _selectors.Add(new SimiliarAssemblySelector(_entryAssembly));

            _selectors.Add(new InitModuleAssemblySelector());
        }

        private void ExcuteSelector()
        {
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory);
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            _selectors.ForEach(p => _moduleAssemblies.AddRange(p.SelectModuleAssembly(allAssemblies)));
        }

        protected void LoadAssemblies(string path)
        {
            foreach (string file in Directory.GetFiles(path, "*.dll"))
            {
                var assemblyName = AssemblyName.GetAssemblyName(file);
                AppDomain.CurrentDomain.Load(assemblyName);
            }
        }
    }

}
