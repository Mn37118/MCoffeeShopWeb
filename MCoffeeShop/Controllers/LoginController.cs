using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCoffeeShop.Models;

namespace MCoffeeShop.Controllers
{
    public class LoginController : Controller
    {
        MCoffeeShopDBDataContext _context = new MCoffeeShopDBDataContext();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model)
        {
            var result = new AccountModel().Login(model.UserName, model.Password);
            if (result && ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "UserName or Password is incorrect.");
            }
            return View(model);
        }
    }
}