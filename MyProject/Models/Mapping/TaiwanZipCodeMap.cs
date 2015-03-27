using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    [MetadataType(typeof(TaiwanZipCodeMetadata))]
    [Table("TaiwanZipCode")]
    public partial class TaiwanZipCode
    {
        public  class  TaiwanZipCodeMetadata
        {
        [Key]
        [Column(Order = 0)]
        [Required(ErrorMessage = "這是必填欄位")]
        //[Range(xx,xx,ErrorMessage = "必須在 xx 到 xx 之間")]
        [Display(Name = "ID")]
        [ExportColumn(Name = "ID", Order = 1)]
        //[UIHint("Int32")]
        public Int32 ID { get; set; }
      
        [Required(ErrorMessage = "這是必填欄位")]
        //[Range(xx,xx,ErrorMessage = "必須在 xx 到 xx 之間")]
        [Display(Name = "郵遞區號")]
        [ExportColumn(Name = "郵遞區號", Order = 1)]
        //[UIHint("Int32")]
        public Int32 Zip { get; set; }
      
        [Required(ErrorMessage = "這是必填欄位")]
        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "縣市")]
        [ExportColumn(Name = "縣市", Order = 1)]
        //[UIHint("String")]
        public String CityName { get; set; }


        [Required(ErrorMessage = "這是必填欄位")]
        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "鄉鎮")]
        [ExportColumn(Name = "鄉鎮", Order = 1)]
        //[UIHint("String")]
        public String Town { get; set; }


        [Required(ErrorMessage = "這是必填欄位")]
        //[Range(xx,xx,ErrorMessage = "必須在 xx 到 xx 之間")]
        [Display(Name = "序號")]
        [ExportColumn(Name = "序號", Order = 1)]
        //[UIHint("Int32")]
        public Int32 Sequence { get; set; }
      
        [Required(ErrorMessage = "這是必填欄位")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "建立日期")]
        [ExportColumn(Name = "建立日期", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime CreateDate { get; set; }
      
    }
  }
}
 
