using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.Entity
{
    [Table("Document")]
    public class Document
    {
        public int ID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss}", ApplyFormatInEditMode = false)]
        public DateTime UploadDate { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public int UserID { get; set; }
    }
}
