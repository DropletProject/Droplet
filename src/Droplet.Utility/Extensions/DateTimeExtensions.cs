using System;
using System.Collections.Generic;
using System.Text;

namespace Droplet.Utility.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime origin = new DateTime(1970, 1, 1);

        /// <summary>
        /// DateTime转时间戳
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToTimestamp(this DateTime value)
        {
            value = value.AddHours(-8);
            return (int)(value - origin).TotalSeconds;
        }

        /// <summary>
        /// DateTime转时间戳
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToLongTimestamp(this DateTime value)
        {
            value = value.AddHours(-8);
            return (long)(value - origin).TotalSeconds;
        }


        /// <summary>
        /// 时间戳转DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this int value)
        {
            return origin.AddSeconds(value).AddHours(8);
        }

        /// <summary>
        /// 时间戳转DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long value)
        {
            return origin.AddSeconds(value).AddHours(8);
        }

        /// <summary>
        /// 时间戳转可空DateTime
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? ToNullableDateTime(this long value)
        {
            if (value == 0)
                return null;
            else
                return ToDateTime(value);
        }
    }
}
