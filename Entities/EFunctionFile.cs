using System.Collections.Generic;

namespace RestInPeace.Entities {
    public class EFunctionFile {
        public string frontendClassName { get; set; }
        public string frontendFileName { get; set; }
        public string csharpFileName { get; set; }
        public List<EFunction> functionList { get; set; }
    }
}