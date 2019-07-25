#region Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RestInPeace.Entities;
#endregion

namespace RestInPeace.Codes {
    public class AngularEntityWriter {

        #region WriteFiles
        static public bool WriteFiles(List<EEntityFile> filesList) {
            try {
                Logger.LogInfo("\n\nWriting entity files to " + Globals.angularEntitiesFolder + "========================================================");
                foreach (EEntityFile eEntityFile in filesList) {
                    if (eEntityFile.onlyImport) continue;
                    if (!WriteTypescriptEntityFile(eEntityFile, Globals.angularEntitiesFolder)) return false;
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
                string typescriptFileName = eEntityFile.className.ToLower() + ".ts";
                string completeTypescriptFilePath = entityDirectory + "/" + typescriptFileName;
                Logger.LogInfo("Writing " + typescriptFileName);
                StringBuilder newFileContent = new StringBuilder();
                string imports = GetFileImports(eEntityFile);
                if (!string.IsNullOrEmpty(imports)) newFileContent.Append(imports);
                newFileContent.Append("\nexport class " + eEntityFile.className + "{");
                foreach (EEntityProperty eEntityProperty in eEntityFile.propertyList) {
                    if(!eEntityProperty.isList)newFileContent.Append("\n\t" + eEntityProperty.typescriptPropName + ":" + eEntityProperty.typescriptTypeName + ";");
                    else newFileContent.Append("\n\t" + eEntityProperty.typescriptPropName + ":" + eEntityProperty.typescriptTypeName + "[];");
                }
                newFileContent.Append("\n}");
                //Logger.LogInfo(newFileContent.ToString());
                string oldContent = "";
                if (File.Exists(completeTypescriptFilePath)) oldContent = File.ReadAllText(completeTypescriptFilePath);
                if (newFileContent.ToString() != oldContent) File.WriteAllText(completeTypescriptFilePath, newFileContent.ToString());
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region GetFileImports
        static private string GetFileImports(EEntityFile eEntityFile) {
            try {
                string imports = "";
                List<EEnumFile> restInPeaceEnums = CSharpEnumReader.fileList;
                foreach (EEntityProperty eEntityProperty in eEntityFile.propertyList) {
                    if (eEntityProperty.typescriptTypeName == "string" ||
                        eEntityProperty.typescriptTypeName == "number" ||
                        eEntityProperty.typescriptTypeName == "boolean"||
                        eEntityProperty.typescriptTypeName == "object"||
                        eEntityProperty.typescriptTypeName=="Date") continue;

                    if (eEntityProperty.typescriptTypeName.Contains("TimeSpan")) {
                        if(!imports.Contains("TimeSpan")) imports += "\nimport {TimeSpan} from 'typescript-dotnet-amd/System/Time/TimeSpan';";
                        continue;
                    }else {
                        Logger.LogInfo("entidade:"+eEntityProperty.csharpTypeName);
                        if (eEntityProperty.csharpTypeName.ToLower() == "entitytype") {
                        
                        }
                        string import = "";
                        if(restInPeaceEnums.Any(x=>String.Equals(x.enumName, eEntityProperty.csharpTypeName, StringComparison.CurrentCultureIgnoreCase))) import+="\nimport { " + eEntityProperty.csharpTypeName + " } from '../../enums/restinpeace/" + eEntityProperty.csharpTypeName.ToLower() + "';";
                        else import = "\nimport {" + eEntityProperty.csharpTypeName + "} from './"+eEntityProperty.csharpTypeName.ToLower()+"';";
                        if (!imports.Contains(import)) imports += import;
                    }
                }
                if (!string.IsNullOrEmpty(imports)) imports += "\n";
                return imports;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return "";
        }
        #endregion
    }
}