using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCoffeeShop.Models;

namespace MCoffeeShop.Controllers
{
    public class CategoryController : Controller
    {
        MCoffeeShopDBDataContext _context = new MCoffeeShopDBDataContext();
        private List<CartModel> ListCart = new List<CartModel>();


        // GET: Category
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if(file != null)
            {
                string rootPath = Server.MapPath("~/");
                string fileName = System.IO.Path.GetFileName(file.FileName);
                string destFile = System.IO.Path.Combine(rootPath, "images\\" + fileName);
                file.SaveAs(destFile);
            }
            return View();
        }
        public ActionResult ListProducts()
        {
            var products = _context.Products.ToList();
            return Json(products, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Create([Bind(Exclude ="ProductID")] Product product)    
        {
            if (ModelState.IsValid)
            {
                string rootPath = Server.MapPath("~/");
                string fileName = System.IO.Path.GetFileName(product.ImagePath);
                product.ImagePath = fileName;
                product.DateAdd = DateTime.Now.ToString();
                product.Status = "AVAILABLE";
                _context.Products.InsertOnSubmit(product);
                _context.SubmitChanges();
            }
            return Json(product, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Update(Product product)
        {
            if (ModelState.IsValid)
            {
                string rootPath = Server.MapPath("~/");
                string fileName = System.IO.Path.GetFileName(product.ImagePath);
                product.ImagePath = fileName;
                var pr = _context.Products.ToList().Find(m => m.ProductID == product.ProductID);
                pr.ImagePath = product.ImagePath;
                pr.ProductName = product.ProductName;
                pr.Status = product.Status;
                pr.Price = product.Price;
                _context.SubmitChanges();
            }
            return Json(product, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Get(int id)
        {
            var product = _context.Products.ToList().Find(m => m.ProductID == id);
            string rootPath = Server.MapPath("~/");
            string fileName = System.IO.Path.GetFileName(product.ImagePath);
            string destFile = System.IO.Path.Combine(rootPath, "images\\" + fileName);
            product.ImagePath = destFile;

            return Json(product, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var product = _context.Products.ToList().Find(m => m.ProductID == id);
            if(product != null)
            {
                _context.Products.DeleteOnSubmit(product);
                _context.SubmitChanges();
            }
            return Json(product, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Cart()
        {
            ListCart = Session["CartItem"] as List<CartModel>;
            if (ListCart == null)
            {
                return RedirectToAction("Index", "Category");
            }
            return View(ListCart);
        }

        [HttpPost]
        public ActionResult AddToCart(int id)
        {
            CartModel cart = new CartModel();
            var product = _context.Products.ToList().Find(m => m.ProductID == id);
            if (product.Status == "AVAILABLE")
            {
                if (Session["CartCounter"] != null)
                {
                    ListCart = Session["CartItem"] as List<CartModel>;
                }
                if (ListCart.Any(model => model.ProductID == id))
                {
                    cart = ListCart.Single(model => model.ProductID == id);
                    cart.Quantity = cart.Quantity + 1;
                    cart.Total = cart.Quantity * cart.Price;
                }
                else
                {
                    cart.ProductID = product.ProductID;
                    cart.ProductName = product.ProductName;
                    cart.Price = Convert.ToDouble(product.Price);
                    cart.Type = product.Type;
                    cart.ImagePath = product.ImagePath;
                    cart.Quantity = 1;
                    cart.Total = Convert.ToDouble(product.Price);
                    ListCart.Add(cart);
                }

                Session["CartCounter"] = ListCart.Count;
                Session["CartItem"] = ListCart;
            }    
            else
            {
                return Json(new { Success = false, Counter = 0 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new {Success = true ,Counter = ListCart.Count }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubToCart(int id)
        {
            if (Session["CartCounter"] != null)
            {
                ListCart = Session["CartItem"] as List<CartModel>;
            }
            if (ListCart.Count > 1)
            {
                var cart = ListCart.Single(model => model.ProductID == id);
                if (cart.Quantity > 1)
                {
                    cart.Quantity = cart.Quantity - 1;
                    cart.Total = cart.Quantity * cart.Price;
                }
                else
                {
                    ListCart.Remove(cart);
                }
                Session["CartCounter"] = ListCart.Count;
                Session["CartItem"] = ListCart;
            }
            else if (ListCart.Count == 1)
            {
                var cart = ListCart.Single(model => model.ProductID == id);
                if (cart.Quantity > 1)
                {
                    cart.Quantity = cart.Quantity - 1;
                    cart.Total = cart.Quantity * cart.Price;
                    Session["CartCounter"] = ListCart.Count;
                    Session["CartItem"] = ListCart;
                }
                else
                {
                    ListCart.Remove(cart);
                    Session["CartCounter"] = null;
                    Session["CartItem"] = null;
                }
            }
            return Json(new { Success = true, Counter = ListCart.Count }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddOrder()
        {
            int OrderId = 0;
            ListCart = Session["CartItem"] as List<CartModel>;
            Order orderObj = new Order()
            {
                OrderDate = DateTime.Now,
                OrderNumber = string.Format("{0:ddmmyyyyHHmmsss}",DateTime.Now)
            };
            _context.Orders.InsertOnSubmit(orderObj);
            _context.SubmitChanges();
            OrderId = orderObj.OrderId;

            foreach(var item in ListCart)
            {
                OrderDetail objOrderDetail = new OrderDetail();
                objOrderDetail.ProductId = item.ProductID;
                objOrderDetail.ProductName = item.ProductName;
                objOrderDetail.Type = item.Type;
                objOrderDetail.Price = item.Price;
                objOrderDetail.Quantity = item.Quantity;
                objOrderDetail.Total = item.Total;
                objOrderDetail.OrderId = orderObj.OrderId;
                _context.OrderDetails.InsertOnSubmit(objOrderDetail);
                _context.SubmitChanges();
            }

            Session["CartCounter"] = null;
            Session["CartItem"] = null;
            
            return RedirectToAction("Index");
        }
    }
}