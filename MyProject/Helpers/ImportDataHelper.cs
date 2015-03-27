using System;
using System.Collections.Generic;
?using System.Data.Entity;
?using System.IO;
?using System.Linq;
?using System.Text;
?using DocumentFormat.OpenXml.Office.CustomUI;
?using MyProject.Models;
using LinqToExcel;


namespace MyProject.Helpers
{
    public class ImportDataHelper
    {
        /// <summary>
        /// 檢查匯入的 Excel 資料.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="importData">The import zip codes.</param>
        /// <returns></returns>
        /// 

        List<string> companyList = new List<string>();
         
        public CheckResult CheckImportData(
            string fileName,
            List<potentialCustomer> importData)
        {
            var result = new CheckResult();
            // 建立該上傳檔案的 物件
            var targetFile = new FileInfo(fileName);
            // 判斷檔案是否存在
            if (!targetFile.Exists)
            {
                result.ID = Guid.NewGuid();
                result.Success = false;
                result.ErrorCount = 0;
                result.ErrorMessage = "匯入的資料檔案不存在";
                return result;
            }

            var excelFile = new ExcelQueryFactory(fileName);

            // excel 欄位對映 到 table 欄位

            excelFile.AddMapping<potentialCustomer>(x => x.postcode, "郵遞區號");
            excelFile.AddMapping<potentialCustomer>(x => x.companyName, "公司名稱");
            excelFile.AddMapping<potentialCustomer>(x => x.address, "地址");
            excelFile.AddMapping<potentialCustomer>(x => x.tel, "電話");
            excelFile.AddMapping<potentialCustomer>(x => x.contanter, "聯絡人");
            excelFile.AddMapping<potentialCustomer>(x => x.comment, "備註");
            excelFile.AddMapping<potentialCustomer>(x => x.filterReason, "過濾原因");
            excelFile.AddMapping<potentialCustomer>(x => x.printType, "客戶類型");
            //excelFile.AddMapping<potentialCustomer>(x => x.isPrint, "是否列印標籤");

            //指定分頁名稱
            var excelContent = excelFile.Worksheet<potentialCustomer>("潛在客戶");

            int errorCount = 0;
            int rowIndex = 1;

            using (var db = new backup_reportContext())
            {
                // 取出目前所有的 公司名稱
                companyList =db.potentialCustomers.OrderBy(x => x.companyName).Select(x => x.companyName).Distinct().ToList();
            }

                var importErrorMessages = new List<string>();
   
            //檢查資料
            foreach (var row in excelContent)
            {
                var errorMessage = new StringBuilder();
                var _poCustomer = new potentialCustomer();
                //判斷 excel 欄位資料是否為新增
                if (!companyList.Contains(row.companyName))
                {
                    _poCustomer.id = Guid.NewGuid();
                    _poCustomer.createDate = DateTime.Today;
                }

                //判斷 excel 欄位資料是否為修改
                if (companyList.Contains(row.companyName))
                {
                    _poCustomer.modifyDate = DateTime.Now;
                    if (row.isPrint != null || row.filterReason != null || row.printType != null)
                    {
                        _poCustomer.manModifyDate = DateTime.Now;
                    }
                }


                    _poCustomer.postcode = row.postcode;
                    //_poCustomer.companyName = row.companyName;
                    _poCustomer.tel = row.tel;
                    _poCustomer.contanter = row.contanter;
                    _poCustomer.comment = row.comment;
                    _poCustomer.filterReason = row.filterReason;
                    _poCustomer.printType = row.printType;
                    //_poCustomer.isPrint = row.isPrint;

                    //Address
                    if (string.IsNullOrWhiteSpace(row.address))
                    {
                        errorMessage.Append("地址 - 不可空白. ");
                    }
                    _poCustomer.address = row.address;

                    //CompanyName
                    if (string.IsNullOrWhiteSpace(row.companyName))
                    {
                        errorMessage.Append("公司名稱名稱 - 不可空白. ");
                    }
                    _poCustomer.companyName = row.companyName;
                
                //=============================================================================
                if (errorMessage.Length > 0)
                {
                    errorCount += 1;
                    importErrorMessages.Add(string.Format(
                        "第 {0} 列資料發現錯誤：{1}{2}",
                        rowIndex,
                        errorMessage,
                        "<br/>"));
                }
                importData.Add(_poCustomer);
                rowIndex += 1;
            }

            try
            {
                result.ID = Guid.NewGuid();
                result.Success = errorCount.Equals(0);
                result.RowCount = importData.Count;
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
        /// <param name="importData">The import zip codes.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SaveImportData(IEnumerable<potentialCustomer> importData)
        {
            try
            {
                //先砍掉全部資料
                //using (var db = new backup_reportContext())
                //{
                //    foreach (var item in db.potentialCustomers.OrderBy(x => x.companyName))
                //    {
                //        db.potentialCustomers.Remove(item);
                //    }
                //    db.SaveChanges();
                //}

                //只將 db 目前沒有的公司資料 , 存到資料庫
                using (var db = new backup_reportContext())
                {
                    foreach (var item in importData)
                    {
                        if (! companyList.Contains(item.companyName))
                        {
                            item.id = Guid.NewGuid();
                            db.Entry(item).State=EntityState.Added;
                            db.potentialCustomers.Add(item);
                        }
                    }
                    db.GetValidationErrors();
                    db.SaveChanges();
                    
                }
            }
            catch (Exception ex)
            {
                throw  ;
            }
        }
    }
}
 
