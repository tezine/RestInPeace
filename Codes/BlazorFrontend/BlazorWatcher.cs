using System;
using System.IO;
using DotnetBase.Codes;
using RestInPeace.Codes.QMLTypescriptFrontend;
using RestInPeace.Enums;

namespace RestInPeace.Codes.BlazorFrontend {
    public class BlazorWatcher {
          #region Constructor
        public BlazorWatcher( string frontendPath) {
            try {                
                Globals.blazorServicesFolder = frontendPath + "/Services/RestInPeace";
                Globals.blazorEntityFolder = frontendPath + "/Entities/RestInPeace";
                if(!Directory.Exists(Globals.blazorServicesFolder))Directory.CreateDirectory(Globals.blazorServicesFolder);
                if(!Directory.Exists(Globals.blazorEntityFolder))Directory.CreateDirectory(Globals.blazorEntityFolder);
                DirectoryInfo dirInfo=new DirectoryInfo(frontendPath);
                Globals.blazorNamespaceName = dirInfo.Name;
                if(!Helper.ReadKeyIfNotOk(BlazorFunctionWriter.WriteFiles()))Environment.Exit(-1);
                Helper.ReadKeyIfNotOk(BlazorEntityWriter.WriteFiles(frontendPath, CSharpEntityReader.fileList));
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic| DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"SUCCESS");
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        #endregion                
    }
}