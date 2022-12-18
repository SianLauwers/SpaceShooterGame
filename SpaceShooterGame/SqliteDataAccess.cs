using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Data.SQLite;
using Dapper;
using SpaceShooterGame;

namespace PeopleListAddSqlite
{
    public class SqliteDataAccess
    {
        public static List<User> LoadPeople()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<User>("select * from HighScore order by Id desc", new DynamicParameters());
                return output.ToList();
            }
        }
        public static void SavePerson(User highscore)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute("insert into HighScore (HighScore) values (@HighScore)", highscore);
            }
        }
        private static string LoadConnectionString(string id = "Default")
        {
            // go to App.config and return the string called "Default"
            // need to add Reference: ConfigurationManager and using System.Configuration
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}