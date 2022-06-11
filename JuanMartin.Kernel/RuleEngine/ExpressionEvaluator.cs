/*
Expression evaluator precedence and fact logic from 'Simple Rule Engine' Copyright (C) 2005 by Sierra Digital Solutions Corp (GNU license)
*/

using JuanMartin.Kernel.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JuanMartin.Kernel.RuleEngine
{
    public class ExpressionEvaluator
    {
        /// <summary>
        ///  <see cref="https://www.fileformat.info/info/charset/UTF-8/list.htm"/> for character conversions
        /// </summary>
        //Regular expression for tokenizing expressions to be evaluated
        private static readonly string _singleCharacters = @"[\w\d\x21\x22\x23\x24\x25\x26\x2e\x3a\x3f\x40\x5b\x5c\x5d\x5e\x5f\x7b\x7c\x7d\x7e]";
        private static readonly string _singleCharactersWithSpace = @"[\s\w\d\x21\x22\x23\x24\x25\x26\x28\x29\x2a\x2b\x2c\x2d\x2e\x2f\x3a\x3d\x3f\x40\x5b\x5c\x5d\x5e\x5f\x7b\x7c\x7d\x7e]";
        private static readonly string _expressionRegEx = string.Format(@"((?:\x27{0}*\x27)|(?:\b{1}+\b)|\x29|\x28|\x2c|>=|<=|!=|==|<|>|AND|OR|NOT|ISNULL|XOR|\x2b|\x2d|\x2a|\x2f)",   _singleCharactersWithSpace , _singleCharacters);

        //LinkedList of characters and their replacement values, for characters not supported inside an a token in the expression by the regex above
        //because they are restricted symbols for the evaluator, like sinqle quote or open parenthesis
        private static readonly Dictionary<string, string> _invalidCharacters = new Dictionary<string, string>()
        {
            {"\'", "{quote}"},
            {"-", "{minus}"},
            {"+", "{plus}"},
            {"*", "{star}"},
            {"/", "{slash}"},
            {"<", "{less}"},
            {">", "{greater}"},
            {"(", "{open}"},
            {")", "{close}"}
        };

        //Lists of the parsed expression  as symbols, infix as it read in the string expression, postfix as it is needed for evaluation
        private readonly List<Symbol> _inFixExpression;
        private readonly List<Symbol> _postFixExpression;

        //Dictionary of namespace aliases used to resolve namespaces of macros
        private readonly Dictionary<string, Symbol> _aliases = new Dictionary<string, Symbol>();

        public ExpressionEvaluator(Dictionary<string, Symbol> Aliases)
        {
            _inFixExpression = new List<Symbol>();
            _postFixExpression = new List<Symbol>();

            if (Aliases == null)
                AddDefaultAliases();
            else
                _aliases = Aliases;
        }

        public ExpressionEvaluator()
            : this(null)
        {
        }

        public List<Symbol> InFix
        {
            get { return _inFixExpression; }
        }

        public List<Symbol> PostFix
        {
            get { return _postFixExpression; }
        }

        public void ClearStack()
        {
            _inFixExpression.Clear();
            _postFixExpression.Clear();
        }

        public static string EncodeToken(string Token)
        {
            foreach (string original in _invalidCharacters.Keys)
                Token = Token.Replace(original, _invalidCharacters[original]);

            return Token;
        }

        public static string DecodeToken(string Token)
        {
            foreach (string original in _invalidCharacters.Keys)
                Token = Token.Replace(_invalidCharacters[original], original);

            return Token;
        }

        public void Parse(string expression)
        {
            Debug.Write("Parsing to inFix Expression: " + expression + " : ");

            //Reset stacks to parse a new expression 
            ClearStack();

            //Tokenize string
            Regex regex = new Regex(_expressionRegEx, RegexOptions.IgnoreCase);
            string[] tokens = regex.Split(expression);

            foreach(string token in tokens)
            {
                if (token == null || token == String.Empty) continue; //Workaround: sometimes regex will bring back empty entries, skip these

                Symbol symbol = ParseToSymbol(token);

                //Add the new symbol to the collection only if its name is not blank which means not a valid symbol
                if (symbol.Name != string.Empty)
                    _inFixExpression.Add(symbol);
                            
                Debug.Write(symbol.Name + "|");
            }

            Debug.WriteLine("");

            //Convert postfix stack after parse to postfix for easier evaluation
            InfixToPostfix();
        }

        private Symbol ParseToSymbol(string token)
        {
            //If tokens were encoded by the uer when calling the evaluator
            //decode them before storing them into Symbols
            token = DecodeToken(token);

            Symbol symbol = new Symbol();

            if (IsOpenParanthesis(token))
            {
                symbol.Name = token;
                symbol.Type = Symbol.TokenType.OpenBracket;
            }
            else if (IsCloseParanthesis(token))
            {
                symbol.Name = token;
                symbol.Type = Symbol.TokenType.CloseBracket;
            }
            else if (IsFunction(token) != null) //Isfunction must come before Isvariable because its an exact match where the other is not
            {
                symbol.Name = token;
                symbol.Type = Symbol.TokenType.Function;
            }
            else if (IsOperator(token))
            {
                symbol.Name = token;
                symbol.Type = Symbol.TokenType.Operator;
            }
            else if (UtilityType.IsBoolean(token))
            {
                symbol.Name = token;
                symbol.Value = new Value(Value.Parse(token));
                symbol.Type = Symbol.TokenType.Value;
            }
            else if (IsFact(token))
            {
                symbol.Name = token;
                symbol.Type = Symbol.TokenType.Fact;
            }
            else if (UtilityType.IsNumber(token))
            {
                symbol.Name = token;
                symbol.Value = new Value(Value.Parse(token));
                symbol.Type = Symbol.TokenType.Value;
            }
            else if (UtilityType.IsString(token))
            {
                symbol.Name = token;
                symbol.Value = new Value(Value.Parse(token));
                symbol.Type = Symbol.TokenType.Value;
            }
            else if (token.Trim() == ",")
            {
                //Skip commas added for parameter separation in functions
            }
            else
            {
                //Who knows what it is so throw an exception
                throw new Exception("Error parsing token or invalid token: " + token);
            }

            return symbol;
        }

        private bool IsFact(string Token)
        {
            if (Token == null)
                return false;

            bool result = true;
            Symbol property = IsProperty(Token);

            if (property == null)
            {
                //Variables must have the first digit as a letter and the remaining as numbers and letters
                if (!Char.IsLetter(Token[0]))
                {
                    result = false;
                    return result;
                }

                foreach (char c in Token.ToCharArray())
                {
                    if (Char.IsLetter(c) || Char.IsNumber(c))
                        continue;

                    result = false;
                    break;
                }
            }

            return result;
        }

        private bool IsOpenParanthesis(string Token)
        {
            if (Token == null)
                return false;

            bool result = false;
            if (Token == "(")
                result = true;

            return result;
        }

        private bool IsCloseParanthesis(string Token)
        {
            if (Token == null)
                return false;

            bool result = false;
            if (Token == ")")
                result = true;

            return result;
        }

        private bool IsOperator(string Token)
        {
            if (Token == null)
                return false;

            //If is not an exact match return false
            bool result = false;

            switch (Token)
            {
                case "+":
                case "-":
                case "/":
                case "*":
                case "^":
                case "==":
                case "!=":
                case ">=":
                case "<=":
                case ">":
                case "<":
                case "AND":
                case "OR":
                case "NOT":
                case "XOR":
                    result = true;
                    break;
            }

            return result;
        }

        private MethodInfo IsFunction(string Token)
        {
            if (Token == null)
                return null;

            Symbol alias = Alias.GetDeclaringType(Token, _aliases);

            Type type = null;
            if (alias.Type == Symbol.TokenType.Alias || alias.Type == Symbol.TokenType.Value)
                type = (Type)alias.Value.Result;
            else if (alias.Type == Symbol.TokenType.Instance)
                type = alias.Value.Result.GetType();

            if (type == null || type.IsEnum)
                return null;

            try
            {
                //Use a lambda expression to get only the methods that return a typeof Value
                //in case the method [Macro] was overrided. If the macros are overrided
                //themselves use more conditions in the where clause, get BindingFlags.Static
                //and BindingFlags.Instance to get both static and non-static methods.
                MethodInfo method = (from m in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                                     where m.Name == alias.Name &&
                                     m.ReturnType == typeof(Value)
                                     select m).Single();

                //Use the GetCustomAttributes to identify methods marked with the [Macro] attribute
                var attributes = method.GetCustomAttributes(typeof(Macro), true);

                if (attributes != null && attributes.Length > 0)
                {
                    //If the attributes collection has members then the method if a 'Macro'
                    return method;
                }
            }
            catch (InvalidOperationException e)
            {
                //Lambda expression above raises this exception if this function is not found in the kernel assembly; 
                //matching by nane and if method returns a 'Value'.
                throw new Exception(String.Format("Method {0} does not exist.", Token), e);
            }

            return null;
        }

        public Symbol IsProperty(string Token)
        {
            if (Token == null)
                return null;

            Symbol alias = Alias.GetDeclaringType(Token, _aliases);
            Type type = (Type)alias.Value.Result;

            if (type == null)
                return null;

            string propertyName = alias.Name;

            try
            {
                if (type.IsEnum)
                {
                    object value = Enum.Parse(type, propertyName);

                    Symbol result = new Symbol(propertyName, new Value(value), Symbol.TokenType.Value);

                    return result;
                }
                else if (type.IsClass)
                {
                    //Use a lambda expression to get only the property that matches the passed public static property name
                    PropertyInfo property = (from p in type.GetProperties(BindingFlags.Public | BindingFlags.Static)
                                             where p.Name == propertyName
                                             select p).Single();

                    //Enumarators types must be declared outside of classes as public
                    object obj = System.Activator.CreateInstance(type);
                    Value value = (Value)property.GetValue(obj, null);

                    Symbol result = new Symbol(property.Name, value, Symbol.TokenType.Value);

                    return result;
                }
            }
            catch (InvalidOperationException e)
            {
                throw new Exception(String.Format("Constant/static property {0} does not exist.", Token), e);
            }

            return null;
        }

        public void InfixToPostfix()
        {
            Debug.Write("Parsing in fix stack to post fix stack: ");

            _postFixExpression.Clear();

            Stack postfixStack = new Stack();

            foreach (Symbol token in _inFixExpression)
            {
                if (token.Type == Symbol.TokenType.Value || token.Type == Symbol.TokenType.Fact)
                {
                    //Push value and facts into postfix expression
                    Debug.Write(token.Name + "|");
                    _postFixExpression.Add(token);
                }
                else if (token.Type == Symbol.TokenType.Operator || token.Type == Symbol.TokenType.Function)
                {

                    while (postfixStack.Count > 0 && !DeterminePrecedence(token, (Symbol)postfixStack.Peek()))
                    {
                        Debug.Write(((Symbol)postfixStack.Peek()).Name + "|");
                        _postFixExpression.Add((Symbol)postfixStack.Pop());

                    }
                    postfixStack.Push(token);

                }
                else if (token.Type == Symbol.TokenType.OpenBracket)
                {
                    postfixStack.Push(token);
                }
                else if (token.Type == Symbol.TokenType.CloseBracket)
                {
                    //Pop off stack to '(', discard '('.
                    while (((Symbol)postfixStack.Peek()).Type != Symbol.TokenType.OpenBracket)
                    {
                        Debug.Write(((Symbol)postfixStack.Peek()).Name + "|");
                        _postFixExpression.Add((Symbol)postfixStack.Pop());
                    }
                    postfixStack.Pop(); //Discard '('
                }
                else
                {
                    throw new Exception("Illegal symbol: " + token.Name);
                }
            }

            //Now we pop off whats left on the stack
            while (postfixStack.Count > 0)
            {
                Debug.Write(((Symbol)postfixStack.Peek()).Name + "|");
                _postFixExpression.Add((Symbol)postfixStack.Pop());
            }

            Debug.WriteLine("");
        }

        private bool DeterminePrecedence(Symbol higher, Symbol lower)
        {
            int s1 = Precedence(higher);
            int s2 = Precedence(lower);

            if (s1 > s2)
                return true;
            else
                return false;
        }

        private int Precedence(Symbol Token)
        {
            int result = 0;

            switch (Token.Name)
            {
                case "*":
                case "/":
                case "%":
                    result = 32;
                    break;
                case "+":
                case "-":
                    result = 16;
                    break;
                case ">":
                case "<":
                case ">=":
                case "<=":
                    result = 8;
                    break;
                case "==":
                case "!=":
                    result = 4;
                    break;
                case "NOT":
                    result = 3;
                    break;
                case "AND":
                    result = 2;
                    break;
                case "XOR":
                case "OR":
                    result = 1;
                    break;
            }

            //Functions have the highest priority
            if (Token.Type == Symbol.TokenType.Function)
                result = 64;

            return result;
        }

        private Symbol[] ParameterArray(Stack Arguments, int ArgumentCount, Dictionary<string, Symbol> Facts)
        {
            Symbol[] parameters = new Symbol[ArgumentCount];

            //Postfix logic invert order of function arguments so load operands from stack in reverse order in parameters array
            for (int i = ArgumentCount - 1; i >= 0; i--)
            {
                Symbol argument = (Symbol)Arguments.Pop();

                if (argument.Type == Symbol.TokenType.Fact)
                    parameters[i] = (Symbol)Facts[argument.Name].Clone();
                else
                    parameters[i] = argument;
            }

            return parameters;
        }

        private Symbol EvaluateFunction(string Function, Stack Operands, Dictionary<string, Symbol> Facts)
        {
            Symbol result = new Symbol();
            string argumentName = string.Empty;
            Value value = null;

            MethodInfo method = IsFunction(Function);

            if (method != null)
            {
                try
                {
                    object[] parameters = null;

                    //If isfunction returns an object the method is a macro
                    ParameterInfo[] parameterInfo = method.GetParameters();
                    int parameterCount = parameterInfo.Length;

                    if (parameterCount > 0)
                    {
                        parameters = new object[parameterCount];

                        //Macros do no support arrays as parameters. So if a parameter is of array type then assume is a parameter array.

                        //Postfix logic invert order of function arguments so load operands from stack in reverse order in parameters array
                        for (int i = parameterCount - 1; i >= 0; i--)
                        {
                            if (parameterInfo[i].ParameterType.BaseType.Name == "Array")
                            {
                                //Since a parameter type ia array, means the method has a param array, one, so the operands that
                                //belong to the array are the operand count is the total operands count - (the method paramcount-1).
                                parameters[i] = ParameterArray(Operands, (Operands.Count - (parameterCount - 1)), Facts);
                            }
                            else
                            {
                                Symbol Operand = (Symbol)Operands.Pop();

                                Symbol argument = (Symbol)Operand.Clone();
                                argumentName = argument.Name;

                                if (argument.Type == Symbol.TokenType.Fact)
                                    parameters[i] = (Symbol)Facts[argumentName].Clone();
                                else
                                    parameters[i] = argument;
                            }
                        }
                    }

                    //If the method is static then pass no instance else is not use the instance in the alias dictionary,
                    //index the dictionary by the object name in the token
                    if (method.Attributes.ToString().IndexOf(BindingFlags.Static.ToString()) != -1)
                        value = (Value)method.Invoke(null, parameters); //Since the instance parameter is null, the method being called must be static
                    else
                    {
                        Symbol instance = Alias.GetDeclaringType(Function, _aliases);

                        if (instance.Value.Result != null)
                            value = (Value)method.Invoke(instance.Value.Result, parameters);
                    }

                    result.Name = "result";
                    result.Value = value;
                    result.Type = Symbol.TokenType.Value;

                    Debug.WriteLine(String.Format("ExecuteFunction {0} = {1}", method.Name, value.Result));
                }
                catch (KeyNotFoundException e)
                {
                    throw new Exception(String.Format("Argumment ({0}) in {1} does not exist in fact base.", argumentName, Function), e);
                }
            }

            return result;
        }

        public Symbol Evaluate(Dictionary<string, Symbol> Facts)
        {
            Stack operandStack = new Stack();

            foreach (Symbol token in _postFixExpression)
            {
                if (token.Type == Symbol.TokenType.Value)
                {
                    operandStack.Push(token);
                }
                else if (token.Type == Symbol.TokenType.Operator)
                {
                    Symbol result = new Symbol(); //Arithmetic expression result
                    Symbol OperandLeft;
                    Symbol OperandRight;

                    switch (token.Name)
                    {
                        case "+":
                            OperandRight = (Symbol)operandStack.Pop(); //This operation requires two parameters
                            OperandLeft = (Symbol)operandStack.Pop();
                            result = EvaluateArithmeticOperartion("Add", OperandLeft, OperandRight);
                            break;
                        case "-":
                            OperandRight = (Symbol)operandStack.Pop(); //This operation requires two parameters
                            OperandLeft = (Symbol)operandStack.Pop();
                            result = EvaluateArithmeticOperartion("Substract", OperandLeft, OperandRight);
                            break;
                        case "*":
                            OperandRight = (Symbol)operandStack.Pop(); //This operation requires two parameters
                            OperandLeft = (Symbol)operandStack.Pop();
                            result = EvaluateArithmeticOperartion("Multiply", OperandLeft, OperandRight);
                            break;
                        case "/":
                            OperandRight = (Symbol)operandStack.Pop(); //This operation requires two parameters
                            OperandLeft = (Symbol)operandStack.Pop();
                            result = EvaluateArithmeticOperartion("Divide", OperandLeft, OperandRight);
                            break;
                        case "==":
                            OperandRight = (Symbol)operandStack.Pop(); //This operation requires two parameters
                            OperandLeft = (Symbol)operandStack.Pop();
                            result = EvaluateLogicOperation("Equals", OperandLeft, OperandRight);
                            break;
                        case "!=":
                            OperandRight = (Symbol)operandStack.Pop(); //This operation requires two parameters
                            OperandLeft = (Symbol)operandStack.Pop();
                            result = EvaluateLogicOperation("NotEquals", OperandLeft, OperandRight);
                            break;
                        case ">":
                            OperandRight = (Symbol)operandStack.Pop(); //This operation requires two parameters
                            OperandLeft = (Symbol)operandStack.Pop();
                            result = EvaluateLogicOperation("GreaterThan", OperandLeft, OperandRight);
                            break;
                        case "<":
                            OperandRight = (Symbol)operandStack.Pop(); //This operation requires two parameters
                            OperandLeft = (Symbol)operandStack.Pop();
                            result = EvaluateLogicOperation("LessThan", OperandLeft, OperandRight);
                            break;
                        case ">=":
                            OperandRight = (Symbol)operandStack.Pop(); //This operation requires two parameters
                            OperandLeft = (Symbol)operandStack.Pop();
                            result = EvaluateLogicOperation("GreaterThanEqual", OperandLeft, OperandRight);
                            break;
                        case "<=":
                            OperandRight = (Symbol)operandStack.Pop(); //This operation requires two parameters
                            OperandLeft = (Symbol)operandStack.Pop();
                            result = EvaluateLogicOperation("LessThanEqual", OperandLeft, OperandRight);
                            break;
                        case "AND":
                            OperandRight = (Symbol)operandStack.Pop(); //This operation requires two parameters
                            OperandLeft = (Symbol)operandStack.Pop();
                            result = EvaluateLogicOperation("And", OperandLeft, OperandRight);
                            break;
                        case "OR":
                            OperandRight = (Symbol)operandStack.Pop(); //This operation requires two parameters
                            OperandLeft = (Symbol)operandStack.Pop();
                            result = EvaluateLogicOperation("Or", OperandLeft, OperandRight);
                            break;
                        case "NOT":
                            OperandLeft = (Symbol)operandStack.Pop(); //This operation requires one parameters
                            result = EvaluateLogicOperation("Not", OperandLeft);
                            break;
                        default:
                            throw new Exception(String.Format("Invalid operator: {0} of type", token.Name, token.Type));
                    }

                    operandStack.Push(result);
                }
                else if (token.Type == Symbol.TokenType.Function)
                {
                    Symbol result = EvaluateFunction(token.Name, operandStack, Facts);

                    operandStack.Push(result);

                    if (result.Type == Symbol.TokenType.Invalid)
                        throw new Exception(String.Format("Function {0} is not a valid macro or it has missing arguments", token.Name));
                }
                else if (token.Type == Symbol.TokenType.Fact)
                {
                    Symbol result = new Symbol();
                    Symbol fact = null;
                    Symbol property = IsProperty(token.Name);

                    if (property != null && property.Type != Symbol.TokenType.Invalid)
                    {
                        result = (Symbol)property.Clone();
                        result.Type = Symbol.TokenType.Value;

                        operandStack.Push(result);
                    }
                    else
                    {
                        try
                        {
                            if (Facts != null)
                                fact = Facts[token.Name];

                            if (fact == null)
                                throw new Exception(String.Format("Fact {0} does not exist in this scope", token.Name));
                            else
                            {
                                result = (Symbol)fact.Clone();
                                result.Type = Symbol.TokenType.Fact;

                                operandStack.Push(result);
                            }
                        }
                        catch (KeyNotFoundException e)
                        {
                            throw new Exception(String.Format("Fact {0} does not exist in this scope", token.Name), e);
                        }
                    }

                    Debug.WriteLine(String.Format("ExpressionEvaluator FACT {0} = {1}", fact.Name, fact.Value));
                    continue;
                }
                else
                {
                    throw new Exception(String.Format("Invalid symbol type: {0} of type {1}", token.Name, token.Type));
                }
            }

            Symbol returnValue = (Symbol)operandStack.Pop();

            if (operandStack.Count > 0)
                throw new Exception("Invalid expr0ession: There is no result evaluated in stack.");

            return returnValue;
        }

        public Symbol Evaluate()
        {
            return Evaluate(null);
        }

        private Symbol EvaluateArithmeticOperartion(string OperationName, Symbol OperandLeft, Symbol OperandRight)
        {
            Symbol result = new Symbol();

            try
            {
                switch (OperationName)
                {
                    case "Add":
                        result.Value = UtilityMath.Add(OperandLeft, OperandRight);
                        result.Type = Symbol.TokenType.Value;
                        break;
                    case "Substract":
                        result.Value = UtilityMath.Substract(OperandLeft, OperandRight);
                        result.Type = Symbol.TokenType.Value;
                        break;
                    case "Multiply":
                        result.Value = UtilityMath.Multiply(OperandLeft, OperandRight);
                        result.Type = Symbol.TokenType.Value;
                        break;
                    case "Divide":
                        result.Value = UtilityMath.Divide(OperandLeft, OperandRight);
                        result.Type = Symbol.TokenType.Value;
                        break;
                    default:
                        result.Type = Symbol.TokenType.Invalid;
                        result.Value = new Value((Value)null);
                        break;
                }

                Debug.WriteLine(String.Format("{0} {1} {2} -> {3}", OperandLeft.Value.Result, OperationName, OperandRight.Value.Result, result.Value.Result));
            }
            catch
            {
                result.Type = Symbol.TokenType.Invalid;
                result.Value = new Value((Value)null);
            }

            return result;
        }

        private Symbol EvaluateLogicOperation(string OperationName, Symbol OperandLeft, Symbol OperandRight)
        {
            Value value = new Value();

            switch (OperationName)
            {
                case "Equals":
                    value = UtilityLogic.Equals(OperandLeft, OperandRight);
                    break;
                case "NotEquals":
                    value = UtilityLogic.NotEquals(OperandLeft, OperandRight);
                    break;
                case "And":
                    value = UtilityLogic.And(OperandLeft, OperandRight);
                    break;
                case "Or":
                    value = UtilityLogic.Or(OperandLeft, OperandRight);
                    break;
                case "Not":
                    value = UtilityLogic.Not(OperandLeft);
                    break;
                case "GreaterThan":
                    value = UtilityLogic.GreaterThan(OperandLeft, OperandRight);
                    break;
                case "GreaterThanEqual":
                    value = UtilityLogic.GreaterThanEqual(OperandLeft, OperandRight);
                    break;
                case "LessThan":
                    value = UtilityLogic.LessThan(OperandLeft, OperandRight);
                    break;
                case "LessThanEqual":
                    value = UtilityLogic.LessThanEqual(OperandLeft, OperandRight);
                    break;
            }

            Symbol result = new Symbol(OperationName);

            if (value.Type != null)
            {
                result.Value = value;
                result.Type = Symbol.TokenType.Value;
            }

            Debug.WriteLine(String.Format("{0} {1} {2} -> {3}", OperandLeft, OperationName, OperandRight, result));

            return result;
        }

        private Symbol EvaluateLogicOperation(string OperationName, Symbol OperandLeft)
        {
            return EvaluateLogicOperation(OperationName, OperandLeft, null);
        }

        private void AddAlias(string Name, string ReferenceType)
        {
            Symbol alias = new Symbol(Name, ReferenceType, Symbol.TokenType.Value);

            if (!_aliases.ContainsValue(alias))
                _aliases.Add(alias.Name, alias);
        }

        private void AddDefaultAliases()
        {
            this.AddAlias("Date", "JuanMartin.Kernel.Utilities.UtilityDate");
            this.AddAlias("DaysOfWeek", "JuanMartin.Kernel.Utilities.DaysOfWeek");
            this.AddAlias("Type", "JuanMartin.Kernel.Utilities.UtilityType");
            this.AddAlias("Logic", "JuanMartin.Kernel.Utilities.UtilityLogic");
            this.AddAlias("Math", "JuanMartin.Kernel.Utilities.UtilityMath");
            this.AddAlias("String", "JuanMartin.Kernel.Utilities.UtilityString");
        }

    }
}
