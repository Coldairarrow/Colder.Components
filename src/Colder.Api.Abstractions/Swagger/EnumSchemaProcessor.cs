using Namotion.Reflection;
using NJsonSchema.Generation;
using System;
using System.Linq;
using System.Text;

namespace Colder.Api.Abstractions.Swagger;

/// <summary>
/// 
/// </summary>
internal class EnumSchemaProcessor : ISchemaProcessor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public void Process(SchemaProcessorContext context)
    {
        var type = context.ContextualType.Type;
        if (!type.IsEnum)
        {
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine(type.GetXmlDocsSummary());

        var members = type.GetMembers();
        var schema = context.Schema;
        for (var i = 0; i < schema.Enumeration.Count; i++)
        {
            var item = schema.Enumeration.ElementAt(i);
            var enumName = Enum.GetName(type, item);

            var summary = members.FirstOrDefault(a => a.Name == enumName)
                                 .GetXmlDocsSummary();

            sb.AppendLine($"{item} = {summary}");
        }

        schema.Description = sb.ToString();
    }
}
