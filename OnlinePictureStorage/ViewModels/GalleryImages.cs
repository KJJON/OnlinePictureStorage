using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinePictureStorage.ViewModels
{
    public class GalleryImages
    {
        [DataType(DataType.MultilineText)]
        public List<string> Path { get; set; }
    }
}
