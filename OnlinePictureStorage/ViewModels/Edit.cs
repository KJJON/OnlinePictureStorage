using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePictureStorage.ViewModels
{
    public class Edit
    {
        [Required]
        [DataType(DataType.Text)]
        public string City { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Photographer { get; set; }
    }
}
