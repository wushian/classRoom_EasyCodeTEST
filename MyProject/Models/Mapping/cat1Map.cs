using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    [MetadataType(typeof(cat1Metadata))]
    [Table("cat1")]
    public partial class cat1
    {
        public  class  cat1Metadata
        {
        [Key]
        [Column(Order = 0)]
        [Required(ErrorMessage = "�o�O�������")]
        //[Range(xx,xx,ErrorMessage = "�����b xx �� xx ����")]
        [Display(Name = "id")]
        [ExportColumn(Name = "id", Order = 1)]
        //[UIHint("Int32")]
        public Int32 id { get; set; }
      
        [Required(ErrorMessage = "�o�O�������")]
        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "�W��")]
        [ExportColumn(Name = "�W��", Order = 1)]
        //[UIHint("String")]
        public String name { get; set; }


    }
  }
}
 
