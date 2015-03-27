using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.ComponentModel.DataAnnotations;
using MyProject.Models;
using MyProject.Filters;


namespace MyProject.Helpers
{
    public class ExportColumnAttributeHelper<T> where T : class
    {
        public static List<ExportColumnObject> GetExportColumns()
        {
            // ���X Metadataattribute ���]�w�� ���ର �A�����O
            var _attr = typeof (T).GetCustomAttributes(typeof(MetadataTypeAttribute),true)[0];
            var metadata = (_attr as MetadataTypeAttribute).MetadataClassType;

            var infos = metadata.GetProperties();

            var result = infos.Select(x => x.Name)
                              .Select(propertyName => GetProperty(metadata, propertyName))
                              .Select(GetExportColumnInstance)
                              .Where(instance => instance != null)
                              .ToList();

            return result.OrderBy(x => x.Order).ToList();
        }

        /// <summary>
        /// Gets the export column instance.
        /// </summary>
        /// <param name="pInfo">The p information.</param>
        /// <returns></returns>
        public static ExportColumnObject GetExportColumnInstance(PropertyInfo pInfo)
        {
            if (null == pInfo) return null;
            try
            {
                // GetCustomAttribute �u����X current property �� attribute , �õL�k���� partial class �� �]�w��
                var arr = pInfo.GetCustomAttributes(typeof(ExportColumnAttribute), true);

                if (arr.Length.Equals(0)) return null;

                var attr = arr[0] as ExportColumnAttribute;

                var instance = new ExportColumnObject()
                {
                    ColumnName = pInfo.Name,
                    Name = attr.Name,
                    Order = attr.Order
                };

                return instance;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propName">Name of the property.</param>
        /// <returns></returns>
        private static PropertyInfo GetProperty(Type type, string propName)
        {
            try
            {
                var infos = type.GetProperties()
                                .Where(info => propName.ToLower().Equals(info.Name.ToLower()));

                foreach (var info in infos)
                {
                    return info;
                }
            }
            catch
            {
                throw;
            }
            return null;
        }
    }
}
 
