using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using OnlinePictureStorage.Constants;
using OnlinePictureStorage.ViewModels;

namespace OnlinePictureStorage.Pages
{
    [Authorize]

    public class ShareModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string pid { set; get; }

        [BindProperty]
        public Share SModel { get; set; }

        private readonly UserManager<IdentityUser> userManager;

        public ShareModel(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public void OnGet()
        {
        }

        public  IActionResult OnPostSubmit(string returnURL = null)
        {
            string userid = userManager.GetUserId(HttpContext.User);
            string guestid = GetGuestId(SModel.Email);

            AddShare(userid, guestid, pid);

            return RedirectToPage("/Home");
        }

        public string GetGuestId(string email)
        {
            string guestid = "";

            using SqlConnection connection = new SqlConnection(Connections.sqlConnectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT  Id "
                                + "FROM AspNetUsers "
                                + "WHERE NormalizedEmail  = UPPER(@email)";

            command.Parameters.AddWithValue("@email", email);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    guestid = String.Format("{0}", reader[0]);
                }
            }
            finally
            {
                reader.Close();
            }

            return guestid;
        }

        public void AddShare(string ownerid, string guestid, string pictureid)
        {
            using SqlConnection connection = new SqlConnection(Connections.sqlConnectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO SharedPictures (ownerid, guestid, pictureid) "
                                + "VALUES(@oid, @gid, @pid)";

            command.Parameters.AddWithValue("@oid", ownerid);
            command.Parameters.AddWithValue("@gid", guestid);
            command.Parameters.Add("@pid", SqlDbType.UniqueIdentifier).Value = new Guid(pid);

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }
    }
}
