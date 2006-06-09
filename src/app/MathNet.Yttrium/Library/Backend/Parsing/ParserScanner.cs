#region Copyright 2001-2006 Christoph Daniel Rüegg [GPL]
//Math.NET Symbolics: Yttrium, part of Math.NET
//Copyright (c) 2001-2006, Christoph Daniel Rueegg, http://cdrnet.net/.
//All rights reserved.
//This Math.NET package is available under the terms of the GPL.

//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Exceptions;
using MathNet.Symbolics.StdPackage.Structures;
using MathNet.Symbolics.Workplace;

namespace MathNet.Symbolics.Backend.Parsing
{
    public class ParserScanner
    {
        private ParserMarker tokenizer;
        private Context context;
        private MathSystem system;

        public ParserScanner(TextReader reader, MathSystem system)
        {
            tokenizer = new ParserMarker(reader, system.Context);
            this.system = system;
            this.context = system.Context;
        }

        #region OLD System Structures
        //private bool IsSystemEndStructure(LexerToken token)
        //{
        //    if(!token.IsType(TokenTypes.SystemTextIdentifier))
        //        return false;
        //    return !(token.Text.Equals("if") || token.Text.Equals("while") || token.Text.Equals("for"));
        //}
        //private MathExpression ScanSystemStructure()
        //{
        //    LexerToken ahead = tokenizer.LookaheadFistToken;
        //    if(ahead.Text == "if")
        //        return ScanCondition();
        //    if(ahead.Text == "while")
        //        return ScanWhileLoop();
        //    if(ahead.Text == "for")
        //        return ScanForLoop();
        //    throw new UnexpectedTokenParserException(ahead);
        //}
        //private MathCondition ScanCondition()
        //{
        //    tokenizer.Match(TokenTypes.SystemTextIdentifier, "if");
        //    MathCondition condition = ScanSubCondition();
        //    tokenizer.Match(TokenTypes.SystemTextIdentifier, "end");
        //    return condition;
        //}
        //private MathCondition ScanSubCondition()
        //{
        //    MathExpression condition = ScanTerm();
        //    tokenizer.Match(TokenTypes.SystemTextIdentifier, "then");
        //    MathExpressionCollection trueStatements = ScanMultipleScopes();
        //    if(tokenizer.LookaheadFistToken.IsType(TokenTypes.SystemTextIdentifier))
        //    {
        //        if(tokenizer.LookaheadFistToken.Text == "elseif")
        //        {
        //            tokenizer.Match(TokenTypes.SystemTextIdentifier, "elseif");
        //            MathCondition sub = ScanSubCondition();
        //            return new MathCondition(condition, trueStatements, new MathExpressionCollection(new MathExpression[] { sub }));
        //        }
        //        if(tokenizer.LookaheadFistToken.Text == "else")
        //        {
        //            tokenizer.Match(TokenTypes.SystemTextIdentifier, "else");
        //            return new MathCondition(condition, trueStatements, ScanMultipleScopes());
        //        }
        //    }
        //    return new MathCondition(condition, trueStatements);
        //}
        //private MathWhileLoop ScanWhileLoop()
        //{
        //    tokenizer.Match(TokenTypes.SystemTextIdentifier, "while");
        //    MathExpression condition = ScanTerm();
        //    tokenizer.Match(TokenTypes.SystemTextIdentifier, "do");
        //    MathExpressionCollection statements = ScanMultipleScopes();
        //    tokenizer.Match(TokenTypes.SystemTextIdentifier, "end");
        //    return new MathWhileLoop(condition, statements);
        //}
        //private MathForLoop ScanForLoop()
        //{
        //    tokenizer.Match(TokenTypes.SystemTextIdentifier, "for");
        //    MathExpression condition = ScanTerm();
        //    tokenizer.Match(TokenTypes.SystemTextIdentifier, "do");
        //    MathExpressionCollection statements = ScanMultipleScopes();
        //    tokenizer.Match(TokenTypes.SystemTextIdentifier, "end");
        //    return new MathForLoop(condition, statements);
        //}
        #endregion

