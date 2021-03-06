using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using ShaderTools.Hlsl.Compilation;
using ShaderTools.Editor.VisualStudio.Core.Parsing;
using ShaderTools.Editor.VisualStudio.Core.Tagging;
using ShaderTools.Editor.VisualStudio.Core.Util;
using ShaderTools.Editor.VisualStudio.Core.Util.Extensions;
using ShaderTools.Editor.VisualStudio.Hlsl.Parsing;
using ShaderTools.Editor.VisualStudio.Hlsl.Tagging.Highlighting.Highlighters;
using ShaderTools.Editor.VisualStudio.Hlsl.Util.Extensions;

namespace ShaderTools.Editor.VisualStudio.Hlsl.Tagging.Highlighting
{
    internal sealed class HighlightingTagger : AsyncTagger<HighlightTag>
    {
        private readonly ITextBuffer _textBuffer;
        private readonly ITextView _textView;
        private readonly ImmutableArray<IHighlighter> _highlighters;
        private readonly VisualStudioVersion _vsVersion;

        private readonly List<ITagSpan<HighlightTag>> _emptyList = new List<ITagSpan<HighlightTag>>();

        public HighlightingTagger(ITextBuffer textBuffer, BackgroundParser backgroundParser, ITextView textView, ImmutableArray<IHighlighter> highlighters, IServiceProvider serviceProvider)
        {
            backgroundParser.SubscribeToThrottledSemanticModelAvailable(BackgroundParserSubscriptionDelay.OnIdle,
                async x => await InvalidateTags(x.Snapshot, x.CancellationToken));

            textView.Caret.PositionChanged += OnCaretPositionChanged;

            _textBuffer = textBuffer;
            _textView = textView;
            _highlighters = highlighters;

            var dte = serviceProvider.GetService<SDTE, DTE>();
            _vsVersion = VisualStudioVersionUtility.FromDteVersion(dte.Version);
        }

        private async void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            await InvalidateTags(_textBuffer.CurrentSnapshot, CancellationToken.None);
        }

        protected override Tuple<ITextSnapshot, List<ITagSpan<HighlightTag>>> GetTags(ITextSnapshot snapshot, CancellationToken cancellationToken)
        {
            if (snapshot != _textBuffer.CurrentSnapshot)
                return Tuple.Create(snapshot, _emptyList);

            var unmappedPosition = _textView.GetPosition(snapshot);
            if (unmappedPosition == null)
                return Tuple.Create(snapshot, _emptyList);

            SemanticModel semanticModel;
            if (!snapshot.TryGetSemanticModel(cancellationToken, out semanticModel))
                return Tuple.Create(snapshot, _emptyList);

            var syntaxTree = semanticModel.SyntaxTree;
            var position = syntaxTree.MapRootFilePosition(unmappedPosition.Value);

            var tagSpans = semanticModel.GetHighlights(position, _highlighters)
                .Select(span => (ITagSpan<HighlightTag>) new TagSpan<HighlightTag>(
                    new SnapshotSpan(snapshot, span.Span.Start, span.Span.Length),
                    new HighlightTag(_vsVersion, span.IsDefinition)));

            return Tuple.Create(snapshot, tagSpans.ToList());
        }
    }
}