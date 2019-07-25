using System;
using System.IO;
using System.Text.RegularExpressions;
using RestInPeace.Codes.FlutterFrontend;
using RestInPeace.Enums;

namespace RestInPeace.Codes {
    public class AngularWatcher {

        
        #region Constructor
        public AngularWatcher( string frontendPath, bool generateEntities, string restInPeaceVersion) {
            try {
                Globals.angularServicesFolder = frontendPath + "/services/restinpeace";
                Globals.angularEnumsFolder = frontendPath + "/enums/restinpeace";
                if(!Directory.Exists(Globals.angularServicesFolder))Directory.CreateDirectory(Globals.angularServicesFolder);
                if(!Directory.Exists(Globals.angularEnumsFolder))Directory.CreateDirectory(Globals.angularEnumsFolder);
                if (generateEntities) {
                    Globals.angularEntitiesFolder = frontendPath + "/entities/restinpeace";
                    if(!Directory.Exists(Globals.angularEntitiesFolder))Directory.CreateDirectory(Globals.angularEntitiesFolder);
                    Helper.ReadKeyIfNotOk(AngularEntityWriter.WriteFiles(CSharpEntityReader.fileList));
                }
                if(!Helper.ReadKeyIfNotOk(AngularFunctionWriter.WriteFiles(CSharpEntityReader.fileList, restInPeaceVersion)))Environment.Exit(-1);
                Helper.ReadKeyIfNotOk(AngularEnumWriter.WriteFiles(CSharpEnumReader.fileList));
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic| DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"SUCCESS");
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        #endregion
        

        #region ConvertToAngularTypeName
        static public string ConvertToAngularTypeName(string dotnetTypeName) {
            switch (dotnetTypeName) {
                case "Int32":
                case "Int64":
                case "int":
                case "double":
                case "Decimal":
                case "decimal":
                case "float":
                    return "number";
                case "DateTime":
                    return "string";
                case "bool":
                    return "boolean";
            }
            var regexDoubleList=new Regex("List<List<(\\w+)>>", RegexOptions.None);
            var doubleListGroupNames = regexDoubleList.GetGroupNames();
            var doubleListGroupNumbers = regexDoubleList.GetGroupNumbers();
            var doubleMatch = regexDoubleList.Match(dotnetTypeName);
            if (doubleMatch.Success) return doubleMatch.Groups[1].Value + "[][]";
            
            var regex = new Regex("List<(\\w+)>", RegexOptions.None);
            var groupNames = regex.GetGroupNames();
            var groupNumbers = regex.GetGroupNumbers();
            var match = regex.Match(dotnetTypeName);
            if (match.Success) return match.Groups[1].Value + "[]";           
            return dotnetTypeName;
        }
        #endregion
    }
}