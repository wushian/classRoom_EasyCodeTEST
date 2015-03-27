using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    [MetadataType(typeof(teacherMetadata))]
    [Table("teacher")]
    public partial class teacher
    {
        public  class  teacherMetadata
        {
        [Key]
        [Column(Order = 0)]
        [Required(ErrorMessage = "這是必填欄位")]
        [ScaffoldColumn(false)]
        public Guid id { get; set; }
       
        [Required(ErrorMessage = "這是必填欄位")]
        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "姓名")]
        [ExportColumn(Name = "姓名", Order = 1)]
        //[UIHint("String")]
        public String name { get; set; }


        [StringLength(20, ErrorMessage = "不能超過 20 個字元")]
        [Display(Name = "性別")]
        [ExportColumn(Name = "性別", Order = 1)]
        //[UIHint("String")]
        public String gender { get; set; }


        [Required(ErrorMessage = "這是必填欄位")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "建立日期")]
        [ExportColumn(Name = "建立日期", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime createDate { get; set; }
      
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "生日")]
        [ExportColumn(Name = "生日", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime? birthDay { get; set; }

        [StringLength(100, ErrorMessage = "不能超過 100 個字元")]
        [Display(Name = "地址")]
        [ExportColumn(Name = "地址", Order = 1)]
        //[UIHint("String")]
        public String address { get; set; }


        [StringLength(100, ErrorMessage = "不能超過 100 個字元")]
        [Display(Name = "公司地址")]
        [ExportColumn(Name = "公司地址", Order = 1)]
        //[UIHint("String")]
        public String officeAddress { get; set; }


        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "電話")]
        [ExportColumn(Name = "電話", Order = 1)]
        //[UIHint("String")]
        public String telPhone { get; set; }


        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "手機")]
        [ExportColumn(Name = "手機", Order = 1)]
        //[UIHint("String")]
        public String cellPhone { get; set; }


        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "職稱")]
        [ExportColumn(Name = "職稱", Order = 1)]
        //[UIHint("String")]
        public String job { get; set; }


        [StringLength(200, ErrorMessage = "不能超過 200 個字元")]
        [Display(Name = "專長")]
        [ExportColumn(Name = "專長", Order = 1)]
        //[UIHint("String")]
        public String ability { get; set; }


        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "學歷")]
        [ExportColumn(Name = "學歷", Order = 1)]
        //[UIHint("String")]
        public String educational { get; set; }


    }
  }
}
 
