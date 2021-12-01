using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;
using System.Web;

namespace Library.Core.Common
{
    public static class Util
    {
        public const string DateFormat = "dd-MMM-yyyy";
        public const string TimeFormat = "hh:mm:ss tt";
        public const string EntryTimeFormat = "hh:mm tt";
        public const string DateTimeFormat = "yyyy-MM-dd hh:mm:ss.fff";
        public static string ConvertedDateFormat = "MM/dd/yyyy";
        public static string DbDateFormat = "yyyy-MM-dd";

        public static string GetDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                    typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }

        private static string GetDescription(string enumValue, string defDesc)
        {
            var fi = enumValue.GetType().GetField(enumValue);
            if (null != fi)
            {
                var attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return defDesc;
        }

        public static string GetDescription(string enumValue)
        {
            return GetDescription(enumValue, string.Empty);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <returns></returns>
        //public static T GetClaims<T>() where T : class, new()
        //{
        //    try
        //    {
        //        var objType = typeof(T);
        //        var obj = Activator.CreateInstance<T>(); //hence the new() contsraint
        //        if (HttpContext.Current.User is ClaimsPrincipal principal)
        //        {
        //            foreach (Claim claim in principal.Claims)
        //            {
        //                //Debug.WriteLine(objType.Name + " = new " + objType.Name + "();");
        //                var property =
        //                        objType.GetProperty(claim.Type,
        //                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        //                if (property == null || !property.CanWrite)
        //                {
        //                    //Debug.WriteLine("//Property " + column.ColumnName + " not in object");
        //                    continue; //or throw
        //                }
        //                if (property.PropertyType.Name == "Guid")
        //                {
        //                    property.SetValue(obj, Guid.Parse(claim.Value), null);
        //                }
        //                else
        //                {
        //                    var value = claim.Value;
        //                    if (string.IsNullOrEmpty(value)) value = null;
        //                    property.SetValue(obj, value, null);
        //                }
        //                //Debug.WriteLine("obj." + property.Name + " = row[\"" + column.ColumnName + "\"];");
        //            }
        //        }
        //        return obj;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}