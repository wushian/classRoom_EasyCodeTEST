using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    [MetadataType(typeof(courseIntroMetadata))]
    [Table("courseIntro")]
    public partial class courseIntro
    {
        public  class  courseIntroMetadata
        {
        [Key]
        [Column(Order = 0)]
        [Required(ErrorMessage = "這是必填欄位")]
        [ScaffoldColumn(false)]
        public Guid id { get; set; }
       
        [Required(ErrorMessage = "這是必填欄位")]
        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "名稱")]
        [ExportColumn(Name = "名稱", Order = 1)]
        //[UIHint("String")]
        public String name { get; set; }


        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "說明")]
        [ExportColumn(Name = "說明", Order = 1)]
        //[UIHint("String")]
        public String describe { get; set; }


        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "圖片")]
        [ExportColumn(Name = "圖片", Order = 1)]
        //[UIHint("String")]
        public String pic2 { get; set; }


    }
  }
}
 
