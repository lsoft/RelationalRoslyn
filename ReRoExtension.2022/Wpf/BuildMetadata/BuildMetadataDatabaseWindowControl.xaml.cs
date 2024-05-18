using ReRoExtension.Helper;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReRoExtension.Wpf.BuildMetadata
{
    /// <summary>
    /// Interaction logic for BuildMetadataDatabaseWindowControl.
    /// </summary>
    public partial class BuildMetadataDatabaseWindowControl : UserControl
    {
        private readonly BuildMetadataDatabaseViewModel _viewmodel;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildMetadataDatabaseWindowControl"/> class.
        /// </summary>
        public BuildMetadataDatabaseWindowControl(
            )
        {
            _viewmodel = new BuildMetadataDatabaseViewModel(
                );
            this.DataContext = _viewmodel;

            this.InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var dataGrid = sender as DataGrid;
                var selectedCell = dataGrid.CurrentCell;
                var cellContent = (TextBlock)selectedCell.Column.GetCellContent(selectedCell.Item);
                var cellValue = cellContent.Text;

                Clipboard.SetText(cellValue);

                MessageBox.Show(
                    cellValue,
                    "Cell Value",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message + Environment.NewLine + ex.StackTrace,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _viewmodel.UpdateAll();
        }

        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == Key.Enter && e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control))
            {
                _viewmodel.ExecuteQueryCommand.Execute(null);
                e.Handled = true;
            }
        }

    }

}