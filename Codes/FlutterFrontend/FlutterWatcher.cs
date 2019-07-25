using System;
using System.IO;
using DotnetBase.Codes;
using RestInPeace.Codes.QMLTypescriptFrontend;
using RestInPeace.Enums;

namespace RestInPeace.Codes.FlutterFrontend {
    public class FlutterWatcher {
        
        #region Constructor
        public FlutterWatcher( string frontendPath) {
            try {                
                Globals.flutterServicesFolder = frontendPath + "/lib/services/restinpeace";
                Globals.flutterEntitiesFolder = frontendPath + "/lib/entities/restinpeace";
                Globals.flutterEnumFolder = frontendPath + "/lib/enums/restinpeace";
                DirectoryInfo dirInfo=new DirectoryInfo(frontendPath);
                Globals.flutterPackageName = dirInfo.Name;
                if(!Directory.Exists(Globals.flutterServicesFolder))Directory.CreateDirectory(Globals.flutterServicesFolder);
                if(!Directory.Exists(Globals.flutterEntitiesFolder))Directory.CreateDirectory(Globals.flutterEntitiesFolder);
                if(!Directory.Exists(Globals.flutterEnumFolder))Directory.CreateDirectory(Globals.flutterEnumFolder);
                //FileHelper.DeleteFilesWithExtensions(Globals.flutterServicesFolder, ".dart");
                //FileHelper.DeleteFilesWithExtensions(Globals.flutterEntitiesFolder, ".dart");
                if(!Helper.ReadKeyIfNotOk(FlutterFunctionWriter.WriteFiles()))Environment.Exit(-1);
                Helper.ReadKeyIfNotOk(FlutterEntittyWriter.WriteFiles(CSharpEntityReader.fileList));
                Helper.ReadKeyIfNotOk(FlutterEnumWriter.WriteFiles(CSharpEnumReader.fileList));
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic| DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"SUCCESS");
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        #endregion
        
        
        #region GetFlutterType
        static public string GetFlutterType(string csharpTypeName, out bool isAnotherEntityImport) {
            isAnotherEntityImport = false;
            try {
                string ret = "";
                string typeName = StringHelper.RemoveString(csharpTypeName, "?");//nao tem ? em dart
                switch (typeName) {
                    case "string":
                        return "String";
                    case "DateTime":
                        return "DateTime";
                    case "bool":
                        return "bool";
                    case "Int32":
                    case "Int64":                        
                    case "int":
                    case "float":
                    case "double":
                    case "decimal":
                    case "Decimal":
                        return "int";
                    case "object":
                        return "dynamic";
                    default:
                        isAnotherEntityImport = true;
                        return csharpTypeName;
                }
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return "";
        }
        #endregion
    }
}