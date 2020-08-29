using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace AgapeApp
{
    public partial class MainForm : Form
    {
       
        Connection con = new Connection();

        public MainForm()
        {
            InitializeComponent();
            DateTime dt = DateTime.Now;
            lbl_Date.Text = dt.ToShortDateString();
            lbl_Date.Show();
            hidden_q_id.Text = null;
            try
            {
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
            con.ActiveCon().Close();
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
            if (main_Panel.Enabled == false)
            {
                MessageBox.Show("Please, click on the New button to add new Disciple", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(txt_First_Name.Text))
            {
                MessageBox.Show("First Name cannot be empty ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(txt_Last_Name.Text))
            {
                MessageBox.Show("Last Name cannot be empty ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(txt_Mentor.Text))
            {
                MessageBox.Show("Mentor Name cannot be empty ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(cmb_Dept.Text))
            {
                MessageBox.Show("Department cannot be empty ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string fname ;
            string lname ;
            string mentor ;
            string dept ;            
            fname = txt_First_Name.Text.ToUpperInvariant().Trim();
            lname = txt_Last_Name.Text.ToUpperInvariant().Trim();
            mentor = txt_Mentor.Text.ToUpperInvariant().Trim();
            dept = cmb_Dept.Text.ToUpperInvariant().Trim();
            if (isDisciplePresent())
            {
                if (isSameMonth())
                {
                    //MessageBox.Show("You can add to existing Record or update disciple record", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //return;
                    string discipleUpdateSql = string.Format($@"UPDATE Disciple SET First_Name = '{fname}',Last_Name = '{lname}', Mentor = '{mentor}', Department = '{dept}'
                                    WHERE DiscipleID = {hiddenLabel.Text}");
                    MySqlCommand cmd = new MySqlCommand(discipleUpdateSql, con.ActiveCon());
                    cmd.ExecuteNonQuery();
                    string questionUpdateSql = string.Format($@"UPDATE Question SET Name ='{fname}' , Date ='{lbl_Date.Text}', Month = '{cmb_Month.Text}'
                                                ,Believer = {que_1.Checked},Meets_at_least_once_a_week = {que_2.Checked},
                                                Uses_Life_Agape_Materials = {que_3.Checked}, Have_Met_a_least_8_times_in_the_past_3_months = {que_4.Checked},Has_First_Disciple = {que_5.Checked},
                                                First_Disciple_shares = {que_6.Checked},Has_Second_Disciple = {que_7.Checked}, Second_Disciple_Shares ={que_8.Checked}, DiscipleID ={hiddenLabel.Text}
                                            Where Question.QuestionID = {hidden_q_id.Text} ");
                    cmd = new MySqlCommand(questionUpdateSql, con.ActiveCon());
                    cmd.ExecuteNonQuery();
                    MessageBox.Show($"{fname} Record Successfully Updated", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    main_Panel.Enabled = false;
                    ClearData();
                    show_Data();
                    return;
                }
                else if(!isSameMonth())
                {
                    // MessageBox.Show("insert record into the Question Table", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    string questionInsertSql = string.Format($@"INSERT INTO Question(Name, Month, Date,Believer,Meets_at_least_once_a_week,
                                                            Uses_Life_Agape_Materials, Have_Met_a_least_8_times_in_the_past_3_months,Has_First_Disciple,
                                                            First_Disciple_shares,Has_Second_Disciple, Second_Disciple_Shares, DiscipleID )
                                                VALUES ('{fname}', '{cmb_Month.Text}', '{lbl_Date.Text}', {que_1.Checked},
                                                        {que_2.Checked}, {que_3.Checked}, {que_4.Checked}, {que_5.Checked},
                                                        {que_6.Checked}, {que_7.Checked}, {que_8.Checked}, {hiddenLabel.Text})");
                    MySqlCommand cmd = new MySqlCommand(questionInsertSql, con.ActiveCon());
                    cmd.ExecuteNonQuery();
                    MessageBox.Show($"Question Record for {fname} Successfully Updated", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearData();
                    show_Data();
                    return;
                }
            }
            else
            {       
                // MessageBox.Show("Insert new record", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string discipleSql = string.Format($@"INSERT INTO Disciple(First Name,Last Name, Date, Mentor, Department)
                                          VALUES ('{fname}','{lname}',CUR_DATE(),'{mentor}','{dept}')");
                MySqlCommand cmd = new MySqlCommand(discipleSql, con.ActiveCon());
                cmd.ExecuteNonQuery();
                string getDiscipleID = "SELECT MAX(Disciple.DiscipleID) FROM Disciple ";
                cmd = new MySqlCommand(getDiscipleID, con.ActiveCon());
                int DiscipleID = Convert.ToInt32(cmd.ExecuteScalar());

                string questionSql = string.Format($@"INSERT INTO Question(Name, Month, Date,Believer,Meets_at_least_once_a_week,
                                                        Uses_Life_Agape_Materials, Have_Met_a_least_8_times_in_the_past_3_months,Has_First_Disciple,
                                                        First_Disciple_shares,Has_Second_Disciple, Second_Disciple_Shares, DiscipleID )
                                            VALUES ('{fname}', '{cmb_Month.Text}', '{lbl_Date.Text}', {que_1.Checked},
                                                    {que_2.Checked}, {que_3.Checked}, {que_4.Checked}, {que_5.Checked},
                                                    {que_6.Checked}, {que_7.Checked}, {que_8.Checked}, {DiscipleID})");
                cmd = new MySqlCommand(questionSql, con.ActiveCon());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Record Successfully Created", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                main_Panel.Enabled = false;
                ClearData();
                show_Data();
                return;
            }

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
            string error_msg = "";
           string q = @"SELECT Disciple.First Name, 
                     Last Name, Mentor, Department, Question.Month, Question.Believer,
                     Meets_at_least_once_a_week, Uses_Life_Agape_Materials,
                     Have_Met_a_least_8_times_in_the_past_3_months, Has_First_Disciple,
                     First_Disciple_shares, Has_Second_Disciple, Second_Disciple_Shares,  Disciple.DiscipleID, Question.QuestionID 
                    FROM   Disciple
                     INNER JOIN
                     Question ON Disciple.DiscipleID = Question.DiscipleID
                    ORDER BY Disciple.Date DESC; ";
            try
            {
                MySqlCommand cmd = new MySqlCommand(q, con.ActiveCon());
                MySqlDataAdapter a = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                a.SelectCommand = cmd;
                a.Fill(dt);
                //foreach (DataRow item in dt.Rows)
                //{
                //    int n = results.Rows.Add();
                //    results.Rows[n].Cells[0].Value = item["First Name"].ToString();

                //}
                lblCount.Text = "Row Count: " + dt.Rows.Count;                
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
                        string query = string.Format($@"SELECT D.First Name,
                 D.Last Name, D.Mentor,
                D.Department, Q.Month, Q.Believer,
                Q.Meets_at_least_once_a_week, Q.Uses_Life_Agape_Materials,
                Q.Have_Met_a_least_8_times_in_the_past_3_months, Q.Has_First_Disciple,
                 Q.First_Disciple_shares,Q.Has_Second_Disciple, Q.Second_Disciple_Shares, D.DiscipleID, Q.QuestionID
                FROM Disciple D INNER JOIN Question Q ON D.DiscipleID = Q.DiscipleID
                        WHERE First Name Like '{word}%' or Last Name Like '{ word }%' or Department Like '{ word }%' or Mentor Like '{ word}%' or Q.Month Like '{word}%'");
                        MySqlCommand cmd = new MySqlCommand(query, con.ActiveCon());
                        MySqlDataAdapter a = new MySqlDataAdapter(cmd);
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
            hidden_q_id.ResetText();
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
            //cmb_Month.Enabled = false;
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
            hiddenLabel.Text = results.SelectedRows[0].Cells[13].Value.ToString();
            hidden_q_id.Text = results.SelectedRows[0].Cells[14].Value.ToString();

        }
        #endregion

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogresult = MessageBox.Show($"Do you want to delete the Record of {txt_First_Name.Text}", " Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogresult == DialogResult.Yes)
                {
                    string queryData = string.Format($@"Delete from Question where QuestionID = {hidden_q_id.Text} ");
                    MySqlCommand cmd = new MySqlCommand(queryData, con.ActiveCon());
                    cmd.ExecuteNonQuery();
                    //TO DO: CHECK TO ENSURE THAT THERE ARE NO QUESTIONS REMAINING FOT THE DISCIPLE
                    string dataCheck = string.Format($@"Select * from question where discipleID = {hiddenLabel.Text}");
                    MySqlCommand cmd3 = new MySqlCommand(dataCheck, con.ActiveCon());
                    MySqlDataAdapter ad = new MySqlDataAdapter(cmd3);
                    DataTable dt = new DataTable();
                    ad.SelectCommand = cmd3;
                    ad.Fill(dt);
                    if (dt.Rows.Count == 0)
                    {
                        string queryData2 = string.Format($@"Delete from Disciple where discipleID = {hiddenLabel.Text}  ");
                        MySqlCommand cmd2 = new MySqlCommand(queryData2, con.ActiveCon());
                        cmd2.ExecuteNonQuery();                       
                    }
                    ClearData();
                    this.Cursor = Cursors.WaitCursor;
                    show_Data();
                    this.Cursor = Cursors.Default;
                    return;
                }
               
            }
            catch (Exception)
            {
                MessageBox.Show(" No Disciple is Selected", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        public bool isDisciplePresent()
        {
            try
            {
                string queryDisciple = string.Format($@"Select First_Name, Last_Name from Disciple 
                                                where First_Name = '{txt_First_Name.Text.Trim()}'
                                                and Last_Name = '{txt_Last_Name.Text.Trim()}'
                                                and Mentor = '{txt_Mentor.Text.Trim()}'");
                MySqlDataAdapter a = new MySqlDataAdapter(queryDisciple, con.ActiveCon());
                DataTable dt = new DataTable();
                a.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    return true;
                }
                //else
                //{
                //    return false;
                //}

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
            
        }

        public bool isSameMonth()
        {
            string queryQuestion = string.Format($@"Select Month FROM Question 
                                               WHERE QuestionID = {hidden_q_id.Text}");
            MySqlCommand cmd = new MySqlCommand(queryQuestion, con.ActiveCon());
            MySqlDataAdapter a = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            a.SelectCommand = cmd;
            a.Fill(dt);
            if ((string)dt.Rows[0].ItemArray[0] == cmb_Month.Text)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }//End Form1

}//End Namespace
