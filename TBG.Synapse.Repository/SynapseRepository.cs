using Dapper;
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
                Insert(connection, item);
            }
        }

        public void Insert(SqlConnection connection, T item)
        {
            connection.Open();
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
                Insert(connection, items);
            }
        }

        public void Insert(SqlConnection connection, IEnumerable<T> items)
        {
            connection.Open();
            var properties = typeof(T).GetProperties();
            var columns = string.Join(", ", properties.Select(p => $"[{p.Name}]"));
            var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));
            var tableName = typeof(T).Name;
            var sql = $"INSERT INTO {tableName}({columns}) VALUES ({values})";
            connection.Execute(sql, items);
        }
        #endregion

        #region Insert Identity
        public int InsertIdentity(SqlConnection connection, T item)
        {
            connection.Open();
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

        public int InsertIdentity(T item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return InsertIdentity(connection, item);
            }
        }

        public IEnumerable<int> InsertIdentity(SqlConnection connection, IEnumerable<T> items)
        {
            connection.Open();
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

        public IEnumerable<int> InsertIdentity(IEnumerable<T> items)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return InsertIdentity(connection, items);
            }
        }
        #endregion

        #region Update
        public void Update(T item)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                Update(connection, item);
            }
        }

        public void Update(SqlConnection connection, T item)
        {
            connection.Open();
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
                Update(connection, items);
            }
        }

        public void Update(SqlConnection connection, IEnumerable<T> items)
        {
            connection.Open();
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
                Delete(connection, item);
            }
        }

        public void Delete(SqlConnection connection, T item)
        {
            connection.Open();
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
                Delete(connection, items);
            }
        }

        public void Delete(SqlConnection connection, IEnumerable<T> items)
        {
            connection.Open();
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
                return Get(connection, where, orderBy);
            }
        }

        public IEnumerable<T> Get(SqlConnection connection, string where = "", string orderBy = "")
        {
            connection.Open();
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
}