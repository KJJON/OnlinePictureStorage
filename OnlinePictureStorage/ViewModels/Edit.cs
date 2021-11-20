using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePictureStorage.ViewModels
{
    public class Edit
    {
        [DataType(DataType.Text)]
        public string City { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DataType(DataType.Text)]
        public string Photographer { get; set; }
    }
}
