using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
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

    public class FileModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string pid { get; set; }

        [BindProperty]
        public Edit EModel { get; set; }
        
        [BindProperty]
        public Guest GModel { get; set; }

        public string link { get; set; }
        public string fname { get; set; }

        public char oorg { get; set;  }

        public string mphotographer { get; set; }
        public string mcity { get; set; }
        public string mcapturedate { get; set; }
        public List<string> mguests { get; set; }

        private readonly UserManager<IdentityUser> userManager;

        public FileModel(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public IActionResult OnGet()
        {
            string userid = userManager.GetUserId(HttpContext.User);

            if (Ownership(pid, userid))
            {
                oorg = 'o';
            }
            else if(IsGuest(pid, userid))
            {
                oorg = 'g';
            }
            else
            {
                return RedirectToPage("/Privacy");
            }

            fname =  GetLink(pid);
            link = Connections.blobLink + fname;
            GetOriginal(pid);
            GetGuests(pid);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            UpdateDatabase(pid, EModel.Photographer, EModel.City, EModel.Date);

            return RedirectToPage("/Home");
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(Connections.blobConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(Connections.blobContainer);
            BlobClient blobClient = containerClient.GetBlobClient(GetLink(pid));
            await blobClient.DeleteIfExistsAsync();

            DeleteData(pid);

            return RedirectToPage("/Home");
        }

        public  IActionResult OnPostDeclineShare()
        {
            string userid = userManager.GetUserId(HttpContext.User);

            DeleteShare(pid, userid);

            return RedirectToPage("/SharedWithMe");
        }

        public IActionResult OnPostUnshare()
        {
            string guestid = GetGuestId(GModel.Email);

            DeleteShare(pid, guestid);

            return RedirectToPage("/Home");
        }

        public bool Ownership(string pid, string userid)
        {
            string gotuserid = "";

            using SqlConnection connection = new SqlConnection(Connections.sqlConnectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT userid "
                                + "FROM UserPictures "
                                + "WHERE pictureid = @pid";

            command.Parameters.AddWithValue("@pid", pid);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    gotuserid = String.Format("{0}", reader[0]);
                }
            }
            finally
            {
                reader.Close();
            }

            return (userid == gotuserid);
        }

        public bool IsGuest(string pictureid, string guestid)
        {
            string gotguestid = "";

            using SqlConnection connection = new SqlConnection(Connections.sqlConnectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT guestid "
                                + "FROM SharedPictures "
                                + "WHERE pictureid = @pid";

            command.Parameters.AddWithValue("@pid", pid);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    gotguestid = String.Format("{0}", reader[0]);
                }
            }
            finally
            {
                reader.Close();
            }

            return (guestid == gotguestid);
        }

        public string GetLink(string pictureid)
        {
            string gotlink ="";

            using SqlConnection connection = new SqlConnection(Connections.sqlConnectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT path "
                                + "FROM Pictures "
                                + "WHERE pictureid = @pid";

            command.Parameters.AddWithValue("@pid", pictureid);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    gotlink = String.Format("{0}", reader[0]);
                }
            }
            finally
            {
                reader.Close();
            }

            return gotlink;
        }

        public void UpdateDatabase(string pid, string photographer, string city, DateTime capturedate)
        {
            using (SqlConnection connection = new SqlConnection(Connections.sqlConnectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Pictures "
                                    + "SET photographer = @photo, city = @city, capturedate = @date "
                                    + "WHERE pictureid = @pid";

                command.Parameters.AddWithValue("@photo", photographer);
                command.Parameters.AddWithValue("@city", city);
                command.Parameters.Add("@date", SqlDbType.DateTime).Value = capturedate;
                command.Parameters.AddWithValue("@pid", pid);

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public void GetOriginal(string pid)
        {

            using SqlConnection connection = new SqlConnection(Connections.sqlConnectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT photographer, city, capturedate "
                                + "FROM Pictures "
                                + "WHERE pictureid = @pid";

            command.Parameters.AddWithValue("@pid", pid);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    mphotographer = String.Format("{0}", reader[0]);
                    mcity = String.Format("{0}", reader[1]);
                    mcapturedate = (Convert.ToDateTime(reader[2])).ToString("yyyy-MM-dd");
                }
            }
            finally
            {
                reader.Close();
            }

        }

        public void GetGuests(string pictureid)
        {
            List<string> guests = new List<string>();

            using SqlConnection connection = new SqlConnection(Connections.sqlConnectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT guestid "
                                + "FROM SharedPictures "
                                + "WHERE pictureid = @pid";

            command.Parameters.AddWithValue("@pid", pid);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    guests.Add(String.Format("{0}", reader[0]));
                }
            }
            finally
            {
                reader.Close();
            }

            mguests = guests;
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

        public void DeleteData(string pid)
        {
            using (SqlConnection connection = new SqlConnection(Connections.sqlConnectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "DELETE "
                                    + "FROM UserPictures "
                                    + "WHERE pictureid = @pid";

                command.Parameters.AddWithValue("@pid", pid);

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }

            using (SqlConnection connection = new SqlConnection(Connections.sqlConnectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "DELETE "
                                    + "FROM Pictures "
                                    + "WHERE pictureid = @pid";

                command.Parameters.AddWithValue("@pid", pid);

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        public void DeleteShare(string pid, string uid)
        {
            using (SqlConnection connection = new SqlConnection(Connections.sqlConnectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "DELETE "
                                    + "FROM SharedPictures "
                                    + "WHERE pictureid = @pid AND guestid = @uid";

                command.Parameters.AddWithValue("@pid", pid);
                command.Parameters.AddWithValue("@uid", uid);

                connection.Open();

                command.ExecuteNonQuery();

                connection.Close();
            }
        }
    }
}
