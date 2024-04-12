using System;
using System.Data.Common;
using System.Globalization;

namespace ReRoExtension.Helper
{
    public static partial class SqlHelper
    {
        private static readonly string StartGo = "GO" + Environment.NewLine;
        private static readonly string MiddleGo = Environment.NewLine + "GO" + Environment.NewLine;
        private static readonly string EndGo = Environment.NewLine + "GO";


        public static async Task ExecuteBatchAsync(
            this DbConnection connection,
            string batchesBody
            )
        {
            var batches = batchesBody.SplitBatch();
            foreach (var batch in batches)
            {
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = batch;
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception excp)
                {
                    throw new Exception(
                        $"Batch failed: " + Environment.NewLine + batch,
                        excp
                        );

                }
            }

        }

        public static string[] SplitBatch(
            this string sqlBatch
            )
        {
            while (sqlBatch.StartsWith(StartGo, true, CultureInfo.InvariantCulture))
            {
                sqlBatch = sqlBatch.Substring(StartGo.Length);
            }

            while (sqlBatch.EndsWith(EndGo, true, CultureInfo.InvariantCulture))
            {
                sqlBatch = sqlBatch.Substring(0, sqlBatch.Length - EndGo.Length);
            }

            var batches = sqlBatch.Split(new string[] { MiddleGo }, StringSplitOptions.RemoveEmptyEntries);

            return
                batches;
        }

    }
}
