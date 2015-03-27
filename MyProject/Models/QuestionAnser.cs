using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    public partial class QuestionAnser
    {
        public Guid id { get; set; }
        public Int32? cat1 { get; set; }
        public Int32? cat2 { get; set; }
        public String pic1 { get; set; }
        public String pic2 { get; set; }
        public String describe { get; set; }

    }
}
 
