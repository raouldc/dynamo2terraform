using dynamo2terraform.Services;

namespace dynamo2terraform
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var file =
				"/Users/raoul/Documents/Development/dynamo2terraform/dyanmo2terraform/dyanmo2terraform/TestFiles/Models/ApplicationGrant.cs";
			var tree = ClassLoader.GetSyntaxTreeFromPath(file);
			var table = DynamoParserService.Parse(tree);
		}
	}
}
