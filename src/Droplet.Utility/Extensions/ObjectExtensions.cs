using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Droplet.Utility.Extensions
{
    public static class ObjectExtensions
    {
        public static string GetDescription(this object @this, bool inherit = false)
        {
            if (@this.GetType().IsEnum)
                return GetEnumDescription(@this);

            var attr = @this.GetType().GetCustomAttribute<DescriptionAttribute>(inherit);
            if (attr == null)
                return string.Empty;

            return attr.Description;
        }

        private static string GetEnumDescription(object enumObj)
        {
            var field = enumObj.GetType().GetField(enumObj.ToString());
            var attr = field.GetCustomAttribute<DescriptionAttribute>();
            if (attr == null)
                return string.Empty;

            return attr.Description;
        }
    }
}
