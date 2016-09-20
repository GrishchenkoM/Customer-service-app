using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        //public async Task<ActionResult> Index()
        //{
        //    var list = await GetUsers();

        //    ViewBag.UserList = list;

        //    return View();
        //}

        //public async Task<IList<ApplicationUser>> GetUsers()
        //{
        //    return await _context.Users.ToListAsync();
        //}

        [Authorize(Roles = "admin, manager, user")]
        public ActionResult Index()
        {
            IList<string> roles = new List<string> {"Роль не определена"};
            var userManager = HttpContext.GetOwinContext()
                .GetUserManager<ApplicationUserManager>();
            var user = userManager.FindByEmail(User.Identity.Name);

            if (user != null)
                roles = userManager.GetRoles(user.Id);

            ViewBag.Roles = roles;
            ViewBag.UserList = _context.Users.ToList();
            
            return View();
        }

        #region Other Pages
        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        #endregion

        private readonly ApplicationDbContext _context;
    }
}