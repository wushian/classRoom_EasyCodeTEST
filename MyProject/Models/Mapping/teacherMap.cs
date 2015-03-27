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
        [Required(ErrorMessage = "�o�O�������")]
        [ScaffoldColumn(false)]
        public Guid id { get; set; }
       
        [Required(ErrorMessage = "�o�O�������")]
        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "�m�W")]
        [ExportColumn(Name = "�m�W", Order = 1)]
        //[UIHint("String")]
        public String name { get; set; }


        [StringLength(20, ErrorMessage = "����W�L 20 �Ӧr��")]
        [Display(Name = "�ʧO")]
        [ExportColumn(Name = "�ʧO", Order = 1)]
        //[UIHint("String")]
        public String gender { get; set; }


        [Required(ErrorMessage = "�o�O�������")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "�إߤ��")]
        [ExportColumn(Name = "�إߤ��", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime createDate { get; set; }
      
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "�ͤ�")]
        [ExportColumn(Name = "�ͤ�", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime? birthDay { get; set; }

        [StringLength(100, ErrorMessage = "����W�L 100 �Ӧr��")]
        [Display(Name = "�a�}")]
        [ExportColumn(Name = "�a�}", Order = 1)]
        //[UIHint("String")]
        public String address { get; set; }


        [StringLength(100, ErrorMessage = "����W�L 100 �Ӧr��")]
        [Display(Name = "���q�a�}")]
        [ExportColumn(Name = "���q�a�}", Order = 1)]
        //[UIHint("String")]
        public String officeAddress { get; set; }


        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "�q��")]
        [ExportColumn(Name = "�q��", Order = 1)]
        //[UIHint("String")]
        public String telPhone { get; set; }


        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "���")]
        [ExportColumn(Name = "���", Order = 1)]
        //[UIHint("String")]
        public String cellPhone { get; set; }


        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "¾��")]
        [ExportColumn(Name = "¾��", Order = 1)]
        //[UIHint("String")]
        public String job { get; set; }


        [StringLength(200, ErrorMessage = "����W�L 200 �Ӧr��")]
        [Display(Name = "�M��")]
        [ExportColumn(Name = "�M��", Order = 1)]
        //[UIHint("String")]
        public String ability { get; set; }


        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "�Ǿ�")]
        [ExportColumn(Name = "�Ǿ�", Order = 1)]
        //[UIHint("String")]
        public String educational { get; set; }


    }
  }
}
 
