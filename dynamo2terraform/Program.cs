using System;
using System.Collections.Generic;
using System.IO;
using dynamo2terraform.Services;
using Fluid;
using Microsoft.Extensions.CommandLineUtils;

namespace dynamo2terraform
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var app = new CommandLineApplication {
				Name = "dynamo2terraform"
			};
			app.HelpOption("-?|-h|--help");

			var inputFilePathOption = app.Option("-i|--input <path>",
				"The path to the input C# DynamoDB Model decorated with DynamoDBAttributes for parsing",
				CommandOptionType.SingleValue);

			var templateFilePathOption = app.Option("-t|--template <path>",
				"The path to the liquid template to be used for generating the output",
				CommandOptionType.SingleValue);

			app.OnExecute(() => {
				if (!inputFilePathOption.HasValue() || !File.Exists(inputFilePathOption.Value())) {
					Console.WriteLine("Could not find Input file at the path provided");
					return 0;
				}

				if (!templateFilePathOption.HasValue() || !File.Exists(templateFilePathOption.Value())) {
					Console.WriteLine("Could not find Liquid template file at path provided");
					return 0;
				}

				var tree = ClassLoader.GetSyntaxTreeFromPath(inputFilePathOption.Value());
				var table = DynamoParserService.Parse(tree);

				var liquidTemplate = File.ReadAllText(templateFilePathOption.Value());


				IEnumerable<string> errors;
				if (FluidTemplate.TryParse(liquidTemplate, out var template, out errors)) {
					var context = new TemplateContext();
					context.MemberAccessStrategy.Register(typeof(DynamoDbTable)); // Allows any public property of the model to be used
					context.MemberAccessStrategy.Register(typeof(DynamoDbAttribute));
					context.MemberAccessStrategy.Register(typeof(DynamoDbGlobalSecondaryIndex));
					context.SetValue("table", table);

					Console.WriteLine(template.Render(context));
				}

				return 0;
			});

			app.Execute(args);
		}
	}
}
