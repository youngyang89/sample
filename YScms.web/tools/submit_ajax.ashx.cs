using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using System.Web.SessionState;
using YScms.Web.UI;
using YScms.Common;
using LitJson;
using System.IO;
using System.Linq;
using System.Configuration;

namespace YScms.Web.tools
{
    /// <summary>
    /// AJAX提交处理
    /// </summary>
    public class submit_ajax : IHttpHandler, IRequiresSessionState
    {
        Model.siteconfig siteConfig = new BLL.siteconfig().loadConfig();

        public void ProcessRequest(HttpContext context)
        {
            //取得处事类型
            string action = DTRequest.GetQueryString("action");

            switch (action)
            {
                #region 内容方法
        
                case "trade":
                    trade(context);
                    break;
                case "loginReg":
                    loginReg(context);
                    break;
                case "outReg":
                    outReg(context);
                    break;
                #endregion
            }
        }

       
        /// <summary>
        /// 验证是否登陆
        /// </summary>
        /// <param name="context"></param>
        public void loginReg(HttpContext context)
        {
            //0 表示未登录
            //1 表示已登陆
            if (context.Session["UserName"] == null)
            {

                context.Response.Write("0");
            }
            else
            {
                context.Response.Write("1");
            }
        }

        /// <summary>
        /// 退出登陆
        /// </summary>
        /// <param name="context"></param>
        public void outReg(HttpContext context)
        {
            HttpContext.Current.Session["UserName"] = null;

            context.Response.Write("操作成功");
        }


        private void trade(HttpContext context)
        {
            string province = DTRequest.GetStringValue("province", "全国");

            string industry = DTRequest.GetFormString("industry");
            if (industry.Trim() == "请选择行业") { industry = ""; }

            string ruorchu = DTRequest.GetStringValue("ruorchu", "outprovince");
            string numbers = DTRequest.GetStringValue("numbers", "10");

            string datetype = DTRequest.GetStringValue("datetype", "月");
            string[] datas = GetDate(datetype);
            string timeB = datas[0];
            string timeE = datas[1];


            BLL.article bll = new BLL.article();

            string id = DTRequest.GetFormString("id");
            string[] strid = id.Split(',');
            for (int i = 0; i < strid.Length - 1; i++)
            {
                Model.article model = bll.GetModel(Convert.ToInt32(strid[i]));
                string chart = bll.GetChartsTitle(model.charts_id);
                //把图标类型和Model传给公共方法 处理数据和业务逻辑
                tool(chart, model, province, datetype, timeB, timeE, industry, ruorchu, numbers);
            }
        }


        /// <summary>
        /// 公用方法
        /// </summary>
        /// <param name="chart"></param>
        public bool tool(string chart, Model.article model, string province, string type, string timeB, string timeE, string industry, string ruOrChu, string numbers)
        {
            bool result = false;

            //在这之前为减少代码冗余利用公共方法把展示类型赋值成对应的数据库字段 方便扩展维护
            model = UpdateDisplayType(model);

            switch (chart)
            {
                case "柱状图":
                    result = AddBarChart(model, province, timeB, timeE, industry, type, ruOrChu, numbers);
                    break;
                case "折线图":
                    result = AddLineChart(model, province, timeB, timeE, industry, type, ruOrChu, numbers);
                    break;
                case "区域折线图":
                    result = AddAreaLineChart(model, province, timeB, timeE, industry, type, ruOrChu, numbers);
                    break;
                case "漏斗图":
                    result = AddFunnelChart(model, province, timeB, timeE, industry, type, ruOrChu, numbers);
                    break;
                case "标准饼图":
                    result = AdddPieChart(model, province, timeB, timeE, industry, type, ruOrChu, numbers);
                    break;
                case "散点图":
                    result = AddScatterChart(model, province, timeB, timeE, industry, type, ruOrChu, numbers);
                    break;
                case "地图标注":
                    result = AddMapLabelingChart(model, province, timeB, timeE, industry, type, ruOrChu, numbers);
                    break;
                //case "标准环形图":
                //    result = AddStandardAnnularChart(id);
                //    break;
                //case "南丁格尔玫瑰图":
                //    result = AddStandardPieChart(id);
                //    break;
                //case "嵌套饼图":
                //    result = AddNestedPieChart(id);
                //    break;
                //case "环形图":
                //    result = AddAnnularChart(id);
                //    break;
                default:
                    break;
            }
            return result;
        }


