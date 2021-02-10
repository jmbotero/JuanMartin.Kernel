using System;
using System.Collections.Generic;

namespace JuanMartin.Kernel.RuleEngine
{
    public class Alias
    {
        public static Symbol GetDeclaringType(string Token, Dictionary<string, Symbol> Aliases)
        {
            //Always return a symbol, in case of an empty alias reutrn a null symbol

            //Parse namespace from function name
            int namespacePosition = Token.LastIndexOf('.');

            //If token is a string for example from a fact or function argument do not process periods in it
            if (Token.IndexOf('\'') != -1)
                return new Symbol("alias::null", new Value((Value)null), Symbol.TokenType.Alias); ;

            if (namespacePosition <= 0)
                return new Symbol("alias::null", new Value((Value)null), Symbol.TokenType.Alias); ;

            string typeName = Token.Substring(0, namespacePosition);
            string memberName = Token.Substring(namespacePosition + 1);

            //use reflection to instantiate utility class matching the library name
            if (typeName.Length == 0)
                return new Symbol("alias::null", new Value((Value)null), Symbol.TokenType.Alias); ;

            Symbol alias;

            //look for the type name in the aliases dictionary, if not found assume there is no alias and Token contains the full namespace
            try
            {
                alias = Aliases[typeName];
            }
            catch (KeyNotFoundException e)
            {
                //do nothing, return empty symbol and exception is thrown by caller method
                throw new Exception(String.Format("Alias for {0} does not exist.", typeName), e);
            }
            catch (NullReferenceException e)
            {
                //do nothing, return empty symbol and exception is thrown by caller method
                throw new Exception("Aliases dictionary not defined in current engine.", e);
            }

            //Set the default alias as the full type name in the Token
            Symbol result = new Symbol(memberName, typeName, alias.Type); ;

            if (alias.Type == Symbol.TokenType.Alias)
                result.Value.Result = Type.GetType((string)alias.Value.Result);
            else if (alias.Type == Symbol.TokenType.Instance)
                result.Value = alias.Value;
            else
                throw new Exception(string.Format("Type set for alias '{0}' is not a supported Alias type", alias.Type.ToString()));

            return result;
        }
    }
}
