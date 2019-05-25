using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingWebsite.Extensions
{
    /// <summary>
    /// extention to help get property values in IEnumerableExtentions 
    /// </summary>
    public static class ReflectionExtension
    {
        /// <summary>
        /// this enables us to get property values in our IEnumerableExtentions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetPropertyValue<T>(this T item, string propertyName)
        {
            return item.GetType().GetProperty(propertyName).GetValue(item, null).ToString();
        }
    }
}
