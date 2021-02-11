using System.Collections.Generic;

namespace CodeGen.Utilities
{
    public class Config
    {

        public string SnippetFolder { get; set; }
        public string BaseFolder { get; set; }
        public string RequestSnippetName { get; set; }
        public string RequestSnippetParams { get; set; }
        public string ResponseSnippetParam { get; set; }
        public string ResponseSnippetName { get; set; }



        public string BusinessSnippetName { get; set; }
        public string IbusinessSnippetName { get; set; }
        public string ServiceSnippetName { get; set; }
        public string IServiceSnippetName { get; set; }
        public string APISnippetName { get; set; }
        public string OperationBusinessResultFile { get; set; }
        public string OperationServiceResultFile { get; set; }
        public string OperationIBusinessResultFile { get; set; }
        public string OperationIServiceResultFile { get; set; }
        public string OperationApiControllerResultFile { get; set; }

        public IList<Operation> Operations { get; set; }

    }

    public class Operation {
        public string OperationName { get; set; }
        public string RequestName { get; set; }
        public string ResponseName { get; set; }
        public string ModelPath { get; set; }
        public IList<Property> Properties { get; set; }
        public Property ResponseParam { get; set; }

    }

    public class Property
    {
        public string propertyName { get; set; }
        public string propertyType { get; set; }

    }
}