using CodeGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen
{
    class Program
    {
        static void Main(string[] args)
        {
         
            
            Config config = Utilities.utilities.LoadConf();
            string SnippetFolder = config.SnippetFolder;
            string BaseFolder = config.BaseFolder;
            string RequestSnippetName = config.RequestSnippetName;
            string RequestSnippetParams = config.RequestSnippetParams;
            string ResponseSnippetName = config.ResponseSnippetName;
            string ResponseSnippetParam = config.ResponseSnippetParam;

            string OperationBusinessResultFile = config.OperationBusinessResultFile;
            string OperationServiceResultFile = config.OperationServiceResultFile;
            string OperationIBusinessResultFile = config.OperationIBusinessResultFile;
            string OperationIServiceResultFile = config.OperationIServiceResultFile;
            string OperationApiControllerResultFile = config.OperationApiControllerResultFile;

            foreach (var operation in config.Operations)
            {
                Utilities.utilities.GenerateRequest(BaseFolder, SnippetFolder, operation.RequestName, operation.ModelPath, RequestSnippetName, RequestSnippetParams, operation.Properties);
                Utilities.utilities.GenerateResponse(BaseFolder, SnippetFolder, operation.ResponseName, operation.ModelPath, ResponseSnippetName, ResponseSnippetParam, operation.ResponseParam != null ? operation.ResponseParam :null);
            }






        }
    }
}
