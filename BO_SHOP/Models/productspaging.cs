using DAL_SHOP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BO_SHOP.Models
{
    public class productspaging
    {
        public List<ProductRow> ds_p = new List<ProductRow>();
        public int total;
        public int? pageCurrent;
        public int? view;
        public int? deletesuccess;
        public int? trending;
    }
}