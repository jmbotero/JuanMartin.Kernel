using JuanMartin.Kernel.Extesions;
using JuanMartin.Kernel.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace JuanMartin.Kernel.Processors
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CommandLine
    {
        [JsonProperty]
        public string Line { get; set; }
        [JsonProperty]
        public List<CommandLineOption> Options { get; set; }
        [JsonProperty]
        public bool IsCaseSensitive { get; private set; }
        [JsonProperty]
        public bool AllowBundling { get; private set; }
        [JsonProperty]
        public string CultureInfo { get; private set; }
        [JsonProperty]
        public string Version { get; private set; }
        public int OptionCount => Options.Count;
        public CommandLineOption this[int i]
        {
            get { return Options[i]; }
        }
        public CommandLineOption this[string optionName]
        {
            get { return Options.FirstOrDefault(o => o.Name == optionName); }
        }

        public bool Contains(string optionName)
        {
            if (this[optionName] != null)
                return true;
     
            return false;
        }

        [JsonConstructor]
        public CommandLine()
        {}
        public CommandLine(string line, Dictionary<string,string> lineTokens = null, string fileName = "")
        {
            // use default
            if (fileName == string.Empty)
            {
                // This will get the current WORKING directory (i.e. \bin\Debug)
                string workingDirectory = Environment.CurrentDirectory;
                // This will get the current PROJECT directory
                string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;

                if(projectDirectory.Contains("\\bin"))
                    projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

                fileName = projectDirectory + @"\commandline.settings.json";

                if (!File.Exists(fileName))
                    throw new FileNotFoundException(fileName);
            }
            Options = new List<CommandLineOption>();
            LoadCommandLineSettings(fileName);
            Line = line;
            Parse(line, lineTokens);
        }

        private void Parse(string line, Dictionary<string, string> lineTokens)
        {
            if (line != null)
            {
                if (line[0] != '-' || line.Substring(0, 2) != "--")
                    throw new ArgumentException($"Command line '{line}' is not formatted correctly: does not begin with an option defined with '-' or '--'.");

                // replace tokens specifid in line
                if(lineTokens != null)
                {
                    foreach(var token in lineTokens)
                    {
                        line = line.Replace(token.Key, token.Value);
                    }
                }

                if(line.Contains("_token_"))
					throw new ArgumentException($"Command line '{line}' contains unreplaceable tokens.");
				
                // get option name/Values from line in dictionary
				var options = line.Split(new string[] { "-", "--" }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(s => s.Split('='))
                                    .ToDictionary(s => s.First().Trim(), s => (s.Last() == s.First()) ? string.Empty : s.Last().Trim());

                // convert to command line options
                //foreach(var item in options)
                //{
                //    var isSingle = (item.Value == "null") ? true : false;
                //    var value = (isSingle) ? string.Empty : item.Value;

                //    Options.Add(new CommandLineOption(item.Key, value, "string"));
                //}

                var usedNames = (from option in options select option.Key).ToHashSet();
                var requiredNames = (from option in Options
                                      where option.IsRequired == true
                                      select option.Name).ToHashSet();
                foreach (var n in usedNames)
                {
                    var dependency = (from option in Options
                                      where option.Dependencies.Contains(n)
                                      select n).FirstOrDefault();

                    if(dependency != null)
                        requiredNames.Add(dependency);
                }

                var validNames = (from option in Options
                                   select option.Name).ToList();
                validNames.AddRange((from option in Options where option.Abbreviation != option.Name select option.Abbreviation));

                foreach (var n in usedNames)
                {
                    if (!validNames.Contains(n))
                        throw new ArgumentException($"Command line '{line}' is not formatted correctly. Option {n} is not defined.");
                }
                foreach (var n in requiredNames)
                {
                    if (!usedNames.Contains(n))
                        throw new ArgumentException($"Command line '{line}' is not formatted correctly. Required option {n} is not present.");
                }

                // process values
                // creating object of CultureInfo for string-array parsing
                CultureInfo cultures = null;
                try
                {
                    cultures = new CultureInfo(CultureInfo);
                }
                catch
                {
                    throw new ArgumentException("Culture info argument not defined in settings file.");
                }
                Type systemType = null;

                foreach (var o in options)
                {
                    object value = null;
                    var option = this[o.Key];

                    if (option == null)
                        throw new ArgumentOutOfRangeException($"Option {o.Key} not found in command line settings file.");

                    option.Status = CommandLineOption.OptionStatus.assigned;
                    string optionType = option.ValueType;

                    if (optionType != null)
                    {
                        if (!option.IsSingle) // do not assign value to singles as these do no have command line values
                        {
                            var actualValue = o.Value;

                            switch (optionType)
                            {
                                case "System.Int32[]":
                                    {
										// http://regexstorm.net/tester?p=%5e%28x%7c%5cd%2b%29%28%28%2c%7c%3a%29%5cd%2b%29*%24&i=1%3a4%2c6
										var numericPattern = new Regex("^(x|\\d+)((,|:)\\d+)*$");

                                        if (actualValue == "x")                             
                                            value = null;
                                        else if (actualValue.IsNullOrEmptyOrZero(checkZero: false) && numericPattern.IsMatch(actualValue))
                                        {
                                            // parse ranges and single entries
                                            //value = actualValue.Split(',').Select(i => Convert.ToInt32(i, cultures)).ToArray();
                                            var items = new List<int>();
                                            var values = actualValue.Split(',').ToArray();
                                            foreach (string v in values)
                                            {
                                                if (!v.Contains(":"))
                                                {
                                                    items.Add(Convert.ToInt32(v));
                                                }
                                                else
                                                {
                                                    var i = v.IndexOf(":");
                                                    string beginParameter = v.Substring(0, i);
                                                    string endParameter = v.Substring(i + 1);
													int begin = Convert.ToInt32(beginParameter);
													int end   = (i<v.Length)?Convert.ToInt32(endParameter) :0;
                                                    items.AddRange(Enumerable.Range(begin,end).ToArray<int>());
                                                }
                                        }
												value = items.ToArray(); // convert to array to avoid 'recquired iconvertible'  exception
                                            }
                                        else
                                            throw new ArgumentException($"Cannot parse value ({actualValue}) as an integer or comma-separaated list of integers or  integer range (defined as begin:end).");
                                        break;
                                    }
                                case "System.Int32":
                                case "System.Boolean":
                                case "System.String":
                                    {
                                        value = actualValue;
                                        break;
                                    }
                                default:
                                    throw new TypeLoadException($"Type {optionType} not supported.");
                            }
                        }
                        else // if single and boolean value is true else use value in settings as default
                        {
                            switch (optionType)
                            {
                                case "System.Int32[]":
                                    {
                                        value = option.Value.ToString().Split(',').Select(i => Convert.ToInt32(i, cultures)).ToArray();
                                        break;
                                    }
                                case "System.Boolean":
                                    {
                                        value = true;
                                        break;
                                    }
                                case "System.Int32":
                                case "System.String":
                                    {
                                        value = option.Value;
                                        break;
                                    }
                                default:
                                    throw new TypeLoadException($"Type {optionType} not supported.");
                            }
                        }
                       
                        systemType = UtilityType.ParseType(optionType);
                        try
                        {
                            option.Value = Convert.ChangeType(value, systemType);
                        }
                        catch (Exception e)
                        {
                            throw new TypeLoadException($"Error changing option's type ({optionType}): {e.Message}.");
                        }       
                    }
                }

                //remove unused options: leaveonly those specified in command line
                for (var i = Options.Count - 1; i >= 0; i--)
                {
                    var option = Options[i];

                    if (!usedNames.Contains(option.Name))
                        Options.Remove(option);
                }

                var words = line.Split(' ').ToArray();
                if (line.Length > 0 && options.Count == 0 || words.Length == 0)
                    throw new ArgumentException($"Command line '{line}' is not formatted correctly. No option/value pairs defined.");
            }
        }

        private void LoadCommandLineSettings(string fileName)
        {
            var json = string.Empty;
            if (fileName != null && fileName.Length > 0)
            {
                using (var reader = new StreamReader(fileName, Encoding.UTF8))
                {
                    json = reader.ReadToEnd();
                }

                if (json != string.Empty)
                {
                    var cmd = JsonConvert.DeserializeObject<CommandLine>(json);

                    IsCaseSensitive = cmd.IsCaseSensitive;
                    AllowBundling = cmd.AllowBundling;
                    CultureInfo = cmd.CultureInfo;
                    Version = cmd.Version;

                    Options = cmd.Options.ToList();
                }
                else
                    throw new FileLoadException();
            }
        }
    }
}