        /// <summary>
        /// 此方法用于把展示类型赋值成对应的数据库字段 返回Model
        /// </summary>
        public Model.article UpdateDisplayType(Model.article model)
        {
            string displayType = model.fields["displayType"].Trim();
            string groupBy = model.fields["groupBy"].Trim();
            switch (displayType)
            {
                case "行业":
                    displayType = "industry";
                    break;
                case "地区":
                    displayType = "outflow";
                    break;
                case "交易时间":
                    displayType = "trade_date";
                    break;
                default:
                    displayType = "";
                    break;
            }

            switch (groupBy)
            {
                case "行业":
                    groupBy = "industry";
                    break;
                case "地区":
                    groupBy = "outflow";
                    break;
                case "交易时间":
                    groupBy = "trade_date";
                    break;
                default:
                    groupBy = "";
                    break;
            }

            //重新赋值
            model.fields["displayType"] = displayType;

            model.fields["groupBy"] = groupBy;
            return model;
        }


        /// <summary>
        /// 将string数组转化为float数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public double[] StrToFloat(string[] str)
        {
            double[] intArr = new double[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                intArr[i] = double.Parse(str[i]);
            }
            return intArr;
        }


        /// <summary>
        /// 处理数据库获取数据
        /// </summary>
        /// <param name="model"></param>
        /// <param name="province"></param>
        /// <param name="timeB"></param>
        /// <param name="timeE"></param>
        /// <param name="industry"></param>
        /// <returns></returns>
        public DataSet GetData(Model.article model, string province, string timeB, string timeE, string industry, string type, string ruOrChu, string numbers)
        {
            DataSet ds = new DataSet();
            ds = bll.GetChartData(0, model.fields["sources"], model.fields["displayType"], province, timeB, timeE, industry, type, model.fields["groupBy"], ruOrChu);
            return ds;
        }


        /// <summary>
        /// 写JS
        /// </summary>
        /// <param name="model"></param>
        /// <param name="option"></param>
        public void WriteJs(Model.article model, object option)
        {
            // 基于准备好的dom，初始化echarts图表  容器ID为内容ID
            string myChart = "var myChart = echarts.init(document.getElementById('" + model.id + "'));";

            //序列化
            string barChart = "option=" + Newtonsoft.Json.JsonConvert.SerializeObject(option);

            // 为echarts对象加载数据 
            string setOption = ";myChart.setOption(option)";

            barChart = myChart + barChart + setOption;

            //生成JS
            string jsname = model.id + ".js";
            string path = HttpContext.Current.Server.MapPath("/templates/default/js/" + jsname + "");

            // Delete the file if it exists. 
            if (File.Exists(path)) { File.Delete(path); }

            // Create the file. 
            StreamWriter sr = File.CreateText(path);
            sr.WriteLine(barChart);
            sr.Close();
        }




        /// <summary>
        /// 获取Series 用于displayType 和 groupBy不同的情况下 柱状图,折线图,区域折线图
        /// </summary>
        /// <param name="temp1">数据源</param>
        /// <param name="strLegend">name值</param>
        /// <param name="strxAxis">对应X轴值</param>
        /// <param name="model">Model</param>
        /// <returns></returns>
        public object[] GetSeries(DataTable temp1, string[] strLegend, string[] strxAxis, Model.article model, string chart, int state)
        {
            List<object> listBar2 = new List<object>();

