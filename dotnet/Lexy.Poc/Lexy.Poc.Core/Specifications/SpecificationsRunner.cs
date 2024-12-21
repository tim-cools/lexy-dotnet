using System;
using System.IO;
using System.Linq;
using Lexy.Poc.Core.Parser;

namespace Lexy.Poc.Core.Specifications
{
    public class SpecificationsRunner : ISpecificationsRunner
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ISpecificationRunnerContext context;

        public SpecificationsRunner(IServiceProvider serviceProvider, ISpecificationRunnerContext context)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.context = context;
        }

        public void RunAll(string folder)
        {
            GetRunners(folder);

            var runners = context.FileRunners;
            var countScenarios = context.CountScenarios();
            Console.WriteLine($"Specifications found: {countScenarios}");
            if (runners.Count == 0)
            {
                throw new InvalidOperationException($"No specifications found");
            }

            runners.ForEach(runner => runner.Run());

            context.LogGlobal($"Specifications succeed: {countScenarios - context.Failed} / {countScenarios}");

            foreach (var message in context.Messages)
            {
                Console.WriteLine(message);
            }

            if (context.Failed > 0)
            {
                Failed(context);
            }
        }

        private static void Failed(ISpecificationRunnerContext context)
        {
            Console.WriteLine("--------------- FAILED PARSER LOGGING ---------------");
            foreach (var runner in context.FailedScenariosRunners())
            {
                Console.WriteLine(runner.ParserLogging());
            }

            throw new InvalidOperationException($"Specifications failed: {context.Failed}");
        }

        private void GetRunners(string folder)
        {
            var absoluteFolder = GetAbsoluteFolder(folder);

            Console.WriteLine($"Specifications folder: {absoluteFolder}");

            AddFolder(absoluteFolder);
        }

        private void AddFolder(string folder)
        {
            var files = Directory.GetFiles(folder, $"*.{LexySourceDocument.FileExtension}");

            files
                .OrderBy(name => name)
                .Select(fileName => SpecificationFileRunner.Create(fileName, serviceProvider, context))
                .ForEach(context.Add);

            Directory.GetDirectories(folder)
                .OrderBy(name => name)
                .ForEach(AddFolder);
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