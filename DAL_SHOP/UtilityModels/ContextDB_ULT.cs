using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DAL_SHOP.UtilityModels
{
    public static class ContextDB_ULT
    {
        public static string connectionString = ConfigurationManager.ConnectionStrings["ContextDB"].ConnectionString;
    }
}
