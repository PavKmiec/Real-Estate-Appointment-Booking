using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingWebsite.Extensions
{
    // extensions method class should be static 
    public static class IEnumerableExtensions
    {
        // all methods should also be static for extension methods


        // we are converting IEnumerable of product types to selectListItem - this is so we can use this as drop down list to choose value from
        // first argument of extention method should be of extended class proceded by "this" keyword
        // second argument snice it it a drop down list we will have an integer "selectedValue"
        // our last parameter in int because producTypes has an Id that is int
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue)
        {
            // passing the collection that we have in IEnumerable;  using linq 
            return from item in items
                select new SelectListItem
                {
                    // w don not have anything tha would get the name property from this item collection so another Extention is needed
                    Text = item.GetPropertyValue("Name"),
                    Value = item.GetPropertyValue("Id"),
                    Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                };
        }


        /// <summary>
        /// Because we use this for dropdown list of Sales Person the above extention will not work as User Id is a string
        /// So we need another method for Sales Person Drop Down list and we need to pass in a string selectedValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> ToSelectListItemString<T>(this IEnumerable<T> items, string selectedValue)
        {
            // this makes sure that we wont get an exception thrown
            if (selectedValue == null)
            {
                selectedValue = "";
            }

            // passing the collection that we have in IEnumerable;  using linq 
            return from item in items
                select new SelectListItem
                {
                    Text = item.GetPropertyValue("Name"),
                    Value = item.GetPropertyValue("Id"),
                    Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                };
        }
    }
}
