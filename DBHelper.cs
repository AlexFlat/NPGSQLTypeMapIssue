using Npgsql;
using NPGSQLTypeMapIssue.Domain;
using System.Collections.Generic;
using System.Data;

namespace NPGSQLTypeMapIssue
{
    public class DBHelper
    {      
        private const string POSTGRESDB = "postgres";
        public static void RegisterDapperNPGSQL()
        {
            InitGlobalTypeMapper();
        }

        public static void InitGlobalTypeMapper()
        {
            NpgsqlConnection.GlobalTypeMapper.MapComposite<NPGSQLTypeMapIssue.Domain.Poco>("poco");
        }

        public NpgsqlConnection GetDbConnection(string connectionString, bool loadCompositeTypes)
        {
            var fullConnString = connectionString;
            if (loadCompositeTypes)
            {
                fullConnString += ";Load Table Composites=true;";
            }
            return new NpgsqlConnection(fullConnString);
        }

        private void ExecuteNonQuery(string connectionString, string script, bool loadTypes)
        {
            using (var connection = GetDbConnection(connectionString, loadTypes))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand(script, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            };
        }

        public void Uninstall(string connectionString)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);

            // Create a dynamic script that terminates all backend connections to the target database
            // and then drops it.
            var dropDbScript = $@"SELECT pg_terminate_backend(pid)
                    FROM pg_stat_activity
                    WHERE datname = '{connectionStringBuilder.Database}';
                    DROP DATABASE IF EXISTS {connectionStringBuilder.Database};";

            connectionStringBuilder.Database = POSTGRESDB;
            connectionStringBuilder.Pooling = false;

            ExecuteNonQuery(connectionStringBuilder.ConnectionString, dropDbScript, false);
        }

        public void Install(string connectionString)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
            var tenantDatabaseName = connectionStringBuilder.Database;
            connectionStringBuilder.Pooling = false;

            var scriptDB = $"CREATE DATABASE {tenantDatabaseName}";
            connectionStringBuilder.Database = POSTGRESDB;

            ExecuteNonQuery(connectionStringBuilder.ConnectionString, scriptDB, false);

            var scriptTable = @"CREATE TABLE
                poco(
                    id bigserial NOT NULL PRIMARY KEY,
                    title text NOT NULL,
                    created_by text NOT NULL
                ) WITH(
                    OIDS = FALSE
                );";

            connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString);
            connectionStringBuilder.Pooling = false;
            ExecuteNonQuery(connectionStringBuilder.ConnectionString, scriptTable, false);
        }

        public void AddPoco(IDbConnection connection, IDbTransaction dbTransaction, Poco item)
        {
            var insert = "INSERT INTO poco(title, created_by) values(@title, @created_by)";
            var cmd = connection.CreateCommand();
            cmd.CommandText = insert;
            cmd.Parameters.Add(new NpgsqlParameter("title", item.Title));
            cmd.Parameters.Add(new NpgsqlParameter("created_by", item.CreatedBy));
            cmd.ExecuteNonQuery();
        }

        public List<Poco> GetPoco(string connectionString)
        {
            var getScript = "select poco from poco";
            using (var connection = GetDbConnection(connectionString, true))
            {
                connection.Open();
                using (var cmd = new NpgsqlCommand(getScript, connection))
                {
                    var rdr = cmd.ExecuteReader();
                    var result = new List<Poco>();
                    while (rdr.Read())
                    {
                        var poco = rdr.GetFieldValue<Poco>(0);
                        result.Add(poco);
                    }
                    return result;
                }
            };
        }

        public void Add(Poco newItem, string connString)
        {
            using (var conn = GetDbConnection(connString, true))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    AddPoco(conn, tx, newItem);
                    tx.Commit();
                    conn.Close();
                }
            }
            var results = GetPoco(connString);
        }
    }
}
