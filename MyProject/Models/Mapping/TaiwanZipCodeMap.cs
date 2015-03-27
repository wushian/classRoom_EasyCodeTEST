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
        [Required(ErrorMessage = "�o�O�������")]
        //[Range(xx,xx,ErrorMessage = "�����b xx �� xx ����")]
        [Display(Name = "ID")]
        [ExportColumn(Name = "ID", Order = 1)]
        //[UIHint("Int32")]
        public Int32 ID { get; set; }
      
        [Required(ErrorMessage = "�o�O�������")]
        //[Range(xx,xx,ErrorMessage = "�����b xx �� xx ����")]
        [Display(Name = "�l���ϸ�")]
        [ExportColumn(Name = "�l���ϸ�", Order = 1)]
        //[UIHint("Int32")]
        public Int32 Zip { get; set; }
      
        [Required(ErrorMessage = "�o�O�������")]
        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "����")]
        [ExportColumn(Name = "����", Order = 1)]
        //[UIHint("String")]
        public String CityName { get; set; }


        [Required(ErrorMessage = "�o�O�������")]
        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "�m��")]
        [ExportColumn(Name = "�m��", Order = 1)]
        //[UIHint("String")]
        public String Town { get; set; }


        [Required(ErrorMessage = "�o�O�������")]
        //[Range(xx,xx,ErrorMessage = "�����b xx �� xx ����")]
        [Display(Name = "�Ǹ�")]
        [ExportColumn(Name = "�Ǹ�", Order = 1)]
        //[UIHint("Int32")]
        public Int32 Sequence { get; set; }
      
        [Required(ErrorMessage = "�o�O�������")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "�إߤ��")]
        [ExportColumn(Name = "�إߤ��", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime CreateDate { get; set; }
      
    }
  }
}
 
