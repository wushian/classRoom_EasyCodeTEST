using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    [MetadataType(typeof(cat2Metadata))]
    [Table("cat2")]
    public partial class cat2
    {
        public  class  cat2Metadata
        {
        [Key]
        [Column(Order = 0)]
        [Required(ErrorMessage = "這是必填欄位")]
        //[Range(xx,xx,ErrorMessage = "必須在 xx 到 xx 之間")]
        [Display(Name = "id")]
        [ExportColumn(Name = "id", Order = 1)]
        //[UIHint("Int32")]
        public Int32 id { get; set; }
      
        [Required(ErrorMessage = "這是必填欄位")]
        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "名稱")]
        [ExportColumn(Name = "名稱", Order = 1)]
        //[UIHint("String")]
        public String name { get; set; }


    }
  }
}
 
