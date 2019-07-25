#region Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RestInPeace.Entities;
using RestInPeace.Enums;
#endregion

namespace RestInPeace.Codes.QMLTypescriptFrontend {
    public class QmlTSEntityWriter {
        #region WriteFiles
        static public bool WriteFiles(List<EEntityFile> filesList) {
            try {
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic|DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"Writing Typescript entities definitions (d.ts) to " + Globals.frontendRestInPeaceFolder );
                foreach (EEntityFile eEntityFile in filesList) {
                    if (eEntityFile.onlyImport) continue;
                    if (!WriteTypescriptEntityFile(eEntityFile, Globals.frontendRestInPeaceFolder)) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region WriteTypescriptEntityFile
        static private bool WriteTypescriptEntityFile(EEntityFile eEntityFile, string entityDirectory) {
            try {
                string typescriptEntityFileName = eEntityFile.className + ".d.ts";
                string completeFilePath = entityDirectory + "/" + typescriptEntityFileName;
                Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"\t" + typescriptEntityFileName);
                StringBuilder fileContent = new StringBuilder();
                fileContent.Append("\n");
                fileContent.Append("export declare class " + eEntityFile.className + " {\n");
                foreach (EEntityProperty eEntityProperty in eEntityFile.propertyList) {
                    if (string.IsNullOrEmpty(eEntityProperty.name)) continue;
                    if (!WriteProperty(eEntityProperty, out string propertyLine)) {
                        Logger.LogError("unable to write property in file " + typescriptEntityFileName);
                        return false;
                    }
                    fileContent.Append(propertyLine);
                }
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
                if (eEntityProperty.csharpTypeName.Contains("?")) {
                    propertyLine = "\t" + eEntityProperty.name + "?: " + GetPropertyType(eEntityProperty)+";\n";
                    return true;
                } else propertyLine = "\t" + eEntityProperty.name + ": " + GetPropertyType(eEntityProperty)+";\n";
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region GetPropertyType
        static private string GetPropertyType(EEntityProperty eEntityProperty) {
            try {
                string ret = "";
                switch (eEntityProperty.csharpTypeName) {
                    case "string":
                        return "string";
                    case "bool":
                        return "boolean";
                    case "Int32":
                    case "Int64":                        
                    case "int":
                    case "float":
                    case "double":
                    case "decimal":
                    case "Decimal":
                        return "number";
                        default:
                            return "any";
                }
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return "";
        }
        #endregion
    }
}