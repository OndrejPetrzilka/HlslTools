﻿using System.Collections.Generic;
using System.Linq;
using ShaderTools.Core.Text;
using ShaderTools.Hlsl.Syntax;
using Xunit;

namespace ShaderTools.Tests.Hlsl.Syntax
{
    public class SyntaxTreeNavigationTests
    {
        [Fact]
        public void TestGetPreviousTokenIncludingSkippedTokens()
        {
            var text =
@"cbuffer Globals {
  int a;
  garbage
  int b;
}";
            var tree = SyntaxFactory.ParseSyntaxTree(SourceText.From(text));
            Assert.Equal(text, tree.Root.ToFullString());

            var tokens = tree.Root.DescendantTokens(descendIntoTrivia: true).Where(t => t.Span.Length > 0).ToList();
            Assert.Equal(11, tokens.Count);
            Assert.Equal("garbage", tokens[6].Text);

            var list = new List<SyntaxToken>(tokens.Count);
            var token = tree.Root.GetLastToken(includeSkippedTokens: true);
            while (token != null)
            {
                list.Add(token);
                token = token.GetPreviousToken(includeSkippedTokens: true);
            }
            list.Reverse();

            Assert.Equal(tokens.Count, list.Count);
            for (int i = 0; i < tokens.Count; i++)
            {
                Assert.Equal(list[i], tokens[i]);
            }
        }
    }
}