        public void NextStatement()
        {
            ScanCommand();
            while(tokenizer.LookaheadFistToken.IsType(TokenTypes.Executor))
                tokenizer.Match(TokenTypes.Executor);
        }

        public void AllStatements()
        {
            while(!tokenizer.IsEndOfFile)
            {
                ScanCommand();
                if(!tokenizer.LookaheadFistToken.IsType(TokenTypes.Executor))
                    break;
                while(tokenizer.LookaheadFistToken.IsType(TokenTypes.Executor))
                    tokenizer.Match(TokenTypes.Executor);
            }
        }

        private void ScanCommand()
        {
            LexerToken t = tokenizer.LookaheadFistToken;
            if(t.IsType(TokenTypes.DefineKeyword))
                ScanDefine();
            else if(t.IsType(TokenTypes.InstantiateKeyword))
                ScanInstantiate();
            else if(t.IsType(TokenTypes.SignalKeyword))
                ScanSignalDeclaration();
            else if(t.IsType(TokenTypes.BusKeyword))
                ScanBusDeclaration();
            else if(t.IsType(TokenTypes.TextIdentifier) && tokenizer.LookaheadToken(1).IsType(TokenTypes.Assignment))
                ScanSignalAssignment();
            else
                ScanSignalExpression();
        }

        #region Define
        private void ScanDefine()
        {
            tokenizer.Match(TokenTypes.DefineKeyword, "define");
            switch(tokenizer.LookaheadFistToken.Text)
            {
                case "entity":
                    ScanDefineEntity(); break;
                case "architecture":
                    ScanDefineArchitecture(); break;
                default:
                    throw new ParsingException("Parsing failed. Parser Scanner detected unexpected definition token '" + tokenizer.LookaheadFistToken.Text + "'");
            }
        }

        private Entity ScanDefineEntity()
        {
            tokenizer.Match(TokenTypes.TextIdentifier, "entity");
            MathIdentifier entityId = ScanEntityMathIdentifierOrLabel(true);
            string symbol = ScanLiteral();
            InfixNotation notation;
            switch(tokenizer.MatchGet(TokenTypes.TextIdentifier).Text)
            {
                case "pre":
                    notation = InfixNotation.PreOperator; break;
                case "post":
                    notation = InfixNotation.PostOperator; break;
                case "left":
                    notation = InfixNotation.LeftAssociativeInnerOperator; break;
                case "right":
                    notation = InfixNotation.RightAssociativeInnerOperator; break;
                default:
                    notation = InfixNotation.None; break;
            }
            int precedence = -1;
            if(tokenizer.LookaheadFistToken.IsType(TokenTypes.Integer))
                precedence = (int)ScanInteger();

            string[] inputs, outputs, buses;
            if(tokenizer.LookaheadFistToken.Text == "in")
            {
                tokenizer.Match(TokenTypes.TextIdentifier,"in");
                inputs = ScanTextIdentifierList().ToArray();
            }
            else
                inputs = new string[0];
            if(tokenizer.LookaheadFistToken.Text == "out")
            {
                tokenizer.Match(TokenTypes.TextIdentifier, "out");
                outputs = ScanTextIdentifierList().ToArray();
            }
            else
                outputs = new string[0];
            if(tokenizer.LookaheadFistToken.Text == "bus")
            {
                tokenizer.Match(TokenTypes.TextIdentifier, "bus");
                buses = ScanTextIdentifierList().ToArray();
            }
            else
                buses = new string[0];
            Entity entity = new Entity(symbol, entityId,notation,precedence, inputs, outputs, buses);
            context.Library.Entities.Add(entity);
            return entity;
        }

