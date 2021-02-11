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
            string APISnippetName = config.APISnippetName;
            string IServiceSnippetName = config.IServiceSnippetName;
            string IbusinessSnippetName = config.IbusinessSnippetName;
            string BusinessSnippetName = config.BusinessSnippetName;
            string ServiceSnippetName = config.ServiceSnippetName;
            string OperationBusinessResultFile = config.OperationBusinessResultFile;
            string OperationServiceResultFile = config.OperationServiceResultFile;
            string OperationIBusinessResultFile = config.OperationIBusinessResultFile;
            string OperationIServiceResultFile = config.OperationIServiceResultFile;
            string OperationApiControllerResultFile = config.OperationApiControllerResultFile;

            foreach (var operation in config.Operations)
            {
                 Utilities.utilities.GenerateRequest(BaseFolder, SnippetFolder, operation.RequestName, operation.ModelPath, RequestSnippetName, RequestSnippetParams, operation.Properties);
                 Utilities.utilities.GenerateResponse(BaseFolder, SnippetFolder, operation.ResponseName, operation.ModelPath, ResponseSnippetName, ResponseSnippetParam, operation.ResponseParam != null ? operation.ResponseParam :null);
                Utilities.utilities.GenerateApiController(BaseFolder,SnippetFolder, operation.ResponseName, operation.RequestName, 
                    operation.ModelPath, APISnippetName, OperationApiControllerResultFile, operation.OperationName); 
                Utilities.utilities.GenerateIservice(BaseFolder,SnippetFolder, operation.ResponseName, operation.RequestName, 
                    operation.ModelPath, IServiceSnippetName, OperationIServiceResultFile, operation.OperationName);
                Utilities.utilities.GenerateService(BaseFolder,SnippetFolder, operation.ResponseName, operation.RequestName, 
                    operation.ModelPath, ServiceSnippetName, OperationServiceResultFile, operation.OperationName,
                    operation.ResponseParam != null ? operation.ResponseParam : null, operation.Properties != null ? operation.Properties : null);
                Utilities.utilities.GenerateIBusiness(BaseFolder,SnippetFolder, operation.ResponseName, operation.RequestName, 
                    operation.ModelPath, IbusinessSnippetName, OperationIBusinessResultFile, operation.OperationName,
                    operation.ResponseParam != null ? operation.ResponseParam : null, operation.Properties != null ? operation.Properties : null);

                Utilities.utilities.GenerateBusiness(BaseFolder,SnippetFolder, operation.ResponseName, operation.RequestName, 
                    operation.ModelPath, BusinessSnippetName, OperationBusinessResultFile, operation.OperationName,
                    operation.ResponseParam != null ? operation.ResponseParam : null, operation.Properties != null ? operation.Properties : null);
            }

             




        }
    }
}
