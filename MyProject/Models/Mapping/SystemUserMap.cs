using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    [MetadataType(typeof(SystemUserMetadata))]
    [Table("SystemUser")]
    public partial class SystemUser
    {
        public  class  SystemUserMetadata
        {
        [Key]
        [Column(Order = 0)]
        [Required(ErrorMessage = "這是必填欄位")]
        [ScaffoldColumn(false)]
        public Guid ID { get; set; }
       
        [Required(ErrorMessage = "這是必填欄位")]
        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "姓名")]
        [ExportColumn(Name = "姓名", Order = 1)]
        //[UIHint("String")]
        public String Name { get; set; }


        [Required(ErrorMessage = "這是必填欄位")]
        [StringLength(50, ErrorMessage = "不能超過 50 個字元")]
        [Display(Name = "帳號")]
        [ExportColumn(Name = "帳號", Order = 1)]
        //[UIHint("String")]
        public String Account { get; set; }


        [Required(ErrorMessage = "這是必填欄位")]
        [StringLength(100, ErrorMessage = "不能超過 100 個字元")]
        [Display(Name = "密碼")]
        [ExportColumn(Name = "密碼", Order = 1)]
        //[UIHint("String")]
        public String Password { get; set; }


        [Required(ErrorMessage = "這是必填欄位")]
        [StringLength(256, ErrorMessage = "不能超過 256 個字元")]
        [Display(Name = "Salt")]
        [ExportColumn(Name = "Salt", Order = 1)]
        //[UIHint("String")]
        public String Salt { get; set; }


        [Required(ErrorMessage = "這是必填欄位")]
        [StringLength(256, ErrorMessage = "不能超過 256 個字元")]
        [Display(Name = "電子郵件")]
        [ExportColumn(Name = "電子郵件", Order = 1)]
        //[UIHint("String")]
        public String Email { get; set; }


        [Required(ErrorMessage = "這是必填欄位")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "建立日期")]
        [ExportColumn(Name = "建立日期", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime CreateDate { get; set; }
      
        [Required(ErrorMessage = "這是必填欄位")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "更新日期")]
        [ExportColumn(Name = "更新日期", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime UpdateDate { get; set; }
      
    }
  }
}
 