        private void ScanDefineArchitecture()
        {
            tokenizer.Match(TokenTypes.TextIdentifier, "architecture");
            MathIdentifier architectureId = ScanEntityMathIdentifierOrLabel(true);
            MathIdentifier entityId = ScanEntityMathIdentifierOrLabel(true);
            Entity entity = context.Library.LookupEntity(entityId);
            tokenizer.Match("{");
            MathSystem originalSystem = system;
            MathSystem tempSystem = new MathSystem(system.Context);
            system = tempSystem;
            while(tokenizer.LookaheadFistToken.Text != "}")
                NextStatement();
            foreach(string input in entity.InputSignals)
                tempSystem.PromoteAsInput(tempSystem.LookupNamedSignal(input));
            foreach(string output in entity.OutputSignals)
                tempSystem.AddSignalTree(tempSystem.LookupNamedSignal(output), tempSystem.GetAllInputs(), true, false);
            ReadOnlyCollection<Signal> leafs = tempSystem.GetAllLeafSignals();
            foreach(Signal s in leafs)
            {
                Signal so;
                if(originalSystem.TryLookupNamedSignal(s.Label, out so) && so.Value != null)
                    s.PostNewValue(so.Value);
            }
            context.Scheduler.SimulateInstant();
            system = originalSystem;
            tokenizer.Match("}");
            tempSystem.RemoveUnusedObjects();
            tempSystem.PublishToLibrary(architectureId, entity.EntityId);
        }
        #endregion

        #region Instantiate
        private Port ScanInstantiate()
        {
            tokenizer.Match(TokenTypes.InstantiateKeyword, "instantiate");
            Entity entity = ScanEntity();
            Signal[] inputs; //= new Signal[entity.InputSignals.Length];
            Signal[] outputs; // = new Signal[entity.OutputSignals.Length];
            Bus[] buses; // = new Bus[entity.Buses.Length];
            if(tokenizer.LookaheadFistToken.Text == "in")
            {
                tokenizer.Match(TokenTypes.TextIdentifier, "in");
                if(tokenizer.LookaheadToken(1).IsType(TokenTypes.Association))
                {
                    List<SignalAssociation> associations = ScanSignalExpressionAssociationList();
                    inputs = new Signal[entity.InputSignals.Length];
                    foreach(SignalAssociation sa in associations)
                        inputs[Array.IndexOf<string>(entity.InputSignals, sa.Label)] = sa.Association;
                }
                else
                {
                    List<Signal> associations = ScanSignalExpressionList();
                    inputs = associations.ToArray();
                }
            }
            else
                inputs = new Signal[entity.InputSignals.Length];
            if(tokenizer.LookaheadFistToken.Text == "out")
            {
                tokenizer.Match(TokenTypes.TextIdentifier, "out");
                if(tokenizer.LookaheadToken(1).IsType(TokenTypes.Association))
                {
                    List<SignalAssociation> associations = ScanSignalAssociationList();
                    outputs = new Signal[entity.OutputSignals.Length];
                    foreach(SignalAssociation sa in associations)
                        outputs[Array.IndexOf<string>(entity.OutputSignals, sa.Label)] = sa.Association;
                }
                else
                {
                    List<Signal> associations = ScanSignalList();
                    outputs = associations.ToArray();
                }
            }
            else
                outputs = new Signal[entity.OutputSignals.Length];
            if(tokenizer.LookaheadFistToken.Text == "bus")
            {
                tokenizer.Match(TokenTypes.TextIdentifier, "bus");
                if(tokenizer.LookaheadToken(1).IsType(TokenTypes.Association))
                {
                    List<BusAssociation> associations = ScanBusAssociationList();
                    buses = new Bus[entity.Buses.Length];
                    foreach(BusAssociation ba in associations)
                        buses[Array.IndexOf<string>(entity.Buses, ba.Label)] = ba.Association;
                }
                else
                {
                    List<Bus> associations = ScanBusList();
                    buses = associations.ToArray();
                }
            }
            else
                buses = new Bus[entity.Buses.Length];
            Port port = entity.InstantiatePort(context, inputs, outputs, buses);
            system.AddPortTree(port, false, false);
            return port;
        }
        #endregion

