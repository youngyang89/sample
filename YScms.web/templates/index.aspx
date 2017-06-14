<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="YScms.aspx.index" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>电商流向分析</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="pragma" content="no-cache" />
    <meta http-equiv="Cache-Control" content="no-cache,must-revalidate" />

    <link type="text/css" rel="stylesheet" href="css/css.css" />

    <script src="../scripts/jquery/jquery-1.10.2.min.js" type="text/javascript"></script>

    <script src="../scripts/datepicker/WdatePicker.js" type="text/javascript"></script>
    
    <script type="text/javascript" src="js/menutime.js"></script>

    <script type="text/javascript" src="js/jqselect.js"></script>

    <script type="text/javascript" src="js/selectbox.js"></script>

    <script type="text/javascript" src="js/function.js"></script>
    
    <script type="text/javascript" src="js/color.js"></script>
    
    <script type="text/javascript"> 

    </script> 
</head>
<body <%--oncontextmenu="return false" onselectstart="return false"--%>>
    <form id="form1" runat="server">
    <div class="page">
        <div class="header">
            <a data-type="china-city" href="/templates/default/index.html"><div class="logo">经济社会感知系统</div></a>
            <ul class="site-nav">
                <li class="line"></li>
                <li class="selected"><a data-type="china-city" href="/templates/index.aspx">电商流向分析</a></li>
                <li class="line"></li>
                <li><a data-type="china-city" href="/news.aspx">电商量化分析</a></li>
                <li class="line"></li>
                <li><a href="javascript:void(0);">可视分析工具</a>
                    <dl class="nav_bd" style="display:none">
                        <dd><a data-type="china-city" href="/macro.aspx">定制化</a></dd>
                        <dd><a data-type="china-city" href="/contrast.aspx">差异化</a></dd>
                    </dl>
                </li>
                <li class="line"></li>
            </ul>
            <div class="contact">
                <a href="index.aspx" class="c1">还原</a>
                <asp:LinkButton ID="lbtOut" runat="server" OnClick="tc_out" class="c2">退出</asp:LinkButton>
            </div>
        </div>
        <div class="wrapper" >
            <div id="main" class="map china-map"></div>

            <script type="text/javascript" src="../scripts/YiShaEcharts/YiShaEcharts-plain-map.js"></script>

            <script type="text/javascript" id="ajax_data"></script>

            <script type="text/javascript" id="index"></script>

        </div>
        <div class="location-detail_l">
            <div class="location">
                <!--地区选择-->
                <div class="comct">
                    <span class="icontext">地区：</span>
                    <input type="text" class="region" id="region" value="全国" readonly="readonly"/>
                    <div id="provinces" class="provinces" style="display:none;">
                        <div class="text"><a accesskey="全国">全国</a></div>
                        <div class="area">
                            <div class="area_l">A-G:</div>
                            <div class="area_r">
                                <a accesskey="安徽省">安徽</a>
                                <a accesskey="澳门特别行政区">澳门</a>
                                <a accesskey="北京市">北京</a>
                                <a accesskey="重庆市">重庆</a>
                                <a accesskey="福建省">福建</a>
                                <a accesskey="广东省">广东</a>
                                <a accesskey="甘肃省">甘肃</a>
                                <a accesskey="广西壮族自治区">广西</a>
                                <a accesskey="贵州省">贵州</a>
                            </div>
                        </div>
                        <div class="area">
                            <div class="area_l">H-J:</div>
                            <div class="area_r">
                                <a accesskey="河北省">河北</a>
                                <a accesskey="黑龙江省">黑龙江</a>
                                <a accesskey="河南省">河南</a>
                                <a accesskey="湖南省">湖南</a>
                                <a accesskey="湖北省">湖北</a>
                                <a accesskey="海南省">海南</a>
                                <a accesskey="吉林省">吉林</a>
                                <a accesskey="江苏省">江苏</a>
                                <a accesskey="江西省">江西</a> 
                            </div>
                        </div>
                        <div class="area">
                            <div class="area_l">L-S:</div>
                            <div class="area_r">
                                <a accesskey="辽宁省">辽宁</a>
                                <a accesskey="内蒙古自治区">内蒙古</a>
                                <a accesskey="宁夏回族自治区">宁夏</a>
                                <a accesskey="青海省">青海</a>
                                <a accesskey="上海市">上海</a>
                                <a accesskey="四川省">四川</a>
                                <a accesskey="山东省">山东</a>
                                <a accesskey="山西省">山西</a>
                                <a accesskey="陕西省">陕西</a>
                            </div>
                        </div>
                        <div class="area">
                            <div class="area_l">T-Z:</div>
                            <div class="area_r">
                                <a accesskey="天津市">天津</a>
                                <a accesskey="台湾省">台湾</a>
                                <a accesskey="西藏自治区">西藏</a>
                                <a accesskey="香港特别行政区">香港</a>
                                <a accesskey="新疆维吾尔自治区">新疆</a>
                                <a accesskey="云南省">云南</a>
                                <a accesskey="浙江省">浙江</a>
                            </div>
                        </div>
                    </div>
                </div>
                <!--地区选择:end-->
                <div class="clear"></div>
                <div style="padding: 10px 0px;">
                    <div class="type-select">
                        <select class="" id="datetype">
                            <option value="年" selected="selected">年</option>
                            <option value="季">季</option>
                            <option value="月">月</option>
                            <option value="日">日</option>
                        </select>
                    </div>
                    <div class="date-select">
                        <select class="" id="qbdate"></select>
                    </div>
                    <input class="date-input" id="bdate" type="text" readonly="readonly" />
                    -
                    <div class="date-select">
                        <select class="" id="qedate"></select>
                    </div>
                    <input class="date-input" id="edate" type="text" readonly="readonly" />
                </div>
                <div class="clear">
                </div>
                <div id="firstpane" class="menu_list">
                    <p class="menu_head">选择行业</p>
                    <div class="menu_body sidebar">
                        <ul id="menu">
                            <asp:Repeater ID="rpt_industry" runat="server" OnItemDataBound="rpt_industry_ItemDataBound">
                                <ItemTemplate>
                                    <li class="p_li<%#Eval("industry_id")%>"><a href="javascript:void(0);">
                                        <%#Eval("industry_name")%></a>
                                        <div class="dropdown_4columns">
                                            <div class="col_1">
                                                <div class="htitle">
                                                    <span onclick="checkItems(this, 'li<%#Eval("industry_id")%>');">全选</span>
                                                </div>
                                                <ul>
                                                    <asp:Repeater ID="rpt_industry_sub" runat="server">
                                                        <ItemTemplate>
                                                            <li>
                                                                <input name="industry" class='li<%#Eval("industry_pid")%>' type="checkbox" value="<%#Eval("industry_name")%>" />
                                                                <span title='<%#Eval("industry_name")%>'><%# sub(Eval("industry_name").ToString())%></span>
                                                            </li>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </ul>
                                            </div>
                                        </div>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>
                </div>
                <div class="industry" id="industry_lbl" style=" margin-top:10px;color: #AEB6BC;font-family:san-serif;font-size: 12px; padding: 5px 0;">
                    <p>
                        <div style=" margin-top:-3px;float:left">是否带时间轴：</div>
                        <span><input type="checkbox" value="是否带时间轴" id="isOrno_time"/></span>
                    </p>
                    <p>
                        <input type="radio" value="卖出" id="chu" name="CR" checked="checked" />卖出
                        <input type="radio" value="买入" id="ru"  name="CR" />买入
                    </p>
                    <p>
                        <div style=" margin-top:-4px;float:left">您所选的行业：</div>
                        <span><img alt="" src="img/hidet.png" onclick="clearItems('industry');" /></span>
                    </p>
                    <dl></dl>
                </div>
                <div class="clear"></div>
                <div class="industry_input">
                    <input type="button" value="查询" id="search" />
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>

<script type="text/javascript">
    //设置页面高度
    var MS = "IE";
    if (navigator.userAgent.indexOf("Firefox") > 0) {
        MS = "FF";
    }
    if (document.getElementById("main") != null) {
        if (document.documentElement.clientHeight > 200) {
            document.getElementById("main").style.height = document.documentElement.clientHeight - 88 + "px";
        }
    }
    var t = 0;
    window.onresize = function() {
        var now = new Date();
        now = now.getTime();
        if (now - t > 30) {
            if (document.getElementById("main") != null) {
                if (document.documentElement.clientHeight > 200) {
                    document.getElementById("main").style.height = document.documentElement.clientHeight - 88 + "px";
                }
                t = now;
            }
        }
    }
</script>

