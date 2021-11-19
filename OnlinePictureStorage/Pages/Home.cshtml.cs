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

namespace OnlinePictureStorage.Pages.Shared
{
    [Authorize]   

    public class HomeModel : PageModel
    {
        public List<string> paths = new List<string>();

        private readonly UserManager<IdentityUser> userManager;

        public HomeModel(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }
        
        public void OnGet()
        {
            string userid = userManager.GetUserId(HttpContext.User);

            List<string> pathlist = new List<string>();

            using SqlConnection connection = new SqlConnection(Connections.sqlConnectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT UserPictures.pictureid, Pictures.path "
                                + "FROM UserPictures "
                                + "INNER JOIN Pictures ON UserPictures.pictureid = Pictures.pictureid "
                                + "WHERE UserPictures.userid = @uid";

            command.Parameters.AddWithValue("@uid", userid);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            try 
            {
                while(reader.Read())
                {
                    pathlist.Add(Connections.blobLink +  String.Format("{0}",reader[1]));
                }
            }
            finally
            {
                reader.Close();
                paths = pathlist;
            }

            connection.Close();
        }
    }
}