        #region Signal Declaration & Assignment
        private Signal ScanSignalDeclaration()
        {
            tokenizer.Match(TokenTypes.SignalKeyword, "signal");
            if(tokenizer.LookaheadToken(1).IsType(TokenTypes.Assignment))
                return ScanSignalAssignment();
            else
            {
                string name = tokenizer.MatchGet(TokenTypes.TextIdentifier).Text;
                return system.CreateNamedSignal(name); 
            }
        }

        private Signal ScanSignalAssignment()
        {
            string signalName = ScanTextIdentifier();
            tokenizer.Match(TokenTypes.Assignment, "<-");
            Signal s = ScanSignalExpression();
            system.AddSignalTree(s, false, false);
            system.AddNamedSignal(signalName, s);
            return s;
        }
        #endregion

        #region Bus Declaration
        private Bus ScanBusDeclaration()
        {
            tokenizer.Match(TokenTypes.BusKeyword, "bus");
            string name = tokenizer.MatchGet(TokenTypes.TextIdentifier).Text;
            return system.CreateNamedBus(name);
        }
        #endregion

        #region Association
        private struct SignalAssociation
        {
            public string Label;
            public Signal Association;
        }
        private struct BusAssociation
        {
            public string Label;
            public Bus Association;
        }
        private SignalAssociation ScanSignalAssociation()
        {
            SignalAssociation sa;
            sa.Label = ScanTextIdentifier();
            tokenizer.Match(TokenTypes.Association, "->");
            sa.Association = ScanSignal();
            return sa;
        }
        private List<SignalAssociation> ScanSignalAssociationList()
        {
            List<SignalAssociation> associations = new List<SignalAssociation>();
            associations.Add(ScanSignalAssociation());
            while(tokenizer.LookaheadFistToken.IsType(TokenTypes.Separator))
            {
                tokenizer.Match(TokenTypes.Separator);
                associations.Add(ScanSignalAssociation());
            }
            return associations;
        }
        private SignalAssociation ScanSignalExpressionAssociation()
        {
            SignalAssociation sa;
            sa.Label = ScanTextIdentifier();
            tokenizer.Match(TokenTypes.Association, "->");
            sa.Association = ScanSignalExpression();
            return sa;
        }
        private List<SignalAssociation> ScanSignalExpressionAssociationList()
        {
            List<SignalAssociation> associations = new List<SignalAssociation>();
            associations.Add(ScanSignalExpressionAssociation());
            while(tokenizer.LookaheadFistToken.IsType(TokenTypes.Separator))
            {
                tokenizer.Match(TokenTypes.Separator);
                associations.Add(ScanSignalExpressionAssociation());
            }
            return associations;
        }
        private BusAssociation ScanBusAssociation()
        {
            BusAssociation ba;
            ba.Label = ScanTextIdentifier();
            tokenizer.Match(TokenTypes.Association, "->");
            ba.Association = ScanBus();
            return ba;
        }
        private List<BusAssociation> ScanBusAssociationList()
        {
            List<BusAssociation> associations = new List<BusAssociation>();
            associations.Add(ScanBusAssociation());
            while(tokenizer.LookaheadFistToken.IsType(TokenTypes.Separator))
            {
                tokenizer.Match(TokenTypes.Separator);
                associations.Add(ScanBusAssociation());
            }
            return associations;
        }
        #endregion

