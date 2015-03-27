using MyProject.Helpers;

namespace MyProject.Helpers
{
    using MimeKit;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;
    using MyProject.Models;

    public class ImportMailHelper
    {
        public CheckResult CheckImportData(string fileName, Mail importMail, string fileSavedPath)
        {
            string str = Path.Combine(HttpContext.Current.Server.MapPath(fileSavedPath), fileName);
            CheckResult result = new CheckResult();
            StringBuilder builder = new StringBuilder();
            FileInfo info = new FileInfo(str);
            if (!info.Exists)
            {
                result.ID = Guid.NewGuid();
                result.Success = false;
                result.ErrorCount = 0;
                result.ErrorMessage = "匯入的資料檔案不存在";
                return result;
            }
            string str2 = (info.Length / 1024) + "K";
            MimeMessage message = MimeMessage.Load(str, new CancellationToken());
            List<MimePart> list = new List<MimePart>();
            List<Multipart> list2 = new List<Multipart>();
            new List<TextPart>();
            MimeIterator iterator = new MimeIterator(message);
            string path = fileSavedPath + "/" + message.MessageId;
            string str4 = HttpContext.Current.Server.MapPath(path);
            if (!Directory.Exists(str4))
            {
                Directory.CreateDirectory(str4);
            }
            StringBuilder builder2 = new StringBuilder();
            int num = 0;
            while (iterator.MoveNext())
            {
                Multipart parent = iterator.Parent as Multipart;
                MimePart current = iterator.Current as MimePart;
                if (((parent != null) && (current != null)) && current.IsAttachment)
                {
                    list2.Add(parent);
                    list.Add(current);
                }
                if ((((current != null) && !current.IsAttachment) && (string.IsNullOrEmpty(current.FileName) && ((current as TextPart).ContentType.MediaType == "text"))) && (num < 1))
                {
                    string htmlSource = (current as TextPart).Text.Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("&nbsp;", string.Empty);
                    if (builder2.GetHashCode() != htmlSource.GetHashCode())
                    {
                        builder2.Append(RemoveHTMLTag(htmlSource));
                        num++;
                    }
                }
            }
            StringBuilder builder3 = new StringBuilder();
            foreach (MimePart part2 in list)
            {
                builder3.Append(part2.FileName);
            }
            string str6 = (builder2.ToString().Length >= 500) ? builder2.ToString().Substring(0, 500) : builder2.ToString();
            importMail.id = Guid.NewGuid();
            importMail.from = message.From.First<InternetAddress>().ToString();
            importMail.to = message.To.ToString();
            importMail.bcc = message.Bcc.Count.ToString();
            importMail.createdate = message.Date.DateTime;
            importMail.content = str6;
            importMail.subject = message.Subject;
            importMail.size = str2;
            importMail.attachment = builder3.ToString();
            foreach (MimePart part3 in list)
            {
                string str7 = part3.FileName;
                using (FileStream stream = File.Create(str4 + "/" + str7))
                {
                    CancellationToken cancellationToken = new CancellationToken();
                    part3.ContentObject.DecodeTo(stream, cancellationToken);
                }
            }
            int num2 = 0;
            if (string.IsNullOrEmpty(importMail.from))
            {
                builder.Append("未指定寄件者");
                num2 += num2;
            }
            if (string.IsNullOrEmpty(importMail.to))
            {
                builder.Append("未指定收件者");
                num2 += num2;
            }
            try
            {
                result.ID = Guid.NewGuid();
                result.Success = num2.Equals(0);
                result.RowCount = 1;
                result.ErrorCount = num2;
                result.ErrorMessage = "";
                File.Delete(str);
                return result;
            }
            catch (Exception exception)
            {
                result.ID = Guid.NewGuid();
                result.Success = num2.Equals(0);
                result.RowCount = 1;
                result.ErrorCount = num2;
                result.ErrorMessage = exception.Message;
                return result;
            }
        }

        public static string RemoveHTMLTag(string htmlSource)
        {
            return Regex.Replace(Regex.Replace(Regex.Replace(htmlSource, "<.*?>", string.Empty), @"//\~=", string.Empty), "<.*?>", string.Empty);
        }

        public void SaveImportData(Mail importMail)
        {
            try
            {
                using (backup_reportContext context = new backup_reportContext())
                {
                    context.Mails.Add(importMail);
                    context.GetValidationErrors();
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ;
            }
        }
    }
}
 
