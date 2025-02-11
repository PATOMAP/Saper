using Saper.Core;
using Saper.MVVM.Model;
using Saper.MVVM.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Drawing;

namespace Saper.MVVM.Model
{

    class DatabaseSaper
    {
        private SqlConnection _sqlConnection;
        private SqlCommand _cmd = new SqlCommand();
        private SqlDataAdapter _da = new SqlDataAdapter();

        public DatabaseSaper()
        {

        }

        public void mycon()
        {
            String Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            _sqlConnection = new SqlConnection(Conn);
            _sqlConnection.Open(); 
        }

        public bool CheckName(string name, string password, bool statNewUser)
        {
            try
            {
               
                mycon();
                

                
                string selectQuery = "SELECT * FROM UserSaper WHERE Name = @Name";
                _cmd = new SqlCommand(selectQuery, _sqlConnection);
                _cmd.Parameters.AddWithValue("@Name", name);
                _cmd.CommandType = CommandType.Text;

                DataTable dt = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(dt);

                if (dt.Rows.Count > 0 && statNewUser==false)
                {
                    string sql = "SELECT Password FROM UserSaper WHERE Name = @Name";
                    using (SqlCommand cmd = new SqlCommand(sql, _sqlConnection))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.CommandType = CommandType.Text;
                        var result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            string hashedPassword = result.ToString();

                            
                            bool isValidPassword = BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);

                            if (isValidPassword)
                            {
                                return true;
                            }
                            else
                            {
                                MessageBox.Show("Wrong password!");
                                return false;
                            }
                        }
                    }

                }
                else if (dt.Rows.Count==0 && statNewUser == true)
                {
                    string hashpassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);
                    string insertQuery = "INSERT INTO UserSaper (Name, Password) VALUES (@Name, @Password)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, _sqlConnection))
                    {
                        cmd.Parameters.AddWithValue("@Name", name);
                        cmd.Parameters.AddWithValue("@Password", hashpassword);
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        return true;
                    }
      

                }
                else
                {
                    if(dt.Rows.Count == 0 && statNewUser == false)
                    MessageBox.Show("User doesn't exist!");
                    else
                    MessageBox.Show("User exist!");

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd: " + ex.Message);
                return false;
            }
            finally
            {
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    _sqlConnection.Close();
                }
            }
        }
        public UserRecord InfUser(UserInfo user)
        {
            try
            {
                mycon();
                string sql = "SELECT Time,Streak FROM Results WHERE Name = @Name AND Level=@Level";
                using (SqlCommand cmd = new SqlCommand(sql, _sqlConnection))
                {
                    DataTable dt = new DataTable();

                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Level", user.Level);
                    cmd.CommandType = CommandType.Text;

                    _da = new SqlDataAdapter(cmd);

                    _da.Fill(dt);
                    UserRecord usr = new UserRecord();
                    usr.Name = user.Name;
                    usr.Level = user.Level;
                    if (dt.DefaultView.Count == 0)
                    {                       
                        usr.Streak = 0;
                        usr.BestTime=new TimeSpan(0);
                        int userId=0;
                        string sql1= "SELECT Id FROM UserSaper WHERE Name = @Name";
                        string sql2 = "INSERT INTO Results (Name,Time,Level,Streak,IdUser) Values (@Name,@Time,@Level,@Streak,@IdUser)";
                        using (SqlCommand cmd1 = new SqlCommand(sql1, _sqlConnection))
                        {
                            cmd1.Parameters.AddWithValue("@Name", usr.Name);
                            object result = cmd1.ExecuteScalar(); 

                            if (result != null)
                                userId = Convert.ToInt32(result);

                        }
                        using (SqlCommand cmd2 = new SqlCommand(sql2, _sqlConnection))
                        {
                            cmd2.Parameters.AddWithValue("@Name", usr.Name);
                            cmd2.Parameters.AddWithValue("@Time", usr.BestTime);
                            cmd2.Parameters.AddWithValue("@Level", usr.Level);
                            cmd2.Parameters.AddWithValue("@Streak", usr.Streak);
                            cmd2.Parameters.AddWithValue("@IdUser", userId);
                            cmd2.ExecuteNonQuery(); 



                        }

                    }
                    else
                    {

                        usr.Streak = Convert.ToInt16(dt.Rows[0]["Streak"]);
                        usr.BestTime = TimeSpan.Parse(dt.Rows[0]["Time"].ToString());
                    }
                    return usr;


                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    _sqlConnection.Close();
                }
            }


            return null;
        }

        public void UpdateResults(UserRecord user)
        {
            try
            {
                mycon();
                string sql = "Update Results Set Streak=@Streak,Time=@Time WHERE Name = @Name AND Level=@Level";
                using (SqlCommand cmd = new SqlCommand(sql, _sqlConnection))
                {

                    cmd.Parameters.AddWithValue("@Time", user.BestTime);
                    cmd.Parameters.AddWithValue("@Streak", user.Streak);
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Level", user.Level);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    _sqlConnection.Close();
                }
            }
        }
        public DataTable DisplayResults(int level,int tab)
        {
            try
            {
                mycon();
                DataTable dt = new DataTable();
                string sql;
                if (tab == 0)
                    sql = "SELECT Name,Time FROM Results Where Level=@Level";
                else
                    sql = "SELECT Name,Streak FROM Results Where Level=@Level";

                _cmd = new SqlCommand(sql, _sqlConnection);
                _cmd.Parameters.AddWithValue("@Level", level);
                _cmd.CommandType = CommandType.Text;          
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(dt);

                return dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            finally
            {
                if (_sqlConnection.State == ConnectionState.Open)
                {
                    _sqlConnection.Close();
                }
            }
        }
    }
}
