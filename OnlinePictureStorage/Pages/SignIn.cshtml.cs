using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlinePictureStorage.ViewModels;

namespace OnlinePictureStorage.Pages
{
    public class SignInModel : PageModel
    {

        private readonly SignInManager<IdentityUser> signInManager;

        [BindProperty]
        public Login LModel { get; set; }

        public SignInModel(SignInManager<IdentityUser> signInManager)
        {
            this.signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string returnURL = null)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, false);

                if (result.Succeeded)
                {
                    if (returnURL == null || returnURL == "/")
                    {
                        return RedirectToPage("/Home");
                    }
                    else
                    {
                        return RedirectToPage(returnURL);
                    }
                }

                ModelState.AddModelError("", "Username or Password incorrect");
            }

            return Page();
        }
    }
}
