using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Notes.Models
{
    public class UserView
    {
        public User User { get; set; }

        public HttpPostedFileBase Photo { get; set; }
}
}