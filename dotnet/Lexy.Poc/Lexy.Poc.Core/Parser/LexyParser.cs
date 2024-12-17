using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lexy.Poc.Core.Language;

namespace Lexy.Poc.Core.Parser
{
    public class LexyParser
    {
        public Components ParseFile(string fileName)
        {
            var code = File.ReadAllLines(fileName);

            return Parse(code);
        }

        public Components Parse(string[] code)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));

            var context = new ParserContext();
            var components = new Components();
            var currentIndent = 0;

            IRootComponent root = null;
            IComponent currentComponent = null;

            var componentStack = new Stack<IComponent>();
            var lines = code.Select((line, index) => new Line(index, line, code));

            foreach (var line in lines)
            {
                context.ProcessLine(line);

                var indent = line.Indent();
                if (indent == 0 && !line.IsComment() && !line.IsEmpty())
                {
                    root = GetToken(line, context);
                    components.Add(root);

                    currentComponent = root;
                    currentIndent = indent;

                    componentStack.Clear();
                    continue;
                }

                if (line.IsComment() || line.IsEmpty())
                {
                    currentComponent?.Parse(context);
                    continue;
                }

                if (currentComponent == null)
                {
                    context.Fail("Unexpected line: " + line.Content);
                    continue;
                }

                if (indent <= currentIndent)
                {
                    currentComponent = componentStack.Pop();
                }

                try
                {
                    var component = currentComponent.Parse(context);
                    if (component != currentComponent)
                    {
                        componentStack.Push(currentComponent);
                        currentComponent = component;
                        currentIndent = indent;
                    }
                }
                catch (Exception e)
                {
                    currentComponent = null;
                }
            }

            return components;
        }

        private IRootComponent GetToken(Line line, ParserContext context)
        {
            var tokenName = ComponentName.Parse(line, context);

            return tokenName?.Name switch
            {
                null => null,
                TokenNames.FunctionComponent => Function.Parse(tokenName),
                TokenNames.EnumComponent => EnumDefinition.Parse(tokenName),
                TokenNames.ScenarioComponent => Scenario.Parse(tokenName),
                TokenNames.TableComponent => Table.Parse(tokenName),
                _ => throw new InvalidOperationException($"Unknown keyword: {tokenName.Name}")
            };
        }
    }
}