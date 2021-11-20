using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlinePictureStorage.ViewModels;
using System.IO;
using Azure.Storage.Blobs.Models;
using OnlinePictureStorage.Constants;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Identity;

namespace OnlinePictureStorage.Pages
{
    [Authorize]

    public class UploadModel : PageModel
    {
        
        [BindProperty]
        public Upload UModel { get; set; }
        
        private readonly UserManager<IdentityUser> userManager;

        public UploadModel(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {

            List<string> exten = new List<string>();
            exten.Add(".bmp");
            exten.Add(".ico");
            exten.Add(".jpeg");
            exten.Add(".jpg");
            exten.Add(".gif");
            exten.Add(".tiff");
            exten.Add(".png");


            string guid = $@"{Guid.NewGuid()}";
            string path = guid + Path.GetExtension(UModel.File.FileName);

            if (!exten.Contains(Path.GetExtension(UModel.File.FileName)))
                return RedirectToPage("/Home");

            BlobServiceClient blobServiceClient = new BlobServiceClient(Connections.blobConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(Connections.blobContainer);
            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(path);

            BlobHttpHeaders httpHeaders = new BlobHttpHeaders()
            {
                ContentType = UModel.File.ContentType
            };

            await blobClient.UploadAsync(UModel.File.OpenReadStream(), httpHeaders);

            InseertIntoDatabase(guid, path, UModel.Photographer, UModel.City, UModel.Date);

            return RedirectToPage("/Home");
        }

        public void InseertIntoDatabase(string guid, string path, string photographer, string city, DateTime capturedate)
        {

            using (SqlConnection connection = new SqlConnection(Connections.sqlConnectionString))
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Pictures (pictureid, path, photographer, city, capturedate) " 
                                        + "VALUES(@id, @path, @photo, @city, @date)";
       
                    command.Parameters.Add("@id", SqlDbType.UniqueIdentifier).Value = new Guid(guid);
                    command.Parameters.AddWithValue("@path", path);
                    command.Parameters.AddWithValue("@photo", photographer);
                    command.Parameters.AddWithValue("@city", city);
                    command.Parameters.Add("@date", SqlDbType.DateTime).Value = capturedate;

                    connection.Open();

                    command.ExecuteNonQuery();

                    connection.Close();
                }

            using (SqlConnection connection = new SqlConnection(Connections.sqlConnectionString))
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO UserPictures (userid, pictureid) "
                                        + "VALUES(@uid, @pid)";

                    command.Parameters.AddWithValue("@uid", userManager.GetUserId(HttpContext.User));
                    command.Parameters.Add("@pid", SqlDbType.UniqueIdentifier).Value = new Guid(guid);
                    
                    connection.Open();

                    command.ExecuteNonQuery();

                    connection.Close();
                }
        }
    }
}
