using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Notes.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Display(Name = "E-Mail")]
        [Required(ErrorMessage = "You must enter a {0}")]
        [StringLength(100, ErrorMessage =
            "The field {0} can contain maximun {1} and minimum {2} characters",
            MinimumLength = 20)]
        [DataType(DataType.EmailAddress)]
        [Index("UserNameIndex", IsUnique = true)]
        public string UserName { get; set; }

        [Column("FirstName")]
        [Required(ErrorMessage = "You must enter a {0}")]
        [StringLength(30, ErrorMessage =
            "The field {0} can contain maximun {1} and minimum {2} characters",
            MinimumLength = 3)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "You must enter a {0}")]
        [StringLength(30, ErrorMessage =
            "The field {0} can contain maximun {1} and minimum {2} characters",
            MinimumLength = 3)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name ="User")]
        public string FullName { get { return string.Format("{0} {1}", this.FirstName, this.LastName); } }

        [Required(ErrorMessage = "You must enter a {0} is required")]
        [StringLength(30, ErrorMessage =
            "The field {0} can contain maximun {1} and minimum {2} characters",
            MinimumLength = 3)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "You must enter a {0} is required")]
        [StringLength(100, ErrorMessage =
            "The field {0} can contain maximun {1} and minimum {2} characters",
            MinimumLength = 3)]
        public string Address { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Photo { get; set; }

        [Display(Name = "Is Student")]
        public bool IsStudent { get; set; }

        [Display(Name = "Is Teacher")]
        public bool IsTeacher { get; set; }
    }
}