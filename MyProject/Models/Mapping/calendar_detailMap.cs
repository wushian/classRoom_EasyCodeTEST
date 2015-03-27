using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    [MetadataType(typeof(calendar_detailMetadata))]
    [Table("calendar_detail")]
    public partial class calendar_detail
    {
        public  class  calendar_detailMetadata
        {
        [Key]
        [Column(Order = 0)]
        [Required(ErrorMessage = "這是必填欄位")]
        [ScaffoldColumn(false)]
        public Guid id { get; set; }
       
        [Required(ErrorMessage = "這是必填欄位")]
        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "主旨")]
        [ExportColumn(Name = "主旨", Order = 1)]
        //[UIHint("String")]
        public String subject { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "開始時間")]
        [ExportColumn(Name = "開始時間", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime? startDateTime { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "結束時間")]
        [ExportColumn(Name = "結束時間", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime? endDateTime { get; set; }

        //[Range(xx,xx,ErrorMessage = "必須在 xx 到 xx 之間")]
        [Display(Name = "學員費用")]
        [ExportColumn(Name = "學員費用", Order = 1)]
        //[UIHint("Int32")]
        public Int32? feeStaff { get; set; }

        //[Range(xx,xx,ErrorMessage = "必須在 xx 到 xx 之間")]
        [Display(Name = "非學員費用")]
        [ExportColumn(Name = "非學員費用", Order = 1)]
        //[UIHint("Int32")]
        public Int32? feeNonStaff { get; set; }

        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "聯絡人")]
        [ExportColumn(Name = "聯絡人", Order = 1)]
        //[UIHint("String")]
        public String contact { get; set; }


        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "聯絡電話")]
        [ExportColumn(Name = "聯絡電話", Order = 1)]
        //[UIHint("String")]
        public String contactTel { get; set; }


        [StringLength(200, ErrorMessage = "不能超過 200 個字元")]
        [Display(Name = "聯絡地址")]
        [ExportColumn(Name = "聯絡地址", Order = 1)]
        //[UIHint("String")]
        public String describe { get; set; }


    }
  }
}
 
