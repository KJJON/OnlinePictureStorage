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

namespace OnlinePictureStorage.Pages
{
    [Authorize]

    public class UploadModel : PageModel
    {
        
        [BindProperty]
        public Upload UModel { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=sqlvayljnkyyyv2fm2;AccountKey=kTfCbLOY+eT2l4l5DdL3c8QaXvq9t58gRzVOLGRmn3h0fNwzK5HIB76mGP414VH9SrdWBRhUucPuD7YbCNpY5Q==;EndpointSuffix=core.windows.net";
            string container = "ops-picturestorage";
            string guid = $@"{Guid.NewGuid()}" + Path.GetExtension(UModel.File.FileName);

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);
            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(guid);

            BlobHttpHeaders httpHeaders = new BlobHttpHeaders()
            {
                ContentType = UModel.File.ContentType
            };

            await blobClient.UploadAsync(UModel.File.OpenReadStream(), httpHeaders);

            return Page();
        }
    }
}
