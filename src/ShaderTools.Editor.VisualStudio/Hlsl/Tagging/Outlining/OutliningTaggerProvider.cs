﻿using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using ShaderTools.Editor.VisualStudio.Core.Tagging;
using ShaderTools.Editor.VisualStudio.Hlsl.Options;
using ShaderTools.Editor.VisualStudio.Hlsl.Util.Extensions;

namespace ShaderTools.Editor.VisualStudio.Hlsl.Tagging.Outlining
{
    [Export(typeof(ITaggerProvider))]
    [Export(typeof(OutliningTaggerProvider))]
    [ContentType(HlslConstants.ContentTypeName)]
    [TagType(typeof(IOutliningRegionTag))]
    internal sealed class OutliningTaggerProvider : ITaggerProvider
    {
        [Import]
        public IHlslOptionsService OptionsService { get; set; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return AsyncTaggerUtility.CreateTagger<OutliningTagger, T>(buffer,
                () => new OutliningTagger(buffer, buffer.GetBackgroundParser(), OptionsService));
        }
    }
}