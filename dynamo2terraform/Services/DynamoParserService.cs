using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dynamo2terraform.Services
{
	public static class DynamoParserService
	{
		public static DynamoDbTable Parse(SyntaxTree tree)
		{
			var syntaxRoot = tree.GetRoot();
			var properties = syntaxRoot.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();
			return new DynamoDbTable {
				HashKey = GetHashKey(properties),
				RangeKey = GetRangeKey(properties)
			};
		}

		private static IEnumerable<PropertyDeclarationSyntax> GetNodesWithAttributeName(string attributeName,
			IEnumerable<PropertyDeclarationSyntax> syntaxNodes)
		{
			return syntaxNodes.Where(p => p.DescendantNodes()
					.OfType<AttributeSyntax>()
					.Any(a => a.DescendantNodes()
						.OfType<IdentifierNameSyntax>()
						.Any(i => i.DescendantTokens()
							.Any(dt => dt.ValueText == attributeName))))
				.ToList();
		}

		private static string GetPropertyName(PropertyDeclarationSyntax node)
		{
			return node.Identifier.ToString();
		}

		private static DynamoDbAttribute GetHashKey(IEnumerable<PropertyDeclarationSyntax> node)
		{
			var hashKeyNode = GetNodesWithAttributeName(Constants.HashKeyAttribute, node).FirstOrDefault();
			if (hashKeyNode == null) return null;
			return new DynamoDbAttribute {
				Name = GetPropertyName(hashKeyNode),
				Type = GetPropertyTypeAsDynamoType(hashKeyNode)
			};
		}

		private static DynamoDbAttribute GetRangeKey(IEnumerable<PropertyDeclarationSyntax> node)
		{
			var rangeKeyNode = GetNodesWithAttributeName(Constants.RangeKeyAttribute, node).FirstOrDefault();
			if (rangeKeyNode == null) return null;
			return new DynamoDbAttribute {
				Name = GetPropertyName(rangeKeyNode),
				Type = GetPropertyTypeAsDynamoType(rangeKeyNode)
			};
		}

		private static string GetPropertyTypeAsDynamoType(PropertyDeclarationSyntax hashKeyNode)
		{
			var type = hashKeyNode.Type.ToString();
			switch (type) {
				case "string":
					return "S";
				case "int":
					return "N";
				default:
					return null;
			}
		}
	}
}
