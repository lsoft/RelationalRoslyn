using System;
using System.Windows;
using System.Windows.Controls;

namespace ReRoExtension.Wpf.BuildMetadata
{
    /// <summary>
    /// Interaction logic for BuildMetadataDatabaseWindowControl.
    /// </summary>
    public partial class BuildMetadataDatabaseWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildMetadataDatabaseWindowControl"/> class.
        /// </summary>
        public BuildMetadataDatabaseWindowControl(
            )
        {
            var viewmodel = new BuildMetadataDatabaseViewModel(
                );
            this.DataContext = viewmodel;

            this.InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            var selectedCell = dataGrid.CurrentCell;
            var cellContent = (TextBlock)selectedCell.Column.GetCellContent(selectedCell.Item);
            var cellValue = cellContent.Text;
            MessageBox.Show(
                cellValue,
                "Cell Value",
                MessageBoxButton.OK,
                MessageBoxImage.Information
                );
        }
    }

}