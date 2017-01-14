using AzureKit.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AzureKit.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly IUserProfileRepository _repo;

        public UserProfileController(IUserProfileRepository repo)
        {
            _repo = repo;
        }

        // GET: UserProfile
        public async Task<ActionResult> Index()
        {
            var cp = User as ClaimsPrincipal;
            var email = cp.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
                ?? cp.FindFirst("emails");
            if (string.IsNullOrWhiteSpace(email?.Value))
            {
                return HttpNotFound();
            }
            UserNotificationChoices settings = await _repo.GetUserNotificationChoices(email.Value);
            if (settings == null)
            {
                return View("SqlUnavailable");
            }
            return View("Profile", settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(UserNotificationChoices model)
        {
            var cp = User as ClaimsPrincipal;
            var email = cp.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
                ?? cp.FindFirst("emails");
            if (string.IsNullOrWhiteSpace(email?.Value))
            {
                return HttpNotFound();
            }
            await _repo.SetEmailNotifications(email.Value, model.NotificationEmailsEnabled);
            return RedirectToAction("Index");
        }
    }
}