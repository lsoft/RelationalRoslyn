using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReRoExtension.RelationalModel
{
    public sealed class ResultSet
    {
        private readonly List<string> _columnNames;
        private readonly List<string[]> _rows = new();

        public IReadOnlyList<string> ColumnNames => _columnNames;

        public IReadOnlyList<string[]> Rows => _rows;

        public static readonly ResultSet Empty = new ResultSet(["-"], ["No data found"]);
        public static ResultSet FromException(Exception excp) => new ResultSet(["Exception Message", "Exception Stack"], [excp.Message, excp.StackTrace]);

        public ResultSet(
            List<string> columnNames
            )
        {
            _columnNames = columnNames;
        }

        private ResultSet(
            List<string> columnNames,
            string[] row
            )
        {
            _columnNames = columnNames;

            AddRow(row);
        }


        public void AddRow(
            string[] row
            )
        {
            if (row.Length != _columnNames.Count)
            {
                throw new InvalidOperationException("Data row must have same size as header row.");
            }

            _rows.Add(row);
        }

    }
}
