using System;
using System.IO;
using RestInPeace.Enums;

namespace RestInPeace.Codes {
    public class DotnetWatcher {

        public DotnetWatcher(string frontendPath) {
            Globals.dotnetServiceFolder = frontendPath + "/Services/RestInPeace";
            Globals.dotnetEntityFolder = frontendPath + "/Entities/RestInPeace";
            if(!Directory.Exists(Globals.dotnetServiceFolder))Directory.CreateDirectory(Globals.dotnetServiceFolder);
            if(!Directory.Exists(Globals.dotnetEntityFolder))Directory.CreateDirectory(Globals.dotnetEntityFolder);
            DirectoryInfo dirInfo=new DirectoryInfo(frontendPath);
            Globals.dotnetNamespace = dirInfo.Name;
            if(!Helper.ReadKeyIfNotOk(DotnetFunctionWriter.WriteFiles()))Environment.Exit(-1);
            Helper.ReadKeyIfNotOk(DotnetEntityWriter.WriteFiles(frontendPath, CSharpEntityReader.fileList));
            Logger.LogInfoIfDebugLevel(DebugLevels.Basic| DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"SUCCESS");
        }
    }
}