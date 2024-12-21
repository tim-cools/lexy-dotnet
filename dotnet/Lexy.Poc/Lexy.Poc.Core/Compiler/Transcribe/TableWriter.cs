using System;
using Lexy.Poc.Core.Language;

namespace Lexy.Poc.Core.Transcribe
{
    internal class TableWriter : IRootTokenWriter
    {
        public GeneratedClass CreateCode(ClassWriter writer, IRootComponent component, Components components)
        {
            if (!(component is Table table))
            {
                throw new InvalidOperationException("Root token not table");
            }

            var className = table.Name.Value;
            var rowName = $"{className}Row";

            writer.OpenScope($"public class {className}");

            RenderRowClass(writer, components, rowName, table);

            writer.WriteLine($"private static IList<{rowName}> _value = new List<{rowName}>();");

            WriteStaticConstructur(writer, className, table, rowName);

            writer.WriteLine($"public int Count => _value.Count;");
            writer.WriteLine($"public IEnumerable<{rowName}> Value => _value;");
            writer.CloseScope();

            return new GeneratedClass(table, className);
        }

        private static void WriteStaticConstructur(ClassWriter writer, string className, Table table, string rowName)
        {
            writer.OpenScope($"static {className}()");

            foreach (var row in table.Rows)
            {
                writer.OpenScope($"_value.Add(new {rowName}");
                for (var index = 0; index < table.Headers.Values.Count; index++)
                {
                    var header = table.Headers.Values[index];
                    var value = row.Values[index];
                    writer.WriteLine($"{header.Name} = {value.Value},");
                }

                writer.CloseScope(");");
            }

            writer.CloseScope();
        }

        private static void RenderRowClass(ClassWriter writer, Components components, string rowName, Table table)
        {
            writer.OpenScope($"public class {rowName}");

            foreach (var header in table.Headers.Values)
            {
                var type = components.MapType(header.Type);
                writer.WriteLine($"public {type} {header.Name} {{ get; set; }}");
            }

            writer.CloseScope();
        }
    }
}