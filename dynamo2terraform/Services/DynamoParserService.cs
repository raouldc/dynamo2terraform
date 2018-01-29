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
			var properties = syntaxRoot.DescendantNodes().ToList();
			return new DynamoDbTable {
				HashKey = GetHashKey(properties),
				RangeKey = GetRangeKey(properties),
				GlobalSecondaryIndexes = GetGlobalSecondaryIndexes(properties)
			};
		}

		private static List<DynamoDbGlobalSecondaryIndex> GetGlobalSecondaryIndexes(
			IEnumerable<SyntaxNode> syntaxNodes)
		{
			var nodes = GetNodesWithAttributeName<FieldDeclarationSyntax>(Constants.IndexAttribute, syntaxNodes);

			var list = new List<DynamoDbGlobalSecondaryIndex>();

			foreach (var node in nodes) {
				var variableName = node.Declaration.Variables.First().Identifier.ToString();
				var value = node.Declaration.Variables.First().Initializer.Value.ToString().Replace("\"", "");

				var projectionType = GetProjectionTypeFromNodeWithIndexAttribute(node);
				var hashKeyProperty =
					GetPropertyFromAttributeAndIndexName(Constants.GSIHashKeyAttribute, variableName, syntaxNodes);
				var rangeKeyProperty =
					GetPropertyFromAttributeAndIndexName(Constants.GSIRangeKeyAttribute, variableName, syntaxNodes);

				list.Add(new DynamoDbGlobalSecondaryIndex {
					HashKey = GetPropertyName(hashKeyProperty),
					RangeKey = GetPropertyName(rangeKeyProperty),
					Name = value,
					ProjectionType = projectionType?.ToUpper()
				});
			}

			return list;
		}

		private static string GetProjectionTypeFromNodeWithIndexAttribute(FieldDeclarationSyntax node)
		{
			return (node
				.AttributeLists
				.SelectMany(a => a.Attributes)
				.Select(x => x.ArgumentList)
				.SelectMany(a => a.Arguments)
				.First()
				.Expression as MemberAccessExpressionSyntax)?.Name.ToString();
		}

		private static PropertyDeclarationSyntax GetPropertyFromAttributeAndIndexName(string attributeName, string indexName,
			IEnumerable<SyntaxNode> syntaxNodes)
		{
			return GetNodesWithAttributeName
					<PropertyDeclarationSyntax>(attributeName, syntaxNodes)
				.First(n => n
					.AttributeLists
					.SelectMany(a => a.Attributes)
					.Select(x => x.ArgumentList)
					.SelectMany(a => a.Arguments)
					.Any(x => x.ToString() == indexName)
				);
		}

		private static IEnumerable<T> GetNodesWithAttributeName<T>(string attributeName,
			IEnumerable<SyntaxNode> syntaxNodes) where T : SyntaxNode
		{
			return syntaxNodes
				.OfType<T>()
				.Where(p => p.DescendantNodes()
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

		private static DynamoDbAttribute GetHashKey(IEnumerable<SyntaxNode> node)
		{
			var hashKeyNode = GetNodesWithAttributeName<PropertyDeclarationSyntax>(Constants.HashKeyAttribute, node)
				.FirstOrDefault();
			if (hashKeyNode == null) return null;
			return new DynamoDbAttribute {
				Name = GetPropertyName(hashKeyNode),
				Type = GetPropertyTypeAsDynamoType(hashKeyNode)
			};
		}

		private static DynamoDbAttribute GetRangeKey(IEnumerable<SyntaxNode> node)
		{
			var rangeKeyNode = GetNodesWithAttributeName<PropertyDeclarationSyntax>(Constants.RangeKeyAttribute, node)
				.FirstOrDefault();
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
