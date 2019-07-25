using System;
using System.Diagnostics;
using RestInPeace.Enums;

namespace RestInPeace.Codes {
    public class Logger {

        static public void LogError(Exception ex) {
            Console.WriteLine(ex.ToString());
            Debug.WriteLine(ex.ToString());
        }

        static public void LogError(string txt) {
            Console.WriteLine(txt);
            Debug.WriteLine(txt);
        }

        static public void LogInfo(string txt) {
            Console.WriteLine(txt);
            Debug.WriteLine(txt);
        }
        
        static public void LogInfoIfDebugLevel(DebugLevels debugLevels, string txt) {
            bool hasFlag = (Globals.debugLevel & debugLevels) > 0;
            if (!hasFlag) return;
            Console.WriteLine(txt);
            Debug.WriteLine(txt);
        }
    }
}