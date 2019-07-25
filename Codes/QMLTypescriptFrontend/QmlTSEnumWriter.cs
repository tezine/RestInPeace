using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RestInPeace.Entities;
using RestInPeace.Enums;

namespace RestInPeace.Codes.QMLTypescriptFrontend {
    public class QmlTSEnumWriter {
        #region WriteFiles
        static public bool WriteFiles(List<EEnumFile> filesList) {
            try {
                string frontEndEnumsFolder = Globals.frontendRestInPeaceFolder + "/Enums";
                if (!Directory.Exists(frontEndEnumsFolder)) Directory.CreateDirectory(frontEndEnumsFolder);
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic| DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"Writing Typescript enums to " + frontEndEnumsFolder);
                foreach (EEnumFile eEnumFile in filesList) {
                    if (!WriteTypescriptEnumFile(eEnumFile, frontEndEnumsFolder)) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region WriteTypescriptEntityFile
        static public bool WriteTypescriptEnumFile(EEnumFile eEnumFile, string frontendDirectory, bool export=true) {
            try {
                string typescriptEnumFilename = eEnumFile.enumName + ".ts";
                string completeFilePath = frontendDirectory + "/" + typescriptEnumFilename;
                Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"\t" + typescriptEnumFilename);
                StringBuilder fileContent = new StringBuilder();
                fileContent.Append("\n");
                if(export)fileContent.Append("export enum " + eEnumFile.enumName + " {\n");
                else fileContent.Append("enum " + eEnumFile.enumName + " {\n");
                foreach (EEnumValue eEnumValue in eEnumFile.valueList) {
                    if (string.IsNullOrEmpty(eEnumValue.name)) continue;
                    if (!string.IsNullOrEmpty(eEnumValue.value)) fileContent.Append("\t" + eEnumValue.name + "= " + eEnumValue.value + ",\n");
                    else fileContent.Append("\t" + eEnumValue.name +",\n");
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
    }
}