using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ToDoApp.Common.Utils;
using ToDoApp.DL;
using ToDoApp.Model;
using ToDoApp.Model.Models.DTO;

namespace ToDoApp.BL
{
    public class BaseBL : IBaseBL
    {
        protected IBaseDL _dl;

        private static Dictionary<string, List<string>> ValidColumnOfTable {  get; set; } = new Dictionary<string, List<string>>();
        public async Task<List<string>> GetValidColumns(string tableName)
        {
            try
            {
                // Nếu column đã tồn tại khi gọi ra từ ban đầu thì sẽ trả về column luôn mà không cần gọi lại vào db (cache)
                if (ValidColumnOfTable.TryGetValue(tableName, out List<string> columns)) { return columns; }
                // Nếu ban đầu chưa lấy ra column này thì gọi về db
                else {
                    using (var connection = _dl.GetConnection(""))
                    {
                        var databaseColumn = await _dl.QueryUsingCommandTextAsync<DatabaseColumn>($"SHOW COLUMNS FROM {tableName};", null, connection: connection);
                        columns = databaseColumn.Select(x => x.field).ToList();
                    }
                    ValidColumnOfTable.Add(tableName, columns);
                }
                return columns;
            }
            catch (Exception e) {
                return [];
            }
        }

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
            var props = baseModel.GetType().GetProperties();
            baseModel.ModifiedDate = DateTime.Now;
            baseModel.ModifiedBy = "current user";         
            var param = new Dictionary<string, object>();
            var columns  = new List<string>();
            string whereCondition = string.Empty;
            foreach (var prop in props) { 
                // When update, CreatedDate and CreatedBy meant to not change
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
                        // Set the WHERE condition based on the primary key
                        whereCondition = $"{prop.Name} = {propParam}";
                        // Add the primary key parameter and value to the dictionary/
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

        public async Task<PagingResponse> GetPaging<T>(PagingRequest pagingRequest) where T : IBaseModel
        {
            var response  = new PagingResponse();
            var sql = "SELECT {0} FROM {1} WHERE {2}";

            var instance  = Activator.CreateInstance<T>();
            var tableName = instance.GetTableName() ?? instance.GetType().Name;
            var column = "*";
            if (pagingRequest.Column != null) { 
                // Xử lý các cột ko hợp lệ
                var columnFromClient = pagingRequest.Column.Split(",");
                var validColumn = (await GetValidColumns(tableName)).Select(col => col.ToLower());
                column = string.Join(",", columnFromClient.Where(col => validColumn.Contains(col.ToLower().Trim())));

            }
        }
        // Build WHERE CONDTION
        public async Task<string> BuildPagingCommandText(Type type, PagingRequest pagingRequest, Dictionary<string, object> param, bool disabledLimit = false)
        {
            var builder  = new StringBuilder();
            int pageIndex = pagingRequest.PageIndex;
            int pageSize = pagingRequest.PageSize;
            var conditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(pagingRequest.Filter)) { 
                var filter = JsonConvert.DeserializeObject<JArray>(pagingRequest.Filter);
                var condition = BuildCondition

            }
        }
        private string BuildCondition(JToken filter, Dictionary<string, object> param) {
            // Nếu có filter và nhiều hơn 1 filter.
            if (filter != null && filter?.Count() > 0) {
                string whereCondition = "";
                int index = 0;
                foreach (var item in filter) {
                    if (item == null) return whereCondition;
                    if (item.GetType() == typeof(JValue)) {
                        if (index == 0) {
                            var column = item.ToString();
                            whereCondition += " " + column + " ";
                        } else
                        if (index == 2){ 
                            var stringParam = Utils.
                        }
                }
            }
        }
    }
}
