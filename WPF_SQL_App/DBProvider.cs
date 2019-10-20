using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using WPF_SQL_App.Models;
using System.IO;

namespace WPF_SQL_App
{
    class DBProvider : IDBProvider
    {
        //TODO: this settings must be stored on environment or CI files

        private readonly string rootLocation;
        private const string _dbName = "single_pass_db.db";

        public string SQLiteDBPath => Path.Combine(rootLocation, _dbName);
        public string SQLiteConnectionString => $"Data Source={SQLiteDBPath}; Version=3;";
        public DBProvider()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            rootLocation = Path.GetDirectoryName(assemblyLocation);

        }

        public async Task<UserModel> Login(string username, string password)
        {
            using (var connection = new SQLiteConnection(SQLiteConnectionString))
            {
                try
                {
                    connection.Open();

                    var query = "SELECT * FROM user WHERE username = @username AND password = @password";
                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserModel
                            {
                                Id = (long)reader["id"],
                                Username = (string)reader["username"],
                                Password = (string)reader["password"]
                            };
                        }
                    }


                }
                finally
                {

                    connection.Close();
                }

            }

            return null;
        }


        public IEnumerable<BoxModel> GetAllBoxes(long userId)
        {
            IList<BoxModel> results = new List<BoxModel>();

            using (var connection = new SQLiteConnection(SQLiteConnectionString))
            {
                try
                {
                    connection.Open();

                    var query = "SELECT * FROM box JOIN user_box ON user_box.box_id = box.id WHERE user_box.user_id = @id ORDER BY box.title;";
                    SQLiteCommand command = new SQLiteCommand(query, connection);

                    command.Parameters.AddWithValue("@id", userId);


                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new BoxModel
                            {
                                Id = (long)reader["id"],
                                Title = (string)reader["title"]
                            });
                        }
                    }

                }
                finally
                {
                    connection.Close();
                }

            }



            return results;
        }




        public void AddBox(string boxName, long userId)
        {

            using (var connection = new SQLiteConnection(SQLiteConnectionString))
            {
                try
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var insertBoxQuery = "INSERT INTO box (title) VALUES (@title)";

                            var insertedBoxId = -1L;
                            var insertedRelationsId = -1L;

                            using (var command = new SQLiteCommand(insertBoxQuery, connection))
                            {
                                command.Parameters.AddWithValue("@title", boxName);
                                var rowAffected = command.ExecuteNonQuery();
                                insertedBoxId = connection.LastInsertRowId;
                            }

                            if (insertedBoxId == -1L)
                            {
                                transaction.Rollback();
                                return;
                            }

                            var insertRelationsQuery = "INSERT INTO user_box (user_id, box_id) VALUES (@uid, @bid)";

                            using (var command = new SQLiteCommand(insertRelationsQuery, connection))
                            {
                                command.Parameters.AddWithValue("@uid", userId);
                                command.Parameters.AddWithValue("@bid", insertedBoxId);
                                var rowAffected = command.ExecuteNonQuery();
                                insertedRelationsId = connection.LastInsertRowId;

                            }
                            if (insertedRelationsId == -1L)
                            {
                                transaction.Rollback();
                                return;
                            }
                            transaction.Commit();

                        }
                        catch
                        {
                            transaction.Rollback();
                        }



                    }
                }
                finally
                {
                    connection.Close();
                }

            }
        }

        public void CountCredentialsInBaskets(long userId)
        {
            var query = @"SELECT box.id, box.title, COUNT(credentials.id) AS credentials_count 
                        FROM box 
                        JOIN user_box ON user_box.box_id = box.id 
                        JOIN credentials ON credentials.box_id = box.id 
                        WHERE user_box.user_id = @id \
                        GROUP BY box.id";



        }


        public int CountAvailableForMe(long userId)
        {
            var result = 0;
            using (var connection = new SQLiteConnection(SQLiteConnectionString))
            {
                try
                {
                    connection.Open();

                    var query = "SELECT COUNT(*) FROM box JOIN user_box ON user_box.box_id = box.id WHERE user_box.user_id = @id;";
                    using (var command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", userId);
                        result = Convert.ToInt32((long)command.ExecuteScalar());
                    }
                    

                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }
    }
}
