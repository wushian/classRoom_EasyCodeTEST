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
    public class QuestionAnserController : Controller
    {
          //private string fileSavedPath = WebConfigurationManager.AppSettings["UploadPath"];

		private GenericUnitOfWork uow = null;
		// 宣告預設建構式
		public QuestionAnserController()
		{
		    this.uow = new GenericUnitOfWork();
		}

		public QuestionAnserController(GenericUnitOfWork uow_)
		{
		   this.uow = uow_;
		}

        private classRoomWebSiteDBContext db = new classRoomWebSiteDBContext();
        
        // GET: /QuestionAnser/
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
                ExportColumnAttributeHelper<QuestionAnser>.GetExportColumns()
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
            ViewData["cat1SortParm"] = sortOrder == "cat1_asc" ? "cat1_desc" : "cat1_asc";
            ViewData["cat2SortParm"] = sortOrder == "cat2_asc" ? "cat2_desc" : "cat2_asc";
            ViewData["pic1SortParm"] = sortOrder == "pic1_asc" ? "pic1_desc" : "pic1_asc";
            ViewData["pic2SortParm"] = sortOrder == "pic2_asc" ? "pic2_desc" : "pic2_asc";
            ViewData["describeSortParm"] = sortOrder == "describe_asc" ? "describe_desc" : "describe_asc";

            var Query = from _QuestionAnser in db.QuestionAnser
                        select new QuestionAnser() {
                            id = _QuestionAnser.id
                           ,cat1 = _QuestionAnser.cat1
                           ,cat2 = _QuestionAnser.cat2
                           ,pic1 = _QuestionAnser.pic1
                           ,pic2 = _QuestionAnser.pic2
                           ,describe = _QuestionAnser.describe
                        };

            Query = db.QuestionAnser.AsQueryable();

            try {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["SearchField"])) && !string.IsNullOrEmpty(Convert.ToString(Session["SearchCondition"])) && !string.IsNullOrEmpty(Convert.ToString(Session["SearchText"])))
                {
                    SearchField = Convert.ToString(Session["SearchField"]);
                    SearchCondition = Convert.ToString(Session["SearchCondition"]);
                    SearchText = Convert.ToString(Session["SearchText"]);

                    if (SearchCondition == "Contains") {
                        Query = Query.Where(p => 
                                                 ("Id".ToString().Equals(SearchField) && p.id.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Cat1".ToString().Equals(SearchField) && p.cat1.Value.ToString().Contains(SearchText)) 
                                                 || ("Cat2".ToString().Equals(SearchField) && p.cat2.Value.ToString().Contains(SearchText)) 
                                                 || ("Pic1".ToString().Equals(SearchField) && p.pic1.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Pic2".ToString().Equals(SearchField) && p.pic2.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Describe".ToString().Equals(SearchField) && p.describe.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Starts with...") {
                        Query = Query.Where(p => 
                                                 ("Id".ToString().Equals(SearchField) && p.id.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Cat1".ToString().Equals(SearchField) && p.cat1.Value.ToString().StartsWith(SearchText)) 
                                                 || ("Cat2".ToString().Equals(SearchField) && p.cat2.Value.ToString().StartsWith(SearchText)) 
                                                 || ("Pic1".ToString().Equals(SearchField) && p.pic1.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Pic2".ToString().Equals(SearchField) && p.pic2.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Describe".ToString().Equals(SearchField) && p.describe.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Equals") {
                         if ("Cat1".Equals(SearchField)) { var mcat1 = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.cat1 == mcat1); }
                        else if ("Cat2".Equals(SearchField)) { var mcat2 = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.cat2 == mcat2); }
                        else if ("Pic1".Equals(SearchField)) { var mpic1 = System.Convert.ToString(SearchText); Query = Query.Where(p => p.pic1 == mpic1); }
                        else if ("Pic2".Equals(SearchField)) { var mpic2 = System.Convert.ToString(SearchText); Query = Query.Where(p => p.pic2 == mpic2); }
                        else if ("Describe".Equals(SearchField)) { var mdescribe = System.Convert.ToString(SearchText); Query = Query.Where(p => p.describe == mdescribe); }
                    } else if (SearchCondition == "More than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Cat1".Equals(SearchField)) { var mcat1 = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.cat1 > mcat1); }
                        else if ("Cat2".Equals(SearchField)) { var mcat2 = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.cat2 > mcat2); }
                    } else if (SearchCondition == "Less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Cat1".Equals(SearchField)) { var mcat1 = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.cat1 < mcat1); }
                        else if ("Cat2".Equals(SearchField)) { var mcat2 = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.cat2 < mcat2); }
                    } else if (SearchCondition == "Equal or more than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Cat1".Equals(SearchField)) { var mcat1 = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.cat1 >= mcat1); }
                        else if ("Cat2".Equals(SearchField)) { var mcat2 = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.cat2 >= mcat2); }
                    } else if (SearchCondition == "Equal or less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Cat1".Equals(SearchField)) { var mcat1 = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.cat1 <= mcat1); }
                        else if ("Cat2".Equals(SearchField)) { var mcat2 = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.cat2 <= mcat2); }
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
                case "cat1_desc":
                    Query = Query.OrderByDescending(s => s.cat1);
                    break;
                case "cat1_asc":
                    Query = Query.OrderBy(s => s.cat1);
                    break;
                case "cat2_desc":
                    Query = Query.OrderByDescending(s => s.cat2);
                    break;
                case "cat2_asc":
                    Query = Query.OrderBy(s => s.cat2);
                    break;
                case "pic1_desc":
                    Query = Query.OrderByDescending(s => s.pic1);
                    break;
                case "pic1_asc":
                    Query = Query.OrderBy(s => s.pic1);
                    break;
                case "pic2_desc":
                    Query = Query.OrderByDescending(s => s.pic2);
                    break;
                case "pic2_asc":
                    Query = Query.OrderBy(s => s.pic2);
                    break;
                case "describe_desc":
                    Query = Query.OrderByDescending(s => s.describe);
                    break;
                case "describe_asc":
                    Query = Query.OrderBy(s => s.describe);
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

        // GET: /QuestionAnser/Details/<id>
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
            QuestionAnser QuestionAnser = db.QuestionAnser.Find(
                                                 id
                                            );
            if (QuestionAnser == null)
            {
                return HttpNotFound();
            }
            return View(QuestionAnser);
        }

        // GET: /QuestionAnser/Create
        public ActionResult Create()
        {
            var vm = new QuestionAnser();
            // vm.
            ViewData.Model = vm;
            return View();
        }

        // POST: /QuestionAnser/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include=
				           "cat1"
				   + "," + "cat2"
				   + "," + "pic1"
				   + "," + "pic2"
				   + "," + "describe"
				  )] QuestionAnser QuestionAnser)
        {
            if (ModelState.IsValid)
            {
                QuestionAnser.id = Guid.NewGuid();
                db.QuestionAnser.Add(QuestionAnser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(QuestionAnser);
        }

        // GET: /QuestionAnser/Edit/<id>
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
            QuestionAnser QuestionAnser = db.QuestionAnser.Find(
                                                 id
                                            );
            if (QuestionAnser == null)
            {
                return HttpNotFound();
            }

            return View(QuestionAnser);
        }

        // POST: /QuestionAnser/Edit/<id>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include=
				     " id"
				   + ",cat1"
				   + ",cat2"
				   + ",pic1"
				   + ",pic2"
				   + ",describe"
				  )] QuestionAnser QuestionAnser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(QuestionAnser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(QuestionAnser);
        }

        // GET: /QuestionAnser/Delete/<id>
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
            QuestionAnser QuestionAnser = db.QuestionAnser.Find(
                                                 id
                                            );
            if (QuestionAnser == null)
            {
                return HttpNotFound();
            }
            return View(QuestionAnser);
        }

        // POST: /QuestionAnser/Delete/<id>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(
                                            Guid id
                                            )
        {
            QuestionAnser QuestionAnser = db.QuestionAnser.Find(
                                                 id
                                            );
            db.QuestionAnser.Remove(QuestionAnser);
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
            SelectListItem Item2 = new SelectListItem { Text = "Cat1", Value = "Cat1" };
            SelectListItem Item3 = new SelectListItem { Text = "Cat2", Value = "Cat2" };
            SelectListItem Item4 = new SelectListItem { Text = "Pic1", Value = "Pic1" };
            SelectListItem Item5 = new SelectListItem { Text = "Pic2", Value = "Pic2" };
            SelectListItem Item6 = new SelectListItem { Text = "Describe", Value = "Describe" };

                 if (select == "Id") { Item1.Selected = true; }
            else if (select == "Cat1") { Item2.Selected = true; }
            else if (select == "Cat2") { Item3.Selected = true; }
            else if (select == "Pic1") { Item4.Selected = true; }
            else if (select == "Pic2") { Item5.Selected = true; }
            else if (select == "Describe") { Item6.Selected = true; }

            list.Add(Item1);
            list.Add(Item2);
            list.Add(Item3);
            list.Add(Item4);
            list.Add(Item5);
            list.Add(Item6);

            return list.ToList();
        }



        public ActionResult Export(string fileName, string selectedColumns)
        {
            //取得原始資料
            var _exportData = Session["ExportData"] as IQueryable<QuestionAnser>;
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
                    SheetName = "QuestionAnser匯出資料",
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
            bool result = !uow.Repository<QuestionAnser>().All().Count().Equals(0);
            jo.Add("Msg", result.ToString());
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
        
        
        private JArray GetExportData()
        {
            var threedays = DateTime.Now.AddDays(-3);
            var query = uow.Repository<QuestionAnser>().All();

            string str_json = JsonConvert.SerializeObject(query, Formatting.Indented);
            JArray jObjects = JsonConvert.DeserializeObject<JArray>(str_json.Trim());


            return jObjects;
        }





        private void ExportData(String Export, GridView gv, DataTable dt)
        {
            if (Export == "Pdf")
            {
                PDFform pdfForm = new PDFform(dt, "Dbo. Question Anser", "Many");
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
 
