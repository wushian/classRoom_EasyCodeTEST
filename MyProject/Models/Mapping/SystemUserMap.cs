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
        [Required(ErrorMessage = "�o�O�������")]
        [ScaffoldColumn(false)]
        public Guid ID { get; set; }
       
        [Required(ErrorMessage = "�o�O�������")]
        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "�m�W")]
        [ExportColumn(Name = "�m�W", Order = 1)]
        //[UIHint("String")]
        public String Name { get; set; }


        [Required(ErrorMessage = "�o�O�������")]
        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "�b��")]
        [ExportColumn(Name = "�b��", Order = 1)]
        //[UIHint("String")]
        public String Account { get; set; }


        [Required(ErrorMessage = "�o�O�������")]
        [StringLength(100, ErrorMessage = "����W�L 100 �Ӧr��")]
        [Display(Name = "�K�X")]
        [ExportColumn(Name = "�K�X", Order = 1)]
        //[UIHint("String")]
        public String Password { get; set; }


        [Required(ErrorMessage = "�o�O�������")]
        [StringLength(256, ErrorMessage = "����W�L 256 �Ӧr��")]
        [Display(Name = "Salt")]
        [ExportColumn(Name = "Salt", Order = 1)]
        //[UIHint("String")]
        public String Salt { get; set; }


        [Required(ErrorMessage = "�o�O�������")]
        [StringLength(256, ErrorMessage = "����W�L 256 �Ӧr��")]
        [Display(Name = "�q�l�l��")]
        [ExportColumn(Name = "�q�l�l��", Order = 1)]
        //[UIHint("String")]
        public String Email { get; set; }


        [Required(ErrorMessage = "�o�O�������")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "�إߤ��")]
        [ExportColumn(Name = "�إߤ��", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime CreateDate { get; set; }
      
        [Required(ErrorMessage = "�o�O�������")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "��s���")]
        [ExportColumn(Name = "��s���", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime UpdateDate { get; set; }
      
    }
  }
}
 
