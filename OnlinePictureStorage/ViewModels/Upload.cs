using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePictureStorage.ViewModels
{
    public class Upload
    {
        [DataType(DataType.Text)]
        public string City { get; set; }

        [DataType(DataType.Date)]
        public string Date { get; set; }

        [DataType(DataType.Text)]
        public string Photographer { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile File { get; set; }
    }
}
