using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;

namespace WebTask.Models
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [DisplayName("First name")]
        public string FirstName { get; set; }
        [DisplayName("Last name")]
        public string LastName { get; set; }
        

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}"),DisplayName("Birth date")]
        public Nullable<System.DateTime> BirthDate { get; set; }

        [DisplayName("Manager")]
        public string Manager { get; set; }
        [DisplayName("Department ID")]
        public int DepartmentId { get; set; }
        [DisplayName("Department Name")]
        public string DepartmentName { get; set; }

        public virtual Address Address { get; set; }

        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
    
        
    }
}