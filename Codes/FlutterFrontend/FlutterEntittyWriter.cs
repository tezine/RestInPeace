using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DotnetBase.Codes;
using RestInPeace.Entities;
using RestInPeace.Enums;

namespace RestInPeace.Codes.FlutterFrontend {
    public class FlutterEntittyWriter {        
                
        #region WriteFiles
        static public bool WriteFiles(List<EEntityFile> filesList) {
            try {
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic|DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"Writing flutter entities to " + Globals.flutterEntitiesFolder );
                foreach (EEntityFile eEntityFile in filesList) {
                    if (eEntityFile.onlyImport) continue;
                    if (!WriteFlutterEntityFile(eEntityFile, Globals.flutterEntitiesFolder)) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
        
        #region WriteTypescriptEntityFile
        static private bool WriteFlutterEntityFile(EEntityFile eEntityFile, string entityDirectory) {
            try {
                string flutterEntityFileName = eEntityFile.className.ToLower() + ".dart";
                string completeFilePath = entityDirectory + "/" + flutterEntityFileName;
                Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"\t" + flutterEntityFileName);
                StringBuilder fileContent = new StringBuilder();
                fileContent.Append("//region imports\n");
                fileContent.Append("//author Bruno Tezine\n");
                fileContent.Append("import \'package:json_annotation/json_annotation.dart\';\n");
                //let's add all imports from other entities
                foreach (EEntityProperty eEntityProperty in eEntityFile.propertyList) {
                    if (string.IsNullOrEmpty(eEntityProperty.name)) continue;
                    FlutterWatcher.GetFlutterType(eEntityProperty.csharpTypeName, out bool isAnotherEntityImport);
                    if (!isAnotherEntityImport) continue;
                    fileContent.Append("import \'package:"+Globals.flutterPackageName+"/entities/restinpeace/"+eEntityProperty.csharpTypeName.ToLower()+".dart\';\n");    
                }
                fileContent.Append("part '"+eEntityFile.className.ToLower()+".g.dart\';\n");                
                fileContent.Append("//endregion\n\n");
                fileContent.Append("@JsonSerializable(nullable: true)\n");
                fileContent.Append($"class {eEntityFile.className}"+"{\n");                                
                foreach (EEntityProperty eEntityProperty in eEntityFile.propertyList) {
                    if (string.IsNullOrEmpty(eEntityProperty.name)) continue;
                    if (!WriteProperty(eEntityProperty, out string propertyLine)) {
                        Logger.LogError("unable to write property in file " + flutterEntityFileName);
                        return false;
                    }
                    fileContent.Append(propertyLine);
                }
                fileContent.Append("\n\t" + eEntityFile.className + "({\n");
                for(int p=0;p<eEntityFile.propertyList.Count;p++) {
                    EEntityProperty eEntityProperty = eEntityFile.propertyList.ElementAt(p);
                    fileContent.Append("\t\tthis." + eEntityProperty.name);
                    if (p != (eEntityFile.propertyList.Count - 1)) fileContent.Append(",\n");
                    else fileContent.Append("\n});\n");
                }                
                fileContent.Append($"\n\tfactory {eEntityFile.className}.fromJson(Map<String, dynamic> json) => _${eEntityFile.className}FromJson(json);\n");
                fileContent.Append($"\tMap<String, dynamic> toJson() => _${eEntityFile.className}ToJson(this);\n");                
                fileContent.Append("}");
                //Logger.LogInfo(newFileContent.ToString());
                string oldContent = "";
                if (File.Exists(completeFilePath)) oldContent = File.ReadAllText(completeFilePath);
                if (fileContent.ToString() != oldContent) File.WriteAllText(completeFilePath, fileContent.ToString());
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
        
        
        #region WriteProperty
        static private bool WriteProperty(EEntityProperty eEntityProperty, out string propertyLine) {
            propertyLine = "";
            try {
                /*if (eEntityProperty.csharpTypeName.Contains("?")) {
                    propertyLine = "\t" + eEntityProperty.name + "?: " + GetPropertyType(eEntityProperty)+";\n";
                    return true;
                } else */propertyLine = "\t" +FlutterWatcher.GetFlutterType(eEntityProperty.csharpTypeName, out bool isAnotherEntityImport)+$" {eEntityProperty.name};\n";
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
        
     

       
    }
}