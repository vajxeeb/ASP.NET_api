using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace Library.Core.Common
{
    public static class DataUtil
    {
        public static DataTable ToDataTable(IList data)
        {
            var itemType = data.GetType().GetGenericArguments()[0];
            var props = TypeDescriptor.GetProperties(itemType);
            var table = new DataTable();
            for (var i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            var values = new object[props.Count];
            foreach (var item in data)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        private static DataTable CreateDataTable<T>() where T : class
        {
            var objType = typeof(T);
            var table = new DataTable(objType.Name);
            var properties = TypeDescriptor.GetProperties(objType);
            foreach (PropertyDescriptor property in properties)
            {
                var propertyType = property.PropertyType;
                if (!CanUseType(propertyType)) continue; //shallow only

                //nullables must use underlying types
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                //enums also need special treatment
                if (propertyType.IsEnum)
                    propertyType = Enum.GetUnderlyingType(propertyType); //probably Int32
                //if you have nested application classes, they just get added. Check if this is valid?
                Debug.WriteLine("table.Columns.Add(\"" + property.Name + "\", typeof(" + propertyType.Name + "));");
                table.Columns.Add(property.Name, propertyType);
            }
            return table;
        }

        public static DataTable ConvertToDataTable<T>(ICollection<T> collection) where T : class
        {
            var table = CreateDataTable<T>();
            var objType = typeof(T);
            var properties = TypeDescriptor.GetProperties(objType);
            //Debug.WriteLine("foreach (" + objType.Name + " item in collection) {");
            foreach (T item in collection)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor property in properties)
                {
                    if (!CanUseType(property.PropertyType)) continue; //shallow only
                    //Debug.WriteLine("row[\"" + property.Name + "\"] = item." + property.Name + "; //.HasValue ? (object)item." + property.Name + ": DBNull.Value;");
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value; //can't use null
                }
                //Debug.WriteLine("//===");
                table.Rows.Add(row);
            }
            return table;
        }

        private static bool CanUseType(Type propertyType)
        {
            //only strings and value types
            if (propertyType.IsArray) return false;
            if (!propertyType.IsValueType && propertyType != typeof(string)) return false;
            return true;
        }

        public static ICollection<T> ConvertToCollection<T>(DataTable dt) where T : class, new()
        {
            if (dt == null || dt.Rows.Count == 0) return null;
            ICollection<T> collection = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                var obj = ConvertDataRowToEntity<T>(row);
                collection.Add(obj);
            }
            return collection;
        }

        public static T ConvertDataRowToEntity<T>(DataRow row) where T : class, new()
        {
            var objType = typeof(T);
            var obj = Activator.CreateInstance<T>(); //hence the new() contsraint
            //Debug.WriteLine(objType.Name + " = new " + objType.Name + "();");
            foreach (DataColumn column in row.Table.Columns)
            {
                //may error if no match
                var property =
                    objType.GetProperty(column.ColumnName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null || !property.CanWrite)
                {
                    //Debug.WriteLine("//Property " + column.ColumnName + " not in object");
                    continue; //or throw
                }
                var value = row[column.ColumnName];
                if (value == DBNull.Value) value = null;
                property.SetValue(obj, value, null);
                //Debug.WriteLine("obj." + property.Name + " = row[\"" + column.ColumnName + "\"];");
            }
            return obj;
        }

        public delegate T OverrideModel<out T>(Dictionary<string, object> fields);

        public static DataSet DataReaderToDataSet(IDataReader reader)
        {
            var dataSet = new DataSet();
            do
            {
                var fieldCount = reader.FieldCount;
                var table = new DataTable();
                for (var i = 0; i < fieldCount; i++)
                {
                    table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                }
                table.BeginLoadData();
                var values = new object[fieldCount];
                while (reader.Read())
                {
                    reader.GetValues(values);
                    table.LoadDataRow(values, true);
                }
                table.EndLoadData();
                dataSet.Tables.Add(table);
            } while (reader.NextResult());
            reader.Close();
            return dataSet;
        }

        public static DataTable DataReaderToDataTable(IDataReader reader)
        {
            var table = new DataTable();
            do
            {
                var fieldCount = reader.FieldCount;
                for (var i = 0; i < fieldCount; i++)
                {
                    table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                }
                table.BeginLoadData();
                var values = new object[fieldCount];
                while (reader.Read())
                {
                    reader.GetValues(values);
                    table.LoadDataRow(values, true);
                }
                table.EndLoadData();
            } while (reader.NextResult());
            reader.Close();
            return table;
        }
    }
}