        #region Left Unary
        private bool IsLeftUnary(LexerToken token)
        {
            return (token.IsType(TokenTypes.SymbolIdentifier) || token.IsType(TokenTypes.TextIdentifier))
                && context.Library.Entities.ContainsSymbol(token.Text, InfixNotation.PreOperator);
        }
        private bool IsRightUnary(LexerToken token)
        {
            return (token.IsType(TokenTypes.SymbolIdentifier) || token.IsType(TokenTypes.TextIdentifier))
                && context.Library.Entities.ContainsSymbol(token.Text, InfixNotation.PostOperator);
        }
        private Signal ScanLeftUnary()
        {
            LexerToken token = tokenizer.ConsumeGet();
            Signal s = ScanOperand();
            return context.Builder.Function(token.Text, InfixNotation.PreOperator, s);
        }
        #endregion

        #region Signal Expression
        private bool IsBinary(LexerToken token)
        {
            return (token.IsType(TokenTypes.SymbolIdentifier) || token.IsType(TokenTypes.TextIdentifier))
                && (context.Library.Entities.ContainsSymbol(token.Text,InfixNotation.LeftAssociativeInnerOperator)
                || context.Library.Entities.ContainsSymbol(token.Text, InfixNotation.RightAssociativeInnerOperator));
        }
        private static bool IsPrecedenceLeftFirst(Entity left, Entity right)
        {
            if(left.PrecedenceGroup < right.PrecedenceGroup)
                return true;
            if(left.PrecedenceGroup > right.PrecedenceGroup)
                return false;
            if(left.Notation == InfixNotation.LeftAssociativeInnerOperator)
                return true;
            if(right.Notation == InfixNotation.RightAssociativeInnerOperator)
                return false;
            return true;
        }
        private Signal ScanSignalExpression()
        {
            /*
                This method is kind of a postfix machine, parsing
                infix expressions to postfix expressions (having
                regard to precedence) using the operator stack and
                evaluates the postfix term to an expression tree
                using the expression stack.
            */

            SignalStack expressionStack = new SignalStack(4, 4096);
            EntityStack operatorStack = new EntityStack(4, 4096);

            expressionStack.Push(ScanOperand());
            Entity lastEntity, topEntity;

            MathNet.Symbolics.Backend.Containers.EntityTable entityTable = context.Library.Entities;
            Library library = context.Library;

            while(true)
            {
                LexerToken ahead = tokenizer.LookaheadFistToken;
                if(!(ahead.IsType(TokenTypes.SymbolIdentifier) || ahead.IsType(TokenTypes.TextIdentifier)))
                    break;
                if(entityTable.ContainsSymbol(ahead.Text,InfixNotation.LeftAssociativeInnerOperator))
                    lastEntity = library.LookupEntity(ahead.Text, InfixNotation.LeftAssociativeInnerOperator, 2, 1, 0);
                else if(entityTable.ContainsSymbol(ahead.Text,InfixNotation.RightAssociativeInnerOperator))
                    lastEntity = library.LookupEntity(ahead.Text, InfixNotation.RightAssociativeInnerOperator, 2, 1, 0);
                else
                    break;
                tokenizer.Consume();

                while(operatorStack.Count > 0 && IsPrecedenceLeftFirst(operatorStack.Peek(), lastEntity))
                {
                    topEntity = operatorStack.Pop();
                    Signal swap = expressionStack.Pop();
                    Signal s = context.Builder.Function(topEntity, expressionStack.Pop(), swap);
                    expressionStack.Push(s);
                }

                operatorStack.Push(lastEntity);
                expressionStack.Push(ScanOperand());
            }

            while(operatorStack.Count > 0)
            {
                topEntity = operatorStack.Pop();
                Signal swap = expressionStack.Pop();
                Signal s = context.Builder.Function(topEntity, expressionStack.Pop(), swap);
                expressionStack.Push(s);
            }

            Signal ret = expressionStack.Pop();
            system.AddSignalTree(ret, false, false);
            return ret;
        }
        private List<Signal> ScanSignalExpressionList()
        {
            List<Signal> signals = new List<Signal>();
            signals.Add(ScanSignalExpression());
            while(tokenizer.LookaheadFistToken.IsType(TokenTypes.Separator))
            {
                tokenizer.Match(TokenTypes.Separator);
                signals.Add(ScanSignalExpression());
            }
            return signals;
        }
        #endregion

