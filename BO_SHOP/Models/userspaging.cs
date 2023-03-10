using DAL_SHOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BO_SHOP
{
    public class userspaging
    {
        public List<User> ds_u =new List<User>();
        public int total;
        public int? pageCurrent;
        public int? role;
        public int? isDeleted;
        public int? deletesuccess;
    }
}