using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using System.Web.SessionState;
using LitJson;
using YScms.Common;
using YScms.Web.DBUtility;
using System.IO;

namespace YScms.Web.tools
{
    /// <summary>
    /// AJAX提交处理
    /// </summary>
    public class submit_ajax2 : IHttpHandler, IRequiresSessionState
    {
        public const string newLine = "\r\n";
        public void ProcessRequest(HttpContext context)
        {
            //取得处事类型
            string action = HttpContext.Current.Request.QueryString["action"];

            switch (action)
            {
                case "trade":
                    trade(context);
                    break;
                case "zb":
                    zb(context);
                    break;
            }
        }

        private void trade(HttpContext context)
        {
            string datetype = DTRequest.GetStringValue("datetype", "年");
            if (datetype == "年")
            {
                trade_year(context);
            }
            if (datetype == "季")
            {
                trade_quarter(context);
            }
            if (datetype == "月")
            {
                trade_month(context);
            }
            if (datetype == "日")
            {
                trade_day(context);
            }
        }

        private void trade_year(HttpContext context)
        {
            string jsname = string.Empty;
            try
            {
                string province = DTRequest.GetStringValue("province", "全部区域");
                string industry = DTRequest.GetStringValue("industry", "全部行业");
                string ruOrchu = DTRequest.GetStringValue("ruOrchu", "outprovince");

                int default_date = DateTime.Now.Year;
                int bdate = DTRequest.GetInt("bdate", default_date);
                int edate = DTRequest.GetInt("edate", bdate);
                if (edate - bdate < 0)
                {
                    context.Response.Write("{\"status\":0, \"msg\":\"所选择的时间段需要对调！\"}");
                    return;
                }
                if (edate - bdate > 5)
                {
                    context.Response.Write("{\"status\":0, \"msg\":\"所选择的时间段超出加载范围！\"}");
                    return;
                }

                jsname = createjs("年", province, industry, bdate, edate, ruOrchu);
            }
            catch
            {
                context.Response.Write("{\"status\":0, \"msg\":\"数据请求失败！\"}");
                return;
            }
            context.Response.Write("{\"status\":1, \"msg\":\"" + jsname + "\"}");
            return;
        }

        private void trade_quarter(HttpContext context)
        {
            string jsname = string.Empty;
            try
            {
                string province = DTRequest.GetStringValue("province", "全部区域");
                string industry = DTRequest.GetStringValue("industry", "全部行业");
                string ruOrchu = DTRequest.GetStringValue("ruOrchu", "outprovince");
                int default_date = Utils.StrToInt(DateTime.Now.Year + "1", 0);
                int bdate = DTRequest.GetInt("bdate", default_date);
                int edate = DTRequest.GetInt("edate", bdate);
                int temp = edate - bdate - 6;
                if (edate - bdate < 0)
                {
                    context.Response.Write("{\"status\":0, \"msg\":\"所选择的时间段需要对调！\"}");
                    return;
                }

                if (temp > 5)
                {
                    context.Response.Write("{\"status\":0, \"msg\":\"所选择的时间段超出加载范围！\"}");
                    return;
                }

                jsname = createjs("季", province, industry, bdate, edate, ruOrchu);
            }
            catch
            {
                context.Response.Write("{\"status\":0, \"msg\":\"数据请求失败！\"}");
                return;
            }
            context.Response.Write("{\"status\":1, \"msg\":\"" + jsname + "\"}");
            return;
        }

