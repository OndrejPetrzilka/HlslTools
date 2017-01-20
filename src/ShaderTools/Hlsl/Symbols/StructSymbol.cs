﻿using System.Collections.Immutable;
using ShaderTools.Core.Symbols;
using ShaderTools.Hlsl.Syntax;
using ShaderTools.Hlsl.Binding;

namespace ShaderTools.Hlsl.Symbols
{
    public sealed class StructSymbol : ClassOrStructSymbol
    {
        public StructTypeSyntax Syntax { get; }

        internal StructSymbol(StructTypeSyntax syntax, Symbol parent, ClassOrStructSymbol baseType, ImmutableArray<InterfaceSymbol> baseInterfaces, Binder binder)
            : base(SymbolKind.Struct, (syntax.Name != null) ? syntax.Name.Text : "<anonymous struct>", parent, baseType, baseInterfaces)
        {
            Syntax = syntax;
            Binder = binder;
        }
    }
}