        #region Signal Operand
        private Signal ScanOperand()
        {
            Signal expression = ScanOperandPart();
            while(IsRightUnary(tokenizer.LookaheadFistToken))
            {
                LexerToken token = tokenizer.ConsumeGet();
                expression = context.Builder.Function(token.Text, InfixNotation.PostOperator, expression);
            }
            return expression;
        }
        private Signal ScanOperandPart()
        {
            LexerToken ahead = tokenizer.LookaheadFistToken;
            if(IsBeginningEncapsulation(ahead))
                return ScanEncapsulation();
            if(IsLeftUnary(ahead))
                return ScanLeftUnary();
            if(ahead.IsType(TokenTypes.TextIdentifier) || ahead.IsType(TokenTypes.SymbolIdentifier))
                return ScanIdentifierInExpression();
            if(ahead.IsType(TokenTypes.Integer))
                return ScanIntegerSignal();
            if(ahead.IsType(TokenTypes.Real))
                return ScanRealSignal();
            if(ahead.IsType(TokenTypes.Literal))
                return ScanLiteralSignal();
            throw new ParsingException("Parsing failed. Parser Scanner detected unexpected operand part token '" + ahead.Text + "'");
        }
        #endregion

        #region Identifier Expression
        /// <summary>Scans a Signal or a Function Expression</summary>
        private Signal ScanIdentifierInExpression()
        {
            if(tokenizer.LookaheadFistToken.IsType(TokenTypes.TextIdentifier)
                 && !tokenizer.LookaheadToken(1).IsType(TokenTypes.LeftList))
                return ScanSignal();
            else
                return ScanFunction();
        }

        private Signal ScanFunction()
        {
            LexerToken token = tokenizer.ConsumeGet();
            List<Signal> a = ScanList();
            Entity e = ScanEntity(token, InfixNotation.None, a.Count);
            if(tokenizer.LookaheadFistToken.IsType(TokenTypes.LeftVector))
            {
                tokenizer.Match(TokenTypes.LeftVector);
                long idx = ScanInteger();
                tokenizer.Match(TokenTypes.RightVector);
                return context.Builder.Functions(e, a)[(int)idx];
            }
            else
                return context.Builder.Function(e, a);
        }
        #endregion

        #region Expression Encapsulations
        private static bool IsBeginningEncapsulation(LexerToken token)
        {
            return token.IsType(TokenTypes.Left);
        }
        private Signal ScanEncapsulation()
        {
            LexerToken ahead = tokenizer.LookaheadFistToken;
            if(ahead.IsType(TokenTypes.LeftList))
                return ScanParenthesisSignal();
            if(ahead.IsType(TokenTypes.LeftVector))
                return ScanVectorSignal();
           if(ahead.IsType(TokenTypes.LeftSet))
                return ScanSetSignal();
           return ScanScalarSignal();
        }

