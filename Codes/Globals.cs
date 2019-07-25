#region Imports
using System.Collections.Generic;
using RestInPeace.Entities;
using RestInPeace.Enums;
#endregion

namespace RestInPeace.Codes {
    static public class Globals {
        static public DebugLevels debugLevel;
        static public string frontendRestInPeaceFolder = "";
        static public string frontendQmlPagesFolder = "";
        static public string frontendQmlScriptsFolder = "";
        
        static public string flutterPackageName = "";
        static public string flutterServicesFolder = "";
        static public string flutterEntitiesFolder = "";
        static public string flutterEnumFolder = "";
        
        static public string blazorServicesFolder = "";
        static public string blazorNamespaceName = "";
        static public string blazorEntityFolder = "";
        
        static public string dotnetEntityFolder = "";
        static public string dotnetNamespace = "";
        static public string dotnetServiceFolder = "";
        
        static public string angularEnumsFolder = "";
        static public string angularServicesFolder = "";
        static public string angularEntitiesFolder = "";

        static public List<EQmlFile> eQMLFormFiles = new List<EQmlFile>();
        static public List<EFunctionFile> backendControllerFiles = new List<EFunctionFile>();
        static public List<EQMLElement> qmlElements=new List<EQMLElement>();
    }
}