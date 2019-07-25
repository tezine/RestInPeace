using System.Collections.Generic;
using RestInPeace.Entities;

namespace RestInPeace.Entities {

    public enum FunctionType {
        GET=1,
        PUT=2,
        POST=3,
        DELETE=4
    }

    public class EFunction {
        public int version { get; set; }
        public FunctionType functionType { get; set; }
        public string returnTypeName { get; set; }
        public string functionName { get; set; }
        public List<EArg> argsList { get; set; }
        public string frontendFunctionContent { get; set; }
        public string qmlFunctionContent { get; set; }
        public string frontendReturnTypeName { get; set; }
    }
}  