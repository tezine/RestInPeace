using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RestInPeace.Entities;
using RestInPeace.Enums;

namespace RestInPeace.Codes.FlutterFrontend {
    public class FlutterEnumWriter {
        
        
        #region WriteFiles
        static public bool WriteFiles(List<EEnumFile> filesList) {
            try {
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic|DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"Writing enums to " + Globals.flutterEnumFolder );
                foreach (EEnumFile eEnumFile in filesList) {
                    if (!WriteFlutterEnumFile(eEnumFile)) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
        
        
        #region WriteFlutterEnumFile
        static public bool WriteFlutterEnumFile(EEnumFile eEnumFile) {
            try {
                if (!eEnumFile.valueList.Any()) return true;                
                string enumFilename = eEnumFile.enumName.ToLower() + ".dart";
                string completeFilePath = Globals.flutterEnumFolder + "/" + enumFilename;
                Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"\t" + enumFilename);
                StringBuilder fileContent = new StringBuilder();
                fileContent.Append("\n");
                //vamos ver se criamos como enum ou como class. Caso tenha valores, temos que criar como classe pq dart não suporta EnumValue=1
                if (string.IsNullOrEmpty(eEnumFile.valueList.FirstOrDefault().value)) {//criamos como enum em dart
                    fileContent.Append("enum " + eEnumFile.enumName + " {\n");
                    foreach (EEnumValue eEnumValue in eEnumFile.valueList) {
                        if (string.IsNullOrEmpty(eEnumValue.name)) continue;
                        string valueLine = $"\t{eEnumValue.name},\n";
                        fileContent.Append(valueLine);
                    }
                } else {//criamos como class em dart
                    fileContent.Append("class " + eEnumFile.enumName + " {\n");
                    foreach (EEnumValue eEnumValue in eEnumFile.valueList) {
                        if (string.IsNullOrEmpty(eEnumValue.name)) continue;
                        string valueLine = "\tstatic const " + eEnumValue.name + "= " + eEnumValue.value + ";\n";
                        fileContent.Append(valueLine);
                    }
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