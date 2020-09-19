using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCoffeeShop.Models
{
    public class CartModel
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public double Price { get; set; }

        public string ImagePath { get; set; }

        public string Type { get; set; }

        public int Quantity { get; set; }

        public double Total { get; set; }
    }
}