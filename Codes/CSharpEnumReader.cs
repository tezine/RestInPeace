using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotnetBase.Codes;
using RestInPeace.Entities;
using RestInPeace.Enums;

namespace RestInPeace.Codes {
    public class CSharpEnumReader {
        static public List<EEnumFile> fileList = new List<EEnumFile>();

        #region AnalyseFiles
        static public bool AnalyseFiles(List<string> enumsRootFolders) {
            try {
                fileList.Clear();
                foreach (string enumFolder in enumsRootFolders) {
                    Logger.LogInfoIfDebugLevel(DebugLevels.Basic|DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"Reading enum files from backend: " + enumFolder);
                    IEnumerable<string> files = Directory.EnumerateFiles(enumFolder, "*.cs", SearchOption.AllDirectories);
                    foreach (string file in files) {
                        if (file.Contains("\\obj\\")) continue;
                        string fileContent=File.ReadAllText(file);
                        if (!fileContent.Contains("[RestInPeaceEnum]")) continue;
                        if (!AnalyseFile(file,fileContent)) return false;
                    }
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region AnalyseFile - //por enquanto suporta só um enum por arquivo
        static private bool AnalyseFile(string path, string fileContent) {
            try {
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Name.ToLower().Contains("respcreateuser")) {
                    Debug.WriteLine("encontrou");
                }
                string[] linesList= fileContent.Split(new[]{ Environment.NewLine }, StringSplitOptions.None);
                var regex = new Regex("(\\w+)\\s*(?:=\\s*(\\d+))?", RegexOptions.None| RegexOptions.Singleline, TimeSpan.FromMilliseconds(1000));                
                EEnumFile eEnumFile = new EEnumFile {enumName = fileInfo.Name.Replace(".cs", String.Empty), valueList = new List<EEnumValue>()};
                Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.All, "\t"+eEnumFile.enumName);
                //let's find the internal start line
                int startLine = 0;
                for(int line=0;line<linesList.Length;line++){
                    if (!linesList.ElementAt(line).Contains("public enum")) continue;
                    startLine = line + 1;
                    break;
                }
                //let's find the internal end line
                int endLine = 0;
                for (int line = startLine; line < linesList.Length; line++) {
                    if (linesList.ElementAt(line).Contains("}")) {
                        endLine = line - 1;
                        break;
                    }
                }               
                for (int l = startLine; l <= endLine; l++) {
                    string lineContent = linesList.ElementAt(l);                                                                          
                    Match match = regex.Match(lineContent);
                    if (!match.Success) continue;
                    EEnumValue eEnumValue = new EEnumValue {name = match.Groups[1].Value, value = match.Groups[2].Value};
                    eEnumFile.valueList.Add(eEnumValue);                                                             
                }
                if (eEnumFile.valueList.Count > 0) fileList.Add(eEnumFile);
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion


        static public bool IsEnum(string typeName) {
            foreach (EEnumFile eEnumFile in fileList) {
                if (eEnumFile.enumName == typeName) return true;
            }
            return false;
        }
           
    }
}