#region Imports
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
#endregion

namespace RestInPeace.Codes.QMLTypescriptFrontend {
    public class QmlJSUpdater {

        #region UpdateJavascriptUIFiles
        static public bool UpdateJavascriptUIFiles() {
            try {
                IEnumerable<string> javascriptScriptFiles = Directory.EnumerateFiles(Globals.frontendRestInPeaceFolder, "*.js", SearchOption.AllDirectories);
                IEnumerable<string> javascriptUIFiles = Directory.EnumerateFiles(Globals.frontendRestInPeaceFolder, "*.js", SearchOption.AllDirectories);
                List<string> restInPeaceJavascriptList=new List<string>();
                foreach (string restInPeaceJavasScriptFile in javascriptScriptFiles) {
                    FileInfo fileInfo = new FileInfo(restInPeaceJavasScriptFile);
                    restInPeaceJavascriptList.Add(Path.GetFileNameWithoutExtension(fileInfo.Name)+"_1");
                }
                
                foreach (string javascriptUIPath in javascriptUIFiles) {
                    if (!UpdateJavascriptUIFile(javascriptUIPath, restInPeaceJavascriptList)) {
                        Logger.LogError("unable to update javascript ui file");
                        return false;
                    }
                }

                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region UpdateJavascriptUIFile
        static private bool UpdateJavascriptUIFile(string completeJavascriptUIPath, List<string> restInPeaceJavascriptFiles) {
            try {
                List<string> contentLines = File.ReadAllLines(completeJavascriptUIPath).ToList();
                if (contentLines.Contains(".import com.tezine.base 1.0 as Base")) return true;
                contentLines.Insert(0, ".import com.tezine.basesingletons 1.0 as BaseSingletons");
                contentLines.Insert(1, ".import com.tezine.base 1.0 as Base");
                contentLines.RemoveAll(x => ((string) x) == "Object.defineProperty(exports, \"__esModule\", { value: true });");
                for (int i = 0; i < contentLines.Count; i++) {
                    if (contentLines[i].Contains("require(\"")) {
                        Debug.WriteLine("linha:"+contentLines.ElementAt(i));
                        var regex = new Regex("var\\s*(\\w+)\\s*=\\s*require", RegexOptions.None);
                        Match match = regex.Match(contentLines[i]);
                        if (!match.Success) {
                            Logger.LogError("unable to identify name in " + completeJavascriptUIPath);
                            return false;
                        }
                        contentLines.RemoveAt(i);
                        //if the require line is not referring a restinpeace script file, we remove it.
                        //ex: var StackView_1 = require("@QtTyped/QtQuick.Controls.2/StackView");
                        if (!restInPeaceJavascriptFiles.Contains(match.Groups[1].Value)) {
                            //let's remove the StackView_1.
                            int indice = -1;
                            while((indice = contentLines.FindIndex(x => x.Contains(match.Groups[1].Value)))>-1){
                                contentLines[indice] = contentLines.ElementAt(indice).Replace(match.Groups[1].Value + ".", "");                                
                            };//match.Groups[1].Value=StackView_1
                            i--;//por conta do contentLines.RemoveAt(i);. melhorar isso
                            continue;
                        }
                        //let's remove the JDrivers_1.
                        int index = contentLines.FindIndex(x => x.Contains(match.Groups[1].Value));
                        contentLines[index] = contentLines.ElementAt(index).Replace(match.Groups[1].Value + ".", "");
                        string realImport = match.Groups[1].Value.Replace("_1", "");
                        contentLines.Insert(2, ".import 'qrc:/Typed/_Internals/RestInPeace/Scripts/" + realImport + ".js' as " + realImport);
                        i--;//por conta do contentLines.RemoveAt(i);
                    }
                }
                File.WriteAllText(completeJavascriptUIPath, string.Join("\n", contentLines.ToArray()));
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
    }
}