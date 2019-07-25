#region Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotnetBase.Codes;
using RestInPeace.Entities;
using RestInPeace.Enums;
#endregion

namespace RestInPeace.Codes.QMLTypescriptFrontend {
    public class QmlTSWatcher {

        #region Constructor
        public QmlTSWatcher( string frontendPath) {
            try {                
                Globals.frontendRestInPeaceFolder = frontendPath + "/RestInPeace";
                if(!Directory.Exists(Globals.frontendRestInPeaceFolder))Directory.CreateDirectory(Globals.frontendRestInPeaceFolder);
                FileHelper.DeleteFilesWithExtensions(Globals.frontendRestInPeaceFolder, ".js", ".mjs",".ts");                
                Globals.frontendQmlPagesFolder = frontendPath + "/QmlPages";
                Globals.frontendQmlScriptsFolder = frontendPath + "/Scripts";
                if (!Helper.ReadKeyIfNotOk(QmlTSFormsReader.AnalyseFiles(frontendPath))) Environment.Exit(-1);                   
                if(!Helper.ReadKeyIfNotOk(QmlMJSFunctionWriter.WriteFiles()))Environment.Exit(-1);
                Helper.ReadKeyIfNotOk(QmlTSScriptWriter.WriteFiles());               
                Helper.ReadKeyIfNotOk(QmlTSEntityWriter.WriteFiles(CSharpEntityReader.fileList));                
                Helper.ReadKeyIfNotOk(QmlTSEnumWriter.WriteFiles(CSharpEnumReader.fileList));                
                Helper.ReadKeyIfNotOk(QmlTSFormsWriter.WriteFiles());                
                Helper.ReadKeyIfNotOk(QmlTSUIWriter.WriteFiles(Globals.frontendQmlPagesFolder));                
                Helper.ReadKeyIfNotOk(QmlJSUpdater.UpdateJavascriptUIFiles());
                Helper.ReadKeyIfNotOk(QmlBasicComponents.WriteTypescriptDefinitionsToFrontend());
                Helper.ReadKeyIfNotOk(CppComponents.WriteTypescriptDefinitionsToFrontend());
                Helper.ReadKeyIfNotOk(DTSWriter.WriteTypescriptEventHandler());
                Helper.ReadKeyIfNotOk(QmlTSResourceWriter.WriteFiles(frontendPath));                
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic| DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"SUCCESS");
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        #endregion                

    }
}