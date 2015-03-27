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
    public class TaiwanZipCodeController : Controller
    {
          //private string fileSavedPath = WebConfigurationManager.AppSettings["UploadPath"];

		private GenericUnitOfWork uow = null;
		// 宣告預設建構式
		public TaiwanZipCodeController()
		{
		    this.uow = new GenericUnitOfWork();
		}

		public TaiwanZipCodeController(GenericUnitOfWork uow_)
		{
		   this.uow = uow_;
		}

        private classRoomWebSiteDBContext db = new classRoomWebSiteDBContext();
        
        // GET: /TaiwanZipCode/
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
                ExportColumnAttributeHelper<TaiwanZipCode>.GetExportColumns()
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
            ViewData["ZipSortParm"] = sortOrder == "Zip_asc" ? "Zip_desc" : "Zip_asc";
            ViewData["CityNameSortParm"] = sortOrder == "CityName_asc" ? "CityName_desc" : "CityName_asc";
            ViewData["TownSortParm"] = sortOrder == "Town_asc" ? "Town_desc" : "Town_asc";
            ViewData["SequenceSortParm"] = sortOrder == "Sequence_asc" ? "Sequence_desc" : "Sequence_asc";
            ViewData["CreateDateSortParm"] = sortOrder == "CreateDate_asc" ? "CreateDate_desc" : "CreateDate_asc";

            var Query = from _TaiwanZipCode in db.TaiwanZipCode
                        select new TaiwanZipCode() {
                            ID = _TaiwanZipCode.ID
                           ,Zip = _TaiwanZipCode.Zip
                           ,CityName = _TaiwanZipCode.CityName
                           ,Town = _TaiwanZipCode.Town
                           ,Sequence = _TaiwanZipCode.Sequence
                           ,CreateDate = _TaiwanZipCode.CreateDate
                        };

            Query = db.TaiwanZipCode.AsQueryable();

            try {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["SearchField"])) && !string.IsNullOrEmpty(Convert.ToString(Session["SearchCondition"])) && !string.IsNullOrEmpty(Convert.ToString(Session["SearchText"])))
                {
                    SearchField = Convert.ToString(Session["SearchField"]);
                    SearchCondition = Convert.ToString(Session["SearchCondition"]);
                    SearchText = Convert.ToString(Session["SearchText"]);

                    if (SearchCondition == "Contains") {
                        Query = Query.Where(p => 
                                                 ("I D".ToString().Equals(SearchField) && p.ID.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Zip".ToString().Equals(SearchField) && p.Zip.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("City Name".ToString().Equals(SearchField) && p.CityName.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Town".ToString().Equals(SearchField) && p.Town.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Sequence".ToString().Equals(SearchField) && p.Sequence.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Create Date".ToString().Equals(SearchField) && p.CreateDate.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Starts with...") {
                        Query = Query.Where(p => 
                                                 ("I D".ToString().Equals(SearchField) && p.ID.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Zip".ToString().Equals(SearchField) && p.Zip.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("City Name".ToString().Equals(SearchField) && p.CityName.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Town".ToString().Equals(SearchField) && p.Town.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Sequence".ToString().Equals(SearchField) && p.Sequence.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Create Date".ToString().Equals(SearchField) && p.CreateDate.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Equals") {
                        if ("I D".Equals(SearchField)) { var mID = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.ID == mID); }
                        else if ("Zip".Equals(SearchField)) { var mZip = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.Zip == mZip); }
                        else if ("City Name".Equals(SearchField)) { var mCityName = System.Convert.ToString(SearchText); Query = Query.Where(p => p.CityName == mCityName); }
                        else if ("Town".Equals(SearchField)) { var mTown = System.Convert.ToString(SearchText); Query = Query.Where(p => p.Town == mTown); }
                        else if ("Sequence".Equals(SearchField)) { var mSequence = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.Sequence == mSequence); }
                        else if ("Create Date".Equals(SearchField)) { var mCreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.CreateDate == mCreateDate); }
                    } else if (SearchCondition == "More than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("I D".Equals(SearchField)) { var mID = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.ID > mID); }
                        else if ("Zip".Equals(SearchField)) { var mZip = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.Zip > mZip); }
                        else if ("Sequence".Equals(SearchField)) { var mSequence = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.Sequence > mSequence); }
                        else if ("Create Date".Equals(SearchField)) { var mCreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.CreateDate > mCreateDate); }
                    } else if (SearchCondition == "Less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("I D".Equals(SearchField)) { var mID = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.ID < mID); }
                        else if ("Zip".Equals(SearchField)) { var mZip = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.Zip < mZip); }
                        else if ("Sequence".Equals(SearchField)) { var mSequence = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.Sequence < mSequence); }
                        else if ("Create Date".Equals(SearchField)) { var mCreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.CreateDate < mCreateDate); }
                    } else if (SearchCondition == "Equal or more than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("I D".Equals(SearchField)) { var mID = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.ID >= mID); }
                        else if ("Zip".Equals(SearchField)) { var mZip = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.Zip >= mZip); }
                        else if ("Sequence".Equals(SearchField)) { var mSequence = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.Sequence >= mSequence); }
                        else if ("Create Date".Equals(SearchField)) { var mCreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.CreateDate >= mCreateDate); }
                    } else if (SearchCondition == "Equal or less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("I D".Equals(SearchField)) { var mID = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.ID <= mID); }
                        else if ("Zip".Equals(SearchField)) { var mZip = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.Zip <= mZip); }
                        else if ("Sequence".Equals(SearchField)) { var mSequence = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.Sequence <= mSequence); }
                        else if ("Create Date".Equals(SearchField)) { var mCreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.CreateDate <= mCreateDate); }
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
                case "Zip_desc":
                    Query = Query.OrderByDescending(s => s.Zip);
                    break;
                case "Zip_asc":
                    Query = Query.OrderBy(s => s.Zip);
                    break;
                case "CityName_desc":
                    Query = Query.OrderByDescending(s => s.CityName);
                    break;
                case "CityName_asc":
                    Query = Query.OrderBy(s => s.CityName);
                    break;
                case "Town_desc":
                    Query = Query.OrderByDescending(s => s.Town);
                    break;
                case "Town_asc":
                    Query = Query.OrderBy(s => s.Town);
                    break;
                case "Sequence_desc":
                    Query = Query.OrderByDescending(s => s.Sequence);
                    break;
                case "Sequence_asc":
                    Query = Query.OrderBy(s => s.Sequence);
                    break;
                case "CreateDate_desc":
                    Query = Query.OrderByDescending(s => s.CreateDate);
                    break;
                case "CreateDate_asc":
                    Query = Query.OrderBy(s => s.CreateDate);
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

        // GET: /TaiwanZipCode/Details/<id>
        public ActionResult Details(
                                      Int32? ID
                                   )
        {
            if (
                    ID == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiwanZipCode TaiwanZipCode = db.TaiwanZipCode.Find(
                                                 ID
                                            );
            if (TaiwanZipCode == null)
            {
                return HttpNotFound();
            }
            return View(TaiwanZipCode);
        }

        // GET: /TaiwanZipCode/Create
        public ActionResult Create()
        {
            var vm = new TaiwanZipCode();
            // vm.
            ViewData.Model = vm;
            return View();
        }

        // POST: /TaiwanZipCode/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include=
				           "Zip"
				   + "," + "CityName"
				   + "," + "Town"
				   + "," + "Sequence"
				   + "," + "CreateDate"
				  )] TaiwanZipCode TaiwanZipCode)
        {
            if (ModelState.IsValid)
            {
                db.TaiwanZipCode.Add(TaiwanZipCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(TaiwanZipCode);
        }

        // GET: /TaiwanZipCode/Edit/<id>
        public ActionResult Edit(
                                   Int32? ID
                                )
        {
            if (
                    ID == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiwanZipCode TaiwanZipCode = db.TaiwanZipCode.Find(
                                                 ID
                                            );
            if (TaiwanZipCode == null)
            {
                return HttpNotFound();
            }

            return View(TaiwanZipCode);
        }

        // POST: /TaiwanZipCode/Edit/<id>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include=
				     " ID"
				   + ",Zip"
				   + ",CityName"
				   + ",Town"
				   + ",Sequence"
				   + ",CreateDate"
				  )] TaiwanZipCode TaiwanZipCode)
        {
            if (ModelState.IsValid)
            {
                db.Entry(TaiwanZipCode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(TaiwanZipCode);
        }

        // GET: /TaiwanZipCode/Delete/<id>
        public ActionResult Delete(
                                     Int32? ID
                                  )
        {
            if (
                    ID == null
               )
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiwanZipCode TaiwanZipCode = db.TaiwanZipCode.Find(
                                                 ID
                                            );
            if (TaiwanZipCode == null)
            {
                return HttpNotFound();
            }
            return View(TaiwanZipCode);
        }

        // POST: /TaiwanZipCode/Delete/<id>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(
                                            Int32? ID
                                            )
        {
            TaiwanZipCode TaiwanZipCode = db.TaiwanZipCode.Find(
                                                 ID
                                            );
            db.TaiwanZipCode.Remove(TaiwanZipCode);
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
            SelectListItem Item2 = new SelectListItem { Text = "Zip", Value = "Zip" };
            SelectListItem Item3 = new SelectListItem { Text = "City Name", Value = "City Name" };
            SelectListItem Item4 = new SelectListItem { Text = "Town", Value = "Town" };
            SelectListItem Item5 = new SelectListItem { Text = "Sequence", Value = "Sequence" };
            SelectListItem Item6 = new SelectListItem { Text = "Create Date", Value = "Create Date" };

                 if (select == "I D") { Item1.Selected = true; }
            else if (select == "Zip") { Item2.Selected = true; }
            else if (select == "City Name") { Item3.Selected = true; }
            else if (select == "Town") { Item4.Selected = true; }
            else if (select == "Sequence") { Item5.Selected = true; }
            else if (select == "Create Date") { Item6.Selected = true; }

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
            var _exportData = Session["ExportData"] as IQueryable<TaiwanZipCode>;
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
                    SheetName = "TaiwanZipCode匯出資料",
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
            bool result = !uow.Repository<TaiwanZipCode>().All().Count().Equals(0);
            jo.Add("Msg", result.ToString());
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
        
        
        private JArray GetExportData()
        {
            var threedays = DateTime.Now.AddDays(-3);
            var query = uow.Repository<TaiwanZipCode>().All();

            string str_json = JsonConvert.SerializeObject(query, Formatting.Indented);
            JArray jObjects = JsonConvert.DeserializeObject<JArray>(str_json.Trim());


            return jObjects;
        }





        private void ExportData(String Export, GridView gv, DataTable dt)
        {
            if (Export == "Pdf")
            {
                PDFform pdfForm = new PDFform(dt, "Dbo. Taiwan Zip Code", "Many");
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
 
