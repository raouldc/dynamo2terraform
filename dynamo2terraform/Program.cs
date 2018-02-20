using System;
using System.Collections.Generic;
using System.IO;
using dynamo2terraform.Services;
using Fluid;

namespace dynamo2terraform
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var file =
				"../dynamo2terraform/TestFiles/Models/Person.cs";
			var tree = ClassLoader.GetSyntaxTreeFromPath(file);
			var table = DynamoParserService.Parse(tree);

			var sourcePath = "../dynamo2terraform/Templates/terraform.liquid";
			var source = File.ReadAllText(sourcePath);


			IEnumerable<string> errors;
			if (FluidTemplate.TryParse(source, out var template, out errors)) {
				var context = new TemplateContext();
				context.MemberAccessStrategy.Register(typeof(DynamoDbTable)); // Allows any public property of the model to be used
				context.MemberAccessStrategy.Register(typeof(DynamoDbAttribute));
				context.MemberAccessStrategy.Register(typeof(DynamoDbGlobalSecondaryIndex));
				context.SetValue("table", table);

				Console.WriteLine(template.Render(context));
			}
		}
	}
}
