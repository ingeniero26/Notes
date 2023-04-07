using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Notes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace Notes.Classes
{
    public class Utilities : IDisposable
    {
        private static ApplicationDbContext userContext =
            new ApplicationDbContext();
        private static NotesContext db = new NotesContext();

        private static void CheckRole(string roleName)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(userContext));
            if(!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));
            }
        }

        private static void CheckSuperUser(string role)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var email = WebConfigurationManager.AppSettings["AdminUser"];
            var password = WebConfigurationManager.AppSettings["AdminPassword"];
            var userAsp = userManager.FindByName(email);
            if(userAsp == null )
            {
                CreateUserASP(email, role, password);
                return;
            }
        }

        public static void CreateUserASP(string email)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = new ApplicationUser
            {
                Email = email,
                UserName = email,
            };
            userManager.Create(userASP, email);
        }
        ///
        public static void CreateUserASP(string email, string roleName)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = new ApplicationUser
            {
                Email = email,
                UserName = email,
            };
            userManager.Create(userASP, email);
            userManager.AddToRole(userASP.Id, roleName);
        }

        public static void CreateUserASP(string email, string roleName, string password)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = new ApplicationUser
            {
                Email = email,
                UserName = email,
            };
            userManager.Create(userASP, email);
            userManager.AddToRole(userASP.Id, roleName);
        }

        //
        public static void AddRoleToUser(string email, string roleName)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var userASP = userManager.FindByEmail(email);
            if(userASP ==null)
            {
                return;
            }
        }
        //
        public static async Task SendEmail(string to, string subject, string body)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(to));
            message.From = new MailAddress(WebConfigurationManager.AppSettings["AdminUser"]);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = WebConfigurationManager.AppSettings["AdminUser"],
                    Password = WebConfigurationManager.AppSettings["AdminPassword"]
                };

                smtp.Credentials = credential;
                smtp.Host = WebConfigurationManager.AppSettings["SMTPName"];
                smtp.Port = int.Parse(WebConfigurationManager.AppSettings["SMTPPort"]);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }
            //
            
        }
        //
        public static async Task SendMail(List<string> mails, string subjet, string body)
        {
            var message = new MailMessage();
            foreach( var to in mails)
            {
                message.To.Add(new MailAddress(to));
            }
            message.From = new MailAddress(WebConfigurationManager.AppSettings["AdminUser"]);
            message.Subject = subjet;
            message.Body = body;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = WebConfigurationManager.AppSettings["AdminUser"],
                    Password = WebConfigurationManager.AppSettings["AdminPassword"]
                };

                smtp.Credentials = credential;
                smtp.Host = WebConfigurationManager.AppSettings["SMTPName"];
                smtp.Port = int.Parse(WebConfigurationManager.AppSettings["SMTPPort"]);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(message);
            }

        }
        //

        public static async Task PasswordRecovery(string email)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

            var userASP = userManager.FindByEmail(email);
            if(userASP == null)
            {
                return;
            }
            var user = db.Users.Where(tp => tp.UserName == email).FirstOrDefault();
            if(user == null)
            {
                return;
            }
            var random = new Random();
            var newPassword = string.Format("{0}{1}{2:04}*",
                user.FirstName.Trim().ToUpper().Substring(0, 1),
                user.LastName.Trim().ToLower(),
                random.Next(100000));

            userManager.RemovePassword(userASP.Id);
            userManager.AddPassword(userASP.Id, newPassword);

            var subject = "Notes Password recovery";
            var body = string.Format(@"<h1> Password Recovery </h1>
                                       <p> Your new password is: <strong> {0} </strong></p> 
                                        <p>please change it for one, that you remember", newPassword);

            await SendEmail(email, subject, body);
        }

        //subir imagenes
        public static string UploadPhoto(HttpPostedFileBase file)
        {
            string path = string.Empty;
            string pic = string.Empty;

            if(file != null)
            {
                pic = Path.GetExtension(file.FileName);
                path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Photos"), pic);
                file.SaveAs(path);
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }
            }
            return pic;
        }

        public void Dispose()
        {
            userContext.Dispose();
            db.Dispose();
        }

    }
}