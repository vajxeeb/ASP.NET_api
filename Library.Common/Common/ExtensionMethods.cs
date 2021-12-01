using System;
using System.Data;
using System.Globalization;

namespace Library.Core.Common
{
    public static class ExtensionMethods
    {
        public static bool ToBoolean(this string input, bool throwException = false)
        {
            var valid = bool.TryParse(input, out bool result);
            if (valid) return result;
            if (throwException)
                throw new FormatException($"'{input}' cannot be converted as boolean");
            return result;
        }

        public static int ToInt(this string input, bool throwException = false)
        {
            var valid = int.TryParse(input, out int result);
            if (valid) return result;
            if (throwException)
                throw new FormatException($"'{input}' cannot be converted as int");
            return result;
        }

        
        public static bool IsZero(this int val)
        {
            return val.Equals(0);
        }

        public static bool IsFalse(this bool val)
        {
            return !val;
        }

        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        public static bool IsNullOrDbNull(this object obj)
        {
            return obj == null || obj == DBNull.Value;
        }

        public static void CloseReader(this IDataReader reader)
        {
            if (reader.IsNotNull() && !reader.IsClosed)
                reader.Close();
        }

        public static object MapField(this object value, Type type)
        {
            if (value.GetType() == type) return value;
            if (value == DBNull.Value) return default(object);
            var uType = Nullable.GetUnderlyingType(type);
            if (uType.IsNull()) uType = type;
            return Convert.ChangeType(value, uType);
        }

        public static string ToDbDate(this DateTime dateTime)
        {
            return dateTime.ToString(Util.DbDateFormat);
        }

        public static string ToDbDate(this string dateTime)
        {
            try
            {
                return DateTime.ParseExact(dateTime.Trim(), Util.DateFormat, CultureInfo.InvariantCulture).ToString(Util.DbDateFormat);
            }
            catch (Exception)
            {
                return DateTime.MinValue.ToString(Util.DbDateFormat);
            }
        }

        public static string ToDbDateTime(this DateTime dateTime)
        {
            return dateTime.ToString(Util.DateTimeFormat);
        }

        public static string ToDbDateTime(this string dateTime)
        {
            try
            {
                return DateTime.ParseExact(dateTime.Trim(), Util.DateFormat, CultureInfo.InvariantCulture).ToString(Util.DateTimeFormat);
            }
            catch (Exception)
            {
                return DateTime.MinValue.ToString(Util.DateTimeFormat);
            }
        }

        public static DateTime ToDbDateTime(this string dateTime, string dateFormat)
        {
            if (string.IsNullOrEmpty(dateTime)) return DateTime.MinValue;
            if (dateTime.Trim().Length <= 0) return DateTime.MinValue;
            try
            {
                var provider = CultureInfo.InvariantCulture;
                return DateTime.ParseExact(dateTime.Trim(), dateFormat, provider);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime ToDate(this string date, bool throwException = false)
        {
            var valid = DateTime.TryParse(date, out DateTime result);
            if (valid) return result;
            if (throwException)
                throw new FormatException($"{date} cannot be converted as DateTime");
            return result;
        }

        public static DateTime ToDateTime(this string dateTime, string dateFormat)
        {
            if (string.IsNullOrEmpty(dateTime)) return DateTime.MinValue;
            if (dateTime.Trim().Length <= 0) return DateTime.MinValue;
            try
            {
                return DateTime.ParseExact(dateTime.Trim(), dateFormat, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
    }
}