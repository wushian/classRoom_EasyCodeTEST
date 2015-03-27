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
    public class courseIntroController : Controller
    {
          //private string fileSavedPath = WebConfigurationManager.AppSettings["UploadPath"];

		private GenericUnitOfWork uow = null;
		// 宣告預設建構式
		public courseIntroController()
		{
		    this.uow = new GenericUnitOfWork();
		}

		public courseIntroController(GenericUnitOfWork uow_)
		{
		   this.uow = uow_;
		}

        private classRoomWebSiteDBContext db = new classRoomWebSiteDBContext();
        
        // GET: /courseIntro/
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
                ExportColumnAttributeHelper<courseIntro>.GetExportColumns()
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
            ViewData["describeSortParm"] = sortOrder == "describe_asc" ? "describe_desc" : "describe_asc";
            ViewData["pic2SortParm"] = sortOrder == "pic2_asc" ? "pic2_desc" : "pic2_asc";

            var Query = from _courseIntro in db.courseIntro
                        select new courseIntro() {
                            id = _courseIntro.id
                           ,name = _courseIntro.name
                           ,describe = _courseIntro.describe
                           ,pic2 = _courseIntro.pic2
                        };

            Query = db.courseIntro.AsQueryable();

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
                                                 || ("Describe".ToString().Equals(SearchField) && p.describe.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Pic2".ToString().Equals(SearchField) && p.pic2.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Starts with...") {
                        Query = Query.Where(p => 
                                                 ("Id".ToString().Equals(SearchField) && p.id.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Name".ToString().Equals(SearchField) && p.name.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Describe".ToString().Equals(SearchField) && p.describe.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Pic2".ToString().Equals(SearchField) && p.pic2.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Equals") {
                         if ("Name".Equals(SearchField)) { var mname = System.Convert.ToString(SearchText); Query = Query.Where(p => p.name == mname); }
                        else if ("Describe".Equals(SearchField)) { var mdescribe = System.Convert.ToString(SearchText); Query = Query.Where(p => p.describe == mdescribe); }
                        else if ("Pic2".Equals(SearchField)) { var mpic2 = System.Convert.ToString(SearchText); Query = Query.Where(p => p.pic2 == mpic2); }
                    } else if (SearchCondition == "More than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                    } else if (SearchCondition == "Less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                    } else if (SearchCondition == "Equal or more than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                    } else if (SearchCondition == "Equal or less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
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
                case "describe_desc":
                    Query = Query.OrderByDescending(s => s.describe);
                    break;
                case "describe_asc":
                    Query = Query.OrderBy(s => s.describe);
                    break;
                case "pic2_desc":
                    Query = Query.OrderByDescending(s => s.pic2);
                    break;
                case "pic2_asc":
                    Query = Query.OrderBy(s => s.pic2);
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

        // GET: /courseIntro/Details/<id>
        public ActionResult Details(
                                      Guid id
                                   )
        {
            if (
                    id == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            courseIntro courseIntro = db.courseIntro.Find(
                                                 id
                                            );
            if (courseIntro == null)
            {
                return HttpNotFound();
            }
            return View(courseIntro);
        }

        // GET: /courseIntro/Create
        public ActionResult Create()
        {
            var vm = new courseIntro();
            // vm.
            ViewData.Model = vm;
            return View();
        }

        // POST: /courseIntro/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include=
				           "name"
				   + "," + "describe"
				   + "," + "pic2"
				  )] courseIntro courseIntro)
        {
            if (ModelState.IsValid)
            {
                courseIntro.id = Guid.NewGuid();
                db.courseIntro.Add(courseIntro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(courseIntro);
        }

        // GET: /courseIntro/Edit/<id>
        public ActionResult Edit(
                                   Guid id
                                )
        {
            if (
                    id == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            courseIntro courseIntro = db.courseIntro.Find(
                                                 id
                                            );
            if (courseIntro == null)
            {
                return HttpNotFound();
            }

            return View(courseIntro);
        }

        // POST: /courseIntro/Edit/<id>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include=
				     " id"
				   + ",name"
				   + ",describe"
				   + ",pic2"
				  )] courseIntro courseIntro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseIntro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(courseIntro);
        }

        // GET: /courseIntro/Delete/<id>
        public ActionResult Delete(
                                     Guid id
                                  )
        {
            if (
                    id == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            courseIntro courseIntro = db.courseIntro.Find(
                                                 id
                                            );
            if (courseIntro == null)
            {
                return HttpNotFound();
            }
            return View(courseIntro);
        }

        // POST: /courseIntro/Delete/<id>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(
                                            Guid id
                                            )
        {
            courseIntro courseIntro = db.courseIntro.Find(
                                                 id
                                            );
            db.courseIntro.Remove(courseIntro);
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
            SelectListItem Item3 = new SelectListItem { Text = "Describe", Value = "Describe" };
            SelectListItem Item4 = new SelectListItem { Text = "Pic2", Value = "Pic2" };

                 if (select == "Id") { Item1.Selected = true; }
            else if (select == "Name") { Item2.Selected = true; }
            else if (select == "Describe") { Item3.Selected = true; }
            else if (select == "Pic2") { Item4.Selected = true; }

            list.Add(Item1);
            list.Add(Item2);
            list.Add(Item3);
            list.Add(Item4);

            return list.ToList();
        }



        public ActionResult Export(string fileName, string selectedColumns)
        {
            //取得原始資料
            var _exportData = Session["ExportData"] as IQueryable<courseIntro>;
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
                    SheetName = "courseIntro匯出資料",
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
            bool result = !uow.Repository<courseIntro>().All().Count().Equals(0);
            jo.Add("Msg", result.ToString());
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
        
        
        private JArray GetExportData()
        {
            var threedays = DateTime.Now.AddDays(-3);
            var query = uow.Repository<courseIntro>().All();

            string str_json = JsonConvert.SerializeObject(query, Formatting.Indented);
            JArray jObjects = JsonConvert.DeserializeObject<JArray>(str_json.Trim());


            return jObjects;
        }





        private void ExportData(String Export, GridView gv, DataTable dt)
        {
            if (Export == "Pdf")
            {
                PDFform pdfForm = new PDFform(dt, "Dbo.Course Intro", "Many");
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
 
