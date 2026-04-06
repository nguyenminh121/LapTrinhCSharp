using LuyenTap.Models;
using Microsoft.Data.SqlClient;

namespace LuyenTap.Services
{
    public class StudentDatabaseReader : IStudentDatabaseReader
    {
        private readonly string _connectionString;

        public StudentDatabaseReader(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MyStudentsConnection") ?? string.Empty;
        }

        public async Task<HomeIndexViewModel> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            var model = new HomeIndexViewModel
            {
                SourceDatabase = "mystudents"
            };

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                model.ErrorMessage = "Connection string MyStudentsConnection chua duoc cau hinh.";
                return model;
            }

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                const string tableQuery = @"
                    SELECT TABLE_SCHEMA, TABLE_NAME
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_TYPE = 'BASE TABLE'
                    ORDER BY TABLE_SCHEMA, TABLE_NAME";

                await using var tableCommand = new SqlCommand(tableQuery, connection);
                await using var tableReader = await tableCommand.ExecuteReaderAsync(cancellationToken);

                var tableTargets = new List<(string Schema, string Name)>();
                while (await tableReader.ReadAsync(cancellationToken))
                {
                    tableTargets.Add((
                        tableReader.GetString(0),
                        tableReader.GetString(1)
                    ));
                }

                await tableReader.CloseAsync();

                foreach (var tableTarget in tableTargets)
                {
                    var tableData = new DatabaseTableData
                    {
                        Schema = tableTarget.Schema,
                        TableName = tableTarget.Name
                    };

                    var safeSchema = EscapeSqlIdentifier(tableTarget.Schema);
                    var safeTable = EscapeSqlIdentifier(tableTarget.Name);
                    var dataQuery = $"SELECT * FROM [{safeSchema}].[{safeTable}]";

                    await using var dataCommand = new SqlCommand(dataQuery, connection);
                    await using var dataReader = await dataCommand.ExecuteReaderAsync(cancellationToken);

                    for (var i = 0; i < dataReader.FieldCount; i++)
                    {
                        tableData.Columns.Add(dataReader.GetName(i));
                    }

                    while (await dataReader.ReadAsync(cancellationToken))
                    {
                        var row = new Dictionary<string, string>();
                        foreach (var column in tableData.Columns)
                        {
                            var value = dataReader[column];
                            row[column] = FormatValue(value);
                        }

                        tableData.Rows.Add(row);
                    }

                    model.Tables.Add(tableData);
                    await dataReader.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessage = $"Khong the doc du lieu tu DB mystudents: {ex.Message}";
            }

            return model;
        }

        private static string EscapeSqlIdentifier(string identifier)
        {
            return identifier.Replace("]", "]]", StringComparison.Ordinal);
        }

        private static string FormatValue(object value)
        {
            if (value == DBNull.Value)
            {
                return "NULL";
            }

            if (value is byte[] bytes)
            {
                return $"(binary: {bytes.Length} bytes)";
            }

            var text = Convert.ToString(value) ?? string.Empty;
            if (text.Length > 200)
            {
                return text[..200] + "...";
            }

            return text;
        }
    }
}
