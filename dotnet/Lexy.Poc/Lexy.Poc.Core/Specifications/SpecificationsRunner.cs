using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Specifications
{
    public class SpecificationsRunner
    {
        private readonly LexyParser parser = new LexyParser();

        public void RunAll(string folder)
        {
            var runners = GetRunners(folder);
            Console.WriteLine($"Specifications found: {runners.Count}");
            if (runners.Count == 0)
            {
                throw new InvalidOperationException($"No specifications found");
            }

            var context = new SpecificationRunnerContext();
            runners.ForEach(runner => runner.Run(context));

            foreach (var message in context.Messages)
            {
                Console.WriteLine(message);
            }

            if (context.Failed > 0)
            {
                throw new InvalidOperationException($"Specifications failed: {context.Failed}");
            }

            Console.WriteLine($"Specifications succeed: {runners.Count}");
        }

        private IList<SpecificationRunner> GetRunners(string folder)
        {
            var absoluteFolder = GetAbsoluteFolder(folder);

            Console.WriteLine($"Specifications folder: {absoluteFolder}");

            var result = new List<SpecificationRunner>();
            AddSpecifications(result, absoluteFolder);
            return result;
        }

        private void AddSpecifications(List<SpecificationRunner> result, string folder)
        {
            var files = Directory.GetFiles(folder, "*.lexy");
            files
                .OrderBy(name => name)
                .SelectMany(ParseFile)
                .ForEach(result.Add);

            Directory.GetDirectories(folder)
                .OrderBy(name => name)
                .ForEach(folder => AddSpecifications(result, folder));
        }

        private IEnumerable<SpecificationRunner> ParseFile(string fileName)
        {
            var context = parser.ParseFile(fileName, false);

            return context
                .Components
                .GetScenarios()
                .Select(scenario => SpecificationRunner.Create(scenario, context.Components));
        }

        private static string GetAbsoluteFolder(string folder)
        {
            var absoluteFolder = Path.IsPathRooted(folder)
                ? folder
                : Path.GetFullPath(folder);
            if (!Directory.Exists(absoluteFolder))
            {
                throw new InvalidOperationException($"Specifications folder doesn't exist: {absoluteFolder}");
            }

            return absoluteFolder;
        }
    }
}