        private void trade_month(HttpContext context)
        {
            string jsname = string.Empty;
            try
            {
                string province = DTRequest.GetStringValue("province", "全部区域");
                string industry = DTRequest.GetStringValue("industry", "全部行业");
                string ruOrchu = DTRequest.GetStringValue("ruOrchu", "outprovince");
                int default_date = Utils.StrToInt(DateTime.Now.ToString("yyyyMM"), 0);
                int bdate = DTRequest.GetInt("bdate", default_date);
                int edate = DTRequest.GetInt("edate", bdate);

                if (edate - bdate < 0)
                {
                    context.Response.Write("{\"status\":0, \"msg\":\"所选择的时间段需要对调！\"}");
                    return;
                }
                if (edate - bdate > 12)
                {
                    context.Response.Write("{\"status\":0, \"msg\":\"所选择的时间段超出加载范围！\"}");
                    return;
                }

                jsname = createjs("月", province, industry, bdate, edate, ruOrchu);
            }
            catch
            {
                context.Response.Write("{\"status\":0, \"msg\":\"数据请求失败！\"}");
                return;
            }
            context.Response.Write("{\"status\":1, \"msg\":\"" + jsname + "\"}");
            return;
        }

        private void trade_day(HttpContext context)
        {
            string jsname = string.Empty;
            try
            {
                string province = DTRequest.GetStringValue("province", "全部区域");
                string industry = DTRequest.GetStringValue("industry", "全部行业");
                string ruOrchu = DTRequest.GetStringValue("ruOrchu", "outprovince");
                int default_date = Utils.StrToInt(DateTime.Now.ToString("yyyyMMdd"), 0);
                int bdate = DTRequest.GetInt("bdate", default_date);
                int edate = DTRequest.GetInt("edate", bdate);

                if (edate - bdate < 0)
                {
                    context.Response.Write("{\"status\":0, \"msg\":\"所选择的时间段需要对调！\"}");
                    return;
                }
                if (edate - bdate > 5)
                {
                    context.Response.Write("{\"status\":0, \"msg\":\"所选择的时间段超出加载范围！\"}");
                    return;
                }


                jsname = createjs("日", province, industry, bdate, edate, ruOrchu);
            }
            catch
            {
                context.Response.Write("{\"status\":0, \"msg\":\"数据请求失败！\"}");
                return;
            }
            context.Response.Write("{\"status\":1, \"msg\":\"" + jsname + "\"}");
            return;
        }

        private void zb(HttpContext context)
        {
            string str_temp = string.Empty;
            DataTable dt = new Query().GetZBList(10, "1=1", "id").Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                str_temp += " '" + dr["outflow"] + "': [" + dr["inflow"] + "," + dr["quantity"] + "],\n";
            }
            str_temp = str_temp != string.Empty ? str_temp.Substring(0, str_temp.Length - 1) : str_temp;
            context.Response.Write(str_temp);
        }


