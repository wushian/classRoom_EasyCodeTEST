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
    public class teacherController : Controller
    {
          //private string fileSavedPath = WebConfigurationManager.AppSettings["UploadPath"];

		private GenericUnitOfWork uow = null;
		// 宣告預設建構式
		public teacherController()
		{
		    this.uow = new GenericUnitOfWork();
		}

		public teacherController(GenericUnitOfWork uow_)
		{
		   this.uow = uow_;
		}

        private classRoomWebSiteDBContext db = new classRoomWebSiteDBContext();
        
        // GET: /teacher/
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
                ExportColumnAttributeHelper<teacher>.GetExportColumns()
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
            ViewData["genderSortParm"] = sortOrder == "gender_asc" ? "gender_desc" : "gender_asc";
            ViewData["createDateSortParm"] = sortOrder == "createDate_asc" ? "createDate_desc" : "createDate_asc";
            ViewData["birthDaySortParm"] = sortOrder == "birthDay_asc" ? "birthDay_desc" : "birthDay_asc";
            ViewData["addressSortParm"] = sortOrder == "address_asc" ? "address_desc" : "address_asc";
            ViewData["officeAddressSortParm"] = sortOrder == "officeAddress_asc" ? "officeAddress_desc" : "officeAddress_asc";
            ViewData["telPhoneSortParm"] = sortOrder == "telPhone_asc" ? "telPhone_desc" : "telPhone_asc";
            ViewData["cellPhoneSortParm"] = sortOrder == "cellPhone_asc" ? "cellPhone_desc" : "cellPhone_asc";
            ViewData["jobSortParm"] = sortOrder == "job_asc" ? "job_desc" : "job_asc";
            ViewData["abilitySortParm"] = sortOrder == "ability_asc" ? "ability_desc" : "ability_asc";
            ViewData["educationalSortParm"] = sortOrder == "educational_asc" ? "educational_desc" : "educational_asc";

            var Query = from _teacher in db.teacher
                        select new teacher() {
                            id = _teacher.id
                           ,name = _teacher.name
                           ,gender = _teacher.gender
                           ,createDate = _teacher.createDate
                           ,birthDay = _teacher.birthDay
                           ,address = _teacher.address
                           ,officeAddress = _teacher.officeAddress
                           ,telPhone = _teacher.telPhone
                           ,cellPhone = _teacher.cellPhone
                           ,job = _teacher.job
                           ,ability = _teacher.ability
                           ,educational = _teacher.educational
                        };

            Query = db.teacher.AsQueryable();

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
                                                 || ("Gender".ToString().Equals(SearchField) && p.gender.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Create Date".ToString().Equals(SearchField) && p.createDate.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Birth Day".ToString().Equals(SearchField) && p.birthDay.Value.ToString().Contains(SearchText)) 
                                                 || ("Address".ToString().Equals(SearchField) && p.address.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Office Address".ToString().Equals(SearchField) && p.officeAddress.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Tel Phone".ToString().Equals(SearchField) && p.telPhone.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Cell Phone".ToString().Equals(SearchField) && p.cellPhone.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Job".ToString().Equals(SearchField) && p.job.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("ability".ToString().Equals(SearchField) && p.ability.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                                 || ("Educational".ToString().Equals(SearchField) && p.educational.ToString().Trim().ToLower().Contains(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Starts with...") {
                        Query = Query.Where(p => 
                                                 ("Id".ToString().Equals(SearchField) && p.id.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Name".ToString().Equals(SearchField) && p.name.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Gender".ToString().Equals(SearchField) && p.gender.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Create Date".ToString().Equals(SearchField) && p.createDate.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Birth Day".ToString().Equals(SearchField) && p.birthDay.Value.ToString().StartsWith(SearchText)) 
                                                 || ("Address".ToString().Equals(SearchField) && p.address.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Office Address".ToString().Equals(SearchField) && p.officeAddress.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Tel Phone".ToString().Equals(SearchField) && p.telPhone.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Cell Phone".ToString().Equals(SearchField) && p.cellPhone.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Job".ToString().Equals(SearchField) && p.job.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("ability".ToString().Equals(SearchField) && p.ability.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                                 || ("Educational".ToString().Equals(SearchField) && p.educational.ToString().Trim().ToLower().StartsWith(SearchText.Trim().ToLower())) 
                                         );
                    } else if (SearchCondition == "Equals") {
                         if ("Name".Equals(SearchField)) { var mname = System.Convert.ToString(SearchText); Query = Query.Where(p => p.name == mname); }
                        else if ("Gender".Equals(SearchField)) { var mgender = System.Convert.ToString(SearchText); Query = Query.Where(p => p.gender == mgender); }
                        else if ("Create Date".Equals(SearchField)) { var mcreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.createDate == mcreateDate); }
                        else if ("Birth Day".Equals(SearchField)) { var mbirthDay = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.birthDay == mbirthDay); }
                        else if ("Address".Equals(SearchField)) { var maddress = System.Convert.ToString(SearchText); Query = Query.Where(p => p.address == maddress); }
                        else if ("Office Address".Equals(SearchField)) { var mofficeAddress = System.Convert.ToString(SearchText); Query = Query.Where(p => p.officeAddress == mofficeAddress); }
                        else if ("Tel Phone".Equals(SearchField)) { var mtelPhone = System.Convert.ToString(SearchText); Query = Query.Where(p => p.telPhone == mtelPhone); }
                        else if ("Cell Phone".Equals(SearchField)) { var mcellPhone = System.Convert.ToString(SearchText); Query = Query.Where(p => p.cellPhone == mcellPhone); }
                        else if ("Job".Equals(SearchField)) { var mjob = System.Convert.ToString(SearchText); Query = Query.Where(p => p.job == mjob); }
                        else if ("ability".Equals(SearchField)) { var mability = System.Convert.ToString(SearchText); Query = Query.Where(p => p.ability == mability); }
                        else if ("Educational".Equals(SearchField)) { var meducational = System.Convert.ToString(SearchText); Query = Query.Where(p => p.educational == meducational); }
                    } else if (SearchCondition == "More than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Create Date".Equals(SearchField)) { var mcreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.createDate > mcreateDate); }
                        else if ("Birth Day".Equals(SearchField)) { var mbirthDay = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.birthDay > mbirthDay); }
                    } else if (SearchCondition == "Less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Create Date".Equals(SearchField)) { var mcreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.createDate < mcreateDate); }
                        else if ("Birth Day".Equals(SearchField)) { var mbirthDay = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.birthDay < mbirthDay); }
                    } else if (SearchCondition == "Equal or more than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Create Date".Equals(SearchField)) { var mcreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.createDate >= mcreateDate); }
                        else if ("Birth Day".Equals(SearchField)) { var mbirthDay = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.birthDay >= mbirthDay); }
                    } else if (SearchCondition == "Equal or less than...") { 
                        if (SearchField.Equals(SearchCondition)) { }
                        else if ("Create Date".Equals(SearchField)) { var mcreateDate = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.createDate <= mcreateDate); }
                        else if ("Birth Day".Equals(SearchField)) { var mbirthDay = System.Convert.ToDateTime(SearchText); Query = Query.Where(p => p.birthDay <= mbirthDay); }
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
                case "birthDay_desc":
                    Query = Query.OrderByDescending(s => s.birthDay);
                    break;
                case "birthDay_asc":
                    Query = Query.OrderBy(s => s.birthDay);
                    break;
                case "address_desc":
                    Query = Query.OrderByDescending(s => s.address);
                    break;
                case "address_asc":
                    Query = Query.OrderBy(s => s.address);
                    break;
                case "officeAddress_desc":
                    Query = Query.OrderByDescending(s => s.officeAddress);
                    break;
                case "officeAddress_asc":
                    Query = Query.OrderBy(s => s.officeAddress);
                    break;
                case "telPhone_desc":
                    Query = Query.OrderByDescending(s => s.telPhone);
                    break;
                case "telPhone_asc":
                    Query = Query.OrderBy(s => s.telPhone);
                    break;
                case "cellPhone_desc":
                    Query = Query.OrderByDescending(s => s.cellPhone);
                    break;
                case "cellPhone_asc":
                    Query = Query.OrderBy(s => s.cellPhone);
                    break;
                case "job_desc":
                    Query = Query.OrderByDescending(s => s.job);
                    break;
                case "job_asc":
                    Query = Query.OrderBy(s => s.job);
                    break;
                case "ability_desc":
                    Query = Query.OrderByDescending(s => s.ability);
                    break;
                case "ability_asc":
                    Query = Query.OrderBy(s => s.ability);
                    break;
                case "educational_desc":
                    Query = Query.OrderByDescending(s => s.educational);
                    break;
                case "educational_asc":
                    Query = Query.OrderBy(s => s.educational);
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

        // GET: /teacher/Details/<id>
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
            teacher teacher = db.teacher.Find(
                                                 id
                                            );
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // GET: /teacher/Create
        public ActionResult Create()
        {
            var vm = new teacher();
            // vm.
            ViewData.Model = vm;
            return View();
        }

        // POST: /teacher/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include=
				           "name"
				   + "," + "gender"
				   + "," + "createDate"
				   + "," + "birthDay"
				   + "," + "address"
				   + "," + "officeAddress"
				   + "," + "telPhone"
				   + "," + "cellPhone"
				   + "," + "job"
				   + "," + "ability"
				   + "," + "educational"
				  )] teacher teacher)
        {
            if (ModelState.IsValid)
            {
                teacher.id = Guid.NewGuid();
                db.teacher.Add(teacher);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(teacher);
        }

        // GET: /teacher/Edit/<id>
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
            teacher teacher = db.teacher.Find(
                                                 id
                                            );
            if (teacher == null)
            {
                return HttpNotFound();
            }

            return View(teacher);
        }

        // POST: /teacher/Edit/<id>
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include=
				     " id"
				   + ",name"
				   + ",gender"
				   + ",createDate"
				   + ",birthDay"
				   + ",address"
				   + ",officeAddress"
				   + ",telPhone"
				   + ",cellPhone"
				   + ",job"
				   + ",ability"
				   + ",educational"
				  )] teacher teacher)
        {
            if (ModelState.IsValid)
            {
                db.Entry(teacher).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(teacher);
        }

        // GET: /teacher/Delete/<id>
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
            teacher teacher = db.teacher.Find(
                                                 id
                                            );
            if (teacher == null)
            {
                return HttpNotFound();
            }
            return View(teacher);
        }

        // POST: /teacher/Delete/<id>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(
                                            Guid id
                                            )
        {
            teacher teacher = db.teacher.Find(
                                                 id
                                            );
            db.teacher.Remove(teacher);
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
            SelectListItem Item3 = new SelectListItem { Text = "Gender", Value = "Gender" };
            SelectListItem Item4 = new SelectListItem { Text = "Create Date", Value = "Create Date" };
            SelectListItem Item5 = new SelectListItem { Text = "Birth Day", Value = "Birth Day" };
            SelectListItem Item6 = new SelectListItem { Text = "Address", Value = "Address" };
            SelectListItem Item7 = new SelectListItem { Text = "Office Address", Value = "Office Address" };
            SelectListItem Item8 = new SelectListItem { Text = "Tel Phone", Value = "Tel Phone" };
            SelectListItem Item9 = new SelectListItem { Text = "Cell Phone", Value = "Cell Phone" };
            SelectListItem Item10 = new SelectListItem { Text = "Job", Value = "Job" };
            SelectListItem Item11 = new SelectListItem { Text = "ability", Value = "ability" };
            SelectListItem Item12 = new SelectListItem { Text = "Educational", Value = "Educational" };

                 if (select == "Id") { Item1.Selected = true; }
            else if (select == "Name") { Item2.Selected = true; }
            else if (select == "Gender") { Item3.Selected = true; }
            else if (select == "Create Date") { Item4.Selected = true; }
            else if (select == "Birth Day") { Item5.Selected = true; }
            else if (select == "Address") { Item6.Selected = true; }
            else if (select == "Office Address") { Item7.Selected = true; }
            else if (select == "Tel Phone") { Item8.Selected = true; }
            else if (select == "Cell Phone") { Item9.Selected = true; }
            else if (select == "Job") { Item10.Selected = true; }
            else if (select == "ability") { Item11.Selected = true; }
            else if (select == "Educational") { Item12.Selected = true; }

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
            list.Add(Item11);
            list.Add(Item12);

            return list.ToList();
        }



        public ActionResult Export(string fileName, string selectedColumns)
        {
            //取得原始資料
            var _exportData = Session["ExportData"] as IQueryable<teacher>;
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
                    SheetName = "teacher匯出資料",
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
            bool result = !uow.Repository<teacher>().All().Count().Equals(0);
            jo.Add("Msg", result.ToString());
            return Content(JsonConvert.SerializeObject(jo), "application/json");
        }
        
        
        private JArray GetExportData()
        {
            var threedays = DateTime.Now.AddDays(-3);
            var query = uow.Repository<teacher>().All();

            string str_json = JsonConvert.SerializeObject(query, Formatting.Indented);
            JArray jObjects = JsonConvert.DeserializeObject<JArray>(str_json.Trim());


            return jObjects;
        }





        private void ExportData(String Export, GridView gv, DataTable dt)
        {
            if (Export == "Pdf")
            {
                PDFform pdfForm = new PDFform(dt, "Dbo.Teacher", "Many");
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
 
