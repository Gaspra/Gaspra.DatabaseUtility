﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace Gaspra.Functions.Correlation.Interfaces
{
    public interface ICorrelationContext
    {
        Guid FunctionCorrelationId { get; }
        DateTimeOffset FunctionTimestamp { get; }
        CancellationTokenSource FunctionCancellationSource { get; }
        string FunctionName { get; set; }
        IEnumerable<IFunctionParameter> FunctionParameters { get; set; }
    }
}
