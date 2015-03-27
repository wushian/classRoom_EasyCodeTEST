using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using PagedList;
using PagedList.Mvc;
using System.Web.Configuration;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MyProject.Models;
using MyProject.Helpers;
using MyProject.UnitOfWork;
using MyProject.CustomResults;

namespace MyProject.Controllers
{
    public class SystemUserController : Controller
    {
          //private string fileSavedPath = WebConfigurationManager.AppSettings["UploadPath"];

		private GenericUnitOfWork uow = null;
		// 宣告預設建構式
		public SystemUserController()
		{
		    this.uow = new GenericUnitOfWork();
		}

		public SystemUserController(GenericUnitOfWork uow_)
		{
		   this.uow = uow_;
		}

        private classRoomWebSiteDBContext db = new classRoomWebSiteDBContext();
        
        // GET: /SystemUser/
        public ActionResult Index(string sortOrder,  
                                  String SearchField,
                                  String SearchCondition,
                                  String SearchText,
                                  String Export,
                                  int? PageSize,
                                  int? page, 
                                  string command)
        {

                     // 匯出資料欄位
            var exportColumns =
                     // 若指定 Auth_Manager 則能取出 partial class 的 Metadata attribute
                ExportColumnAttributeHelper<SystemUser>.GetExportColumns()
                    .Select(c => new SelectListItem()
                    {
                        Value = c.ColumnName,
                        Text = c.Name,
                        Selected = true
                    }).ToList();
            ViewBag.ExportColumns = exportColumns;


            if (command == "Show All") {
                SearchField = null;
                SearchCondition = null;
                SearchText = null;
                Session["SearchField"] = null;
                Session["SearchCondition"] = null;
                Session["SearchText"] = null; } 
            else if (command == "新增") { return RedirectToAction("Create"); } 
            else if (command == "Export") { Session["Export"] = Export; } 
            else if (command == "Search" | command == "Page Size") {
                if (!string.IsNullOrEmpty(SearchText)) {
                    Session["SearchField"] = SearchField;
                    Session["SearchCondition"] = SearchCondition;
                    Session["SearchText"] = SearchText; }
                } 
            if (command == "Page Size") { Session["PageSize"] = PageSize; }

            ViewData["SearchFields"] = GetFields((Session["SearchField"] == null ? "I D" : Convert.ToString(Session["SearchField"])));
            ViewData["SearchConditions"] = Library.GetConditions((Session["SearchCondition"] == null ? "Contains" : Convert.ToString(Session["SearchCondition"])));
            ViewData["SearchText"] = Session["SearchText"];
            ViewData["Exports"] = Library.GetExports((Session["Export"] == null ? "Pdf" : Convert.ToString(Session["Export"])));
            ViewData["PageSizes"] = Library.GetPageSizes();

            ViewData["CurrentSort"] = sortOrder;
            ViewData["IDSortParm"] = sortOrder == "ID_asc" ? "ID_desc" : "ID_asc";
            ViewData["NameSortParm"] = sortOrder == "Name_asc" ? "Name_desc" : "Name_asc";
            ViewData["AccountSortParm"] = sortOrder == "Account_asc" ? "Account_desc" : "Account_asc";
            ViewData["PasswordSortParm"] = sortOrder == "Password_asc" ? "Password_desc" : "Password_asc";
            ViewData["SaltSortParm"] = sortOrder == "Salt_asc" ? "Salt_desc" : "Salt_asc";
            ViewData["EmailSortParm"] = sortOrder == "Email_asc" ? "Email_desc" : "Email_asc";
            ViewData["CreateDateSortParm"] = sortOrder == "CreateDate_asc" ? "CreateDate_desc" : "CreateDate_asc";
            ViewData["UpdateDateSortParm"] = sortOrder == "UpdateDate_asc" ? "UpdateDate_desc" : "UpdateDate_asc";

            var Query = from _SystemUser in db.SystemUser
                        select new SystemUser() {
                            ID = _SystemUser.ID
                           ,Name = _SystemUser.Name
                           ,Account = _SystemUser.Account
                           ,Password = _SystemUser.Password
                           ,Salt = _SystemUser.Salt
                           ,Email = _SystemUser.Email
                           ,CreateDate = _SystemUser.CreateDate
                           ,UpdateDate = _SystemUser.UpdateDate
                        };

            Query = db.SystemUser.AsQueryable();

            try {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["SearchField"])) && !string.IsNullOrEmpty(Convert.ToString(Session["SearchCondition"])) && !string.IsNullOrEmpty(Convert.ToString(Session["SearchText"])))
                {
                    SearchField = Convert.ToString(Session["SearchField"]);
                    SearchCondition = Convert.ToString(Session["SearchCondition"]);
                    SearchText = Convert.ToString(Session["SearchText"]);

                    if (SearchCondition == "Contains") {
                        Query = Query.Where(p => 
                                                 ("I D".ToString().Equals(SearchField) && p.ID.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Name".ToString().Equals(SearchField) && p.Name.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Account".ToString().Equals(SearchField) && p.Account.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Password".ToString().Equals(SearchField) && p.Password.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Salt".ToString().Equals(SearchField) && p.Salt.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Email".ToString().Equals(SearchField) && p.Email.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Create Date".ToString().Equals(SearchField) && p.CreateDate.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Update Date".ToString().Equals(SearchField) && p.UpdateDate.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Starts with...") {
                        Query = Query.Where(p => 
                                                 ("I D".ToString().Equals(SearchField) && p.ID.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Name".ToString().Equals(SearchField) && p.Name.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Account".ToString().Equals(SearchField) && p.Account.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Password".ToString().Equals(SearchField) && p.Password.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Salt".ToString().Equals(SearchField) && p.Salt.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Email".ToString().Equals(SearchField) && p.Email.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Create Date".ToString().Equals(SearchField) && p.CreateDate.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Update Date".ToString().Equals(SearchField) && p.UpdateDate.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Equals") {
                         if ("Name".Equals(SearchField)) { var mName = System.Convert.ToString(SearchText); Query = Query.Where(p => p.Name == mName); }
                        else if ("Account".Equals(SearchField)) { var mAccount = System.Convert.ToString(SearchText); Query = Query.Where(p => p.Account == mAccount); }
                        else if ("Password".Equals(SearchField)) { var mPassword = System.Convert.ToString(SearchText); Query = Query.Where(p => p.Password == mPassword); }
                        else if ("Salt".Equals(SearchField)) { var mSalt = System.Convert.ToString(SearchText); Query = Query.Where(p => p.Salt == mSalt); }
                        else if ("Email".Equals(SearchField)) { var mEmail = System.Convert.ToString(SearchText); Query = Query.Where(p => p.Email == mEmail); }
                        else if ("Create Date".Equals(SearchField)) { var mCreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.CreateDate == mCreateDate); }
                        else if ("Update Date".Equals(SearchField)) { var mUpdateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.UpdateDate == mUpdateDate); }
                    } else if (SearchCondition == "More than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Create Date".Equals(SearchField)) { var mCreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.CreateDate > mCreateDate); }
                        else if ("Update Date".Equals(SearchField)) { var mUpdateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.UpdateDate > mUpdateDate); }
                    } else if (SearchCondition == "Less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Create Date".Equals(SearchField)) { var mCreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.CreateDate < mCreateDate); }
                        else if ("Update Date".Equals(SearchField)) { var mUpdateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.UpdateDate < mUpdateDate); }
                    } else if (SearchCondition == "Equal or more than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Create Date".Equals(SearchField)) { var mCreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.CreateDate >= mCreateDate); }
                        else if ("Update Date".Equals(SearchField)) { var mUpdateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.UpdateDate >= mUpdateDate); }
                    } else if (SearchCondition == "Equal or less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Create Date".Equals(SearchField)) { var mCreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.CreateDate <= mCreateDate); }
                        else if ("Update Date".Equals(SearchField)) { var mUpdateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.UpdateDate <= mUpdateDate); }
                    }
                }
            } catch (Exception) { }

            switch (sortOrder)
            {
                case "ID_desc":
                    Query = Query.OrderByDescending(s => s.ID);
                    break;
                case "ID_asc":
                    Query = Query.OrderBy(s => s.ID);
                    break;
                case "Name_desc":
                    Query = Query.OrderByDescending(s => s.Name);
                    break;
                case "Name_asc":
                    Query = Query.OrderBy(s => s.Name);
                    break;
                case "Account_desc":
                    Query = Query.OrderByDescending(s => s.Account);
                    break;
                case "Account_asc":
                    Query = Query.OrderBy(s => s.Account);
                    break;
                case "Password_desc":
                    Query = Query.OrderByDescending(s => s.Password);
                    break;
                case "Password_asc":
                    Query = Query.OrderBy(s => s.Password);
                    break;
                case "Salt_desc":
                    Query = Query.OrderByDescending(s => s.Salt);
                    break;
                case "Salt_asc":
                    Query = Query.OrderBy(s => s.Salt);
                    break;
                case "Email_desc":
                    Query = Query.OrderByDescending(s => s.Email);
                    break;
                case "Email_asc":
                    Query = Query.OrderBy(s => s.Email);
                    break;
                case "CreateDate_desc":
                    Query = Query.OrderByDescending(s => s.CreateDate);
                    break;
                case "CreateDate_asc":
                    Query = Query.OrderBy(s => s.CreateDate);
                    break;
                case "UpdateDate_desc":
                    Query = Query.OrderByDescending(s => s.UpdateDate);
                    break;
                case "UpdateDate_asc":
                    Query = Query.OrderBy(s => s.UpdateDate);
                    break;
                default:  // Name ascending 
                    Query = Query.OrderBy(s => s.ID);
                    break;
            }

            if (command == "Export") {
                GridView gv = new GridView();
                gv.DataSource = Query.ToList(); 
                gv.DataBind();
                DataTable dt = Library.ToDataTable(Query.ToList());
                ExportData(Export, gv, dt);
            }

            int pageNumber = (page ?? 1);
            int? pageSZ = (Convert.ToInt32(Session["PageSize"]) == 0 ? 5 : Convert.ToInt32(Session["PageSize"]));
            return View(Query.ToPagedList(pageNumber, (pageSZ ?? 5)));
        }

        // GET: /SystemUser/Details/<id>
        public ActionResult Details(
                                      Guid ID
                                   )
        {
            if (
                    ID == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemUser SystemUser = db.SystemUser.Find(
                                                 ID
                                            );
            if (SystemUser == null)
            {
                return HttpNotFound();
            }
            return View(SystemUser);
        }

        // GET: /SystemUser/Create
        public ActionResult Create()
        {
            var vm = new SystemUser();
            // vm.
            ViewData.Model = vm;
            return View();
        }

        // POST: /SystemUser/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include=
				           "Name"
				   + "," + "Account"
				   + "," + "Password"
				   + "," + "Salt"
				   + "," + "Email"
				   + "," + "CreateDate"
				   + "," + "UpdateDate"
				  )] SystemUser SystemUser)
        {
            if (ModelState.IsValid)
            {
                SystemUser.ID = Guid.NewGuid();
                db.SystemUser.Add(SystemUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(SystemUser);
        }

        // GET: /SystemUser/Edit/<id>
        public ActionResult Edit(
                                   Guid ID
                                )
        {
            if (
                    ID == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemUser SystemUser = db.SystemUser.Find(
                                                 ID
                                            );
            if (SystemUser == null)
            {
                return HttpNotFound();
            }

            return View(SystemUser);
        }

        // POST: /SystemUser/Edit/<id>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include=
				     " ID"
				   + ",Name"
				   + ",Account"
				   + ",Password"
				   + ",Salt"
				   + ",Email"
				   + ",CreateDate"
				   + ",UpdateDate"
				  )] SystemUser SystemUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(SystemUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(SystemUser);
        }

        // GET: /SystemUser/Delete/<id>
        public ActionResult Delete(
                                     Guid ID
                                  )
        {
            if (
                    ID == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SystemUser SystemUser = db.SystemUser.Find(
                                                 ID
                                            );
            if (SystemUser == null)
            {
                return HttpNotFound();
            }
            return View(SystemUser);
        }

        // POST: /SystemUser/Delete/<id>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(
                                            Guid ID
                                            )
        {
            SystemUser SystemUser = db.SystemUser.Find(
                                                 ID
                                            );
            db.SystemUser.Remove(SystemUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private static List<SelectListItem> GetFields(String select)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem Item1 = new SelectListItem { Text = "I D", Value = "I D" };
            SelectListItem Item2 = new SelectListItem { Text = "Name", Value = "Name" };
            SelectListItem Item3 = new SelectListItem { Text = "Account", Value = "Account" };
            SelectListItem Item4 = new SelectListItem { Text = "Password", Value = "Password" };
            SelectListItem Item5 = new SelectListItem { Text = "Salt", Value = "Salt" };
            SelectListItem Item6 = new SelectListItem { Text = "Email", Value = "Email" };
            SelectListItem Item7 = new SelectListItem { Text = "Create Date", Value = "Create Date" };
            SelectListItem Item8 = new SelectListItem { Text = "Update Date", Value = "Update Date" };

                 if (select == "I D") { Item1.Selected = true; }
            else if (select == "Name") { Item2.Selected = true; }
            else if (select == "Account") { Item3.Selected = true; }
            else if (select == "Password") { Item4.Selected = true; }
            else if (select == "Salt") { Item5.Selected = true; }
            else if (select == "Email") { Item6.Selected = true; }
            else if (select == "Create Date") { Item7.Selected = true; }
            else if (select == "Update Date") { Item8.Selected = true; }

            list.Add(Item1);
            list.Add(Item2);
            list.Add(Item3);
            list.Add(Item4);
            list.Add(Item5);
            list.Add(Item6);
            list.Add(Item7);
            list.Add(Item8);

            return list.ToList();
        }



        public ActionResult Export(string fileName, string selectedColumns)
        {
            //取得原始資料
            var _exportData = Session["ExportData"] as IQueryable<SystemUser>;
                            //決定匯出 Excel 檔案的檔名
                var exportFileName = string.IsNullOrWhiteSpace(fileName)
                    ? string.Concat(
                        "CustomerData_",
                        DateTime.Now.ToString("yyyyMMddHHmmss"),
                        ".xlsx")
                    : string.Concat(fileName, ".xlsx");
            if (_exportData != null)
            {
                var exportData = _exportData;


                //使用轉換後的匯出資料與使用者所選的匯出欄位，產生作為匯出 Excel 的 DataTable.
                var dt = ExportDataHelper.GetExportDataTable(exportData, selectedColumns);


                return new ExportExcelResult
                {
                    SheetName = "SystemUser匯出資料",
                    FileName = exportFileName,
                    ExportData = dt
                };
            }
            else
            {
                return new ExportExcelResult
                {
                    SheetName = "ID匯出資料",
                    FileName = exportFileName,
                    ExportData = null
                };
            }
        }




        public ActionResult HasData()
        {
            JObject jo = new JObject();
            bool result = !uow.Repository<SystemUser>().All().Count().Equals(0);
            jo.Add("Msg", result.ToString());
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
        
        
        private JArray GetExportData()
        {
            var threedays = DateTime.Now.AddDays(-3);
            var query = uow.Repository<SystemUser>().All();

            string str_json = JsonConvert.SerializeObject(query, Formatting.Indented);
            JArray jObjects = JsonConvert.DeserializeObject<JArray>(str_json.Trim());


            return jObjects;
        }





        private void ExportData(String Export, GridView gv, DataTable dt)
        {
            if (Export == "Pdf")
            {
                PDFform pdfForm = new PDFform(dt, "Dbo. System User", "Many");
                Document document = pdfForm.CreateDocument();
                PdfDocumentRenderer renderer = new PdfDocumentRenderer(true);
                renderer.Document = document;
                renderer.RenderDocument();

                MemoryStream stream = new MemoryStream();
                renderer.PdfDocument.Save(stream, false);

                Response.Clear();
                Response.AddHeader("content-disposition", "attachment;filename=" + "Report.pdf");
                Response.ContentType = "application/Pdf.pdf";
                Response.BinaryWrite(stream.ToArray());
                Response.Flush();
                Response.End();
            }
            else
            {
                Response.ClearContent();
                Response.Buffer = true;
                if (Export == "Excel")
                {
                    Response.AddHeader("content-disposition", "attachment;filename=" + "Report.xls");
                    Response.ContentType = "application/Excel.xls";
                }
                else if (Export == "Word")
                {
                    Response.AddHeader("content-disposition", "attachment;filename=" + "Report.doc");
                    Response.ContentType = "application/Word.doc";
                }
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
        }

    }
}
 
