using System.Collections.Generic;
using RestInPeace.Enums;

namespace RestInPeace.Entities {
    public class EQmlFile {
        public string name { get; set; }
        public string completeFilePath { get; set; }
        public string rootElementName { get; set; }
        public string extendsElementName { get; set; }
        public List<EQMLFormProperty> propertyList { get; set; }=new List<EQMLFormProperty>();
        public string content { get; set; }
        public List<EQMLElement> elementsList { get; set; }=new List<EQMLElement>();
        public QmlFileType qmlFileType { get; set; }
        public List<EImport> typescriptImports { get; set; }=new List<EImport>();//todo passsar a usar mais isso
        public List<ETypescriptFunction> functionList { get; set; }=new List<ETypescriptFunction>();//todo passar a usar mais isso
        public List<EEnumFile> enumList { get; set; }=new List<EEnumFile>();
        public List<EQmlSignal> signalList { get; set; }=new List<EQmlSignal>();
    }
}