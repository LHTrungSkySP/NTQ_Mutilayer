using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_SHOP.UtilityModels
{
    [Serializable]
    public class UserSession
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int role { get; set; }
    }
}
