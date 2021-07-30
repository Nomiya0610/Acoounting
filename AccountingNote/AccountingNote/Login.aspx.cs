using AccountingNote.DBSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AccountingNote
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(this.Session["UserLoginInfo"] != null)//若登入過則導入至資訊面
            {
                this.plcLogin.Visible = false;
                Response.Redirect("/SystemAdmin/UserInfo.aspx");
            }
            else
            {
                this.plcLogin.Visible = true;
            }
           
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string inp_Account = this.txtAccount.Text;//取得使用者資訊
            string inp_PWD = this.txtPWD.Text;

            if (string.IsNullOrWhiteSpace(inp_Account) || string.IsNullOrWhiteSpace(inp_PWD))//檢查是否為空
            {
                this.ltlMsg.Text = "Account / Password is required. ";
                return;//一旦發生錯誤就不跑程式
            }
            var dr = UserInfoManager.GetUserInfoByAccount(inp_Account);
            // Check null
            if(dr == null)
            {
                this.ltlMsg.Text = "Account doesn't exists. ";
                return;
            }
            //Check Account / Password
            if (string.Compare(dr["Account"].ToString(), inp_Account, true) == 0 &&//帳號忽略大小寫的比對
                string.Compare(dr["PWD"].ToString(), inp_PWD,false) ==0 )//密碼則要比對大小寫
            {
                this.Session["Userlogininfo"] = dr["Account"].ToString();//帳號寫到Session
                Response.Redirect("/SystemAdmin/UserInfo.aspx");//導頁至個人資訊頁
            }
            else
            {
                this.ltlMsg.Text = "Login fail. Please check Account / Password. ";
                return;
            }
        }
    }
}