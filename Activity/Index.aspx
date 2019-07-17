<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Activity.Index" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>九宫格大转盘抽奖</title>
    <link rel="stylesheet" type="text/css" href="Public/css/style.css">
    <script type="text/javascript" src="Public/js/jquery.min.js"></script>
    <script type="text/javascript" src="Public/layer/layer.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <!--九宫格图片区-->
        <div id="lottery">
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td class="lottery-unit lottery-unit-0"><img src="Public/images/1.png"></td>
                    <td class="lottery-unit lottery-unit-1"><img src="Public/images/2.png"></td>
                    <td class="lottery-unit lottery-unit-2"><img src="Public/images/thanks.png"></td>
                    <td class="lottery-unit lottery-unit-3"><img src="Public/images/3.png"></td>
                </tr>
                <tr>
                    <td class="lottery-unit lottery-unit-11"><img src="Public/images/4.png"></td>
                    <td colspan="2" rowspan="2"><a href="#"></a></td>
                    <td class="lottery-unit lottery-unit-4"><img  src="Public/images/5.png"></td>
                </tr>
                <tr>
                    <td class="lottery-unit lottery-unit-10"><img src="Public/images/6.png"></td>
                    <td class="lottery-unit lottery-unit-5"><img src="Public/images/7.png"></td>
                </tr>
                <tr>
                    <td class="lottery-unit lottery-unit-9"><img src="Public/images/thanks.png"></td>
                    <td class="lottery-unit lottery-unit-8"><img src="Public/images/8.png"></td>
                    <td class="lottery-unit lottery-unit-7"><img src="Public/images/9.png"></td>
                    <td class="lottery-unit lottery-unit-6"><img src="Public/images/10.png"></td>
                </tr>
            </table>
        </div>
    </form>


    <script type="text/javascript">
        var click = false;  // 初始化click
        // 定义lottery对象
        var lottery = {
            index: 0, //当前转动到哪个位置，起点位置
            count: 0, //总共有多少个位置
            timer: 0, //setTimeout的ID，用clearTimeout清除
            speed: 50, //初始转动速度
            times: 0, //转动次数
            cycle: 50, //转动基本次数：即至少需要转动多少次再进入抽奖环节
            prize: 0, //中奖位置
            init: function (id) {    // 初始化数据
                if ($("#" + id).find(".lottery-unit").length > 0) {
                    $lottery = $("#" + id);
                    $units = $lottery.find(".lottery-unit");
                    this.obj = $lottery;
                    this.count = $units.length;
                    $lottery.find(".lottery-unit-" + this.index).addClass("active");
                }
            },
            roll: function () {     // 九格宫转动效果
                var index = this.index;
                var count = this.count;
                var lottery = this.obj;
                $(lottery).find(".lottery-unit-" + index).removeClass("active");
                index += 1;
                if (index > count - 1) {
                    index = 0;
                }
                $(lottery).find(".lottery-unit-" + index).addClass("active");
                this.index = index;
                return false;
            },
        };
        lottery.init('lottery');    // lottery对象调用初始化方法
        // 单击“开始抽奖”按钮，触发事件
        $("#lottery a").click(function () {
            if (click) {
                return false;
            } else {
                layer.prompt({ title: '请输入手机号', formType: 0 }, function (phone, index) {
                    layer.close(index);         // 关闭弹层
                    /** 判断抽奖条件 **/
                    if (!check_user(phone)) {   // 不满足条件，不能抽奖
                        layer.msg('您今天已经参与过，明日再来', { time: 1000 });
                        return false;
                    } else {
                        getPrizeInfo(phone);
                    }
                });

                return false;
            }
        });
        // 开始抽奖
        function start() {
            lottery.times += 1;
            lottery.roll();     // 调用转动方法
            var prize_site = $("#lottery").attr("prize_site");
            // 转动结束
            if (lottery.times > (lottery.cycle + 10) && lottery.index == prize_site) {
                var prize_id = $("#lottery").attr("prize_id");
                var prize_name = $("#lottery").attr("prize_name");
                clearTimeout(lottery.timer);
                lottery.prize = -1;
                lottery.times = 0;
                click = false;
                show_prize(prize_id, prize_name);
                return false;
            } else {    // 控制转动速度
                if (lottery.times < lottery.cycle) {
                    lottery.speed -= 10;
                } else if (lottery.times == lottery.cycle) {
                    var index = Math.random() * (lottery.count) | 0;
                    lottery.prize = index;
                } else {
                    if (lottery.times > (lottery.cycle + 10) && ((lottery.prize == 0 && lottery.index == 7) || lottery.prize == lottery.index + 1)) {
                        lottery.speed += 110;
                    } else {
                        lottery.speed += 20;
                    }
                }
                // 设置最小转动速度
                if (lottery.speed < 40) {
                    lottery.speed = 40;
                }
                lottery.timer = setTimeout(start, lottery.speed); // 在指定的毫秒数后调用start()函数
            }
            return false;
        }

        //获取奖品信息
        function getPrizeInfo(phone) {
            lottery.speed = 100;
            $.post("SiteServer.ashx?param=get_prize", function (data) { //获取奖品信息，并为相关属性赋值
                $("#lottery").attr("prize_site", data.prize_site);
                $("#lottery").attr("prize_id", data.prize_id);
                $("#lottery").attr("prize_name", data.prize_name);
                $("#lottery").attr("phone", phone);
                var res = start();
                var click = true;
                return false;
            }, "json");
        }

        // 检测用户今天是否可以抽奖
        function check_user(phone) {
            $.ajax({
                url: 'SiteServer.ashx?param=check_user',	// 提交地址
                type: 'post',			// 提交方式
                dateType: 'json',		// 数据类型
                async: false,        	// 设置同步
                data: { "phone": phone },	// 提交数据
                success: function (res) {	// 回调函数
                    result = eval(res);
                }
            })
            return result;
        }
        // 显示中奖信息
        function show_prize(prize_id, prize_name) {
            if (prize_id == 3 || prize_id == 10) {
                layer.msg('感谢参与，明日再来', function () {
                    var phone = $("#lottery").attr("phone");
                    $.post("SiteServer.ashx?param=NoPrize",
                        { "phone": phone });
                });
                return false;
            }
            layer.msg('恭喜您抽中【' + prize_name + '】', { time: 2000 }, function () {
                var phone = $("#lottery").attr("phone");
                layer.open({
                    type: 2,
                    title: '请填写收货地址',
                    maxmin: true,
                    area: ['600px', '400px'],
                    content: 'Views/address.html',
                });
            });
        }
    </script>
</body>
</html>
