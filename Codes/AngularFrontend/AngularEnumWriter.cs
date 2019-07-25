using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RestInPeace.Entities;
using RestInPeace.Enums;

namespace RestInPeace.Codes {
    public class AngularEnumWriter {
        #region WriteFiles
        static public bool WriteFiles(List<EEnumFile> filesList) {
            try {
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic | DebugLevels.Files | DebugLevels.Functions | DebugLevels.All, "Writing enums to " + Globals.angularEnumsFolder);
                foreach (EEnumFile eEnumFile in filesList) {
                    if (!WriteAngularEnumFile(eEnumFile)) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region WriteFlutterEnumFile
        static public bool WriteAngularEnumFile(EEnumFile eEnumFile) {
            try {
                if (!eEnumFile.valueList.Any()) return true;
                string enumFilename = eEnumFile.enumName.ToLower() + ".ts";
                string completeFilePath = Globals.angularEnumsFolder + "/" + enumFilename;
                Logger.LogInfoIfDebugLevel(DebugLevels.Files | DebugLevels.Functions | DebugLevels.All, "\t" + enumFilename);
                StringBuilder fileContent = new StringBuilder();
                fileContent.Append("\n");
                fileContent.Append("export enum " + eEnumFile.enumName + " {\n");
                foreach (EEnumValue eEnumValue in eEnumFile.valueList) {
                    if (string.IsNullOrEmpty(eEnumValue.name)) continue;
                    if(!string.IsNullOrEmpty(eEnumValue.value)) fileContent.Append("\t" + eEnumValue.name + "= " + eEnumValue.value + ",\n");
                    else fileContent.Append($"\t{eEnumValue.name},\n");
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