            foreach (string str1 in strLegend)
            {
                List<double> _data = new List<double>();
                foreach (string str2 in strxAxis)
                {
                    bool te = false;
                    foreach (DataRow str3 in temp1.Rows)
                    {
                        if (str3[model.fields["groupBy"]] + "" == str1 && str3[model.fields["displayType"]] + "" == str2)
                        {
                            double temp = Convert.ToInt64(str3["quantity"]);
                            _data.Add(Math.Round(temp, 2));
                            te = true;
                        }
                    }
                    if (!te)
                    {
                        _data.Add(Math.Round(0.0));
                    }
                }

                //区域折线图
                if (state == 1)
                {
                    //拿到data之后new一个对象
                    Model.AreaLineSeries _bar = new Model.AreaLineSeries(str1, chart, _data.ToArray(), "default");
                    listBar2.Add(_bar);
                }
                //柱状图,折线图
                else if (state == 0)
                {
                    //拿到data之后new一个对象
                    Model.BarSeries _bar = new Model.BarSeries(str1, chart, _data.ToArray());
                    listBar2.Add(_bar);
                }
            }
            return listBar2.ToArray();
        }





        /// <summary>
        /// 获取Series 用于地图
        /// </summary>
        /// <param name="temp1">数据源</param>
        /// <param name="strLegend">name值</param>
        /// <param name="strxAxis">对应X轴值</param>
        /// <param name="model">Model</param>
        /// <returns></returns>
        public object[] GetFunnelSeries(DataTable temp1, string[] strLegend, Model.article model, string chart)
        {

            List<object> listBar = new List<object>();
            //组装data数据
            for (int k = 0; k < strLegend.Length; k++)
            {
                Model.MapData MapData = new YScms.Model.MapData();
                double sum = 0.0;
                for (int n = 0; n < temp1.Rows.Count; n++)
                {
                    if (temp1.Rows[n][model.fields["displayType"]].ToString().Trim() == strLegend[k].Trim())
                    {
                        sum += Convert.ToInt64(temp1.Rows[n]["quantity"]);
                        continue;
                    }
                    else
                    {
                        sum += 0.0;
                        continue;
                    }
                }

                MapData.name = strLegend[k].Trim();
                MapData.value = Math.Round(sum, 2);
                listBar.Add(MapData);
            }
            List<object> listPieSeries = new List<object>();

            Model.MapSeries MapSeries = new YScms.Model.MapSeries("", chart, "china", false, false, new string[] { }, listBar.ToArray());
            listPieSeries.Add(MapSeries);
            return listPieSeries.ToArray();

        }


  

        /// <summary>
        /// 生成地图标注【值域漫游】图JS
        /// </summary>
        /// <param name="id">内容ID</param>
        /// <returns></returns>
        public bool AddMapLabelingChart(Model.article model, string province, string timeB, string timeE, string industry, string type, string ruOrchu, string numbers)
        {

            try
            {

                string temp = ruOrchu == "outprovince" ? "inflow" : "outflow";
                if (model.fields["displayType"] == "outflow")
                {
                    model.fields["displayType"] = temp;
                }

                //此处根据条件读取数据库返回数据
                DataSet ds = GetData(model, province, timeB, timeE, industry, type, ruOrchu, numbers);

                //总数据
                DataTable temp1 = ds.Tables[0];
                //legend
                DataTable groupBy = new DataTable();
                DataView dv = temp1.DefaultView;
                groupBy = dv.ToTable(true, new string[] { model.fields["displayType"] });

                string[] strLegend = groupBy.AsEnumerable().Select(d => d.Field<string>(model.fields["displayType"])).ToArray();

                //series
                object[] barSeries = GetFunnelSeries(temp1, strLegend, model, "map");

                #region option
                var option = new
                {
                    title = new
                    {
                        text = model.fields["text"],
                        subtext = ""
                    },
                    tooltip = new
                    {
                        trigger = "item",
                        show = false
                    },
                    dataRange = new
                    {
                        min = 0,
                        max = 500,
                        calculable = true,
                        color = new string[] { "maroon", "purple", "red", "orange", "yellow", "lightgreen" }
                    },
                    series = barSeries


                };
                #endregion

                //写JS
                WriteJs(model, option);
            }
            catch (Exception)
            {

                return false;
            }
            return true;
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