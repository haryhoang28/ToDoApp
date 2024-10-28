using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.DL
{
    public interface IBaseDL
    {
        // Create connection
        public IDbConnection GetConnection(string connectionString);
        

        // Execute SQL command and return a single value
        Task<T> ExecuteScalarCommandTextAsync<T>(string command, IDictionary<string, object> param, IDbConnection? connection = null, IDbTransaction? transaction = null);
        // Execute SQL command and return columns affected
        Task<int> ExecuteCommandTextAsync(string command, IDictionary<string, object> param, IDbConnection? connection = null, IDbTransaction? transaction = null);

    }
}
