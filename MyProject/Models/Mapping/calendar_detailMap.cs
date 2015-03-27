using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    [MetadataType(typeof(calendar_detailMetadata))]
    [Table("calendar_detail")]
    public partial class calendar_detail
    {
        public  class  calendar_detailMetadata
        {
        [Key]
        [Column(Order = 0)]
        [Required(ErrorMessage = "�o�O�������")]
        [ScaffoldColumn(false)]
        public Guid id { get; set; }
       
        [Required(ErrorMessage = "�o�O�������")]
        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "�D��")]
        [ExportColumn(Name = "�D��", Order = 1)]
        //[UIHint("String")]
        public String subject { get; set; }


        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "�}�l�ɶ�")]
        [ExportColumn(Name = "�}�l�ɶ�", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime? startDateTime { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [UIHint("Date")]
        [Display(Name = "�����ɶ�")]
        [ExportColumn(Name = "�����ɶ�", Order = 1)]
        //[UIHint("DateTime")]
        public DateTime? endDateTime { get; set; }

        //[Range(xx,xx,ErrorMessage = "�����b xx �� xx ����")]
        [Display(Name = "�ǭ��O��")]
        [ExportColumn(Name = "�ǭ��O��", Order = 1)]
        //[UIHint("Int32")]
        public Int32? feeStaff { get; set; }

        //[Range(xx,xx,ErrorMessage = "�����b xx �� xx ����")]
        [Display(Name = "�D�ǭ��O��")]
        [ExportColumn(Name = "�D�ǭ��O��", Order = 1)]
        //[UIHint("Int32")]
        public Int32? feeNonStaff { get; set; }

        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "�p���H")]
        [ExportColumn(Name = "�p���H", Order = 1)]
        //[UIHint("String")]
        public String contact { get; set; }


        [StringLength(50, ErrorMessage = "����W�L 50 �Ӧr��")]
        [Display(Name = "�p���q��")]
        [ExportColumn(Name = "�p���q��", Order = 1)]
        //[UIHint("String")]
        public String contactTel { get; set; }


        [StringLength(200, ErrorMessage = "����W�L 200 �Ӧr��")]
        [Display(Name = "�p���a�}")]
        [ExportColumn(Name = "�p���a�}", Order = 1)]
        //[UIHint("String")]
        public String describe { get; set; }


    }
  }
}
 
