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
    public class cat2Controller : Controller
    {
          //private string fileSavedPath = WebConfigurationManager.AppSettings["UploadPath"];

		private GenericUnitOfWork uow = null;
		// 宣告預設建構式
		public cat2Controller()
		{
		    this.uow = new GenericUnitOfWork();
		}

		public cat2Controller(GenericUnitOfWork uow_)
		{
		   this.uow = uow_;
		}

        private classRoomWebSiteDBContext db = new classRoomWebSiteDBContext();
        
        // GET: /cat2/
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
                ExportColumnAttributeHelper<cat2>.GetExportColumns()
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

            ViewData["SearchFields"] = GetFields((Session["SearchField"] == null ? "Id" : Convert.ToString(Session["SearchField"])));
            ViewData["SearchConditions"] = Library.GetConditions((Session["SearchCondition"] == null ? "Contains" : Convert.ToString(Session["SearchCondition"])));
            ViewData["SearchText"] = Session["SearchText"];
            ViewData["Exports"] = Library.GetExports((Session["Export"] == null ? "Pdf" : Convert.ToString(Session["Export"])));
            ViewData["PageSizes"] = Library.GetPageSizes();

            ViewData["CurrentSort"] = sortOrder;
            ViewData["idSortParm"] = sortOrder == "id_asc" ? "id_desc" : "id_asc";
            ViewData["nameSortParm"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";

            var Query = from _cat2 in db.cat2
                        select new cat2() {
                            id = _cat2.id
                           ,name = _cat2.name
                        };

            Query = db.cat2.AsQueryable();

            try {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["SearchField"])) && !string.IsNullOrEmpty(Convert.ToString(Session["SearchCondition"])) && !string.IsNullOrEmpty(Convert.ToString(Session["SearchText"])))
                {
                    SearchField = Convert.ToString(Session["SearchField"]);
                    SearchCondition = Convert.ToString(Session["SearchCondition"]);
                    SearchText = Convert.ToString(Session["SearchText"]);

                    if (SearchCondition == "Contains") {
                        Query = Query.Where(p => 
                                                 ("Id".ToString().Equals(SearchField) && p.id.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Name".ToString().Equals(SearchField) && p.name.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Starts with...") {
                        Query = Query.Where(p => 
                                                 ("Id".ToString().Equals(SearchField) && p.id.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Name".ToString().Equals(SearchField) && p.name.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Equals") {
                        if ("Id".Equals(SearchField)) { var mid = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.id == mid); }
                        else if ("Name".Equals(SearchField)) { var mname = System.Convert.ToString(SearchText); Query = Query.Where(p => p.name == mname); }
                    } else if (SearchCondition == "More than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Id".Equals(SearchField)) { var mid = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.id > mid); }
                    } else if (SearchCondition == "Less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Id".Equals(SearchField)) { var mid = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.id < mid); }
                    } else if (SearchCondition == "Equal or more than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Id".Equals(SearchField)) { var mid = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.id >= mid); }
                    } else if (SearchCondition == "Equal or less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Id".Equals(SearchField)) { var mid = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.id <= mid); }
                    }
                }
            } catch (Exception) { }

            switch (sortOrder)
            {
                case "id_desc":
                    Query = Query.OrderByDescending(s => s.id);
                    break;
                case "id_asc":
                    Query = Query.OrderBy(s => s.id);
                    break;
                case "name_desc":
                    Query = Query.OrderByDescending(s => s.name);
                    break;
                case "name_asc":
                    Query = Query.OrderBy(s => s.name);
                    break;
                default:  // Name ascending 
                    Query = Query.OrderBy(s => s.id);
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

        // GET: /cat2/Details/<id>
        public ActionResult Details(
                                      Int32? id
                                   )
        {
            if (
                    id == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cat2 cat2 = db.cat2.Find(
                                                 id
                                            );
            if (cat2 == null)
            {
                return HttpNotFound();
            }
            return View(cat2);
        }

        // GET: /cat2/Create
        public ActionResult Create()
        {
            var vm = new cat2();
            // vm.
            ViewData.Model = vm;
            return View();
        }

        // POST: /cat2/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include=
				           "name"
				  )] cat2 cat2)
        {
            if (ModelState.IsValid)
            {
                db.cat2.Add(cat2);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cat2);
        }

        // GET: /cat2/Edit/<id>
        public ActionResult Edit(
                                   Int32? id
                                )
        {
            if (
                    id == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cat2 cat2 = db.cat2.Find(
                                                 id
                                            );
            if (cat2 == null)
            {
                return HttpNotFound();
            }

            return View(cat2);
        }

        // POST: /cat2/Edit/<id>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include=
				     " id"
				   + ",name"
				  )] cat2 cat2)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cat2).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cat2);
        }

        // GET: /cat2/Delete/<id>
        public ActionResult Delete(
                                     Int32? id
                                  )
        {
            if (
                    id == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cat2 cat2 = db.cat2.Find(
                                                 id
                                            );
            if (cat2 == null)
            {
                return HttpNotFound();
            }
            return View(cat2);
        }

        // POST: /cat2/Delete/<id>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(
                                            Int32? id
                                            )
        {
            cat2 cat2 = db.cat2.Find(
                                                 id
                                            );
            db.cat2.Remove(cat2);
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
            SelectListItem Item1 = new SelectListItem { Text = "Id", Value = "Id" };
            SelectListItem Item2 = new SelectListItem { Text = "Name", Value = "Name" };

                 if (select == "Id") { Item1.Selected = true; }
            else if (select == "Name") { Item2.Selected = true; }

            list.Add(Item1);
            list.Add(Item2);

            return list.ToList();
        }



        public ActionResult Export(string fileName, string selectedColumns)
        {
            //取得原始資料
            var _exportData = Session["ExportData"] as IQueryable<cat2>;
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
                    SheetName = "cat2匯出資料",
                    FileName = exportFileName,
                    ExportData = dt
                };
            }
            else
            {
                return new ExportExcelResult
                {
                    SheetName = "id匯出資料",
                    FileName = exportFileName,
                    ExportData = null
                };
            }
        }




        public ActionResult HasData()
        {
            JObject jo = new JObject();
            bool result = !uow.Repository<cat2>().All().Count().Equals(0);
            jo.Add("Msg", result.ToString());
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
        
        
        private JArray GetExportData()
        {
            var threedays = DateTime.Now.AddDays(-3);
            var query = uow.Repository<cat2>().All();

            string str_json = JsonConvert.SerializeObject(query, Formatting.Indented);
            JArray jObjects = JsonConvert.DeserializeObject<JArray>(str_json.Trim());


            return jObjects;
        }





        private void ExportData(String Export, GridView gv, DataTable dt)
        {
            if (Export == "Pdf")
            {
                PDFform pdfForm = new PDFform(dt, "Dbo.Cat2", "Many");
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
 
