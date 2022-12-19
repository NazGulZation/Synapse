using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace TBG.Synapse.Repository
{
    public class SynapseRepository<T> : List<T>
    {
        private readonly string _connectionString;

        public SynapseRepository(string connectionString)
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException($"{nameof(connectionString)} cannot be null or empty.");
            }

            var type = typeof(T);
            var properties = type.GetProperties();

            var primaryKeyProperties = properties.Where(x => x.GetCustomAttributes<PrimaryKeyAttribute>().Any());

            if (!primaryKeyProperties.Any())
            {
                throw new ArgumentException($"{type.Name} must have at least one property with the PrimaryKeyAttribute.");
            }

            if (primaryKeyProperties.Count() > 1)
            {
                throw new ArgumentException($"{type.Name} cannot have more than one property with the PrimaryKeyAttribute.");
            }

            _connectionString = connectionString;
        }

        #region Insert
        public void Insert(T item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                Insert(connection, item);
            }
        }

        public void Insert(SqlConnection connection, T item)
        {
            var properties = typeof(T).GetProperties();
            var columns = string.Join(", ", properties.Select(p => $"[{p.Name}]"));
            var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            var tableName = typeof(T).Name;
            var sql = $"INSERT INTO {tableName}({columns}) VALUES ({values})";
            connection.Execute(sql, item);
        }

        public void Insert(IEnumerable<T> items)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                Insert(connection, items);
            }
        }

        public void Insert(SqlConnection connection, IEnumerable<T> items)
        {
            var properties = typeof(T).GetProperties();
            var columns = string.Join(", ", properties.Select(p => $"[{p.Name}]"));
            var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            var tableName = typeof(T).Name;
            var sql = $"INSERT INTO {tableName}({columns}) VALUES ({values})";
            connection.Execute(sql, items);
        }
        #endregion

        #region Insert Identity
        public int InsertIdentity(T item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return InsertIdentity(connection, item);
            }
        }
        public int InsertIdentity(SqlConnection connection, T item)
        {
            var properties = typeof(T).GetProperties();
            var columns = string.Join(", ", properties
                .Where(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length == 0)
                .Select(p => $"[{p.Name}]"));
            var values = string.Join(", ", properties
                .Where(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length == 0)
                .Select(p => $"@{p.Name}"));
            var tableName = typeof(T).Name;
            var sql = $"INSERT INTO {tableName}({columns}) VALUES ({values}); SELECT SCOPE_IDENTITY();";
            var id = connection.Query<int>(sql, item).Single();
            return id;
        }

        public IEnumerable<int> InsertIdentity(IEnumerable<T> items)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return InsertIdentity(connection, items);
            }
        }

        public IEnumerable<int> InsertIdentity(SqlConnection connection, IEnumerable<T> items)
        {
            var properties = typeof(T).GetProperties();
            var columns = string.Join(", ", properties
                .Where(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length == 0)
                .Select(p => $"[{p.Name}]"));
            var values = string.Join("), (", items
                .Select(i => string.Join(", ", properties
                    .Where(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length == 0)
                    .Select(p => $"@{p.Name}"))));
            var tableName = typeof(T).Name;
            var sql = $"INSERT INTO {tableName}({columns}) VALUES ({values}); SELECT SCOPE_IDENTITY() FROM {tableName};";
            var ids = connection.Query<int>(sql, items);
            return ids;
        }
        #endregion

        #region Update
        public void Update(T item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                Update(connection, item);
            }
        }

        public void Update(SqlConnection connection, T item)
        {
            var properties = typeof(T).GetProperties();
            var updates = string.Join(", ", properties
                .Where(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length == 0)
                .Select(p => $"[{p.Name}] = @{p.Name}"));
            var primaryKey = $"{properties.Single(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length > 0).Name}";
            var tableName = typeof(T).Name;
            var sql = $"UPDATE {tableName} SET {updates} WHERE [{primaryKey}] = @{primaryKey}";
            connection.Execute(sql, item);
        }

        public void Update(IEnumerable<T> items)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                Update(connection, items);
            }
        }

        public void Update(SqlConnection connection, IEnumerable<T> items)
        {
            var properties = typeof(T).GetProperties();
            var updates = string.Join(", ", properties
                .Where(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length == 0)
                .Select(p => $"[{p.Name}] = @{p.Name}"));
            var primaryKey = $"{properties.Single(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length > 0).Name}";
            var tableName = typeof(T).Name;
            var sql = $"UPDATE {tableName} SET {updates} WHERE [{primaryKey}] = @{primaryKey}";
            connection.Execute(sql, items);
        }
        #endregion

        #region Delete
        public void Delete(T item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                Delete(connection, item);
            }
        }

        public void Delete(SqlConnection connection, T item)
        {
            var properties = typeof(T).GetProperties();
            var primaryKey = $"{properties.Single(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length > 0).Name}";
            var tableName = typeof(T).Name;
            var sql = $"DELETE FROM {tableName} WHERE [{primaryKey}] = @{primaryKey}";
            connection.Execute(sql, item);
        }

        public void Delete(IEnumerable<T> items)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                Delete(connection, items);
            }
        }

        public void Delete(SqlConnection connection, IEnumerable<T> items)
        {
            var properties = typeof(T).GetProperties();
            var primaryKey = $"{properties.Single(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length > 0).Name}";
            var tableName = typeof(T).Name;
            var sql = $"DELETE FROM {tableName} WHERE [{primaryKey}] = @{primaryKey}";
            connection.Execute(sql, items);
        }
        #endregion

        #region GET
        public IEnumerable<T> Get(string where = "", string orderBy = "")
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return Get(connection, where, orderBy);
            }
        }

        public IEnumerable<T> Get(SqlConnection connection, string where = "", string orderBy = "")
        {
            var tableName = typeof(T).Name;
            var sql = $"SELECT * FROM {tableName}";
            if (!string.IsNullOrWhiteSpace(where))
            {
                sql += $" WHERE {where}";
            }
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                sql += $" ORDER BY {orderBy}";
            }
            return connection.Query<T>(sql);
        }
        #endregion GET

        #region GET
        public IEnumerable<T> GetFromSP(string storedProcedureName, object parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return GetFromSP(connection, storedProcedureName, parameters);
            }
        }

        public IEnumerable<T> GetFromSP(SqlConnection connection, string storedProcedureName, object parameters = null)
        {
            return connection.Query<T>(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
        }
        #endregion

        #region Create Table
        public void CreateTable()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var properties = typeof(T).GetProperties();
                var columns = string.Join(", ", properties.Select(p => $"[{p.Name}] {GetDbType(p)}"));
                var primaryKey = $"[{properties.Single(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length > 0).Name}]";
                var tableName = typeof(T).Name;
                var sql = $"IF NOT EXISTS (SELECT * FROM sysobjects WHERE NAME='{tableName}' and xtype='U') " +
                    $"CREATE TABLE {tableName} ({columns}, PRIMARY KEY ({primaryKey}))";
                connection.Execute(sql);
            }
        }

        public void CreateTableIdentity()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var properties = typeof(T).GetProperties();
                var columns = string.Join(", ", properties.Where(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length == 0).Select(p => $"[{p.Name}] {GetDbType(p)}"));
                var primaryKey = $"[{properties.Single(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), false).Length > 0).Name}]";
                var tableName = typeof(T).Name;
                var sql = $"IF NOT EXISTS (SELECT * FROM sysobjects WHERE NAME='{tableName}' and xtype='U') " +
                    $"CREATE TABLE {tableName} ({primaryKey} INT IDENTITY(1,1) PRIMARY KEY, {columns})";
                connection.Execute(sql);
            }
        }

        private string GetDbType(PropertyInfo property)
        {
            string returnString = string.Empty;
            switch (property.PropertyType.Name)
            {
                case "String":
                    var attr = property.GetCustomAttributes(typeof(StrLengthAttribute), false);
                    returnString = attr.Length > 0 ? $"VARCHAR({((StrLengthAttribute)attr[0]).Length})" : "VARCHAR(50)";
                    break;
                case "Int32":
                    returnString = "INT";
                    break;
                case "Boolean":
                    returnString = "BIT";
                    break;
                case "DateTime":
                    returnString = "DATETIME";
                    break;
                case "Decimal":
                    returnString = "DECIMAL(18,2)";
                    break;
                case "Float":
                    returnString = "FLOAT";
                    break;
                default:
                    throw new ArgumentException("Invalid Type");
            }

            var isNullAttr = property.GetCustomAttributes(typeof(NotNullAttribute), false);
            if (isNullAttr.Length > 0)
            {
                returnString += " NOT NULL";
            }
            return returnString;
        }
        #endregion
    }

    public static class SynapseRepository
    {
        public static void ExecuteSqlQuery(string sqlQuery, string connectionString)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                ExecuteSqlQuery(sqlQuery, connection);
            }
        }

        public static void ExecuteSqlQuery(string sqlQuery, IDbConnection connection)
        {
            connection.Execute(sqlQuery);
        }

        public static void ExecuteStoredProcedure(string storedProcedureName, DynamicParameters parameters, string connectionString)
        {
            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                ExecuteStoredProcedure(storedProcedureName, parameters, connection);
            }
        }

        public static void ExecuteStoredProcedure(string storedProcedureName, DynamicParameters parameters, IDbConnection connection)
        {
            connection.Execute(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
        }
    }
}