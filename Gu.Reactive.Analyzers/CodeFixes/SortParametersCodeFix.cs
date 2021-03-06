namespace Gu.Reactive.Analyzers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.Threading;
    using System.Threading.Tasks;
    using Gu.Roslyn.AnalyzerExtensions;
    using Gu.Roslyn.CodeFixExtensions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SortParametersCodeFix))]
    [Shared]
    public class SortParametersCodeFix : CodeFixProvider
    {
        /// <inheritdoc/>
        public override ImmutableArray<string> FixableDiagnosticIds { get; } =
            ImmutableArray.Create(GUREA13SyncParametersAndArgs.DiagnosticId);

        /// <inheritdoc/>
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        /// <inheritdoc/>
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                          .ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.TryFindNodeOrAncestor(diagnostic, out ConstructorInitializerSyntax initializer))
                {
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            "Sort parameters.",
                            cancellationToken => ApplySortParametersAsync(context, initializer, cancellationToken),
                            nameof(SortParametersCodeFix)),
                        diagnostic);
                }
            }
        }

        private static async Task<Document> ApplySortParametersAsync(CodeFixContext context, ConstructorInitializerSyntax initializer, CancellationToken cancellationToken)
        {
            var parameterList = initializer.FirstAncestor<ConstructorDeclarationSyntax>().ParameterList;
            var parameters = new List<ParameterSyntax>(parameterList.Parameters);
            var arguments = initializer.ArgumentList.Arguments;
            parameters.Sort((x, y) => IndexOf(x, arguments).CompareTo(IndexOf(y, arguments)));
            var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken)
                                             .ConfigureAwait(false);
            for (var i = 0; i < parameterList.Parameters.Count; i++)
            {
                editor.ReplaceNode(parameterList.Parameters[i], parameters[i]);
            }

            return editor.GetChangedDocument();
        }

        private static int IndexOf(ParameterSyntax parameter, SeparatedSyntaxList<ArgumentSyntax> arguments)
        {
            return arguments.IndexOf(x => ((IdentifierNameSyntax)x.Expression).Identifier.ValueText == parameter.Identifier.ValueText);
        }
    }
}
