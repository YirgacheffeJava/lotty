<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Activity.Views.Index" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>获奖名单</title>
    <link href="../Public/bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../Public/css/style.css" rel="stylesheet" />
</head>
<body style="color: white">
    <form id="form1" runat="server">
        <div id="list" style="width:574px;height:584px;margin:20px auto 0;padding:50px 55px;background-color: #ea0049">
            <div class="prize-title">
                获奖名单
            </div>
            <table class="table">
                <thead class="prize-list-thead">
                <tr>
                    <th>姓名</th>
                    <th>手机号</th>
                    <th>奖品</th>
                    <th>参与日期</th>
                </tr>
                </thead>
                <tbody>
                <!--判断中奖名单集合是否为空-->
                <%if (prizeList != null && prizeList.Count > 0)
                {
                    //遍历中奖名单集合并绑定数据
                    foreach(Activity.Models.PrizeList list in prizeList){    %>          
                        <tr>
                            <td><%=list.username %></td>
                            <td><%=list.phone %></td>
                            <td><%=list.prize_name %></td>
                            <td><%=list.add_date.ToString("yyyy-MM-dd") %></td>
                        </tr>                
                <%  }
                }%>
                <!--循环遍历数据结束-->
                </tbody>
            </table>
            <!--分页开始-->
            <div class="page" id="PageChange" runat="server"></div>
            <!--分页结束-->
        </div>
    </form>
</body>
</html>
