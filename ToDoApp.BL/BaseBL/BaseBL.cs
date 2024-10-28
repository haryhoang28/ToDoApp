using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            var sql = "INSERT INTO {tableName} ({columns}) VALUES ({values}); SELECT LAST_INSERT_ID()";
            // using reflections
            baseModel.CreatedDate = DateTime.Now;
            baseModel.CreatedBy = "current user"; // Todo: làm tính năng đăng nhập
            var props = baseModel.GetType().GetProperties();
            var param = new Dictionary<string, object>();
            var columns = new List<string>();
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
            sql = sql.Replace("{tableName}", tablename) // để ý viết hoa thuong
                        .Replace("{columns}", string.Join(",", columns))
                        .Replace("{values}", string.Join(",", columns.Select(col => $"@{col}")));

            using (var conn = _dl.GetConnection(""))
            {
                var lastId = await _dl.ExecuteScalarCommandTextAsync<int>(sql, param, conn);
                baseModel.SetPrimaryKey(lastId);
            }
            return baseModel;
            
        }

        public async Task<T> UpdateOne<T>(T baseModel) where T : IBaseModel, IModificationInfo
        {
            var tableName = baseModel.GetTableName();
            var sql = "UPDATE {0} SET {1} WHERE {2}";
            baseModel.ModifiedDate = DateTime.Now;
            baseModel.ModifiedBy = "current user";
            var props = baseModel.GetType().GetProperties();    
            var param = new Dictionary<string, object>();
            var columns  = new List<string>();
            string whereCondition = string.Empty;
            foreach (var prop in props) { 
                if (prop.Name == "CreatedDate" || prop.Name == "CreatedBy")
                {
                    continue;
                }
                var value = prop.GetValue(baseModel);
                if (value != null) { 
                    var propParam = $"@{prop.Name}";

                    // Update based on pk
                    if (prop.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault() != null)
                    {
                        whereCondition = $"{prop.Name} = {propParam}";
                        param.Add(propParam, value);
                    }
                    else
                    // Update based on column
                    if (prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() != null) {
                        columns.Add($"{ prop.Name} = { propParam}");
                        param.Add(propParam, value);
                    }
                }
            }
            sql = string.Format(sql, new string[] {tableName, string.Join(",", columns),whereCondition});
            var success = false;
            using (var conn = _dl.GetConnection(""))
            {
                var res = await _dl.ExecuteCommandTextAsync(sql, param, conn);
                success = res > 0;
            }
            if (success)
            {
                return baseModel;
            }
            return default(T);
        }

        public async Task<bool> DeleteOne<T>(int id) where T : IBaseModel
        {
            var sql = "DELETE FROM {0} WHERE {1}";
            var baseModel = Activator.CreateInstance<T>();
            var tableName = baseModel.GetTableName();
            var props = baseModel.GetType().GetProperties();
            // Find the property that is marked with the KeyAttribute
            var keyProp = props.FirstOrDefault(prop => prop.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault() != null);
            var keyValue = id;
            var whereCondition = string.Empty;
            var param = new Dictionary<string, object>();
            // Define the key parameter name (for use in the WHERE clause)
            var keyParam = $"@{keyProp.Name}";
            // Set the WHERE condition to match the primary key with the parameter
            whereCondition = $"{keyProp.Name} = {keyParam}";
            param.Add(keyParam, keyValue);

            
            sql = string.Format (sql, new string[] {tableName, whereCondition});
            var success = false;
            using (var conn = _dl.GetConnection(""))
            {
                var res = await _dl.ExecuteCommandTextAsync(sql, param, conn);
                success = res > 0;
            }
            
            return success;
        }  
    }
}
