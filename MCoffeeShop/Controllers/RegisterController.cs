using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCoffeeShop.Models;   

namespace MCoffeeShop.Controllers
{
    public class RegisterController : Controller
    {
        MCoffeeShopDBDataContext _context = new MCoffeeShopDBDataContext();
        // GET: Register
        public ActionResult Index()
        {
            TempData["Message"] = "Hello";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(RegisterModel model)
        {
            var result = new AccountModel().IsNameExitst(model.UserName);
            if (ModelState.IsValid)
            {
                if (!result)
                {
                    Account account = new Account
                    {
                        UserName = model.UserName,
                        Password = model.Password
                    };
                    _context.Accounts.InsertOnSubmit(account);
                    _context.SubmitChanges();
                    TempData["Message"] = "Register success";
                }
                else
                {
                    ModelState.AddModelError("", "Your name was availible!");
                }
            }
            return View(model);
        }
    }
}