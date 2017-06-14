using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YScms.Web.DBUtility;

namespace YScms.aspx
{
    public partial class index : System.Web.UI.Page, System.Web.SessionState.IRequiresSessionState
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserName"] == null)
            {
                Response.Redirect("/templates/login.html");
            }
            else
            {
                string strSql = "industry_id>0 and industry_pid=0";
                rpt_industry.DataSource = new Query().GetIndustryList(0, strSql, "industry_id desc");
                rpt_industry.DataBind();
            }
        }


        protected void rpt_industry_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Repeater rpt_industry_sub = (Repeater)e.Item.FindControl("rpt_industry_sub");
                DataRowView drv = (DataRowView)e.Item.DataItem;
                string pid = drv["industry_id"] + "";
                string strSql = "industry_pid=" + pid;
                rpt_industry_sub.DataSource = new Query().GetIndustryList(0, strSql, "industry_id asc");
                rpt_industry_sub.DataBind();
            }
        }
        protected void tc_out(object sender, EventArgs e)
        {
            Session["UserName"] = null;
            Response.Redirect("/templates/login.html");
        }

        public string sub(string str)
        {
            string strs = "";
            str = str.Trim();
            if (str.Length > 4)
            {
                strs = str.Substring(0, 4);
                strs += "..";
                return strs;
            }
            else
            {
                return str;
            }
        }
    }
}
