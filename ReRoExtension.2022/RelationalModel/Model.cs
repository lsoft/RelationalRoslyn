using ReRoExtension.Entity;
using ReRoExtension.Helper;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.SQLite;
using LinqToDB.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReRoExtension.RelationalModel
{
    public enum ModelStateEnum
    {
        NotBuild,
        BuildingNow,
        Built,
        Error
    }

    public sealed class Model : IDisposable
    {
        private readonly SqliteConnection _connection;
        
        private DataConnection _dataConnection;

        private EntityModel? _inMemoryModel;

        public ModelStateEnum State
        {
            get;
            private set;
        } = ModelStateEnum.NotBuild;

        public string? BuildingProgressMessage
        {
            get;
            private set;
        }

        public string? ErrorMessage
        {
            get;
            private set;
        }

        public DateTime? BuildingFinishTime
        {
            get;
            private set;
        }

        public string ModelStatus
        {
            get
            {
                switch (State)
                {
                    case ModelStateEnum.NotBuild:
                        return $"Model not build yet";
                    case ModelStateEnum.BuildingNow:
                        return $"Model building progress: {BuildingProgressMessage}";
                    case ModelStateEnum.Built:
                        return $"Model built at {BuildingFinishTime.Value}. Projects: {_inMemoryModel.Projects.Count}, Named Types: {_inMemoryModel.Projects.Sum(p => p.NamedTypes.Count)}";
                    case ModelStateEnum.Error:
                        return $"Model is in broken state: " + Environment.NewLine + ErrorMessage;
                    default:
                        throw new InvalidOperationException(State.ToString());
                }
            }
        }

        public Model(
            )
        {
            BuildingFinishTime = null;

            _connection = new SqliteConnection("Data Source=:memory:");
        }

        public async Task BuildAsync(
            Action buildingProgressMadeAction
            )
        {
            try
            {
                State = ModelStateEnum.BuildingNow;

                CloseDatabase();

                OpenDatabase();

                await CreateDatabaseStructureAsync(
                    );

                void ReportProgress(string buildingProgressMessage)
                {
                    BuildingProgressMessage = buildingProgressMessage;
                    buildingProgressMadeAction();
                };

                _inMemoryModel = Builder.BuildEntityModel(
                    ReportProgress
                    );

                ReportProgress("Saving data to in-memory sqlite database...");

                await SaveModelAsync(_inMemoryModel);

                State = ModelStateEnum.Built;
            }
            catch (Exception excp)
            {
                ErrorMessage = excp.Message;
                State = ModelStateEnum.Error;
            }
            finally
            {
                BuildingFinishTime = DateTime.Now;
            }
        }

        public async Task<List<ResultSet>> ExecuteQueryAsync(string batch)
        {
            var result = new List<ResultSet>();

            foreach (var query in batch.SplitBatch())
            {
                using var command = _connection.CreateCommand();
                command.CommandText = query;
                command.CommandTimeout = 30;

                using var qr = await command.ExecuteReaderAsync();

                if (!qr.HasRows)
                {
                    continue;
                }

                var fc = qr.FieldCount;

                var columnNameRow = new List<string>();
                for (var i = 0; i < fc; i++)
                {
                    var columnTypeName = qr.GetName(i);
                    columnNameRow.Add(columnTypeName);
                }

                var resultSet = new ResultSet(columnNameRow);

                while (await qr.ReadAsync())
                {
                    var resultRow = new string[fc];
                    for (var i = 0; i < fc; i++)
                    {
                        var columnType = qr.GetDataTypeName(i);
                        switch (columnType)
                        {
                            case "UNIQUEIDENTIFIER":
                                var gv = qr.GetGuid(i);
                                resultRow[i] = gv.ToString();
                                break;
                            case "INTEGER":
                                var iv = qr.GetInt64(i);
                                resultRow[i] = iv.ToString();
                                break;
                            case "bit":
                                var bv = qr.GetBoolean(i);
                                resultRow[i] = bv.ToString();
                                break;
                            case "NVARCHAR":
                                var v = qr.GetString(i);
                                resultRow[i] = v.ToString();
                                break;
                            default:
                                throw new InvalidOperationException($"Unknown column type {columnType}");
                        }
                    }

                    resultSet.AddRow(resultRow);
                }

                result.Add(resultSet);
            }

            return result;
        }

        private async Task SaveModelAsync(EntityModel inMemoryModel)
        {
            await _dataConnection.BulkCopyAsync(
                inMemoryModel.Projects
                );

            foreach (var project in inMemoryModel.Projects)
            {
                await _dataConnection.BulkCopyAsync(
                    project.NamedTypes
                    );
                await _dataConnection.BulkCopyAsync(
                    project.NamedTypes.SelectMany(a => a.ProduceEntities())
                    );

                foreach (var namedType in project.NamedTypes)
                {
                    await _dataConnection.BulkCopyAsync(
                        namedType.Members
                        );
                    await _dataConnection.BulkCopyAsync(
                        namedType.Members.SelectMany(a => a.ProduceEntities())
                        );

                    await _dataConnection.BulkCopyAsync(
                        namedType.FieldMembers
                        );
                    await _dataConnection.BulkCopyAsync(
                        namedType.FieldMembers.SelectMany(a => a.ProduceEntities())
                        );

                    await _dataConnection.BulkCopyAsync(
                        namedType.PropertyMembers
                        );
                    await _dataConnection.BulkCopyAsync(
                        namedType.PropertyMembers.SelectMany(a => a.ProduceEntities())
                        );

                    await _dataConnection.BulkCopyAsync(
                        namedType.MethodMembers
                        );
                    await _dataConnection.BulkCopyAsync(
                        namedType.MethodMembers.SelectMany(a => a.ProduceEntities())
                        );
                }
            }
        }

        private void OpenDatabase()
        {
            _connection.Open();
            _dataConnection = new DataConnection(
                new SqliteDataProvider(),
                _connection
                );

        }

        private async Task CreateDatabaseStructureAsync(
            )
        {
            var createDatabaseScript = ReflectionHelper.GetEmbeddedResource(
                this.GetType().Assembly,
                "ReRoExtension.Resources.DatabaseStructure.sql"
                );

            await SqlHelper.ExecuteBatchAsync(
                _connection,
                createDatabaseScript
                );

            var insertHelpCommand = _connection.CreateCommand();
            insertHelpCommand.CommandText = @"
insert into help
    (help_text)
values
    (@help_text)
";
            insertHelpCommand.Parameters.Add(
                new SqliteParameter(
                    "@help_text",
                    createDatabaseScript
                    )
                );
            await insertHelpCommand.ExecuteNonQueryAsync();
        }

        private void CloseDatabase()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                _dataConnection.Close();
                _connection.Close();
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        #region static 

        private static Model? _currentModel = null;

        public static Model GetModel()
        {
            if (_currentModel is not null)
            {
                return _currentModel;
            }

            _currentModel = new Model();
            return _currentModel;
        }

        #endregion

        #region sqlite

        public class SqliteDataProvider : SQLiteDataProvider
        {
            public SqliteDataProvider() : base("Microsoft.Data.Sqlite")
            {
            }
        }

        #endregion
    }
}
