using System.Collections.Generic;
using ShaderTools.Core.Diagnostics;
using ShaderTools.Core.Text;

namespace ShaderTools.Hlsl.Syntax
{
    public abstract class LocatedNode : SyntaxNode
    {
        public TextSpan Span { get; }
        public string Text { get; }

        protected LocatedNode(SyntaxKind kind, string text, TextSpan span, IEnumerable<Diagnostic> diagnostics)
            : base(kind, diagnostics)
        {
            Text = text;
            Span = span;
        }
    }
}