        private Signal ScanVectorSignal()
        {
            List<Signal> inner = ScanVector();
            return context.Builder.EncapsulateAsVector(inner.ToArray());
        }
        private List<Signal> ScanVector()
        {
            List<Signal> signals;
            tokenizer.Match(TokenTypes.LeftVector);
            if(tokenizer.LookaheadFistToken.IsType(TokenTypes.RightVector))
                signals = new List<Signal>(0);
            else
                signals = ScanSignalExpressionList();
            tokenizer.Match(TokenTypes.RightVector);
            return signals;
        }
        private Signal ScanSetSignal()
        {
            List<Signal> inner = ScanSet();
            return context.Builder.EncapsulateAsSet(inner.ToArray());
        }
        private List<Signal> ScanSet()
        {
            List<Signal> signals;
            tokenizer.Match(TokenTypes.LeftSet);
            if(tokenizer.LookaheadFistToken.IsType(TokenTypes.RightSet))
                signals = new List<Signal>(0);
            else
                signals = ScanSignalExpressionList();
            tokenizer.Match(TokenTypes.RightSet);
            return signals;
        }
        private Signal ScanScalarSignal()
        {
            List<Signal> inner = ScanScalar();
            return context.Builder.EncapsulateAsScalar(inner.ToArray());
        }
        private List<Signal> ScanScalar()
        {
            List<Signal> signals;
            tokenizer.Match(TokenTypes.LeftScalar);
            if(tokenizer.LookaheadFistToken.IsType(TokenTypes.RightScalar))
                signals = new List<Signal>(0);
            else
                signals = ScanSignalExpressionList();
            tokenizer.Match(TokenTypes.RightScalar);
            return signals;
        }
        private Signal ScanParenthesisSignal()
        {
            tokenizer.Match(TokenTypes.LeftList);
            Signal s = ScanSignalExpression();
            tokenizer.Match(TokenTypes.RightList);
            return s;
        }
        private List<Signal> ScanList()
        {
            List<Signal> signals;
            tokenizer.Match(TokenTypes.LeftList);
            if(tokenizer.LookaheadFistToken.IsType(TokenTypes.RightList))
                signals = new List<Signal>(0);
            else
                signals = ScanSignalExpressionList();
            tokenizer.Match(TokenTypes.RightList);
            return signals;
        }
        #endregion

        #region Signal
        private Signal ScanSignal()
        {
            string name = ScanTextIdentifier();
            Signal s;
            if(!system.TryLookupNamedSignal(name, out s))
                s = system.CreateNamedSignal(name);
            return s;
        }
        private List<Signal> ScanSignalList()
        {
            List<Signal> signals = new List<Signal>();
            signals.Add(ScanSignal());
            while(tokenizer.LookaheadFistToken.IsType(TokenTypes.Separator))
            {
                tokenizer.Match(TokenTypes.Separator);
                signals.Add(ScanSignal());
            }
            return signals;
        }
        #endregion

        #region Bus
        private Bus ScanBus()
        {
            string name = ScanTextIdentifier();
            return system.LookupNamedBus(name);
        }
        private List<Bus> ScanBusList()
        {
            List<Bus> buses = new List<Bus>();
            buses.Add(ScanBus());
            while(tokenizer.LookaheadFistToken.IsType(TokenTypes.Separator))
            {
                tokenizer.Match(TokenTypes.Separator);
                buses.Add(ScanBus());
            }
            return buses;
        }
        #endregion

        #region Identifier
        private string ScanTextIdentifier()
        {
            LexerToken token = tokenizer.MatchGet(TokenTypes.TextIdentifier);
            return token.Text;
        }
        private List<string> ScanTextIdentifierList()
        {
            List<string> strings = new List<string>();
            strings.Add(ScanTextIdentifier());
            while(tokenizer.LookaheadFistToken.IsType(TokenTypes.Separator))
            {
                tokenizer.Match(TokenTypes.Separator);
                strings.Add(ScanTextIdentifier());
            }
            return strings;
        }
        private MathIdentifier ScanMathIdentifier()
        {
            LexerToken token = tokenizer.MatchGet(TokenTypes.MathIdentifier);
            return MathIdentifier.Parse(token.Text);
        }
        #endregion

