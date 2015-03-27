using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MyProject.Models;

namespace MyProject.Helpers
{
    public class ExportDataHelper
    {
        public static List<string> GetRemoveColumnNames(
            Dictionary<string, string> allColumns,
            string selectedColumns)
        {
            var allColumnValues = allColumns.Select(x => x.Key).ToList();
            var columns = selectedColumns.Split(',').ToList();

            //�������Q��������
            var removeColumns = allColumnValues.Except(columns).ToList();
            return removeColumns;
        }

        /// <summary>
        /// Gets the export data table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selectedColumns">The selected columns.</param>
        /// <returns></returns>
        public static DataTable GetExportDataTable<T>(
            IEnumerable<T> source,
            string selectedColumns) where T : class
        {
            var exportSource =
                GetExportDataFromSource(source, selectedColumns);

            var deserializerSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = Newtonsoft.Json.DateParseHandling.None,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local
            };

            var exportData =
                JsonConvert.DeserializeObject<DataTable>(exportSource.ToString(),deserializerSettings);

            return exportData;
        }

        /// <summary>
        /// Gets the export data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selectedColumns">The selected columns.</param>
        /// <returns></returns>
        private static JArray GetExportDataFromSource<T>(
            IEnumerable<T> source,
            string selectedColumns) where T : class
        {
            var jObjects = new JArray();

            var columns = selectedColumns.Split(',');
              //�o�̪� T ���ӧ令 Auth_Manager.Auth_ManagerMetadata
            var exportColumns = ExportColumnAttributeHelper<T>
                .GetExportColumns()
                .Where(x=>columns.Contains(x.ColumnName))
                .ToList();

            

            foreach (var item in source)
            {
                Type type = typeof(T);
                var pInfos = type.GetProperties();

                var rowDatas = new List<Tuple<int, string, object>>();

                foreach (var pinfo in pInfos)
                {
                    var cInfo = exportColumns.FirstOrDefault(x => x.ColumnName == pinfo.Name);
                    if (cInfo == null) continue;

                    rowDatas.Add(new Tuple<int, string, object>
                    (
                        item1: cInfo.Order,
                        item2: cInfo.Name,
                        item3: pinfo.GetValue(item) ?? ""
                    ));
                }

                var jo = new JObject();
                foreach (var row in rowDatas.OrderBy(x => x.Item1))
                {
                    jo.Add(row.Item2, JToken.FromObject(row.Item3));
                }
                jObjects.Add(jo);
            }

            return jObjects;
        }
    }
}
 
