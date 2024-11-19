using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.DL
{
    public class BaseDL : IBaseDL
    {
        public IDbConnection GetConnection(string connectionString)
        {
            IDbConnection connection = null;
            if (!string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("dEBUG");

                connection = new MySqlConnection(connectionString);
                Console.WriteLine("dEBUG 2");

            }
            else
            {
                Console.WriteLine("dEBUG");
                connection = new MySqlConnection(DatabaseConstant.ConnectionString);
                Console.WriteLine("dEBUG 2");

            }
            return connection;

        }

        public async Task<T> ExecuteScalarCommandTextAsync<T>(string command, IDictionary<string, object> param, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            IDbConnection conn = transaction?.Connection != null ? transaction?.Connection : connection;
            conn.Open();

            // Execute SQL command and return the first value
            var res = await conn.ExecuteScalarAsync<T>(command, param, transaction, commandType: CommandType.Text);
            conn.Close();
            return res;
        }

        public async Task<int> ExecuteCommandTextAsync(string command, IDictionary<string, object> param, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            IDbConnection conn = transaction?.Connection != null ? transaction?.Connection : connection;
            conn.Open();
            var res = await conn.ExecuteAsync(command, param, transaction, commandType: CommandType.Text);
            conn.Close();
            return res;
        }
    }
}
