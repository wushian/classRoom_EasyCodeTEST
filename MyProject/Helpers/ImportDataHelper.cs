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
        /// �ˬd�פJ�� Excel ���.
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
            // �إ߸ӤW���ɮת� ����
            var targetFile = new FileInfo(fileName);
            // �P�_�ɮ׬O�_�s�b
            if (!targetFile.Exists)
            {
                result.ID = Guid.NewGuid();
                result.Success = false;
                result.ErrorCount = 0;
                result.ErrorMessage = "�פJ������ɮפ��s�b";
                return result;
            }

            var excelFile = new ExcelQueryFactory(fileName);

            // excel ����M �� table ���

            excelFile.AddMapping<potentialCustomer>(x => x.postcode, "�l���ϸ�");
            excelFile.AddMapping<potentialCustomer>(x => x.companyName, "���q�W��");
            excelFile.AddMapping<potentialCustomer>(x => x.address, "�a�}");
            excelFile.AddMapping<potentialCustomer>(x => x.tel, "�q��");
            excelFile.AddMapping<potentialCustomer>(x => x.contanter, "�p���H");
            excelFile.AddMapping<potentialCustomer>(x => x.comment, "�Ƶ�");
            excelFile.AddMapping<potentialCustomer>(x => x.filterReason, "�L�o��]");
            excelFile.AddMapping<potentialCustomer>(x => x.printType, "�Ȥ�����");
            //excelFile.AddMapping<potentialCustomer>(x => x.isPrint, "�O�_�C�L����");

            //���w�����W��
            var excelContent = excelFile.Worksheet<potentialCustomer>("��b�Ȥ�");

            int errorCount = 0;
            int rowIndex = 1;

            using (var db = new backup_reportContext())
            {
                // ���X�ثe�Ҧ��� ���q�W��
                companyList =db.potentialCustomers.OrderBy(x => x.companyName).Select(x => x.companyName).Distinct().ToList();
            }

                var importErrorMessages = new List<string>();
   
            //�ˬd���
            foreach (var row in excelContent)
            {
                var errorMessage = new StringBuilder();
                var _poCustomer = new potentialCustomer();
                //�P�_ excel ����ƬO�_���s�W
                if (!companyList.Contains(row.companyName))
                {
                    _poCustomer.id = Guid.NewGuid();
                    _poCustomer.createDate = DateTime.Today;
                }

                //�P�_ excel ����ƬO�_���ק�
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
                        errorMessage.Append("�a�} - ���i�ť�. ");
                    }
                    _poCustomer.address = row.address;

                    //CompanyName
                    if (string.IsNullOrWhiteSpace(row.companyName))
                    {
                        errorMessage.Append("���q�W�٦W�� - ���i�ť�. ");
                    }
                    _poCustomer.companyName = row.companyName;
                
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
                //���屼�������
                //using (var db = new backup_reportContext())
                //{
                //    foreach (var item in db.potentialCustomers.OrderBy(x => x.companyName))
                //    {
                //        db.potentialCustomers.Remove(item);
                //    }
                //    db.SaveChanges();
                //}

                //�u�N db �ثe�S�������q��� , �s���Ʈw
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
 
