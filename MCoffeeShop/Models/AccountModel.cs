using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace MCoffeeShop.Models
{
    public class AccountModel
    {
        private MCoffeeShopDBDataContext _context = null;

        public AccountModel()
        {
            _context = new MCoffeeShopDBDataContext();
        }

        public bool Login(string UserName, string Password)
        {
            bool? res = false;
            _context.sp_Account_Login_Check(UserName, Password, ref res);

            return (res ?? false);
        }

        public bool IsNameExitst(string UserName)
        {
            bool? res = false;
            _context.sp_Is_Name_Exitst(UserName, ref res);

            return (res ?? false);
        }
    }
}