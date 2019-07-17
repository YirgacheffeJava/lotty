using Activity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Activity
{
    /// <summary>
    /// SiteServer 的摘要说明
    /// </summary>
    public class SiteServer : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string result = "";//定义响应结果全局变量
            //接收客户端所指定的调用参数
            string param = context.Request["param"];
            //如果参数为check_user,表示查询该手机号码今日是否可以抽奖
            if (param == "check_user")
            {
                //设置默认值为false，表示今日不能继续参加抽奖活动
                result = "false";
                //获取手机号码
                string phone = context.Request.Form["phone"];
                //判断手机号码是否为空
                if (phone != "")
                {
                    //实例化数据库上下文类
                    using (activityDataContext DataContext = new activityDataContext())
                    {
                        //查询activity_record表，通过指定的手机号码以及当前系统日期进行查询
                        if (!DataContext.activity_record.Where(W => W.phone == phone && W.add_date.Value.Date.ToString() == DateTime.Now.Date.ToString("yyyy-MM-dd")).Any())
                        {
                            //如果数据不存在，表示该手机号码可以参加抽奖活动
                            result = "true";
                        }
                    }
                }
            }
            //如果参数为get_prize,表示获取抽奖数据信息
            else if (param == "get_prize")
            {
                //定义抽奖数据集合
                IList<Prize> prize = new List<Prize>();
                //向集合添加抽奖数据
                prize.Add(new Prize() { id = 1, prize_name = "奶茶下午茶", percente = 60 });
                prize.Add(new Prize() { id = 2, prize_name = "恭喜老板陈超,充值20", percente = 10 });
                prize.Add(new Prize() { id = 3, prize_name = "恭喜老板杨阳,充值20", percente = 10 });
                prize.Add(new Prize() { id = 4, prize_name = "恭喜老板杭宇,充值20", percente = 1 });
                prize.Add(new Prize() { id = 5, prize_name = "恭喜老板朱云,充值20", percente = 30 });
                prize.Add(new Prize() { id = 6, prize_name = "恭喜穆姐姐,充值20", percente = 1 });
                prize.Add(new Prize() { id = 7, prize_name = "Java程序设计慕课版", percente = 1 });
                prize.Add(new Prize() { id = 8, prize_name = "奶茶下午茶", percente = 1 });
                prize.Add(new Prize() { id = 9, prize_name = "C++奶茶下午茶", percente = 1 });
                prize.Add(new Prize() { id = 10, prize_name = "奶茶下午茶", percente = 40 });
                prize.Add(new Prize() { id = 11, prize_name = "奶茶下午茶", percente = 1 });
                prize.Add(new Prize() { id = 12, prize_name = "ASP.奶茶下午茶", percente = 70 });
                //根据概率值，随机指定一个中奖数据
                Prize SelectedPrize = RandomSelect(prize);
                //将中奖信息返回给客户端
                result = "{\"prize_name\":\"" + SelectedPrize.prize_name + "\",\"prize_site\":\"" + (SelectedPrize.id - 1) + "\",\"prize_id\":\"" + SelectedPrize.id + "\"}";
            }
            //如果参数为NoPrize,表示用户未中奖，则将电话号码保存到数据库
            else if (param == "NoPrize")
            {
                //获取手机号码
                string phone = context.Request.Form["phone"];
                //判断手机号码是否为空
                if (phone != "")
                {
                    //实例化数据库上下文类
                    using (activityDataContext DataContext = new activityDataContext())
                    {
                        //构造activity_record类并赋值，只保存手机号码以及当前系统日期，其他数据设置为默认值
                        activity_record add = new activity_record();
                        add.phone = phone;
                        add.prize_id = 0;
                        add.prize_name = "";
                        add.username = "";
                        add.address = "";
                        add.add_date = DateTime.Now;
                        //将数据实体添加到数据库插入状态中
                        DataContext.activity_record.InsertOnSubmit(add);
                        //提交更改
                        DataContext.SubmitChanges();
                    }
                }
            }
            //如果参数为saveInfo,表示用户已中奖
            else if (param == "saveInfo")
            {
                //设置默认值为0，表示保存失败
                result = "0";                
                string prize_id = context.Request.Form["prize_id"];//获取中奖id
                string prize_name = context.Request.Form["prize_name"];//获取中奖奖品
                string username = context.Request.Form["username"];//获取用户名
                string phone = context.Request.Form["phone"];//获取手机号
                string address = context.Request.Form["address"];//获取地址
                //判断手机号码是否为空
                if (phone != "")
                {
                    //实例化数据库上下文类
                    using (activityDataContext DataContext = new activityDataContext())
                    {
                        //构造activity_record类并赋值全部属性
                        activity_record add = new activity_record();
                        add.phone = phone;
                        add.prize_id = Convert.ToInt32(prize_id);
                        add.prize_name = prize_name;
                        add.username = username;
                        add.address = address;
                        add.add_date = DateTime.Now;
                        //将数据实体添加到数据库插入状态中
                        DataContext.activity_record.InsertOnSubmit(add);
                        //提交更改
                        DataContext.SubmitChanges();
                        //设置返回值为1，表示保存成功
                        result = "1";
                    }
                }
            }
            //将数据结果响应给客户端
            context.Response.Write(result);
        }
        //根据各奖品的概率值，随机指定一个中奖数据
        public Prize RandomSelect(IList<Prize> prize)
        {
            //定义中奖数据实体
            Prize SelectedPrize = null;
            //将中奖产品集合按概率倒排序
            IList<Prize> OrderByDesc = prize.OrderByDescending(O => O.percente).ToList();
            //遍历每一个奖品
            foreach (Prize Ele in OrderByDesc)
            {
                //随机生成一个1到100的数值
                int RandValue = new Random(Guid.NewGuid().GetHashCode()).Next(1, 101);
                //判断随机数小于等于产品概率值
                if (RandValue <= Ele.percente)
                {
                    //指定当前产品为中奖奖品
                    SelectedPrize = Ele;
                    //跳出循环
                    break;
                }
            }
            //返回中奖数据实体
            return SelectedPrize;
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}