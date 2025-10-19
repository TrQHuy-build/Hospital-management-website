using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Clinic_management.Pages.Login
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }

        public void OnPost(string username, string password)
        {
            if (username != "admin" || password != "123")
            {
                ViewData["Error"] = "Sai tài khoản hoặc mật khẩu!";
            }
            else
            {
                Response.Redirect("/Admin");
            }
        }
    }
}
