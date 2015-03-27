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
    public class studentController : Controller
    {
          //private string fileSavedPath = WebConfigurationManager.AppSettings["UploadPath"];

		private GenericUnitOfWork uow = null;
		// 宣告預設建構式
		public studentController()
		{
		    this.uow = new GenericUnitOfWork();
		}

		public studentController(GenericUnitOfWork uow_)
		{
		   this.uow = uow_;
		}

        private classRoomWebSiteDBContext db = new classRoomWebSiteDBContext();
        
        // GET: /student/
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
                ExportColumnAttributeHelper<student>.GetExportColumns()
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
            ViewData["birthDaySortParm"] = sortOrder == "birthDay_asc" ? "birthDay_desc" : "birthDay_asc";
            ViewData["genderSortParm"] = sortOrder == "gender_asc" ? "gender_desc" : "gender_asc";
            ViewData["createDateSortParm"] = sortOrder == "createDate_asc" ? "createDate_desc" : "createDate_asc";
            ViewData["educationSortParm"] = sortOrder == "education_asc" ? "education_desc" : "education_asc";
            ViewData["addressSortParm"] = sortOrder == "address_asc" ? "address_desc" : "address_asc";
            ViewData["companySortParm"] = sortOrder == "company_asc" ? "company_desc" : "company_asc";
            ViewData["telnoSortParm"] = sortOrder == "telno_asc" ? "telno_desc" : "telno_asc";
            ViewData["cellnoSortParm"] = sortOrder == "cellno_asc" ? "cellno_desc" : "cellno_asc";

            var Query = from _student in db.student
                        select new student() {
                            id = _student.id
                           ,name = _student.name
                           ,birthDay = _student.birthDay
                           ,gender = _student.gender
                           ,createDate = _student.createDate
                           ,education = _student.education
                           ,address = _student.address
                           ,company = _student.company
                           ,telno = _student.telno
                           ,cellno = _student.cellno
                        };

            Query = db.student.AsQueryable();

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
                                                 || ("Birth Day".ToString().Equals(SearchField) && p.birthDay.Value.ToString().Contains(SearchText)) 
                                                 || ("Gender".ToString().Equals(SearchField) && p.gender.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Create Date".ToString().Equals(SearchField) && p.createDate.Value.ToString().Contains(SearchText)) 
                                                 || ("Education".ToString().Equals(SearchField) && p.education.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Address".ToString().Equals(SearchField) && p.address.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Company".ToString().Equals(SearchField) && p.company.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Telno".ToString().Equals(SearchField) && p.telno.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Cellno".ToString().Equals(SearchField) && p.cellno.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Starts with...") {
                        Query = Query.Where(p => 
                                                 ("Id".ToString().Equals(SearchField) && p.id.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Name".ToString().Equals(SearchField) && p.name.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Birth Day".ToString().Equals(SearchField) && p.birthDay.Value.ToString().StartsWith(SearchText)) 
                                                 || ("Gender".ToString().Equals(SearchField) && p.gender.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Create Date".ToString().Equals(SearchField) && p.createDate.Value.ToString().StartsWith(SearchText)) 
                                                 || ("Education".ToString().Equals(SearchField) && p.education.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Address".ToString().Equals(SearchField) && p.address.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Company".ToString().Equals(SearchField) && p.company.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Telno".ToString().Equals(SearchField) && p.telno.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Cellno".ToString().Equals(SearchField) && p.cellno.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Equals") {
                         if ("Name".Equals(SearchField)) { var mname = System.Convert.ToString(SearchText); Query = Query.Where(p => p.name == mname); }
                        else if ("Birth Day".Equals(SearchField)) { var mbirthDay = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.birthDay == mbirthDay); }
                        else if ("Gender".Equals(SearchField)) { var mgender = System.Convert.ToString(SearchText); Query = Query.Where(p => p.gender == mgender); }
                        else if ("Create Date".Equals(SearchField)) { var mcreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.createDate == mcreateDate); }
                        else if ("Education".Equals(SearchField)) { var meducation = System.Convert.ToString(SearchText); Query = Query.Where(p => p.education == meducation); }
                        else if ("Address".Equals(SearchField)) { var maddress = System.Convert.ToString(SearchText); Query = Query.Where(p => p.address == maddress); }
                        else if ("Company".Equals(SearchField)) { var mcompany = System.Convert.ToString(SearchText); Query = Query.Where(p => p.company == mcompany); }
                        else if ("Telno".Equals(SearchField)) { var mtelno = System.Convert.ToString(SearchText); Query = Query.Where(p => p.telno == mtelno); }
                        else if ("Cellno".Equals(SearchField)) { var mcellno = System.Convert.ToString(SearchText); Query = Query.Where(p => p.cellno == mcellno); }
                    } else if (SearchCondition == "More than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Birth Day".Equals(SearchField)) { var mbirthDay = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.birthDay > mbirthDay); }
                        else if ("Create Date".Equals(SearchField)) { var mcreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.createDate > mcreateDate); }
                    } else if (SearchCondition == "Less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Birth Day".Equals(SearchField)) { var mbirthDay = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.birthDay < mbirthDay); }
                        else if ("Create Date".Equals(SearchField)) { var mcreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.createDate < mcreateDate); }
                    } else if (SearchCondition == "Equal or more than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Birth Day".Equals(SearchField)) { var mbirthDay = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.birthDay >= mbirthDay); }
                        else if ("Create Date".Equals(SearchField)) { var mcreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.createDate >= mcreateDate); }
                    } else if (SearchCondition == "Equal or less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Birth Day".Equals(SearchField)) { var mbirthDay = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.birthDay <= mbirthDay); }
                        else if ("Create Date".Equals(SearchField)) { var mcreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.createDate <= mcreateDate); }
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
                case "birthDay_desc":
                    Query = Query.OrderByDescending(s => s.birthDay);
                    break;
                case "birthDay_asc":
                    Query = Query.OrderBy(s => s.birthDay);
                    break;
                case "gender_desc":
                    Query = Query.OrderByDescending(s => s.gender);
                    break;
                case "gender_asc":
                    Query = Query.OrderBy(s => s.gender);
                    break;
                case "createDate_desc":
                    Query = Query.OrderByDescending(s => s.createDate);
                    break;
                case "createDate_asc":
                    Query = Query.OrderBy(s => s.createDate);
                    break;
                case "education_desc":
                    Query = Query.OrderByDescending(s => s.education);
                    break;
                case "education_asc":
                    Query = Query.OrderBy(s => s.education);
                    break;
                case "address_desc":
                    Query = Query.OrderByDescending(s => s.address);
                    break;
                case "address_asc":
                    Query = Query.OrderBy(s => s.address);
                    break;
                case "company_desc":
                    Query = Query.OrderByDescending(s => s.company);
                    break;
                case "company_asc":
                    Query = Query.OrderBy(s => s.company);
                    break;
                case "telno_desc":
                    Query = Query.OrderByDescending(s => s.telno);
                    break;
                case "telno_asc":
                    Query = Query.OrderBy(s => s.telno);
                    break;
                case "cellno_desc":
                    Query = Query.OrderByDescending(s => s.cellno);
                    break;
                case "cellno_asc":
                    Query = Query.OrderBy(s => s.cellno);
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

        // GET: /student/Details/<id>
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
            student student = db.student.Find(
                                                 id
                                            );
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: /student/Create
        public ActionResult Create()
        {
            var vm = new student();
            // vm.
            ViewData.Model = vm;
            return View();
        }

        // POST: /student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include=
				           "name"
				   + "," + "birthDay"
				   + "," + "gender"
				   + "," + "createDate"
				   + "," + "education"
				   + "," + "address"
				   + "," + "company"
				   + "," + "telno"
				   + "," + "cellno"
				  )] student student)
        {
            if (ModelState.IsValid)
            {
                student.id = Guid.NewGuid();
                db.student.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: /student/Edit/<id>
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
            student student = db.student.Find(
                                                 id
                                            );
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        // POST: /student/Edit/<id>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include=
				     " id"
				   + ",name"
				   + ",birthDay"
				   + ",gender"
				   + ",createDate"
				   + ",education"
				   + ",address"
				   + ",company"
				   + ",telno"
				   + ",cellno"
				  )] student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: /student/Delete/<id>
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
            student student = db.student.Find(
                                                 id
                                            );
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: /student/Delete/<id>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(
                                            Guid id
                                            )
        {
            student student = db.student.Find(
                                                 id
                                            );
            db.student.Remove(student);
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
            SelectListItem Item3 = new SelectListItem { Text = "Birth Day", Value = "Birth Day" };
            SelectListItem Item4 = new SelectListItem { Text = "Gender", Value = "Gender" };
            SelectListItem Item5 = new SelectListItem { Text = "Create Date", Value = "Create Date" };
            SelectListItem Item6 = new SelectListItem { Text = "Education", Value = "Education" };
            SelectListItem Item7 = new SelectListItem { Text = "Address", Value = "Address" };
            SelectListItem Item8 = new SelectListItem { Text = "Company", Value = "Company" };
            SelectListItem Item9 = new SelectListItem { Text = "Telno", Value = "Telno" };
            SelectListItem Item10 = new SelectListItem { Text = "Cellno", Value = "Cellno" };

                 if (select == "Id") { Item1.Selected = true; }
            else if (select == "Name") { Item2.Selected = true; }
            else if (select == "Birth Day") { Item3.Selected = true; }
            else if (select == "Gender") { Item4.Selected = true; }
            else if (select == "Create Date") { Item5.Selected = true; }
            else if (select == "Education") { Item6.Selected = true; }
            else if (select == "Address") { Item7.Selected = true; }
            else if (select == "Company") { Item8.Selected = true; }
            else if (select == "Telno") { Item9.Selected = true; }
            else if (select == "Cellno") { Item10.Selected = true; }

            list.Add(Item1);
            list.Add(Item2);
            list.Add(Item3);
            list.Add(Item4);
            list.Add(Item5);
            list.Add(Item6);
            list.Add(Item7);
            list.Add(Item8);
            list.Add(Item9);
            list.Add(Item10);

            return list.ToList();
        }



        public ActionResult Export(string fileName, string selectedColumns)
        {
            //取得原始資料
            var _exportData = Session["ExportData"] as IQueryable<student>;
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
                    SheetName = "student匯出資料",
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
            bool result = !uow.Repository<student>().All().Count().Equals(0);
            jo.Add("Msg", result.ToString());
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
        
        
        private JArray GetExportData()
        {
            var threedays = DateTime.Now.AddDays(-3);
            var query = uow.Repository<student>().All();

            string str_json = JsonConvert.SerializeObject(query, Formatting.Indented);
            JArray jObjects = JsonConvert.DeserializeObject<JArray>(str_json.Trim());


            return jObjects;
        }





        private void ExportData(String Export, GridView gv, DataTable dt)
        {
            if (Export == "Pdf")
            {
                PDFform pdfForm = new PDFform(dt, "Dbo.Student", "Many");
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
 
