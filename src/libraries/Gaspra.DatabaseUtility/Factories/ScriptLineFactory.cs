﻿using Gaspra.DatabaseUtility.Interfaces;
using Gaspra.DatabaseUtility.Models.Script;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Gaspra.DatabaseUtility.Factories
{
    public class ScriptLineFactory : IScriptLineFactory
    {
        public Task<IReadOnlyCollection<ScriptLine>> LinesFrom(int indentation, params string[] lines)
        {
            var order = 0;

            var scriptLines = lines
                .Select(l => new ScriptLine(++order, l, indentation))
                .ToList();

            IReadOnlyCollection<ScriptLine> collection = new ReadOnlyCollection<ScriptLine>(scriptLines);

            return Task.FromResult(collection);
        }

        public Task<string> StringFrom(IReadOnlyCollection<ScriptLine> scriptLines)
        {
            var indent = ' ';

            var script = string.Join(
                $"{Environment.NewLine}",
                scriptLines
                    .OrderBy(s => s.Order)
                    .Select(s => $"{new string(indent, 4 * s.Indentation)}{s.Line}"));

            return Task.FromResult(script);
        }
    }
}
