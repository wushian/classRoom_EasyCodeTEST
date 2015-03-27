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
    public class calendar_detailController : Controller
    {
          //private string fileSavedPath = WebConfigurationManager.AppSettings["UploadPath"];

		private GenericUnitOfWork uow = null;
		// 宣告預設建構式
		public calendar_detailController()
		{
		    this.uow = new GenericUnitOfWork();
		}

		public calendar_detailController(GenericUnitOfWork uow_)
		{
		   this.uow = uow_;
		}

        private classRoomWebSiteDBContext db = new classRoomWebSiteDBContext();
        
        // GET: /calendar_detail/
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
                ExportColumnAttributeHelper<calendar_detail>.GetExportColumns()
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
            ViewData["subjectSortParm"] = sortOrder == "subject_asc" ? "subject_desc" : "subject_asc";
            ViewData["startDateTimeSortParm"] = sortOrder == "startDateTime_asc" ? "startDateTime_desc" : "startDateTime_asc";
            ViewData["endDateTimeSortParm"] = sortOrder == "endDateTime_asc" ? "endDateTime_desc" : "endDateTime_asc";
            ViewData["feeStaffSortParm"] = sortOrder == "feeStaff_asc" ? "feeStaff_desc" : "feeStaff_asc";
            ViewData["feeNonStaffSortParm"] = sortOrder == "feeNonStaff_asc" ? "feeNonStaff_desc" : "feeNonStaff_asc";
            ViewData["contactSortParm"] = sortOrder == "contact_asc" ? "contact_desc" : "contact_asc";
            ViewData["contactTelSortParm"] = sortOrder == "contactTel_asc" ? "contactTel_desc" : "contactTel_asc";
            ViewData["describeSortParm"] = sortOrder == "describe_asc" ? "describe_desc" : "describe_asc";

            var Query = from _calendar_detail in db.calendar_detail
                        select new calendar_detail() {
                            id = _calendar_detail.id
                           ,subject = _calendar_detail.subject
                           ,startDateTime = _calendar_detail.startDateTime
                           ,endDateTime = _calendar_detail.endDateTime
                           ,feeStaff = _calendar_detail.feeStaff
                           ,feeNonStaff = _calendar_detail.feeNonStaff
                           ,contact = _calendar_detail.contact
                           ,contactTel = _calendar_detail.contactTel
                           ,describe = _calendar_detail.describe
                        };

            Query = db.calendar_detail.AsQueryable();

            try {
                if (!string.IsNullOrEmpty(Convert.ToString(Session["SearchField"])) && !string.IsNullOrEmpty(Convert.ToString(Session["SearchCondition"])) && !string.IsNullOrEmpty(Convert.ToString(Session["SearchText"])))
                {
                    SearchField = Convert.ToString(Session["SearchField"]);
                    SearchCondition = Convert.ToString(Session["SearchCondition"]);
                    SearchText = Convert.ToString(Session["SearchText"]);

                    if (SearchCondition == "Contains") {
                        Query = Query.Where(p => 
                                                 ("Id".ToString().Equals(SearchField) && p.id.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Subject".ToString().Equals(SearchField) && p.subject.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Start Date Time".ToString().Equals(SearchField) && p.startDateTime.Value.ToString().Contains(SearchText)) 
                                                 || ("End Date Time".ToString().Equals(SearchField) && p.endDateTime.Value.ToString().Contains(SearchText)) 
                                                 || ("Fee Staff".ToString().Equals(SearchField) && p.feeStaff.Value.ToString().Contains(SearchText)) 
                                                 || ("Fee Non Staff".ToString().Equals(SearchField) && p.feeNonStaff.Value.ToString().Contains(SearchText)) 
                                                 || ("Contact".ToString().Equals(SearchField) && p.contact.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Contact Tel".ToString().Equals(SearchField) && p.contactTel.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Describe".ToString().Equals(SearchField) && p.describe.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Starts with...") {
                        Query = Query.Where(p => 
                                                 ("Id".ToString().Equals(SearchField) && p.id.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Subject".ToString().Equals(SearchField) && p.subject.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Start Date Time".ToString().Equals(SearchField) && p.startDateTime.Value.ToString().StartsWith(SearchText)) 
                                                 || ("End Date Time".ToString().Equals(SearchField) && p.endDateTime.Value.ToString().StartsWith(SearchText)) 
                                                 || ("Fee Staff".ToString().Equals(SearchField) && p.feeStaff.Value.ToString().StartsWith(SearchText)) 
                                                 || ("Fee Non Staff".ToString().Equals(SearchField) && p.feeNonStaff.Value.ToString().StartsWith(SearchText)) 
                                                 || ("Contact".ToString().Equals(SearchField) && p.contact.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Contact Tel".ToString().Equals(SearchField) && p.contactTel.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Describe".ToString().Equals(SearchField) && p.describe.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Equals") {
                         if ("Subject".Equals(SearchField)) { var msubject = System.Convert.ToString(SearchText); Query = Query.Where(p => p.subject == msubject); }
                        else if ("Start Date Time".Equals(SearchField)) { var mstartDateTime = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.startDateTime == mstartDateTime); }
                        else if ("End Date Time".Equals(SearchField)) { var mendDateTime = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.endDateTime == mendDateTime); }
                        else if ("Fee Staff".Equals(SearchField)) { var mfeeStaff = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.feeStaff == mfeeStaff); }
                        else if ("Fee Non Staff".Equals(SearchField)) { var mfeeNonStaff = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.feeNonStaff == mfeeNonStaff); }
                        else if ("Contact".Equals(SearchField)) { var mcontact = System.Convert.ToString(SearchText); Query = Query.Where(p => p.contact == mcontact); }
                        else if ("Contact Tel".Equals(SearchField)) { var mcontactTel = System.Convert.ToString(SearchText); Query = Query.Where(p => p.contactTel == mcontactTel); }
                        else if ("Describe".Equals(SearchField)) { var mdescribe = System.Convert.ToString(SearchText); Query = Query.Where(p => p.describe == mdescribe); }
                    } else if (SearchCondition == "More than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Start Date Time".Equals(SearchField)) { var mstartDateTime = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.startDateTime > mstartDateTime); }
                        else if ("End Date Time".Equals(SearchField)) { var mendDateTime = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.endDateTime > mendDateTime); }
                        else if ("Fee Staff".Equals(SearchField)) { var mfeeStaff = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.feeStaff > mfeeStaff); }
                        else if ("Fee Non Staff".Equals(SearchField)) { var mfeeNonStaff = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.feeNonStaff > mfeeNonStaff); }
                    } else if (SearchCondition == "Less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Start Date Time".Equals(SearchField)) { var mstartDateTime = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.startDateTime < mstartDateTime); }
                        else if ("End Date Time".Equals(SearchField)) { var mendDateTime = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.endDateTime < mendDateTime); }
                        else if ("Fee Staff".Equals(SearchField)) { var mfeeStaff = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.feeStaff < mfeeStaff); }
                        else if ("Fee Non Staff".Equals(SearchField)) { var mfeeNonStaff = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.feeNonStaff < mfeeNonStaff); }
                    } else if (SearchCondition == "Equal or more than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Start Date Time".Equals(SearchField)) { var mstartDateTime = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.startDateTime >= mstartDateTime); }
                        else if ("End Date Time".Equals(SearchField)) { var mendDateTime = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.endDateTime >= mendDateTime); }
                        else if ("Fee Staff".Equals(SearchField)) { var mfeeStaff = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.feeStaff >= mfeeStaff); }
                        else if ("Fee Non Staff".Equals(SearchField)) { var mfeeNonStaff = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.feeNonStaff >= mfeeNonStaff); }
                    } else if (SearchCondition == "Equal or less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Start Date Time".Equals(SearchField)) { var mstartDateTime = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.startDateTime <= mstartDateTime); }
                        else if ("End Date Time".Equals(SearchField)) { var mendDateTime = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.endDateTime <= mendDateTime); }
                        else if ("Fee Staff".Equals(SearchField)) { var mfeeStaff = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.feeStaff <= mfeeStaff); }
                        else if ("Fee Non Staff".Equals(SearchField)) { var mfeeNonStaff = System.Convert.ToInt32(SearchText); Query = Query.Where(p => p.feeNonStaff <= mfeeNonStaff); }
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
                case "subject_desc":
                    Query = Query.OrderByDescending(s => s.subject);
                    break;
                case "subject_asc":
                    Query = Query.OrderBy(s => s.subject);
                    break;
                case "startDateTime_desc":
                    Query = Query.OrderByDescending(s => s.startDateTime);
                    break;
                case "startDateTime_asc":
                    Query = Query.OrderBy(s => s.startDateTime);
                    break;
                case "endDateTime_desc":
                    Query = Query.OrderByDescending(s => s.endDateTime);
                    break;
                case "endDateTime_asc":
                    Query = Query.OrderBy(s => s.endDateTime);
                    break;
                case "feeStaff_desc":
                    Query = Query.OrderByDescending(s => s.feeStaff);
                    break;
                case "feeStaff_asc":
                    Query = Query.OrderBy(s => s.feeStaff);
                    break;
                case "feeNonStaff_desc":
                    Query = Query.OrderByDescending(s => s.feeNonStaff);
                    break;
                case "feeNonStaff_asc":
                    Query = Query.OrderBy(s => s.feeNonStaff);
                    break;
                case "contact_desc":
                    Query = Query.OrderByDescending(s => s.contact);
                    break;
                case "contact_asc":
                    Query = Query.OrderBy(s => s.contact);
                    break;
                case "contactTel_desc":
                    Query = Query.OrderByDescending(s => s.contactTel);
                    break;
                case "contactTel_asc":
                    Query = Query.OrderBy(s => s.contactTel);
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

        // GET: /calendar_detail/Details/<id>
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
            calendar_detail calendar_detail = db.calendar_detail.Find(
                                                 id
                                            );
            if (calendar_detail == null)
            {
                return HttpNotFound();
            }
            return View(calendar_detail);
        }

        // GET: /calendar_detail/Create
        public ActionResult Create()
        {
            var vm = new calendar_detail();
            // vm.
            ViewData.Model = vm;
            return View();
        }

        // POST: /calendar_detail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include=
				           "subject"
				   + "," + "startDateTime"
				   + "," + "endDateTime"
				   + "," + "feeStaff"
				   + "," + "feeNonStaff"
				   + "," + "contact"
				   + "," + "contactTel"
				   + "," + "describe"
				  )] calendar_detail calendar_detail)
        {
            if (ModelState.IsValid)
            {
                calendar_detail.id = Guid.NewGuid();
                db.calendar_detail.Add(calendar_detail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(calendar_detail);
        }

        // GET: /calendar_detail/Edit/<id>
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
            calendar_detail calendar_detail = db.calendar_detail.Find(
                                                 id
                                            );
            if (calendar_detail == null)
            {
                return HttpNotFound();
            }

            return View(calendar_detail);
        }

        // POST: /calendar_detail/Edit/<id>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include=
				     " id"
				   + ",subject"
				   + ",startDateTime"
				   + ",endDateTime"
				   + ",feeStaff"
				   + ",feeNonStaff"
				   + ",contact"
				   + ",contactTel"
				   + ",describe"
				  )] calendar_detail calendar_detail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(calendar_detail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(calendar_detail);
        }

        // GET: /calendar_detail/Delete/<id>
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
            calendar_detail calendar_detail = db.calendar_detail.Find(
                                                 id
                                            );
            if (calendar_detail == null)
            {
                return HttpNotFound();
            }
            return View(calendar_detail);
        }

        // POST: /calendar_detail/Delete/<id>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(
                                            Guid id
                                            )
        {
            calendar_detail calendar_detail = db.calendar_detail.Find(
                                                 id
                                            );
            db.calendar_detail.Remove(calendar_detail);
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
            SelectListItem Item2 = new SelectListItem { Text = "Subject", Value = "Subject" };
            SelectListItem Item3 = new SelectListItem { Text = "Start Date Time", Value = "Start Date Time" };
            SelectListItem Item4 = new SelectListItem { Text = "End Date Time", Value = "End Date Time" };
            SelectListItem Item5 = new SelectListItem { Text = "Fee Staff", Value = "Fee Staff" };
            SelectListItem Item6 = new SelectListItem { Text = "Fee Non Staff", Value = "Fee Non Staff" };
            SelectListItem Item7 = new SelectListItem { Text = "Contact", Value = "Contact" };
            SelectListItem Item8 = new SelectListItem { Text = "Contact Tel", Value = "Contact Tel" };
            SelectListItem Item9 = new SelectListItem { Text = "Describe", Value = "Describe" };

                 if (select == "Id") { Item1.Selected = true; }
            else if (select == "Subject") { Item2.Selected = true; }
            else if (select == "Start Date Time") { Item3.Selected = true; }
            else if (select == "End Date Time") { Item4.Selected = true; }
            else if (select == "Fee Staff") { Item5.Selected = true; }
            else if (select == "Fee Non Staff") { Item6.Selected = true; }
            else if (select == "Contact") { Item7.Selected = true; }
            else if (select == "Contact Tel") { Item8.Selected = true; }
            else if (select == "Describe") { Item9.Selected = true; }

            list.Add(Item1);
            list.Add(Item2);
            list.Add(Item3);
            list.Add(Item4);
            list.Add(Item5);
            list.Add(Item6);
            list.Add(Item7);
            list.Add(Item8);
            list.Add(Item9);

            return list.ToList();
        }



        public ActionResult Export(string fileName, string selectedColumns)
        {
            //取得原始資料
            var _exportData = Session["ExportData"] as IQueryable<calendar_detail>;
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
                    SheetName = "calendar_detail匯出資料",
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
            bool result = !uow.Repository<calendar_detail>().All().Count().Equals(0);
            jo.Add("Msg", result.ToString());
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
        
        
        private JArray GetExportData()
        {
            var threedays = DateTime.Now.AddDays(-3);
            var query = uow.Repository<calendar_detail>().All();

            string str_json = JsonConvert.SerializeObject(query, Formatting.Indented);
            JArray jObjects = JsonConvert.DeserializeObject<JArray>(str_json.Trim());


            return jObjects;
        }





        private void ExportData(String Export, GridView gv, DataTable dt)
        {
            if (Export == "Pdf")
            {
                PDFform pdfForm = new PDFform(dt, "Dbo.Calendar Detail", "Many");
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
 
