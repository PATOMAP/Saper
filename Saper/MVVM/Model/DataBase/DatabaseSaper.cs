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
        public static SqlConnection GetConnection()
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            return new SqlConnection(connString);
        }

        public static bool CheckName(string name, string password, bool statNewUser)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string selectQuery = "SELECT * FROM UserSaper WHERE Name = @Name";

                using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);

                    DataTable dataFromDatabase = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataFromDatabase);
                    }

                    if (dataFromDatabase.Rows.Count > 0 && statNewUser == false)
                    {
                        string sql = "SELECT Password FROM UserSaper WHERE Name = @Name";
                        using (SqlCommand cmd2 = new SqlCommand(sql, conn))
                        {
                            cmd2.Parameters.AddWithValue("@Name", name);
                            var result = cmd2.ExecuteScalar();

                            if (result != null)
                            {
                                string hashedPassword = result.ToString();
                                bool isValidPassword = BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);

                                if (isValidPassword)
                                    return true;
                                else
                                {
                                    MessageBox.Show("Wrong password!");
                                    return false;
                                }
                            }
                            return false;
                        }
                    }
                    else if (dataFromDatabase.Rows.Count == 0 && statNewUser == true)
                    {
                        string hashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);
                        string insertQuery = "INSERT INTO UserSaper (Name, Password) VALUES (@Name, @Password)";

                        using (SqlCommand cmd3 = new SqlCommand(insertQuery, conn))
                        {
                            cmd3.Parameters.AddWithValue("@Name", name);
                            cmd3.Parameters.AddWithValue("@Password", hashPassword);
                            cmd3.ExecuteNonQuery();
                        }
                        return true;
                    }
                    else
                    {
                        MessageBox.Show(dataFromDatabase.Rows.Count == 0 && statNewUser == false ? "User doesn't exist!" : "User exists!");
                        return false;
                    }
                }
            }
        }

        public static UserRecord InfUser(UserInfo user)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT Time, Streak FROM Results WHERE Name = @Name AND Level = @Level";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Level", user.Level);

                    DataTable dataFromDatabase = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataFromDatabase);
                    }

                    UserRecord usr = new UserRecord { Name = user.Name, Level = user.Level };

                    if (dataFromDatabase.Rows.Count == 0)
                    {
                        usr.Streak = 0;
                        usr.BestTime = new TimeSpan(0);

                        string sql1 = "SELECT Id FROM UserSaper WHERE Name = @Name";
                        using (SqlCommand cmd2 = new SqlCommand(sql1, conn))
                        {
                            cmd2.Parameters.AddWithValue("@Name", usr.Name);
                            object result = cmd2.ExecuteScalar();

                            if (result != null)
                            {
                                int userId = Convert.ToInt32(result);
                                string sql2 = "INSERT INTO Results (Name, Time, Level, Streak, IdUser) VALUES (@Name, @Time, @Level, @Streak, @IdUser)";

                                using (SqlCommand cmd3 = new SqlCommand(sql2, conn))
                                {
                                    cmd3.Parameters.AddWithValue("@Name", usr.Name);
                                    cmd3.Parameters.AddWithValue("@Time", usr.BestTime);
                                    cmd3.Parameters.AddWithValue("@Level", usr.Level);
                                    cmd3.Parameters.AddWithValue("@Streak", usr.Streak);
                                    cmd3.Parameters.AddWithValue("@IdUser", userId);
                                    cmd3.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    else
                    {
                        usr.Streak = Convert.ToInt16(dataFromDatabase.Rows[0]["Streak"]);
                        usr.BestTime = TimeSpan.Parse(dataFromDatabase.Rows[0]["Time"].ToString());
                    }
                    return usr;
                }
            }
        }

        public static void UpdateResults(UserRecord user)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string sql = "UPDATE Results SET Streak = @Streak, Time = @Time WHERE Name = @Name AND Level = @Level";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Time", user.BestTime);
                    cmd.Parameters.AddWithValue("@Streak", user.Streak);
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Level", user.Level);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable DisplayResults(int level, int tab)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                DataTable dataFromDatabase = new DataTable();
                string sql = tab == 0 ? "SELECT Name, Time FROM Results WHERE Level = @Level" : "SELECT Name, Streak FROM Results WHERE Level = @Level";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Level", level);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dataFromDatabase);
                    }
                }
                return dataFromDatabase;
            }
        }
    }

}
