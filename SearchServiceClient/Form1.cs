using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SearchServiceClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnUpdateIndex_Click(object sender, EventArgs e)
        {
            try
            {
                DoService.SearchServiceClient myClient = new DoService.SearchServiceClient();

                if (myClient.UpdateIndex(-1))
                    txtResults.Text += "Index updated successfully" + Environment.NewLine;
                else
                    txtResults.Text += "Could not update index" + Environment.NewLine;
            }
            catch (Exception ex)
            {
                string aa = string.Empty;
                txtResults.Text = ex.ToString();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //if (txtSearch.Text.Length == 0)
            //{
            //    txtResults.Text += "String text empty" + Environment.NewLine;
            //    return;
            //}
            
            //try
            //{
            //    TestSearchService.SearchServiceClient myClient = new TestSearchService.SearchServiceClient();

            //    var results=  myClient.SearchText(txtSearch.Text, string.Empty, 50, 1);

            //    txtResults.Text += "-----------Result set ---------" + Environment.NewLine;
            //    foreach (IssueDocument art in results)
            //    {
            //        txtResults.Text += string.Format("{0} --  -- ", art.IssueId.ToString()) + Environment.NewLine;
            //    }
            //    txtResults.Text += "-----------End Result set ---------" + Environment.NewLine;
            //}
            //catch (Exception ex)
            //{
            //    txtResults.Text += ex.ToString();
            //}
        }

        private void btnDeleteResults_Click(object sender, EventArgs e)
        {
            txtResults.Text = string.Empty;
        }


    }
}
