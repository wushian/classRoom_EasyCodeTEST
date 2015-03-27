using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    [MetadataType(typeof(studentMetadata))]
    [Table("student")]
    public partial class student
    {
        public  class  studentMetadata
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


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "生日")]
        [ExportColumn(Name = "生日", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime? birthDay { get; set; }

        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "性別")]
        [ExportColumn(Name = "性別", Order = 1)]
        //[UIHint("String")]
        public String gender { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "建立日期")]
        [ExportColumn(Name = "建立日期", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime? createDate { get; set; }

        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "學歷")]
        [ExportColumn(Name = "學歷", Order = 1)]
        //[UIHint("String")]
        public String education { get; set; }


        [StringLength(100, ErrorMessage = "不能超過 100 個字元")]
        [Display(Name = "地址")]
        [ExportColumn(Name = "地址", Order = 1)]
        //[UIHint("String")]
        public String address { get; set; }


        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "公司")]
        [ExportColumn(Name = "公司", Order = 1)]
        //[UIHint("String")]
        public String company { get; set; }


        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "電話")]
        [ExportColumn(Name = "電話", Order = 1)]
        //[UIHint("String")]
        public String telno { get; set; }


        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "手機")]
        [ExportColumn(Name = "手機", Order = 1)]
        //[UIHint("String")]
        public String cellno { get; set; }


    }
  }
}
 
