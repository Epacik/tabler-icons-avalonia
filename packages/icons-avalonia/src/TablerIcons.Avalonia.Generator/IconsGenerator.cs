using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TablerIcons.Avalonia.Generator
{
    [Generator]
    internal class IconsGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new GenerateIconsSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var receiver = context.SyntaxReceiver as GenerateIconsSyntaxReceiver;
            if (receiver is null)
            {
                return;
            }

            var currentDir = Directory.GetCurrentDirectory();

            foreach (var p in receiver.IconsPaths)
            {
                var path = Path.Combine(currentDir, p);
                if (!Directory.Exists(path))
                    continue;

                var files = GetSvgPaths(Directory.GetFiles(path));

            }
        }

        private IEnumerable<(string name, bool filled, (string path, string fill, string stroke) data)>
            GetSvgPaths(string[] paths)
        {
            foreach (var path in paths)
            {
                var content = File.ReadAllText(path);

                var name = Path.GetFileNameWithoutExtension(path);
                var filled = path.Contains("-filled");


                yield return ("", false, ("", "", ""));
            }
        }
    }

    internal class GenerateIconsSyntaxReceiver : ISyntaxReceiver
    {
        public List<string> IconsPaths { get; } = new List<string>();

        public void OnVisitSyntaxNode(SyntaxNode node)
        {
            if (node is AttributeSyntax attributeSyntax)
            {
                var collector = new GenerateIconsAttributeCollector();
                attributeSyntax.Accept(collector);
                IconsPaths.AddRange(collector.IconsPaths);
            }
        }
    }

    internal class GenerateIconsAttributeCollector : CSharpSyntaxVisitor
    {
        private static readonly string[] attributeNames = { "GenerateIcons", "GenerateIconsAttribute" };
        public List<string> IconsPaths { get; } = new List<string>();

        public override void VisitAttribute(AttributeSyntax node)
        {
            base.VisitAttribute(node);

            if (!attributeNames.Contains(node.Name.ToString()))
            {
                return;
            }

            var paths = new List<string>();

            var arguments = node.ArgumentList?.Arguments.ToArray() ?? Array.Empty<AttributeArgumentSyntax>();
            foreach (var syntax in arguments)
            {
                if (syntax.Expression is LiteralExpressionSyntax exp && exp.Kind() == SyntaxKind.StringLiteralExpression)
                {
                    paths.Add(syntax.ToString().Trim('"'));
                }
            }

            IconsPaths.AddRange(paths);
        }
    }
}
