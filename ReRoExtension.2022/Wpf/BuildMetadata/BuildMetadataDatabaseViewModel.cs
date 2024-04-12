using ReRoExtension.Helper;
using ReRoExtension.RelationalModel;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfHelpers;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ReRoExtension.Wpf.BuildMetadata
{
    public sealed class BuildMetadataDatabaseViewModel : BaseViewModel
    {
        private readonly Model _currentModel;

        private string _searchPredicate = "";

        private ObservableCollection<DataGridColumn> _headerColumns = new ();
        private DataTable _dataTable = new DataTable();

        private ICommand? _buildModelCommand;
        private ICommand? _executeQueryCommand;
        private ICommand? _previousResultSetCommand;
        private ICommand? _nextResultSetCommand;

        private List<ResultSet> _resultSets;
        private int _resultSetIndex;

        public string ModelStatus => _currentModel.ModelStatus;

        public string CurrentQuery
        {
            get;
            set;
        } =
$"""
select * from help
--you can run many queries at once, just separate them with GO:
GO
select * from named_types
GO
select * from members
""";


        public string ResultSetIndexAndSize
        {
            get
            {
                if (_resultSets is null || _resultSets.Count == 0)
                {
                    return string.Empty;
                }

                return $"{_resultSetIndex + 1} / {_resultSets.Count}";
            }
        }

        public string TotalRowsMessage => "Всего строк: " + _dataTable.Rows.Count;

        public ObservableCollection<DataGridColumn> HeaderColumns
        {
            get
            {
                return _headerColumns;
            }
            set
            {
                _headerColumns = value;
                base.OnPropertyChanged();
            }
        }

        public DataTable DataTable
        {
            get
            {
                return _dataTable;
            }
            set
            {
                if (_dataTable != value)
                {
                    _dataTable = value;
                }

                base.OnPropertyChanged(nameof(DataTable));
            }
        }

        public string SearchPredicate
        {
            get => _searchPredicate;
            set
            {
                _searchPredicate = value;

                FillDataGrid();

                OnPropertyChanged();
            }
        }

        public ICommand BuildModelCommand
        {
            get
            {
                if (_buildModelCommand is null)
                {
                    _buildModelCommand = new AsyncRelayCommand(
                        async a =>
                        {
                            await TaskScheduler.Default;

                            await _currentModel.BuildAsync(
                                () =>
                                {
                                    OnPropertyChanged();
                                }
                                );

                            OnPropertyChanged();
                        });
                }

                return _buildModelCommand;
            }
        }

        public ICommand ExecuteQueryCommand
        {
            get
            {
                if (_executeQueryCommand is null)
                {
                    _executeQueryCommand = new AsyncRelayCommand(
                        async a =>
                        {
                            await TaskScheduler.Default;

                            List<ResultSet> resultSets;
                            try
                            {
                                resultSets = await _currentModel.ExecuteQueryAsync(
                                    CurrentQuery
                                    );
                            }
                            catch (Exception excp)
                            {
                                resultSets = [ResultSet.FromException(excp)];
                            }

                            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                            if (resultSets.Count == 0)
                            {
                                //no data at all
                                resultSets = [ResultSet.Empty];
                            }

                            _resultSets = resultSets;
                            _resultSetIndex = 0;

                            FillDataGrid();

                            OnPropertyChanged();
                        });
                }

                return _executeQueryCommand;
            }
        }

        public ICommand PreviousResultSetCommand
        {
            get
            {
                if (_previousResultSetCommand is null)
                {
                    _previousResultSetCommand = new RelayCommand(
                        a =>
                        {
                            if (_resultSetIndex > 0)
                            {
                                _resultSetIndex--;
                            }

                            FillDataGrid();

                            OnPropertyChanged();
                        });
                }

                return _previousResultSetCommand;
            }
        }

        public ICommand NextResultSetCommand
        {
            get
            {
                if (_nextResultSetCommand is null)
                {
                    _nextResultSetCommand = new RelayCommand(
                        a =>
                        {
                            if (_resultSetIndex < _resultSets.Count - 1)
                            {
                                _resultSetIndex++;
                            }

                            FillDataGrid();

                            OnPropertyChanged();
                        });
                }

                return _nextResultSetCommand;
            }
        }


        public BuildMetadataDatabaseViewModel(
            )
        {
            _currentModel = Model.GetModel();
        }


        private void FillDataGrid()
        {
            ResultSet resultSet = _resultSets[_resultSetIndex];

            _headerColumns = new ObservableCollection<DataGridColumn>();
            var dataTable = new DataTable();
            for (var i = 0; i < resultSet.ColumnNames.Count; i++)
            {
                var columnName = resultSet.ColumnNames[i];

                var dtcn = i.ToString();

                _headerColumns.Add(
                    new DataGridTextColumn
                    {
                        Header = columnName.Replace("_", "__"),
                        IsReadOnly = true,
                        Binding = new System.Windows.Data.Binding(dtcn)
                    }
                    );

                dataTable.Columns.Add(
                    dtcn,
                    typeof(string)
                    );
            }

            var searchPredicate = _searchPredicate.ToLower();

            foreach (var resultRow in resultSet.Rows)
            {
                if (!string.IsNullOrEmpty(_searchPredicate))
                {
                    if (resultRow.All(v => !v.ToString().ToLower().Contains(searchPredicate)))
                    {
                        continue;
                    }
                }

                dataTable.Rows.Add(resultRow);
            }
            _dataTable = dataTable;
        }

    }

}