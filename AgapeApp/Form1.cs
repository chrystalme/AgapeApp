using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace AgapeApp
{
    public partial class Form1 : Form
    {
        string conn_string = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\ChrystalPro\\Documents\\LifeAgape_Db.accdb;Persist Security Info=False";
        string error_msg = "";
        string q = "";
        OleDbConnection conn = null;
        
        public Form1()
        {
            InitializeComponent();
            DateTime dt = DateTime.Now;
            lbl_Date.Text = dt.ToShortDateString()+ " " + dt.ToShortTimeString();
            lbl_Date.Show();
            try
            {
                conn = new OleDbConnection(conn_string);
                conn.Open();
                show_Data();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

        }
        #region Exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            conn.Close();
            Application.Exit();
        }
        #endregion
        #region New
        private void btn_New_Click(object sender, EventArgs e)
        {
            main_Panel.Enabled = true;
            ClearData();
            txt_First_Name.Focus();
            if (btn_Save.Text != "Save")
            {
                btn_Save.Text = "Save";
            }
            if (cmb_Month.Enabled == false)
            {
                cmb_Month.Enabled = true;
            }


        }
        #endregion
        #region Save Data Or Update
        //Save
        private void btn_Save_Click(object sender, EventArgs e)
        {
            string fname = null;
            string lname = null;
            string mentor = null;
            string dept = null;
            if (string.IsNullOrEmpty(txt_First_Name.Text))
            {
                MessageBox.Show("First Name cannot be empty ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                fname = txt_First_Name.Text.ToUpperInvariant().Trim();
            }
            if (string.IsNullOrEmpty(txt_Last_Name.Text))
            {
                MessageBox.Show("Last Name cannot be empty ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                lname = txt_Last_Name.Text.ToUpperInvariant().Trim();
            }
            if (string.IsNullOrEmpty(txt_Mentor.Text))
            {
                MessageBox.Show("Mentor Name cannot be empty ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                mentor = txt_Mentor.Text.ToUpperInvariant().Trim();
            }
            if (string.IsNullOrEmpty(cmb_Dept.Text))
            {
                MessageBox.Show("Department cannot be empty ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                dept = cmb_Dept.Text.ToUpperInvariant().Trim();
            }
            if (btn_Save.Text == "Save")
            {
                if (main_Panel.Enabled == false)
                {
                    MessageBox.Show("Click on the New button to create Record");
                }
                else
                {

                    try
                    {

                        string discipleSql = string.Format($@"INSERT INTO [Disciple]([First Name],[Last Name], [Date], [Mentor], [Department])
                                          VALUES ('{fname}','{lname}','{lbl_Date.Text}','{mentor}','{dept}')");
                        OleDbCommand cmd = new OleDbCommand(discipleSql, conn);
                        cmd.ExecuteNonQuery();
                        string getDiscipleID = "SELECT MAX([Disciple].[DiscipleID]) FROM [Disciple] ";
                        cmd = new OleDbCommand(getDiscipleID, conn);
                        int DiscipleID = Convert.ToInt32(cmd.ExecuteScalar());

                        string questionSql = string.Format($@"INSERT INTO Question([Name], [Month], [Date],[Believer],[Meets at least once a week],
                                                        [Uses Life Agape Materials], [Have Met at least 8 times in the past 3 months],[Has First Disciple],
                                                        [First Disciple shares],[Has Second Disciple], [Second Disciple Shares], [DiscipleID] )
                                            VALUES ('{fname}', '{cmb_Month.Text}', '{lbl_Date.Text}', {que_1.Checked},
                                                    {que_2.Checked}, {que_3.Checked}, {que_4.Checked}, {que_5.Checked},
                                                    {que_6.Checked}, {que_7.Checked}, {que_8.Checked}, {DiscipleID})");
                        cmd = new OleDbCommand(questionSql, conn);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Record Succefully Created", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        main_Panel.Enabled = false;
                        show_Data();
                        return;
                    }                  
                     catch (Exception ex)
                    {
                        error_msg = ex.Message;
                        MessageBox.Show(error_msg);

                    }
                    finally
                    {
                        ClearData();
                    }
                }
            }
            #region Do_Update
            else
            {
                try
                { 
                string discipleUpdateSql = string.Format($@"UPDATE [Disciple] SET [First Name] = '{fname}',[Last Name] = '{lname}', [Mentor] = '{mentor}', [Department] = '{dept}'
                                    WHERE Disciple.DiscipleID = {hiddenLabel.Text}");
                OleDbCommand cmd = new OleDbCommand(discipleUpdateSql, conn);
                cmd.ExecuteNonQuery(); 
                string questionUpdateSql = string.Format($@"UPDATE Question SET [Name] ='{fname}' , [Month] = '{cmb_Month.Text}', [Date] ='{lbl_Date.Text}'
                                                ,[Believer] = {que_1.Checked},[Meets at least once a week] = {que_2.Checked},
                                                [Uses Life Agape Materials] = {que_3.Checked}, [Have Met at least 8 times in the past 3 months] = {que_4.Checked},[Has First Disciple] = {que_5.Checked},
                                                [First Disciple shares] = {que_6.Checked},[Has Second Disciple] = {que_7.Checked}, [Second Disciple Shares] ={que_8.Checked}, [DiscipleID] ={hiddenLabel.Text}
                                            Where Question.DiscipleID = {hiddenLabel.Text} ");                                           
                cmd = new OleDbCommand(questionUpdateSql, conn);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Record Successfully Updated", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                main_Panel.Enabled = false;
                show_Data();
                return;
                        
                    }
                
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    ClearData();
                }
            }
            #endregion

        }
        #endregion
        
        //Cancel
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            ClearData();
            main_Panel.Enabled = false;

        }
        #region Show Data
        private void show_Data()
        {
            error_msg = "";
            q = "SELECT Disciple.[First Name],"+ 
                " Disciple.[Last Name], Disciple.Mentor,"+
                " Disciple.Department, Question.Month, Question.Believer,"+
                " Question.[Meets at least once a week], Question.[Uses Life Agape Materials],"+
                " Question.[Have Met at least 8 times in the past 3 months], Question.[Has First Disciple],"+
                " Question.[First Disciple shares],Question.[Has Second Disciple], Question.[Second Disciple Shares], Disciple.[DiscipleID] " +
                "FROM Disciple INNER JOIN Question ON Disciple.[DiscipleID] = Question.[DiscipleID] ORDER BY Disciple.[Date] Desc; ";
            try
            {
                OleDbCommand cmd = new OleDbCommand(q, conn);
                OleDbDataAdapter a = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();                
                a.SelectCommand = cmd;
                a.Fill(dt);
                lblCount.Text = "Row Count: "+ dt.Rows.Count;
                results.DataSource = dt;
              
                results.AutoResizeColumns();
                
            }
            catch (Exception ex)
            {
                error_msg = ex.Message;
                MessageBox.Show(error_msg);

            }      

        }
        #endregion
        #region Do Search
        //Search
        private void txt_Search_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)13)
                {
                    if (string.IsNullOrEmpty(txt_Search.Text))
                    {
                        show_Data();
                    }
                    else
                    {
                        string word = txt_Search.Text;
                        string query = string.Format($@"SELECT Disciple.[First Name],
                 Disciple.[Last Name], Disciple.Mentor,
                Disciple.Department, Question.Month, Question.Believer,
                Question.[Meets at least once a week], Question.[Uses Life Agape Materials],
                Question.[Have Met at least 8 times in the past 3 months], Question.[Has First Disciple],
                 Question.[First Disciple shares],Question.[Has Second Disciple], Question.[Second Disciple Shares], Disciple.[DiscipleID] 
                FROM Disciple INNER JOIN Question ON Disciple.[DiscipleID] = Question.[DiscipleID]
                        WHERE [First Name] Like '{word}%' or [Last Name] Like '{ word }%' or [Department] Like '{ word }%' or [Mentor] Like '{ word}%' or Question.Month Like '{word}%'");
                        OleDbCommand cmd = new OleDbCommand(query, conn);
                        OleDbDataAdapter a = new OleDbDataAdapter(cmd);
                        cmd.ExecuteNonQuery();
                        DataTable dt = new DataTable();
                        a.SelectCommand = cmd;
                        a.Fill(dt);
                        results.DataSource = dt;
                        results.AutoResizeColumns();
                        //var query = from o in this.show_Data()
                        //            where o.[First Name].Contains(txt_Search) || o.[Last Name].Contains(txt_Search) || o.Department == txt_Search || o.Mentor == txt_Search
                        //            select o;
                        //results = query.ToList();
                        lblCount.Text = "Row Count: " + dt.Rows.Count;
                    }
                }
               

            }
            catch (Exception ex)
            {

                MessageBox.Show("Error  " + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
#endregion 

        #region Clear Data
        //Clear Data
        private void ClearData()
        {
            txt_First_Name.Clear();
            txt_Last_Name.Clear();
            txt_Mentor.Clear();
            cmb_Dept.ResetText();
            cmb_Month.ResetText();
            que_1.Checked = false;
            que_2.Checked = false;
            que_3.Checked = false;
            que_4.Checked = false;
            que_5.Checked = false;
            que_6.Checked = false;
            que_7.Checked = false;
            que_8.Checked = false;
            hiddenLabel.ResetText();
        }
        #endregion
        #region Set up Data for Edit
        private void results_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            main_Panel.Enabled = true;            
            txt_First_Name.Text = results.SelectedRows[0].Cells[0].Value.ToString();
            txt_Last_Name.Text = results.SelectedRows[0].Cells[1].Value.ToString();
            txt_Mentor.Text = results.SelectedRows[0].Cells[2].Value.ToString();
            cmb_Dept.Text = results.SelectedRows[0].Cells[3].Value.ToString();
            cmb_Month.Text = results.SelectedRows[0].Cells[4].Value.ToString();
            cmb_Month.Enabled = false;            
            if (results.SelectedRows[0].Cells[5].Value.ToString() == "True")
            {
                que_1.Checked = true;
            }
            else
            {
                que_1.Checked = false;
            }
            if (results.SelectedRows[0].Cells[6].Value.ToString() == "True")
            {
                que_2.Checked = true;
            }
            else
            {
                que_2.Checked = false;
            }
            if (results.SelectedRows[0].Cells[7].Value.ToString() == "True")
            {
                que_3.Checked = true;
            }
            else
            {
                que_3.Checked = false;
            }
            if (results.SelectedRows[0].Cells[8].Value.ToString() == "True")
            {
                que_4.Checked = true;
            }
            else
            {
                que_4.Checked = false;
            }
            if (results.SelectedRows[0].Cells[9].Value.ToString() == "True")
            {
                que_5.Checked = true;
            }
            else
            {
                que_5.Checked = false;
            }
            if (results.SelectedRows[0].Cells[10].Value.ToString() == "True")
            {
                que_6.Checked = true;
            }
            else
            {
                que_6.Checked = false;
            }
            if (results.SelectedRows[0].Cells[11].Value.ToString() == "True")
            {
                que_7.Checked = true;
            }
            else
            {
                que_7.Checked = false;
            }
            if (results.SelectedRows[0].Cells[12].Value.ToString() == "True")
            {
                que_8.Checked = true;
            }
            else
            {
                que_8.Checked = false;
            }
           hiddenLabel.Text =  results.SelectedRows[0].Cells[13].Value.ToString();
            btn_Save.Text = "Update";            
        }
        #endregion

        private void btn_Delete_Click(object sender, EventArgs e)
        {
           try
                {
                    MessageBox.Show($"Do you want to delete the Record of {txt_First_Name.Text}", " Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    string queryData = string.Format($@"Delete from Question where discipleID = {hiddenLabel.Text} ");
                    string queryData2 = string.Format($@"Delete from Disciple where discipleID = {hiddenLabel.Text}  ");
                    OleDbCommand cmd = new OleDbCommand(queryData, conn);
                    cmd.ExecuteNonQuery();
                    OleDbCommand cmd2 = new OleDbCommand(queryData2, conn);
                    cmd2.ExecuteNonQuery();
                    ClearData();
                    show_Data();
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message +" No Disciple is Selected", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }                      
        }
        private bool isDisciplePresent()
        {
            string queryData = string.Format($@"Select 1 from Disciple 
                                                where [First Name] like '{txt_First_Name.Text.Trim()}'
                                                and [Last Name] like {txt_Last_Name.Text.Trim()} 
                                                and [Mentor] like {txt_Mentor.Text.Trim()}");
            OleDbDataAdapter cmd = new OleDbDataAdapter(queryData, conn);           
            return true;
        }
    }
}
