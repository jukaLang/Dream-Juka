﻿using System;
using System.Collections.Generic;

namespace DreamCompiler.RoslynCompile
{
    using Antlr4.Runtime.Tree;
    using DreamCompiler.Visitors;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using DreamCompiler.Constants;

    class GenerateStaticMethod
    {
    //    static public MethodDeclarationSyntax CreateStaticMethod(DreamGrammarParser.FunctionDeclarationContext context, DreamRoslynVisitor visitor)
    //    {
    //        String methodName = context.funcName().GetText();
    //        //Since C# main methods musbe be named Main this converts to C# style.
    //        if (methodName.Equals(Constants.JuiliarMain))
    //        {
    //            methodName = Constants.RoslynMain;
    //        }

    //        SyntaxToken name = SyntaxFactory.Identifier(methodName);
    //        SyntaxToken returnType = SyntaxFactory.Lexeme(SyntaxKind.VoidKeyword);

    //        var statementList = new List<StatementSyntax>();
    //        var tlist = new List<SyntaxNodeOrToken>();

    //        foreach (IParseTree child in context.children)
    //        {
    //            CSharpSyntaxNode node = child.Accept(visitor);
    //            if (node != null)
    //            {
    //                if (node is ParameterSyntax)
    //                {
    //                    tlist.Add((ParameterSyntax)node);
    //                }
    //                else if (node is StatementSyntax)
    //                {
    //                    statementList.Add(node as StatementSyntax);
    //                }
    //            }
    //        }

    //        var block = SyntaxFactory.Block().AddStatements(statementList.ToArray());

    //        if (tlist.Count > 0)
    //        {
    //            return SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(returnType), name).WithBody( block)
    //                .WithModifiers( SyntaxFactory.TokenList( new[]{
    //                    SyntaxFactory.Lexeme(SyntaxKind.StaticKeyword),
    //                    SyntaxFactory.Lexeme(SyntaxKind.PublicKeyword)}))
    //                    .WithParameterList( SyntaxFactory.ParameterList( SyntaxFactory.SeparatedList<ParameterSyntax>(tlist)));
    //        }

    //        return SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(returnType), name)
    //            .WithBody(
    //                block)
    //                 .WithModifiers(
    //        SyntaxFactory.TokenList(
    //            new[]{
    //                SyntaxFactory.Lexeme(SyntaxKind.StaticKeyword),
    //                SyntaxFactory.Lexeme(SyntaxKind.PublicKeyword)}));
    //    }
    }
}
