#region Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DotnetBase.Codes;
using RestInPeace.Enums;
#endregion

namespace RestInPeace.Codes.QMLTypescriptFrontend {    
    
    static public class QmlTSResourceWriter {        
        #region WriteFiles
        static public bool WriteFiles(string frontendPath) {
            try {
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic| DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"Writing Resource files...");
                //let's write all that must be inside scripts.qrc
                List<FileInfo> scripts= FileHelper.GetFiles(Globals.frontendQmlScriptsFolder, ".js", ".mjs");                
                if (!WriteResourceFile(scripts, "/Scripts",Globals.frontendQmlScriptsFolder + "/scripts.qrc")) return false;
                //let's write all that must be inside qmlpages.qrc
                List<FileInfo> qmlPageFiles= FileHelper.GetFiles(Globals.frontendQmlPagesFolder, ".qml", ".mjs");
                if (!WriteResourceFile(qmlPageFiles, "/QmlPages",Globals.frontendQmlPagesFolder + "/qmlpages.qrc")) return false;
                //now let's write all RestInPeace files into restinpeace.qrc
                List<FileInfo> restInPeaceScripts= FileHelper.GetFiles(Globals.frontendRestInPeaceFolder,  ".mjs");
                List<FileInfo> restInPeaceEnums= FileHelper.GetFiles(Globals.frontendRestInPeaceFolder+"/Enums",  ".mjs");                
                if (!WriteRestInPeaceFiles(restInPeaceScripts, restInPeaceEnums, Globals.frontendRestInPeaceFolder + "/restinpeace.qrc")) return false;
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region WriteResourceFile
        static private bool WriteRestInPeaceFiles(List<FileInfo> restInPeaceScripts,  List<FileInfo> enums, string completeResourcePath) {
            try {                
                Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"\t" + completeResourcePath);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<RCC>\n");
                stringBuilder.Append($"\t<qresource prefix=\"/RestInPeace\">\n");
                foreach (FileInfo fileInfo in restInPeaceScripts) {
                    stringBuilder.Append($"\t\t<file>{fileInfo.Name}</file>\n");
                }
                foreach (FileInfo fileInfo in enums) {
                    stringBuilder.Append($"\t\t<file>Enums/{fileInfo.Name}</file>\n");
                }                
                stringBuilder.Append("\t</qresource>\n");
                stringBuilder.Append("</RCC>");
                string oldContent = "";
                string fileContent = stringBuilder.ToString();
                if (File.Exists(completeResourcePath)) oldContent = File.ReadAllText(completeResourcePath);
                if (fileContent != oldContent) File.WriteAllText(completeResourcePath, fileContent);
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
        
        
        #region WriteResourceFile
        static private bool WriteResourceFile(List<FileInfo> restInPeaceScripts, string prefix, string completeResourcePath) {
            try {                
                Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"\t" + completeResourcePath);
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("<RCC>\n");
                stringBuilder.Append($"\t<qresource prefix=\"{prefix}\">\n");
                foreach (FileInfo fileInfo in restInPeaceScripts) {
                    stringBuilder.Append($"\t\t<file>{fileInfo.Name}</file>\n");
                }
                stringBuilder.Append("\t</qresource>\n");
                stringBuilder.Append("</RCC>");
                string oldContent = "";
                string fileContent = stringBuilder.ToString();
                if (File.Exists(completeResourcePath)) oldContent = File.ReadAllText(completeResourcePath);
                if (fileContent != oldContent) File.WriteAllText(completeResourcePath, fileContent);
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
    }
}