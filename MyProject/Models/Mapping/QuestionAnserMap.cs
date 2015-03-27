using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    [MetadataType(typeof(QuestionAnserMetadata))]
    [Table("QuestionAnser")]
    public partial class QuestionAnser
    {
        public  class  QuestionAnserMetadata
        {
        [Key]
        [Column(Order = 0)]
        [Required(ErrorMessage = "這是必填欄位")]
        [ScaffoldColumn(false)]
        public Guid id { get; set; }
       
        //[Range(xx,xx,ErrorMessage = "必須在 xx 到 xx 之間")]
        [Display(Name = "主類別")]
        [ExportColumn(Name = "主類別", Order = 1)]
        //[UIHint("Int32")]
        public Int32? cat1 { get; set; }

        //[Range(xx,xx,ErrorMessage = "必須在 xx 到 xx 之間")]
        [Display(Name = "次類別")]
        [ExportColumn(Name = "次類別", Order = 1)]
        //[UIHint("Int32")]
        public Int32? cat2 { get; set; }

        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "圖片")]
        [ExportColumn(Name = "圖片", Order = 1)]
        //[UIHint("String")]
        public String pic1 { get; set; }


        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "圖片")]
        [ExportColumn(Name = "圖片", Order = 1)]
        //[UIHint("String")]
        public String pic2 { get; set; }


        [StringLength(100, ErrorMessage = "不能超過 100 個字元")]
        [Display(Name = "說明")]
        [ExportColumn(Name = "說明", Order = 1)]
        //[UIHint("String")]
        public String describe { get; set; }


    }
  }
}
 
