using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Activity.Models
{
    //中奖奖品实体类
    public class Prize
    {
        public int id { get; set; }
        public string prize_name { get; set; }
        public int percente { get; set; }
    }
    //中奖名单实体类
    public class PrizeList
    {
        public int id { get; set; }
        public string prize_name { get; set; }
        public string username { get; set; }
        public string phone { get; set; }
        public DateTime add_date { get; set; }
    }
}