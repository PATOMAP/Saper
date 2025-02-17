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
                AddValueToString(new List<string> { "@Name" }, new List<object> { name });
                _cmd.CommandType = CommandType.Text;

                DataTable dataFromDatabase = new DataTable();
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(dataFromDatabase);

                if (dataFromDatabase.Rows.Count > 0 && statNewUser==false)
                {
                        string sql = "SELECT Password FROM UserSaper WHERE Name = @Name";
                        _cmd = new SqlCommand(sql, _sqlConnection);
                        AddValueToString(new List<string> { "@Name" },new List<object> {name});
                        _cmd.CommandType = CommandType.Text;
                        var result = _cmd.ExecuteScalar();

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
                else if (dataFromDatabase.Rows.Count==0 && statNewUser == true)
                {
                    string hashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);
                    string insertQuery = "INSERT INTO UserSaper (Name, Password) VALUES (@Name, @Password)";
                    _cmd = new SqlCommand(insertQuery, _sqlConnection);
                    AddValueToString(new List<string> { "@Name", "@Password" }, new List<object> { name, hashPassword });
                    _cmd.CommandType = CommandType.Text;
                    _cmd.ExecuteNonQuery();
                    return true;
                    
      

                }
                else
                {
                    if(dataFromDatabase.Rows.Count == 0 && statNewUser == false)
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
                _cmd = new SqlCommand(sql, _sqlConnection);
                
                DataTable dataFromDatabase = new DataTable();
                AddValueToString(new List<string> { "@Name", "@Level" }, new List<object> { user.Name, user.Level });
                _cmd.CommandType = CommandType.Text;

                _da = new SqlDataAdapter(_cmd);

                _da.Fill(dataFromDatabase);
                UserRecord usr = new UserRecord();
                usr.Name = user.Name;
                usr.Level = user.Level;
                if (dataFromDatabase.DefaultView.Count == 0)
                {                       
                    usr.Streak = 0;
                    usr.BestTime=new TimeSpan(0);
                    int userId=0;
                    string sql1= "SELECT Id FROM UserSaper WHERE Name = @Name";
                    string sql2 = "INSERT INTO Results (Name,Time,Level,Streak,IdUser) Values (@Name,@Time,@Level,@Streak,@IdUser)";
                        _cmd = new SqlCommand(sql1, _sqlConnection);
                    AddValueToString(new List<string> { "@Name" }, new List<object> { usr.Name});
                        object result = _cmd.ExecuteScalar(); 

                        if (result != null)
                            userId = Convert.ToInt32(result);

                    _cmd = new SqlCommand(sql2, _sqlConnection);
                    AddValueToString(new List<string> { "@Name", "@Time","@Level", "@Streak","@IdUser" }, new List<object> { usr.Name, usr.BestTime, usr.Level, usr.Streak, userId });
                    _cmd.ExecuteNonQuery(); 

                }
                else
                {

                    usr.Streak = Convert.ToInt16(dataFromDatabase.Rows[0]["Streak"]);
                    usr.BestTime = TimeSpan.Parse(dataFromDatabase.Rows[0]["Time"].ToString());
                }
                return usr;


                
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
                _cmd = new SqlCommand(sql, _sqlConnection);
                AddValueToString(new List<string> { "@Time", "@Streak", "@Name","@Level"}, new List<object> { user.BestTime, user.Streak, user.Name,  user.Level});
                _cmd.CommandType = CommandType.Text;
                _cmd.ExecuteNonQuery();

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
                DataTable dataFromDatabase = new DataTable();
                string sql;
                if (tab == 0)
                    sql = "SELECT Name,Time FROM Results Where Level=@Level";
                else
                    sql = "SELECT Name,Streak FROM Results Where Level=@Level";

                _cmd = new SqlCommand(sql, _sqlConnection);
                AddValueToString(new List<string> {  "@Level" }, new List<object> { level });
                _cmd.CommandType = CommandType.Text;          
                _da = new SqlDataAdapter(_cmd);
                _da.Fill(dataFromDatabase);

                return dataFromDatabase;
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

        private void AddValueToString(List<string>listName,List<object>listObject)
        {
            for (int i=0;i<listName.Count;i++)
            {
                _cmd.Parameters.AddWithValue(listName[i], listObject[i]);
            }

        }
    }
}
