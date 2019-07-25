using System;
using System.Collections.Generic;
using System.IO;
using TypeSync.Entities;

namespace RestInPeace.Codes {
    public class QMLTypescriptWriter {
        
        #region WriteFiles
        static public bool WriteFiles(List<EFunctionFile> functionFileList, List<EEntityFile> entityFilesList, string appDirectory) {
            try {
                string internalDirectory = appDirectory + "/Typed/Internals";
                Directory.CreateDirectory(internalDirectory+"/Forms");
                Directory.CreateDirectory(internalDirectory+"/RestInPeace/Entities");
                Directory.CreateDirectory(internalDirectory+"/RestInPeace/Enums");
                Directory.CreateDirectory(internalDirectory+"/RestInPeace/Scripts");
                string javascriptFileName = "";
                foreach (EFunctionFile eFile in functionFileList) {
                    if (eFile.csharpFileName.StartsWith("S")) javascriptFileName = eFile.csharpFileName.Remove(0, 1);
                    javascriptFileName = "J" + javascriptFileName + ".js";
                    string completeJavascriptFilePath = scriptsDirectory + "/" + javascriptFileName;
                    string completeTypescriptFilePath = internalDirectory + "/RestInPeace/Scripts/" + javascriptFileName.Replace(".js", ".d.ts");
                    Logger.LogInfo(javascriptFileName + "============================");
                    if (!GetQMLFunctions(eFile)) return false;
                    if (!WriteJavascriptFile(completeJavascriptFilePath, eFile)) return false;
                    if (!WriteTypescriptFile(completeTypescriptFilePath, eFile)) return false;
                }

                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }

            return false;
        }
        #endregion
    }
}