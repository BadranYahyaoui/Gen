using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Utilities
{
    public static class utilities
    {
        public static void CreateFile(string Filepath, string FileName, string FileContent)
        {
            string path = Path.Combine(Filepath, FileName);

            try
            {
                // Create the file, or overwrite if the file exists.
                using (FileStream fs = File.Create(path))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(FileContent);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }

                // Open the stream and read it back.
                using (StreamReader sr = File.OpenText(path))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public static string LoadFile(string FilePath, string FileName) {
            string path = Path.Combine(FilePath, FileName);

            using (System.IO.StreamReader Reader = new System.IO.StreamReader(path))
            {
                string fileContent = Reader.ReadToEnd();
                return fileContent;
            }
        }
        public static void GenerateRequest(string BaseFolder, string SnippetFolder, string RequestName, string ModelPath, string RequestSnippetName, string RequestSnippetParams,
            IList<Property> props)
        {

            string ModelName = Path.GetFileNameWithoutExtension(ModelPath);
            string RequestFileName = RequestName + ".cs";

            //Request Body Content
            string res = Utilities.utilities.LoadFile(SnippetFolder, RequestSnippetName);
            res = res.Replace("#RequestName#", RequestName).Replace("#ModelName#", ModelName);

            //Request props
            var RequestParams = SetRequestParams(SnippetFolder, RequestSnippetParams, props);
            res = res.Replace("#RequestParams#", RequestParams);

            Utilities.utilities.CreateFile(BaseFolder, RequestFileName, res);
        }
        public static void GenerateResponse(string BaseFolder, string SnippetFolder, string ResponseName, string ModelPath, string ResponseSnippetName, 
            string ResponseSnippetParam, Property param)

        {
            string ModelName = Path.GetFileNameWithoutExtension(ModelPath);
            string ResponseFileName = ResponseName + ".cs";

                   //Response Body Content
            string res = Utilities.utilities.LoadFile(SnippetFolder, ResponseSnippetName);
            res = res.Replace("#ResponseName#", ResponseName).Replace("#ModelName#", ModelName);
            if (param != null)
            {
                //Request props
                string res2 = Utilities.utilities.LoadFile(SnippetFolder, ResponseSnippetParam);
                res2 = res2.Replace("#propType# ", param.propertyType).Replace("#propName#", param.propertyName).Replace("#LowerpropName#", param.propertyName.ToLower());
                res = res.Replace("#ResponseParam#", res2);

            }
            else
            {
                res = res.Replace("#ResponseParam#", "");
            }
            Utilities.utilities.CreateFile(BaseFolder, ResponseFileName, res);

        }



        public static List<Tuple<string, string>> ReadClassParams(string ClassFilePath) {
            var props = new List<Tuple<string, string>>();

            string modelName =Path.GetFileName(ClassFilePath);
            // Get a path to the file(s) to compile.
            FileInfo sourceFile = new FileInfo(ClassFilePath);
            Console.WriteLine("Loading file: " + sourceFile.Exists);

            // Prepary a file path for the compiled library.
            string outputName = string.Format(@"{0}\{1}.dll",
                Environment.CurrentDirectory,
                Path.GetFileNameWithoutExtension(sourceFile.Name));

            // Compile the code as a dynamic-link library.
            bool success = Compile(sourceFile, new CompilerParameters()
            {
                GenerateExecutable = false, // compile as library (dll)
                OutputAssembly = outputName,
                GenerateInMemory = false, // as a physical file
            });

            if (success)
            {
                // Load the compiled library.
                Assembly assembly = Assembly.LoadFrom(outputName);
                System.Type ModelType = assembly.GetType(Path.GetFileNameWithoutExtension(sourceFile.Name));
                //object ModelInstance = Activator.CreateInstance(ModelType);
                // Using reflection.  
                var Modelprops = ModelType.GetProperties();
                foreach (var propertyInfo in Modelprops)
                {
                    var propertyType = propertyInfo.PropertyType;
                    string propString = propertyType.Name.ToString();
                    if (propertyType.IsGenericType &&
                            propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        propString = propertyType.GetGenericArguments()[0].Name+"?";
                    }

                    props.Add(new Tuple<string, string>(propString, propertyInfo.Name));
                }
            }
            return props;
          
        }
        private static bool Compile(FileInfo sourceFile, CompilerParameters options)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");

            CompilerResults results = provider.CompileAssemblyFromFile(options, sourceFile.FullName);

            if (results.Errors.Count > 0)
            {
                Console.WriteLine("Errors building {0} into {1}", sourceFile.Name, results.PathToAssembly);
                foreach (CompilerError error in results.Errors)
                {
                    Console.WriteLine("  {0}", error.ToString());
                    Console.WriteLine();
                }
                return false;
            }
            else
            {
                Console.WriteLine("Source {0} built into {1} successfully.", sourceFile.Name, results.PathToAssembly);
                return true;
            }
        }
        public static string SetRequestParams(string SnippetFolder, string RequestSnippeParam, IList<Property> props) {

        var reqProps = new List<Property>();
            string RequestParams="";
            if (props !=null)
            {
                foreach (var item in props)
                {
                    string res = Utilities.utilities.LoadFile(SnippetFolder, RequestSnippeParam);
                    res = res.Replace("#propType# ", item.propertyType).Replace("#propName#", item.propertyName).Replace("#LowerpropName#", item.propertyName.ToLower());
                    RequestParams = RequestParams + res;
                }
            }
            return RequestParams;
        }
        public static Config LoadConf()
        {
         
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            JObject o1 = JObject.Parse(File.ReadAllText(@"../../Config.json"));
            Config Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"../../Config.json"));

            return Config;

        }
        public static void GenerateApiController(string BaseFolder, string SnippetFolder, string ResponseName, string RequestName, string ModelPath, 
            string APIControllerSnippetName, string APIControllerFile, string OperationName)
        {
            string ModelName = Path.GetFileNameWithoutExtension(ModelPath);
            string res = Utilities.utilities.LoadFile(SnippetFolder, APIControllerSnippetName);
            res = res.Replace("#RequestName#", RequestName).Replace("#ModelName#", ModelName)
                .Replace("#ResponseName#", ResponseName).Replace("#OperationName#", OperationName);

             File.AppendAllText(Path.Combine(BaseFolder,APIControllerFile),res + Environment.NewLine);
        }


        public static void GenerateIservice(string BaseFolder, string SnippetFolder, string ResponseName, string RequestName, string ModelPath,
           string IserviceSnippetName, string IServiceFile, string OperationName)
        {
            string ModelName = Path.GetFileNameWithoutExtension(ModelPath);
            string res = Utilities.utilities.LoadFile(SnippetFolder, IserviceSnippetName);
            res = res.Replace("#RequestName#", RequestName).Replace("#ModelName#", ModelName)
                .Replace("#ResponseName#", ResponseName).Replace("#OperationName#", OperationName);

            File.AppendAllText(Path.Combine(BaseFolder, IServiceFile), res + Environment.NewLine);
        }

        public static void GenerateService(string BaseFolder, string SnippetFolder, string ResponseName, string RequestName, string ModelPath,
       string serviceSnippetName, string ServiceFile, string OperationName, Property ResponseParam , IList<Property>  RequestParams)
        {
            string ModelName = Path.GetFileNameWithoutExtension(ModelPath);
            string res = Utilities.utilities.LoadFile(SnippetFolder, serviceSnippetName);
            res = res.Replace("#RequestName#", RequestName)
                .Replace("#ResponseName#", ResponseName).Replace("#OperationName#", OperationName);

            if (ResponseParam != null)
            {
                string str = ResponseParam.propertyType + " " + ResponseParam.propertyName;
               res = res.Replace("#ModelName#", str);

            }
            else
            {
                res = res.Replace("#ModelName#", "Values");
            }
            if (RequestParams != null)
            {
                string rp = "";
                foreach (var item in RequestParams)
                {

                    rp = rp + "request." + item.propertyName+",";
                }
                rp.Replace(",)", ")");
                res = res.Replace("#params#", rp);

            }
            else
            {
                res = res.Replace("#params#", "");
            }

            File.AppendAllText(Path.Combine(BaseFolder, ServiceFile), res + Environment.NewLine);


        }

        public static void GenerateIBusiness(string BaseFolder, string SnippetFolder, string ResponseName, string RequestName, string ModelPath,
       string IBusinessSnippetName, string IBusinessFile, string OperationName, Property ResponseParam, IList<Property> RequestParams)
        {
            string ModelName = Path.GetFileNameWithoutExtension(ModelPath);
            string res = Utilities.utilities.LoadFile(SnippetFolder, IBusinessSnippetName);
            res = res.Replace("#RequestName#", RequestName)
                .Replace("#ResponseName#", ResponseName).Replace("#OperationName#", OperationName);

            if (ResponseParam != null)
            {
                string str = ResponseParam.propertyType + " " + ResponseParam.propertyName;
                res = res.Replace("#ModelName#", str);

            }
            else
            {
                res = res.Replace("#ModelName#", " IList<"+ ModelName + ">");
            }
            if (RequestParams != null)
            {
                string rp = "";
                foreach (var item in RequestParams)
                {

                    rp = rp + item.propertyType +" "+ item.propertyName + ",";
                }
                
                res = res.Replace("#params#", rp);
                res = res.Replace(",)", ")");
            }
            else
            {
                res = res.Replace("#params#", "");
            }

            File.AppendAllText(Path.Combine(BaseFolder, IBusinessSnippetName), res + Environment.NewLine);


        }

        public static void GenerateBusiness(string BaseFolder, string SnippetFolder, string ResponseName, string RequestName, string ModelPath,
      string BusinessSnippetName, string BusinessFile, string OperationName, Property ResponseParam, IList<Property> RequestParams)
        {
            string ModelName = Path.GetFileNameWithoutExtension(ModelPath);
            string res = Utilities.utilities.LoadFile(SnippetFolder, BusinessSnippetName);
            res = res.Replace("#RequestName#", RequestName)
                .Replace("#ResponseName#", ResponseName).Replace("#OperationName#", OperationName);

            if (ResponseParam != null)
            {
                string str = ResponseParam.propertyType + " " + ResponseParam.propertyName;
                res = res.Replace("#ModelName#", str);

            }
            else
            {
                res = res.Replace("#ModelName#", " IList<" + ModelName + ">");
            }
            if (RequestParams != null)
            {
                string rp = "";
                foreach (var item in RequestParams)
                {

                    rp = rp + item.propertyType + " " + item.propertyName + ",";
                }

                res = res.Replace("#params#", rp);
                res = res.Replace(",)", ")");
            }
            else
            {
                res = res.Replace("#params#", "");
            }

            File.AppendAllText(Path.Combine(BaseFolder, BusinessSnippetName), res + Environment.NewLine);


        }
    }
}

