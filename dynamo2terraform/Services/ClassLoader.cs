using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace dynamo2terraform.Services
{
	public static class ClassLoader
	{
		public static SyntaxTree GetSyntaxTreeFromPath(string path)
		{
			using (var stream = File.OpenRead(path)) {
				return CSharpSyntaxTree.ParseText(SourceText.From(stream), path: path);
			}
		}
	}
}
