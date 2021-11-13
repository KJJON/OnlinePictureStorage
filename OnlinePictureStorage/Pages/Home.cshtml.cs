using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlinePictureStorage.Pages.Shared
{
    [Authorize]

    public class HomeModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