        #region 生成JS文件==========================
        protected string createjs(string _datetype, string _province, string _industry, int _bdate, int _edate, string ruOrchu)
        {
            string outflow = ruOrchu == "outprovince" ? "inflow" : "outflow";
            string inflow = ruOrchu == "outprovince" ? "outflow" : "inflow";

            string jsname = "ajax_data.js";
            string path = HttpContext.Current.Server.MapPath("/templates/js/" + jsname + "");

            // Delete the file if it exists. 
            if (File.Exists(path)) { File.Delete(path); }

            // Create the file. 
            StreamWriter sr = File.CreateText(path);
            List<int> j_arrs = new List<int>();
            //日期段
            string timeline_str = string.Empty;
            if (_datetype == "季")
            {
                int _bdyear = Convert.ToInt32(_bdate.ToString().Substring(0, 4));
                int _bdJ = Convert.ToInt32(_bdate.ToString().Substring(4));

                int _edyear = Convert.ToInt32(_edate.ToString().Substring(0, 4));
                int _edJ = Convert.ToInt32(_edate.ToString().Substring(4));
                int j = 1;
                for (int bdyear = _bdyear; bdyear < _edyear; bdyear++)
                {
                    if (j == 1)
                    {
                        int i = _bdJ;
                        for (; i <= 4; i++)
                        {
                            int temp = Convert.ToInt32(bdyear.ToString() + i.ToString());
                            timeline_str += "'" + temp + "',";
                            j_arrs.Add(temp);
                        }
                    }
                    else
                    {
                        int i = 1;
                        for (; i <= 4; i++)
                        {
                            int temp = Convert.ToInt32(bdyear.ToString() + i.ToString());
                            timeline_str += "'" + temp + "',";
                            j_arrs.Add(temp);
                        }
                    }
                    j++;

                }
                for (int k = 1; k <= _edJ; k++)
                {
                    int temp = Convert.ToInt32(_edyear.ToString() + k.ToString());
                    timeline_str += "'" + temp + "',";
                    j_arrs.Add(temp);
                }
            }
            else
            {
                for (int date = _bdate; date <= _edate; date++)
                {
                    timeline_str += "'" + date + "',";
                    j_arrs.Add(date);
                }
            }
            timeline_str = timeline_str != string.Empty ? Utils.DelLastComma(timeline_str) : timeline_str;
            sr.WriteLine("var timeline_data = [" + timeline_str + "];");

            string province_str = string.Empty;
            string max_str = string.Empty;
            string max_arr = string.Empty;
            string markLine_str = string.Empty;
            string province_markLine_str = string.Empty;
            string province_markPoint_str = string.Empty;
            string markPoint_str = string.Empty;
            string markPoint_arr = string.Empty;
            string markLine_arr = string.Empty;
            DataTable line_dt = new DataTable();
            DataTable point_dt = new DataTable();
            DataTable province_line_dt = new DataTable();
            DataTable province_Point_dt = new DataTable();
            if (_datetype == "年")
            {
                if (_province != "全部区域")
                {
                    province_line_dt = new Query().GetTradeYearLineList(0, CombSqlTxt(_province, _industry, _bdate, _edate, "ruandchu"), "sum(quantity) desc").Tables[0];
                    province_Point_dt = new Query().GetTradeYearPointList(0, CombSqlTxt(_province, _industry, _bdate, _edate, "ruandchu"), "sum(quantity) desc", ruOrchu).Tables[0];
                }
                //全国线 不需要动态指定outflow 或者 inflow 
                line_dt = new Query().GetTradeYearLineList(1000, CombSqlTxt(_province, _industry, _bdate, _edate, ruOrchu), "sum(quantity) desc").Tables[0];
                //全国点 
                point_dt = new Query().GetTradeYearPointList(0, CombSqlTxt(_province, _industry, _bdate, _edate, ruOrchu), "sum(quantity) desc", ruOrchu).Tables[0];

            }
            if (_datetype == "季")
            {
                if (_province != "全部区域")
                {
                    province_line_dt = new Query().GetTradeMonthLineList(0, CombSqlTxt(_province, _industry, _bdate, _edate, "ruandchu"), "sum(quantity) desc").Tables[0];
                    province_Point_dt = new Query().GetTradeMonthPointList(0, CombSqlTxt(_province, _industry, _bdate, _edate, "ruandchu"), "sum(quantity) desc", ruOrchu).Tables[0];
                }
                line_dt = new Query().GetTradeMonthLineList(1000, CombSqlTxt(_province, _industry, _bdate, _edate, ruOrchu), "sum(quantity) desc").Tables[0];
                point_dt = new Query().GetTradeMonthPointList(0, CombSqlTxt(_province, _industry, _bdate, _edate, ruOrchu), "sum(quantity) desc", ruOrchu).Tables[0];
            }
            if (_datetype == "月")
            {
                if (_province != "全部区域")
                {
                    province_line_dt = new Query().GetTradeMonthLineList(0, CombSqlTxt(_province, _industry, _bdate, _edate, "ruandchu"), "sum(quantity) desc").Tables[0];
                    province_Point_dt = new Query().GetTradeMonthPointList(0, CombSqlTxt(_province, _industry, _bdate, _edate, "ruandchu"), "sum(quantity) desc", ruOrchu).Tables[0];
                }
                line_dt = new Query().GetTradeMonthLineList(1000, CombSqlTxt(_province, _industry, _bdate, _edate, ruOrchu), "sum(quantity) desc").Tables[0];
                point_dt = new Query().GetTradeMonthPointList(0, CombSqlTxt(_province, _industry, _bdate, _edate, ruOrchu), "sum(quantity) desc", ruOrchu).Tables[0];
            }
            if (_datetype == "日")
            {
                if (_province != "全部区域")
                {
                    province_line_dt = new Query().GetTradeLineList(0, CombSqlTxt(_province, _industry, _bdate, _edate, "ruandchu"), "sum(quantity) desc").Tables[0];
                    province_Point_dt = new Query().GetTradePointList(0, CombSqlTxt(_province, _industry, _bdate, _edate, "ruandchu"), "sum(quantity) desc", ruOrchu).Tables[0];
                }
                line_dt = new Query().GetTradeLineList(1000, CombSqlTxt(_province, _industry, _bdate, _edate, ruOrchu), "sum(quantity) desc").Tables[0];
                point_dt = new Query().GetTradePointList(0, CombSqlTxt(_province, _industry, _bdate, _edate, ruOrchu), "sum(quantity) desc", ruOrchu).Tables[0];
            }

            //点、线
            int index = 0;
            if (_edate == _bdate)
            {
                DataRow[] arraydr1 = line_dt.Select("", "");
                foreach (DataRow dr in arraydr1)
                {
                    markLine_str += "[{name: '" + dr["outflow"] + "', value: " + dr["quantity"] + "}, {name: '" + dr["inflow"] + "'}],";
                }

                DataRow[] arraydr2 = point_dt.Select("", "");
                foreach (DataRow dr in arraydr2)
                {
                    if (index == 0)
                    {
                        max_str += dr["quantity"];
                    }
                    markPoint_str += "{name: '" + dr[outflow] + "', value: " + dr["quantity"] + "},";

                    index++;
                }

                if (_province != "全部区域")
                {
                    DataRow[] province_arraydr1 = province_line_dt.Select("", "");
                    foreach (DataRow dr in province_arraydr1)
                    {
                        province_markLine_str += "[{name: '" + dr["outflow"] + "', value: " + dr["quantity"] + "}, {name: '" + dr["inflow"] + "'}],";
                    }

                    DataRow[] province_arraydr2 = province_Point_dt.Select("", "");

                    foreach (DataRow dr in province_arraydr2)
                    {
                        if (index == 0)
                        {
                            max_str += dr["quantity"];
                        }

                        province_markPoint_str += "{name: '" + dr[outflow] + "', value: " + dr["quantity"] + "},";

                        index++;
                    }
                }

                province_str = GetProvince(_province);
                max_str = max_str == string.Empty ? "0" : max_str;
                markLine_str = markLine_str != string.Empty ? Utils.DelLastComma(markLine_str) : markLine_str;
                markPoint_str = markPoint_str != string.Empty ? Utils.DelLastComma(markPoint_str) : markPoint_str;
                province_markLine_str = province_markLine_str != string.Empty ? Utils.DelLastComma(province_markLine_str) : province_markLine_str;
                province_markPoint_str = province_markPoint_str != string.Empty ? Utils.DelLastComma(province_markPoint_str) : province_markPoint_str;


                sr.WriteLine("var province_data = '" + province_str + "';");
                sr.WriteLine("var max_data = " + max_str + ";");
                sr.WriteLine("var markLine_data = [" + markLine_str + "];");
                sr.WriteLine("var markPoint_data = [" + markPoint_str + "];");
                sr.WriteLine("var province_markLine_data = [" + province_markLine_str + "];");
                sr.WriteLine("var province_markPoint_data = [" + province_markPoint_str + "];");

            }
            else
            {
                //for (int date = _bdate; date <= _edate; date++)
                foreach (int date in j_arrs)
                {
                    index = 0;
                    max_str = string.Empty;
                    markLine_str = string.Empty;
                    markPoint_str = string.Empty;

                    string strFilter = "trade_date = " + date + "";

                    #region 季度日期处理==========================
                    if (_datetype == "季")
                    {
                        int _bdatetemp; int _edatetemp;
                        ComDate(date, date, out _bdatetemp, out _edatetemp);
                        strFilter = "trade_date >= " + _bdatetemp + " and trade_date <= " + _edatetemp + "";
                    }
                    #endregion


                    DataRow[] arraydr1 = line_dt.Select(strFilter, "quantity asc");
                    foreach (DataRow dr in arraydr1)
                    {
                        markLine_str += "[{name: '" + dr["outflow"] + "', value: " + dr["quantity"] + "}, {name: '" + dr["inflow"] + "'}],";
                    }

                    DataRow[] arraydr = point_dt.Select(strFilter, "quantity desc");
                    foreach (DataRow dr in arraydr)
                    {
                        if (index == 0)
                        {
                            max_str += dr["quantity"];
                        }
                        markPoint_str += "{name: '" + dr[outflow] + "', value: " + dr["quantity"] + "},";
                        index++;
                    }
                    max_str = max_str == string.Empty ? "0" : max_str;
                    markLine_str = markLine_str != string.Empty ? Utils.DelLastComma(markLine_str) : markLine_str;
                    markPoint_str = markPoint_str != string.Empty ? Utils.DelLastComma(markPoint_str) : markPoint_str;
                    if (date == _bdate)
                    {
                        sr.WriteLine("var max_data = " + max_str + ";");
                        sr.WriteLine("var markPoint_data = [" + markPoint_str + "];");
                        sr.WriteLine("var markLine_data = [" + markLine_str + "];");
                    }
                    else
                    {
                        max_arr += "" + max_str + ",";
                        markPoint_arr += "[" + markPoint_str + "],";
                        markLine_arr += "[" + markLine_str + "],";
                    }

                }
                province_str = GetProvince(_province);
                max_arr = max_arr != string.Empty ? Utils.DelLastComma(max_arr) : max_arr;
                markPoint_arr = markPoint_arr != string.Empty ? Utils.DelLastComma(markPoint_arr) : markPoint_arr;
                markLine_arr = markLine_arr != string.Empty ? Utils.DelLastComma(markLine_arr) : markLine_arr;

                sr.WriteLine("var province_data = '" + province_str + "';");
                sr.WriteLine("var max_datas = [" + max_arr + "];");
                sr.WriteLine("var markPoint_datas = [" + markPoint_arr + "];");
                sr.WriteLine("var markLine_datas = [" + markLine_arr + "];");
            }
            sr.Close();
            return markPoint_str;
        }
        #endregion

