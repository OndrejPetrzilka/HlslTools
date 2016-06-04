using System.Collections.Generic;

namespace HlslTools.Syntax
{
    public class UnityPassSyntax : SyntaxNode
    {
        public readonly SyntaxToken PassKeyword;
        public readonly SyntaxToken OpenBraceToken;
        public readonly UnityShaderTagsSyntax Tags;
        public readonly List<UnityStatePropertySyntax> StateProperties;
        public readonly UnityCgProgramSyntax CgProgram;
        public readonly SyntaxToken CloseBraceToken;

        public UnityPassSyntax(SyntaxToken passKeyword, SyntaxToken openBraceToken, UnityShaderTagsSyntax tags, List<UnityStatePropertySyntax> stateProperties, UnityCgProgramSyntax cgProgram, SyntaxToken closeBraceToken)
            : base(SyntaxKind.UnityPass)
        {
            RegisterChildNode(out PassKeyword, passKeyword);
            RegisterChildNode(out OpenBraceToken, openBraceToken);
            RegisterChildNode(out Tags, tags);
            RegisterChildNodes(out StateProperties, stateProperties);
            RegisterChildNode(out CgProgram, cgProgram);
            RegisterChildNode(out CloseBraceToken, closeBraceToken);
        }

        public override void Accept(SyntaxVisitor visitor)
        {
            visitor.VisitUnityPass(this);
        }

        public override T Accept<T>(SyntaxVisitor<T> visitor)
        {
            return visitor.VisitUnityPass(this);
        }
    }
}