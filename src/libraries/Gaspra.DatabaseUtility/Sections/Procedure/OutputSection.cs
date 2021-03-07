﻿using Gaspra.DatabaseUtility.Interfaces;
using Gaspra.DatabaseUtility.Models.Database;
using Gaspra.DatabaseUtility.Models.Merge;
using Gaspra.DatabaseUtility.Models.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gaspra.DatabaseUtility.Sections.Procedure
{
    public class OutputSection : IScriptSection
    {
        private readonly IScriptLineFactory _scriptLineFactory;

        public ScriptOrder Order { get; } = new ScriptOrder(new[] { 1, 2, 4 });

        public OutputSection(IScriptLineFactory scriptLineFactory)
        {
            _scriptLineFactory = scriptLineFactory;
        }

        public Task<bool> Valid(IScriptVariables variables)
        {
            var matchOn = variables.MergeIdentifierColumns.Select(c => c.Name);

            var deleteOn = variables.DeleteIdentifierColumns.Select(c => c.Name);

            var deleteOnFactId = matchOn.Where(m => !deleteOn.Any(d => d.Equals(m))).FirstOrDefault();

            return Task.FromResult(!string.IsNullOrWhiteSpace(deleteOnFactId) && deleteOn.Any());
        }

        public async Task<string> Value(IScriptVariables variables)
        {
            var mergeStatement = new List<string>
            {
                $"OUTPUT",
                $"    inserted.{variables.Table.Name}Id,"
            };

            var matchOn = variables.MergeIdentifierColumns.Select(c => c.Name);

            var deleteOn = variables.DeleteIdentifierColumns.Select(c => c.Name);

            var deleteOnFactId = matchOn.Where(m => !deleteOn.Any(d => d.Equals(m))).FirstOrDefault();

            var insertedColumns = variables.Table.Columns.Where(c => matchOn.Any(m => m.Equals(c.Name)));

            foreach (var column in insertedColumns)
            {
                var line = $"    inserted.{column.Name}";

                if (column != insertedColumns.Last())
                {
                    line += ",";
                }

                mergeStatement.Add(line);
            }

            mergeStatement.Add("INTO @InsertedValues");

            var scriptLines = await _scriptLineFactory.LinesFrom(
                1,
                mergeStatement.ToArray()
                );

            return await _scriptLineFactory.StringFrom(scriptLines);
        }


        private static string DataType(Column column)
        {
            var dataType = $"[{column.DataType}]";

            if (column.DataType.Equals("decimal") && column.Precision.HasValue && column.Scale.HasValue)
            {
                dataType += $"({column.Precision.Value},{column.Scale.Value})";
            }
            else if (column.MaxLength.HasValue)
            {
                dataType += $"({column.MaxLength.Value})";
            }

            return dataType;
        }

        private static string NullableColumn(Column column)
        {
            return column.Nullable ? "NULL" : "NOT NULL";
        }
    }
}
