using AutoMapper;
using Droplet.AutoMapper.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Droplet.AutoMapper
{
    public class AutoMapperRegistrar
    {
        private readonly IServiceCollection _services;
        private readonly Assembly[] _assemblies;


        public AutoMapperRegistrar(IServiceCollection services, params Assembly[] assemblies)
        {
            _services = services;
            _assemblies = assemblies;
        }

        public void CreateMap()
        {
            var types = FindAllAutoMapTypes();

            _services.AddAutoMapper( cfg => {
                foreach (var srcType in types)
                {
                    var attr = srcType.GetCustomAttribute<MapAttribute>();
                    if (attr.Target.Count() == 0)
                        throw new ArgumentException("Targe type can not be empty.", "Target");

                    foreach (var aTargetType in attr.Target)
                    {
                        if (attr.Direction.HasFlag(MapFlag.MapTo))
                            cfg.CreateMap(srcType, aTargetType);

                        if (attr.Direction.HasFlag(MapFlag.MapFrom))
                            cfg.CreateMap(aTargetType, srcType);
                    }
                }
            },_assemblies);
        }

        private List<Type> FindAllAutoMapTypes()
        {
            var rs = new List<Type>();
            foreach (var aAssm in _assemblies)
            {
                var types = aAssm.GetTypes().Where(p => p.GetCustomAttribute<MapAttribute>() != null);
                rs.AddRange(types);
            }

            return rs;
        }
    }
}
