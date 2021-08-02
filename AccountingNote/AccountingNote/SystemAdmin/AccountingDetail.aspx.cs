using AccountingNote.Auth;
using AccountingNote.DBSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


namespace AccountingNote.SystemAdmin
{
    public partial class AccountingDetail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AuthManager.IsLogined())
            {
                Response.Redirect("/Login.aspx");
                return;
            }

            string account = this.Session["UserLoginInfo"] as string;
            var currentUser = AuthManager.GetCurrentUser();

            if (currentUser == null)
            {
                this.Session["UserLoginInfo"] = null;
                Response.Redirect("/Login.aspx");
                return;
            }

            if(!this.IsPostBack)
            {
                //check is create mode or edit mode
                if (this.Request.QueryString["ID"] == null)
                {
                    this.btnDelete.Visible = false;
                }
                else
                {
                    this.btnDelete.Visible = true;

                    string idText = this.Request.QueryString["ID"];
                    int id;
                    if (int.TryParse(idText, out id))
                    {
                        var drAccounting = AccountingManager.GetAccounting(id, currentUser.ID);

                        if (drAccounting == null)
                        {
                            this.ltMsg.Text = "Data doesn't exist.";
                            this.btnSave.Visible = false;
                            this.btnDelete.Visible = false;
                        }
                        else
                        {
                            this.ddActType.SelectedValue = drAccounting["ActType"].ToString();
                            this.txtAmount.Text = drAccounting["Amount"].ToString();
                            this.txtCaption.Text = drAccounting["Caption"].ToString();
                            this.txtDesc.Text = drAccounting["body"].ToString();
                        }
                    }
                    else
                    {
                        this.ltMsg.Text = "ID is required. ";
                        this.btnSave.Visible = false;
                        this.btnDelete.Visible = false;
                    }
                }
                
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            List<string> msgList = new List<string>();
            if(!this.CheckInput(out msgList))
            {
                this.ltMsg.Text = string.Join("<br/>", msgList);
                return;
            }


            UserInfoModel currentUser = AuthManager.GetCurrentUser();
            if(currentUser == null)
            { 
                Response.Redirect("/Login.aspx");
                return;
            }

            string userID = currentUser.ID;
            string actTypeText = this.ddActType.SelectedValue;
            string amountText = this.txtAmount.Text;
            string caption = this.txtCaption.Text;
            string body = this.txtDesc.Text;

            int amount = Convert.ToInt32(amountText);
            int actType = Convert.ToInt32(actTypeText);

            string idText = this.Request.QueryString["ID"];
            if(string.IsNullOrWhiteSpace(idText))
            {
                //Execute 'Insert into db'
                 AccountingManager.CreateAccounting(userID, caption, amount, actType, body);
            }
            else
            {
                int id;
                if(int.TryParse(idText,out id))
                {
                    //Execute 'Update db'
                    AccountingManager.UpdateAccounting(id, userID, caption, amount, actType, body);
                }
            }
           
            Response.Redirect("/SystemAdmin/AccountingList.aspx");

        }

        private bool CheckInput(out List<string> errorMsgList)
        {
            List<string> msgList = new List<string>();

            if(this.ddActType.SelectedValue != "0" &&
               this.ddActType.SelectedValue != "1")
            {
                msgList.Add("Type must be 0 or 1 .");
            }

            if (string.IsNullOrWhiteSpace(this.txtAmount.Text))
            {
                msgList.Add("Amount is required.");
            }
            else
            {
                int tempInt;
                if (!int.TryParse(this.txtAmount.Text, out tempInt))
                {
                    msgList.Add("Amount must be a number.");
                }
                if(tempInt < 0 || tempInt > 1000000)
                {
                    msgList.Add("Amount must between 0 and 1,000,000. ");
                }
            }
            errorMsgList = msgList;
            if (msgList.Count == 0)
                return true;
            else
                return false;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string idText = this.Request.QueryString["ID"];
           
            if (string.IsNullOrWhiteSpace(idText))
                return;

            int id;
            if(int.TryParse(idText, out id))
            {
                AccountingManager.DeleteAccounting(id);

            }
            Response.Redirect("/SystemAdmin/AccountingList.aspx");
        }
    }
}