        #region 组合SQL查询语句==========================
        protected string CombSqlTxt(string _province, string _industry, int _bdate, int _edate, string ruOrchu)
        {
            ComDate(_bdate, _edate, out _bdate, out _edate);
            StringBuilder strTemp = new StringBuilder();
            strTemp.Append("trade_date between " + _bdate + " and " + _edate + "");
            if (_province != "全部区域")
            {
                if (ruOrchu == "ruandchu")
                {
                    string[] province_temp = _province.Split(',');
                    //strTemp.Append(" and outprovince in (");
                    strTemp.Append(" and outprovince in (");
                    for (int i = 0; i < province_temp.Length; i++)
                    {
                        if (i == province_temp.Length - 1)
                        {
                            strTemp.Append("'" + province_temp[i] + "'");
                        }
                        else
                        {
                            strTemp.Append("'" + province_temp[i] + "',");
                        }
                    }
                    strTemp.Append(")");

                    strTemp.Append(" and inprovince in (");
                    for (int i = 0; i < province_temp.Length; i++)
                    {
                        if (i == province_temp.Length - 1)
                        {
                            strTemp.Append("'" + province_temp[i] + "'");
                        }
                        else
                        {
                            strTemp.Append("'" + province_temp[i] + "',");
                        }
                    }
                    strTemp.Append(")");
                }
                else
                {
                    string[] province_temp = _province.Split(',');
                    strTemp.Append(" and " + ruOrchu + " in (");
                    for (int i = 0; i < province_temp.Length; i++)
                    {
                        if (i == province_temp.Length - 1)
                        {
                            strTemp.Append("'" + province_temp[i] + "'");
                        }
                        else
                        {
                            strTemp.Append("'" + province_temp[i] + "',");
                        }
                    }
                    strTemp.Append(")");
                }
            }
            if (_industry != "全部行业")
            {
                string[] industry_temp = _industry.Split(',');
                strTemp.Append(" and industrySid in (");
                for (int i = 0; i < industry_temp.Length; i++)
                {
                    if (i == industry_temp.Length - 1)
                    {
                        strTemp.Append("'" + industry_temp[i] + "'");
                    }
                    else
                    {
                        strTemp.Append("'" + industry_temp[i] + "',");
                    }
                }
                strTemp.Append(")");
            }
            return strTemp.ToString();
        }

