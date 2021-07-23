using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models.Entity
{
    [Table("User")]
    public class User
    {
        public int ID { get; set; }
       
        public string FullName { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime Birthdate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public byte[] PersonalPhoto { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss}", ApplyFormatInEditMode = false)]
        public DateTime RegisterDate { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}
