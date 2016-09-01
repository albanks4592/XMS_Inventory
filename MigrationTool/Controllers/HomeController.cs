using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MigrationTool.Models;
using MigrationTool.ViewModels;

namespace MigrationTool.Controllers
{
    [Authorize(Roles = MigrationTool.Roles.View + ", " + MigrationTool.Roles.Edit + ", " + MigrationTool.Roles.Admin)]
    public class HomeController : Controller
    {
        private MigrationToolEntities db = new MigrationToolEntities();

        public ActionResult Index()
        {
            return View(new HomeViewModel(db));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
