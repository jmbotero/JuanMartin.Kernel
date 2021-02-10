/*
 * Created by SharpDevelop.
 * User: juan
 * Date: 12/29/2007
 * Time: 12:50 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Reflection;

namespace JuanMartin.Kernel.Formatters
{
    /// <summary>
    /// Description of AssemblyReader.
    /// </summary>
    public class AssemblyReader : IReader
    {
        private Assembly _assembly;
        private ValueHolder _value;
        private string _assemblyFile;

        private void Initialize(string AssemblyFile)
        {
            _assemblyFile = AssemblyFile;

            try
            {
                _value = new ValueHolder("Assembly", AssemblyFile);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Could not load asssembly {0}: {1}", AssemblyFile, e.Message));
            }

            Load();
        }

        public AssemblyReader(ValueHolder Parameters)
        {
            string assemblyFile = (string)Parameters.Value;

            Initialize(assemblyFile);
        }

        public AssemblyReader(string AssemblyFile)
        {
            Initialize(AssemblyFile);
        }

        public ValueHolder Value
        {
            get { return _value; }
        }

        private void Load()
        {
            ValueHolder _types = new ValueHolder("types");
            ValueHolder _constructors = new ValueHolder("constructors");
            ValueHolder _properties = new ValueHolder("properties");
            ValueHolder _methods = new ValueHolder("methods");
            ValueHolder _events = new ValueHolder("events");

            _value.AddAnnotation(_types);
            _value.AddAnnotation(_constructors);
            _value.AddAnnotation(_properties);
            _value.AddAnnotation(_methods);
            _value.AddAnnotation(_events);

            Type[] atypes;

            try
            {
                _assembly = Assembly.LoadFrom(_assemblyFile);
                atypes = _assembly.GetTypes();

                foreach (Type type in atypes)
                {
                    ConstructorInfo[] constructors = type.GetConstructors();
                    foreach (ConstructorInfo c in constructors)
                        _constructors.AddAnnotation(c.DeclaringType.ToString() + c.Name, c);

                    PropertyInfo[] properties = type.GetProperties();
                    foreach (PropertyInfo p in properties)
                        _properties.AddAnnotation(p.Name, p);

                    EventInfo[] events = type.GetEvents();
                    foreach (EventInfo e in events)
                        _events.AddAnnotation(e.Name, e);

                    MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);
                    String signature = string.Empty;
                    foreach (MethodInfo method in methods)
                    {
                        if (type == method.DeclaringType)
                        {
                            signature = method.Name;
                            signature += " ( ";

                            ParameterInfo[] parameters = method.GetParameters();
                            foreach (ParameterInfo parameter in parameters)
                            {
                                signature += parameter.ParameterType;
                                signature += " ";
                                signature += parameter.Name;
                                signature += ", ";
                            }

                            int trailingComma = signature.LastIndexOf(",");
                            if (trailingComma != -1)
                                signature = signature.Remove(trailingComma, 1);

                            signature += " )";
                        }

                        _methods.AddAnnotation(signature, method);
                    }

                    _types.AddAnnotation(type.FullName, type);
                }
            }
            catch (Exception e)
            {
                //Error loading assembly
                Console.WriteLine(e.Message);
            }
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
