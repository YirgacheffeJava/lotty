using Activity.Models;
using AspPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Activity.Views
{
    public partial class Index : System.Web.UI.Page
    {
        //定义分页类
        Paging paging = null;
        //定义中奖名单集合
        public IList<PrizeList> prizeList = null;
        //定义分页信息，包括每页总数、显示页码数、当前页码、数据总数
        int PageSize = 10, PageNumber = 5, PageIndex = 1, TotalCount = 0;
        //页面初始化的处理方法
        protected void Page_Init(object sender, EventArgs e)
        {
            //实例化分页类
            paging = new Paging(this.PageChange, PageChangeClick);
            //初始化分页类
            paging.Init(PageSize, PageNumber);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //调用获取数据方法
                GetData();
            }
        }
        //获取并绑定中奖名单
        protected void GetData()
        {
            //实例化数据库上下文类
            using (activityDataContext DataContext = new activityDataContext())
            {
                //计算分页数据
                int Skin = (PageIndex - 1) * PageSize;
                //获取数据总数
                TotalCount = DataContext.activity_record.Where(W => W.username != "").Count();
                //获取中奖名单
                prizeList = DataContext.activity_record
                    .Where(W => W.username != "")
                    .Select(S => new PrizeList { id = S.id, username = S.username, prize_name = S.prize_name, phone = S.phone, add_date = S.add_date ?? DateTime.Now.Date }).OrderByDescending(O => O.id)
                    .Skip(Skin).Take(PageSize).ToList();
                //绑定分页控件
                paging.Bind(PageIndex, TotalCount);
            }
        }
        //分页控件的处理方法
        protected void PageChangeClick(object send, int NewIndex)
        {
            //赋值当前页码
            PageIndex = NewIndex;
            //调用获取数据方法，重新绑定页面数据
            GetData();
        }
    }
}