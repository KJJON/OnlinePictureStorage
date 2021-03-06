using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using OnlinePictureStorage.Constants;

namespace OnlinePictureStorage.Pages
{
    [Authorize]

    public class SharedWithMeModel : PageModel
    {
        public List<string> paths = new List<string>();
        public List<string> pids = new List<string>();

        private readonly UserManager<IdentityUser> userManager;

        public SharedWithMeModel(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public void OnGet()
        {
            string userid = userManager.GetUserId(HttpContext.User);

            List<string> pathlist = new List<string>();
            List<string> pictureids = new List<string>();

            using SqlConnection connection = new SqlConnection(Connections.sqlConnectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT SharedPictures.pictureid, Pictures.path "
                                + "FROM SharedPictures "
                                + "INNER JOIN Pictures ON SharedPictures.pictureid = Pictures.pictureid "
                                + "WHERE SharedPictures.guestid = @uid";

            command.Parameters.AddWithValue("@uid", userid);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    pathlist.Add(Connections.blobLink + String.Format("{0}", reader[1]));
                    pictureids.Add(String.Format("{0}", reader[0]));
                }
            }
            finally
            {
                reader.Close();
                paths = pathlist;
                pids = pictureids;
            }

            connection.Close();

        }
    }
}
