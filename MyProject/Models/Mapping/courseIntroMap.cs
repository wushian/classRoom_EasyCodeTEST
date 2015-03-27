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
        [Required(ErrorMessage = "�o�O�������")]
        [ScaffoldColumn(false)]
        public Guid id { get; set; }
       
        [Required(ErrorMessage = "�o�O�������")]
        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "�W��")]
        [ExportColumn(Name = "�W��", Order = 1)]
        //[UIHint("String")]
        public String name { get; set; }


        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "����")]
        [ExportColumn(Name = "����", Order = 1)]
        //[UIHint("String")]
        public String describe { get; set; }


        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "�Ϥ�")]
        [ExportColumn(Name = "�Ϥ�", Order = 1)]
        //[UIHint("String")]
        public String pic2 { get; set; }


    }
  }
}
 
