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

namespace Saper.MVVM.Model.DataBase
{

    public class DatabaseSaper
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
                    AddValueToString(cmd, new List<string> { "@Name" }, new List<object> { name });

                    DataTable dataFromDatabase = new DataTable();
                    using (SqlDataAdapter DataAdapter = new SqlDataAdapter(cmd))
                    {
                        DataAdapter.Fill(dataFromDatabase);
                    }

                    if (dataFromDatabase.Rows.Count > 0 && !statNewUser)
                    {
                        string sql = "SELECT Password FROM UserSaper WHERE Name = @Name";
                        using (SqlCommand cmd2 = new SqlCommand(sql, conn))
                        {
                            AddValueToString(cmd2, new List<string> { "@Name" }, new List<object> { name });
                            var result = cmd2.ExecuteScalar();

                            if (result != null)
                            {
                                string hashedPassword = result.ToString();
                                if (BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword))
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
                    else if (dataFromDatabase.Rows.Count == 0 && statNewUser)
                    {
                        string hashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 13);
                        string insertQuery = "INSERT INTO UserSaper (Name, Password) VALUES (@Name, @Password)";

                        using (SqlCommand cmd3 = new SqlCommand(insertQuery, conn))
                        {
                            AddValueToString(cmd3, new List<string> { "@Name", "@Password" }, new List<object> { name, hashPassword });
                            cmd3.ExecuteNonQuery();
                        }
                        return true;
                    }
                    else
                    {
                        MessageBox.Show(dataFromDatabase.Rows.Count == 0 && !statNewUser ? "User doesn't exist!" : "User exists!");
                        return false;
                    }
                }
            }
        }

        public static UserRecord InfUser(UserInfo userInfo)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string sql = "SELECT Time, Streak FROM Results WHERE Name = @Name AND Level = @Level";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    AddValueToString(cmd, new List<string> { "@Name", "@Level" }, new List<object> { userInfo.Name, userInfo.Level });

                    DataTable dataFromDatabase = new DataTable();
                    using (SqlDataAdapter DataAdapter = new SqlDataAdapter(cmd))
                    {
                        DataAdapter.Fill(dataFromDatabase);
                    }

                    UserRecord userRecord = new UserRecord { Name = userInfo.Name, Level = userInfo.Level };

                    if (dataFromDatabase.Rows.Count == 0)
                    {
                        userRecord.Streak = 0;
                        userRecord.BestTime = new TimeSpan(0);

                        string sql1 = "SELECT Id FROM UserSaper WHERE Name = @Name";
                        using (SqlCommand cmd2 = new SqlCommand(sql1, conn))
                        {
                            AddValueToString(cmd2, new List<string> { "@Name" }, new List<object> { userRecord.Name });
                            object result = cmd2.ExecuteScalar();

                            if (result != null)
                            {
                                int userId = Convert.ToInt32(result);
                                string sql2 = "INSERT INTO Results (Name, Time, Level, Streak, IdUser) VALUES (@Name, @Time, @Level, @Streak, @IdUser)";

                                using (SqlCommand cmd3 = new SqlCommand(sql2, conn))
                                {
                                    AddValueToString(cmd3, new List<string> { "@Name", "@Time", "@Level", "@Streak", "@IdUser" },
                                        new List<object> { userRecord.Name, userRecord.BestTime, userRecord.Level, userRecord.Streak, userId });
                                    cmd3.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    else
                    {
                        userRecord.Streak = Convert.ToInt16(dataFromDatabase.Rows[0]["Streak"]);
                        userRecord.BestTime = TimeSpan.Parse(dataFromDatabase.Rows[0]["Time"].ToString());
                    }
                    return userRecord;
                }
            }
        }

        public static void UpdateResults(UserRecord userRecord)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string sql = "UPDATE Results SET Streak = @Streak, Time = @Time WHERE Name = @Name AND Level = @Level";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    AddValueToString(cmd, new List<string> { "@Time", "@Streak", "@Name", "@Level" },
                        new List<object> { userRecord.BestTime, userRecord.Streak, userRecord.Name, userRecord.Level });
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
                    AddValueToString(cmd, new List<string> { "@Level" }, new List<object> { level });
                    using (SqlDataAdapter DataAdapter = new SqlDataAdapter(cmd))
                    {
                        DataAdapter.Fill(dataFromDatabase);
                    }
                }
                return dataFromDatabase;
            }
        }

        private static void AddValueToString(SqlCommand cmd, List<string> paramNames, List<object> paramValues)
        {
            for (int i = 0; i < paramNames.Count; i++)
            {
                cmd.Parameters.AddWithValue(paramNames[i], paramValues[i]);
            }
        }
    }


}