        protected void ComDate(int _bdate, int _edate, out int _rtbdate, out int _rtedate)
        {
            string _bdatestr = _bdate.ToString(); string _edatestr = _edate.ToString();
            if (_bdatestr.Length == 5 && _edatestr.Length == 5)
            {
                if (_bdatestr == _edatestr)
                {
                    switch (_bdatestr.Substring(4))
                    {
                        case "1":
                            _bdate = Utils.StrToInt(_bdatestr.Substring(0, 4) + "01", 0);
                            _edate = Utils.StrToInt(_bdatestr.Substring(0, 4) + "03", 0); break;
                        case "2":
                            _bdate = Utils.StrToInt(_bdatestr.Substring(0, 4) + "04", 0);
                            _edate = Utils.StrToInt(_bdatestr.Substring(0, 4) + "06", 0); break;
                        case "3":
                            _bdate = Utils.StrToInt(_bdatestr.Substring(0, 4) + "07", 0);
                            _edate = Utils.StrToInt(_bdatestr.Substring(0, 4) + "09", 0); break;
                        case "4":
                            _bdate = Utils.StrToInt(_bdatestr.Substring(0, 4) + "10", 0);
                            _edate = Utils.StrToInt(_bdatestr.Substring(0, 4) + "12", 0); break;
                        default: break;
                    }
                }
                else
                {
                    switch (_bdatestr.Substring(4))
                    {
                        case "1":
                            _bdate = Utils.StrToInt(_bdatestr.Substring(0, 4) + "01", 0); break;
                        case "2":
                            _bdate = Utils.StrToInt(_bdatestr.Substring(0, 4) + "04", 0); break;
                        case "3":
                            _bdate = Utils.StrToInt(_bdatestr.Substring(0, 4) + "07", 0); break;
                        case "4":
                            _bdate = Utils.StrToInt(_bdatestr.Substring(0, 4) + "10", 0); break;
                        default: break;
                    }

                    switch (_edatestr.Substring(4))
                    {
                        case "1":
                            _edate = Utils.StrToInt(_edatestr.Substring(0, 4) + "03", 0); break;
                        case "2":
                            _edate = Utils.StrToInt(_edatestr.Substring(0, 4) + "06", 0); break;
                        case "3":
                            _edate = Utils.StrToInt(_edatestr.Substring(0, 4) + "09", 0); break;
                        case "4":
                            _edate = Utils.StrToInt(_edatestr.Substring(0, 4) + "12", 0); break;
                        default: break;
                    }
                }
            }
            _rtbdate = _bdate;
            _rtedate = _edate;
        }
        #endregion

        #region 省份关系==========================
        protected string GetProvince(string _province)
        {
            string province = string.Empty;
            switch (_province)
            {
                case "全部区域":
                    province = "china"; break;
                case "内蒙古自治区":
                case "黑龙江省":
                    province = _province.Substring(0, 3); break;
                default:
                    province = _province.Substring(0, 2); break;
            }
            return province;
        }
        #endregion

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}