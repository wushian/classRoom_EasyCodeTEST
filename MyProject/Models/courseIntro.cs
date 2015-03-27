using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using MyProject.Filters;

namespace MyProject.Models
{
    public partial class courseIntro
    {
        public Guid id { get; set; }
        public String name { get; set; }
        public String describe { get; set; }
        public String pic2 { get; set; }

    }
}
 
