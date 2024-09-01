using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;

namespace Colder.Api.Abstractions;

internal class ControllerFilterProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        Console.WriteLine("[NSWAG] CONTROLLER " + context.ControllerType.Name + " " + context.MethodInfo.Name);
        return true;
    }
}