        #region Entity
        private string ScanEntitySymbol()
        {
            LexerToken token = tokenizer.MatchGet(TokenTypes.SymbolIdentifier);
            return token.Text;
        }
        private string ScanEntitySymbolLiteral()
        {
            return ScanLiteral();
        }
        private MathIdentifier ScanEntityMathIdentifierOrLabel(bool defaultToWorkDomain)
        {
            if(tokenizer.LookaheadFistToken.IsType(TokenTypes.MathIdentifier)) //id
            {
                LexerToken token = tokenizer.MatchGet(TokenTypes.MathIdentifier);
                return MathIdentifier.Parse(token.Text);
            }
            else //label
            {
                LexerToken token = tokenizer.MatchGet(TokenTypes.TextIdentifier);
                string domain = defaultToWorkDomain ? "Work" : context.Library.Entities.FindDomainOfLabel(token.Text);
                return new MathIdentifier(token.Text, domain);
            }
        }
        private Entity ScanEntity()
        {
            if(tokenizer.LookaheadFistToken.IsType(TokenTypes.Literal)) //symbol
            {
                LexerToken token = tokenizer.MatchGet(TokenTypes.Literal);
                return context.Library.Entities.LookupSymbol(token.Text)[0];
            }
            else if(tokenizer.LookaheadFistToken.IsType(TokenTypes.SymbolIdentifier)) //symbol
            {
                LexerToken token = tokenizer.MatchGet(TokenTypes.SymbolIdentifier);
                return context.Library.Entities.LookupSymbol(token.Text)[0];
            }
            else //label or id
            {
                MathIdentifier entityId = ScanEntityMathIdentifierOrLabel(false);
                return context.Library.LookupEntity(entityId);
            }
        }
        private Entity ScanEntity(LexerToken token, InfixNotation notation, int inputs)
        {
            if(token.IsType(TokenTypes.MathIdentifier))
                return context.Library.LookupEntity(MathIdentifier.Parse(token.Text));
            else if(token.IsType(TokenTypes.Literal) || token.IsType(TokenTypes.SymbolIdentifier)) //symbol
                return context.Library.LookupEntity(token.Text, notation, inputs);
            else //textsymbol or label
            { 
                Entity entity;
                if(context.Library.TryLookupEntity(token.Text, notation, inputs, out entity))
                    return entity;
                else
                {
                    string domain = context.Library.Entities.FindDomainOfLabel(token.Text);
                    return context.Library.LookupEntity(new MathIdentifier(token.Text, domain));
                }
            }
        }
        #endregion

        #region Literal
        private string ScanLiteral()
        {
            LexerToken token = tokenizer.MatchGet(TokenTypes.Literal);
            return token.Text;
        }
        private List<string> ScanLiteralList()
        {
            List<string> literals = new List<string>();
            literals.Add(ScanLiteral());
            while(tokenizer.LookaheadFistToken.IsType(TokenTypes.Separator))
            {
                tokenizer.Match(TokenTypes.Separator);
                literals.Add(ScanLiteral());
            }
            return literals;
        }
        private Signal ScanLiteralSignal()
        {
            LexerToken token = tokenizer.MatchGet(TokenTypes.Literal);
            Signal s = LiteralValue.Constant(context, token.Text);
            system.AddSignal(s);
            return s;
        }
        #endregion

        #region Number
        private long ScanInteger()
        {
            LexerToken token = tokenizer.MatchGet(TokenTypes.Integer);
            return long.Parse(token.Text, Context.NumberFormat);
        }
        private Signal ScanIntegerSignal()
        {
            LexerToken token = tokenizer.MatchGet(TokenTypes.Integer);
            Signal s = IntegerValue.ParseConstant(context, token.Text);
            system.AddSignal(s);
            return s;
        }
        private Signal ScanRealSignal()
        {
            LexerToken token = tokenizer.MatchGet(TokenTypes.Real);
            Signal s = RealValue.ParseConstant(context, token.Text);
            system.AddSignal(s);
            return s;
        }
        #endregion

        #region Reset
        /// <summary>Clear the buffer. Replace the current stream with a new one.</summary>
        public void Reset(TextReader reader, MathSystem system)
        {
            this.system = system;
            this.context = system.Context;
            tokenizer.Reset(reader, context);
        }
        public void Reset()
        {
            tokenizer.Reset();
        }
        #endregion
    }
}
