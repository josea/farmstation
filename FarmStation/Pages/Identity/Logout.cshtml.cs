using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FarmStation.Pages.Identity;

public class LogoutModel : PageModel
{
	public async Task<IActionResult> OnPostAsync()
	{
		// Clear the existing external cookie
		try
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		}
		catch (Exception ex)
		{
			string error = ex.Message;
		}
		return LocalRedirect("/");
	}
}
