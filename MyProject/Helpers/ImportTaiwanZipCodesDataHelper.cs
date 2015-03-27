using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using MyProject.Models;
using LinqToExcel;

namespace MyProject.Helpers
{
    public class ImportTaiwanZipCodesDataHelper
    {
        /// <summary>
        /// �ˬd�פJ�� Excel ���.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="importZipCodes">The import zip codes.</param>
        /// <returns></returns>
        public CheckResult CheckImportData(
            string fileName,
            List<TaiwanZipCode> importZipCodes)
        {
            var result = new CheckResult();

            var targetFile = new FileInfo(fileName);

            if (!targetFile.Exists)
            {
                result.ID = Guid.NewGuid();
                result.Success = false;
                result.ErrorCount = 0;
                result.ErrorMessage = "�פJ������ɮפ��s�b";
                return result;
            }

            var excelFile = new ExcelQueryFactory(fileName);

            //����M
            excelFile.AddMapping<TaiwanZipCode>(x => x.ID, "ID");
            excelFile.AddMapping<TaiwanZipCode>(x => x.Zip, "Zip");
            excelFile.AddMapping<TaiwanZipCode>(x => x.CityName, "CityName");
            excelFile.AddMapping<TaiwanZipCode>(x => x.Town, "Town");
            excelFile.AddMapping<TaiwanZipCode>(x => x.Sequence, "Sequence");

            //SheetName
            var excelContent = excelFile.Worksheet<TaiwanZipCode>("�O�W�l���ϸ�");

            int errorCount = 0;
            int rowIndex = 1;
            var importErrorMessages = new List<string>();

            //�ˬd���
            foreach (var row in excelContent)
            {
                var errorMessage = new StringBuilder();
                var zipCode = new TaiwanZipCode();

                zipCode.ID = row.ID;
                zipCode.Sequence = row.Sequence;
                zipCode.Zip = row.Zip;
                zipCode.CreateDate = DateTime.Now;

                //CityName
                if (string.IsNullOrWhiteSpace(row.CityName))
                {
                    errorMessage.Append("�����W�� - ���i�ť�. ");
                }
                zipCode.CityName = row.CityName;

                //Town
                if (string.IsNullOrWhiteSpace(row.Town))
                {
                    errorMessage.Append("�m���ϦW�� - ���i�ť�. ");
                }
                zipCode.Town = row.Town;

                //=============================================================================
                if (errorMessage.Length > 0)
                {
                    errorCount += 1;
                    importErrorMessages.Add(string.Format(
                        "�� {0} �C��Ƶo�{���~�G{1}{2}",
                        rowIndex,
                        errorMessage,
                        "<br/>"));
                }
                importZipCodes.Add(zipCode);
                rowIndex += 1;
            }

            try
            {
                result.ID = Guid.NewGuid();
                result.Success = errorCount.Equals(0);
                result.RowCount = importZipCodes.Count;
                result.ErrorCount = errorCount;

                string allErrorMessage = string.Empty;

                foreach (var message in importErrorMessages)
                {
                    allErrorMessage += message;
                }

                result.ErrorMessage = allErrorMessage;

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        /// <summary>
        /// Saves the import data.
        /// </summary>
        /// <param name="importZipCodes">The import zip codes.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SaveImportData(IEnumerable<TaiwanZipCode> importZipCodes)
        {
            try
            {
                //���屼�������
                using (var db = new backup_reportContext())
                {
                    foreach (var item in db.TaiwanZipCodes.OrderBy(x => x.ID))
                    {
                        db.TaiwanZipCodes.Remove(item);
                    }
                    db.SaveChanges();
                }

                //�A��פJ����Ƶ��s���Ʈw
                using (var db = new backup_reportContext())
                {
                    foreach (var item in importZipCodes)
                    {
                        db.TaiwanZipCodes.Add(item);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
 
