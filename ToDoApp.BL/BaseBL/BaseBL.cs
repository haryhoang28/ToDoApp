using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.DL;
using ToDoApp.Model;

namespace ToDoApp.BL
{
    public class BaseBL : IBaseBL
    {
        protected IBaseDL _dl;


        public BaseBL(IBaseDL dl)
        {
            this._dl = dl;
        }

        

        public async Task<T> InsertOne<T>(T baseModel) where T : IBaseModel, ICreationInfo
        {
            var tablename = baseModel.GetTableName();
            var sql = "INSERT INTO {tablename} ({columns}) VALUES ({values}); SELECT LAST_INSERTED_ID()";
            // using reflections
            var props = baseModel.GetType().GetProperties();
            var param = new Dictionary<string, object>();
            var columns= new List<string>();
            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() != null)
                {
                    var value = prop.GetValue(baseModel);
                    if (value != null)
                    {
                        columns.Add(prop.Name);
                        param.Add($"@{prop.Name}", value);
                        

                    }
                }
            }
            // Using to replace param
            sql = sql.Replace("{tableName}", tablename)
                        .Replace("{Columns}", string.Join(",", columns))
                        .Replace("{values}", string.Join(".", columns.Select(col => $"@{col}")));

            using (var conn = _dl.GetConnection(""))
            {
                var lastId = await _dl.ExecuteScalarCommandTextAsync<int>(sql, param, conn);
                baseModel.SetPrimaryKey(lastId);
            }
            return baseModel;
            
        }
    }
}
