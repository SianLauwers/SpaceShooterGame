using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooterGame.DAL
{
    class SqliteRepository
    {
        public SqliteRepository()
        {
        }
        public static SqliteConnection DbConnection()
        {
            return new SqliteConnection(@"Data Source=DB.sqlite");
        }

        protected static bool DatabaseExists()
        {
            return File.Exists(@"DB.sqlite");
        }

        protected static void CreateDatabase()
        {
            using (var connection = DbConnection())
            {
                connection.Open();
                connection.Execute(
                    @"CREATE TABLE HighScore 
                    (
                    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
                    HighScore       Int
                    )");
            }
        }
    }
}