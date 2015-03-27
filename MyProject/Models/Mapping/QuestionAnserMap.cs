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
        [Required(ErrorMessage = "�o�O�������")]
        [ScaffoldColumn(false)]
        public Guid id { get; set; }
       
        //[Range(xx,xx,ErrorMessage = "�����b xx �� xx ����")]
        [Display(Name = "�D���O")]
        [ExportColumn(Name = "�D���O", Order = 1)]
        //[UIHint("Int32")]
        public Int32? cat1 { get; set; }

        //[Range(xx,xx,ErrorMessage = "�����b xx �� xx ����")]
        [Display(Name = "�����O")]
        [ExportColumn(Name = "�����O", Order = 1)]
        //[UIHint("Int32")]
        public Int32? cat2 { get; set; }

        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "�Ϥ�")]
        [ExportColumn(Name = "�Ϥ�", Order = 1)]
        //[UIHint("String")]
        public String pic1 { get; set; }


        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "�Ϥ�")]
        [ExportColumn(Name = "�Ϥ�", Order = 1)]
        //[UIHint("String")]
        public String pic2 { get; set; }


        [StringLength(100, ErrorMessage = "����W�L 100 �Ӧr��")]
        [Display(Name = "����")]
        [ExportColumn(Name = "����", Order = 1)]
        //[UIHint("String")]
        public String describe { get; set; }


    }
  }
}
 
