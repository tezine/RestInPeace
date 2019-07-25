using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DotnetBase.Codes;
using RestInPeace.Entities;
using RestInPeace.Enums;

namespace RestInPeace.Codes {
    public class DotnetEntityWriter {
        #region WriteFiles
        static public bool WriteFiles(string frontendPath, List<EEntityFile> filesList) {
            try {
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic | DebugLevels.Files | DebugLevels.Functions | DebugLevels.All, "Writing dotnet entities to " + Globals.dotnetEntityFolder);
                List<string> sharedLibEntities = ReadSharedLibEntitiesClassNames(frontendPath);
                foreach (EEntityFile eEntityFile in filesList) {
                    if (eEntityFile.onlyImport) continue;
                    if (sharedLibEntities.Contains(eEntityFile.className)) continue;
                    if (!WriteDotnetEntityFile(eEntityFile, Globals.dotnetEntityFolder)) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region ReadSharedLibEntitiesClassNames
        static private List<string> ReadSharedLibEntitiesClassNames(string frontendPath) {
            DirectoryInfo saberLabDir = Directory.GetParent(frontendPath).Parent;

            string sharedLibEntitiesDir = "C:\\Projetos\\SaberLab\\SharedLib";//mudar
            //saberLabDir.FullName + "/Libs/SharedLib/Entities";
            if (!Directory.Exists(sharedLibEntitiesDir)) {
                Logger.LogError($"Directory {sharedLibEntitiesDir} does not exist");
                return null;
            }
            IEnumerable<string> files = Directory.EnumerateFiles(sharedLibEntitiesDir, "*.cs", SearchOption.AllDirectories);
            List<string> returnList = new List<string>();
            foreach (string file in files) {
                returnList.Add(StringHelper.RemoveString(file, ".cs"));
            }
            return returnList;
        }
        #endregion

        #region WriteDotnetEntityFile
        static private bool WriteDotnetEntityFile(EEntityFile eEntityFile, string entityDirectory) {
            try {
                string blazorEntityFileName = eEntityFile.className + ".cs";
                string completeFilePath = entityDirectory + "/" + blazorEntityFileName;
                Logger.LogInfoIfDebugLevel(DebugLevels.Files | DebugLevels.Functions | DebugLevels.All, "\t" + blazorEntityFileName);
                StringBuilder fileContent = new StringBuilder();
                fileContent.Append("#region Imports\n");
                fileContent.Append("using System;\n");
                fileContent.Append("using DotnetBase.Codes;\n");
                fileContent.Append("#endregion;\n\n");
                fileContent.Append("namespace " + Globals.dotnetNamespace + ".Entities.RestInPeace {\n");

                fileContent.Append($"\tpublic class {eEntityFile.className}" + "{\n");
                foreach (EEntityProperty eEntityProperty in eEntityFile.propertyList) {
                    if (string.IsNullOrEmpty(eEntityProperty.name)) continue;
                    fileContent.Append($"\t\tpublic {eEntityProperty.csharpTypeName} {eEntityProperty.name} " + "{ get; set; }\n");
                }
                fileContent.Append("\t}\n");
                fileContent.Append("}\n");
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
    }
}