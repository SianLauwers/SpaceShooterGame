using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooterGame.DAL
{
     class CRUDRepository : SqliteRepository
    {
            public CRUDRepository()
            {
                if (!DatabaseExists())
                {
                    CreateDatabase();
                }
            }
            public int Insert(User user)
            {
                string sql = "INSERT INTO HighScore (HighScore) Values (@HighScore);";

                using (var connection = DbConnection())
                {
                    connection.Open();
                    return connection.Execute(sql, user);
                }
            }

            public IEnumerable<User> Delete()
            {
                string sql = "DELETE FROM HighScore;";

                using (var connection = DbConnection())
                {
                    return connection.Query<User>(sql);
                }
            }

            public IEnumerable<User> GetUsers()
            {
                string sql = "SELECT HighScore FROM HighScore;";

                using (var connection = DbConnection())
                {
                    return connection.Query<User>(sql);
                }
            }
        }
}
