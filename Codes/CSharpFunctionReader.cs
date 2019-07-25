#region Imports
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using RestInPeace.Entities;
using RestInPeace.Enums;
#endregion

namespace RestInPeace.Codes {
    public class CSharpFunctionReader {
        
        #region AnalyseFiles
        static public bool AnalyseFiles(List<string> backendRootFolders) {//ex: R:/Projetos/Uttili/Portal/Backend , R:\Projetos\ServerBase
            try {
                Globals.backendControllerFiles.Clear();                
                foreach (string backendRootFolder in backendRootFolders) {
                    Logger.LogInfoIfDebugLevel(DebugLevels.Basic|DebugLevels.Files|DebugLevels.Functions| DebugLevels.All,"Reading functions from backend: " + backendRootFolder );
                    IEnumerable<string> files = Directory.EnumerateFiles(backendRootFolder, "*.cs", SearchOption.AllDirectories);
                    foreach (string file in files) {
                        if (file.Contains("\\obj\\")) continue;
                        if (file.Contains("UserPictures")) {
                        
                        }
                        if (!AnalyseFile(file)) return false;
                    }                    
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region AnalyseFile
        static private bool AnalyseFile(string path) {
            try {
                if (!File.Exists(path)) return true;
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.Name.StartsWith("E")) return true;//entity
                string fullContent = File.ReadAllText(path);
                if (fullContent.Contains("public enum")) return true;//enum
                IEnumerable<string> linesList = fullContent.Split('\n');
                EFunctionFile eFile = new EFunctionFile();
                eFile.csharpFileName = fileInfo.Name.Replace(".cs", String.Empty);
                eFile.functionList = new List<EFunction>();
                if (fullContent.Contains("[RestInPeace")) Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.All, "\t"+eFile.csharpFileName);
                for (int l = 0; l < linesList.Count(); l++) {
                    string lineContent = linesList.ElementAt(l);
                    if (!lineContent.Contains("[RestInPeace")) continue;
                    EFunction eFunction = AnalyseTypeSyncFunction(fileInfo.Name, lineContent, l + 1);
                    if (eFunction == null) return false;
                    eFile.functionList.Add(eFunction);
                }
                if (eFile.functionList.Count > 0) Globals.backendControllerFiles.Add(eFile);
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region AnalyseTypeSyncFunction
        static private EFunction AnalyseTypeSyncFunction(string file, string txt, int lineNumber) {
            try {
                //Logger.LogInfo("analysing typesync function:" + txt);
                var regex = new Regex("\".*\"", RegexOptions.Singleline, TimeSpan.FromMilliseconds(1000));
                Match match = regex.Match(txt);
                if (!match.Success) {
                    Logger.LogError("unable to analyse function in file " + file + " at line number " + lineNumber.ToString() + ": " + txt);
                    return null;
                }
                string content = match.Captures[0].Value;
                content = content.Replace("\"", String.Empty);
                EFunction eFunction = new EFunction();
                if (txt.Contains("RestInPeaceGet")) eFunction.functionType = FunctionType.GET;
                else if (txt.Contains("RestInPeacePut")) eFunction.functionType = FunctionType.PUT;
                else if (txt.Contains("RestInPeacePost")) eFunction.functionType = FunctionType.POST;
                else if (txt.Contains("RestInPeaceDelete")) eFunction.functionType = FunctionType.DELETE;
                eFunction.argsList = new List<EArg>();
                eFunction.version = GetVersion(file, lineNumber, txt);
                if (eFunction.version < 1) {
                    Logger.LogError("unable to detect function version in file " + file + " at line number " + lineNumber.ToString() + ": " + txt);
                    return null;
                }
                if (!GetFunctionNameAndReturnType(file, lineNumber, content, ref eFunction)) {
                    Logger.LogError("unable to get function name in file " + file + " at line number " + lineNumber.ToString() + ": " + txt);
                    return null;
                }
                if (!GetFunctionArguments(file, lineNumber, content, ref eFunction)) {
                    Logger.LogError("unable to get function arguments in file " + file + " at line number " + lineNumber.ToString() + ": " + txt);
                    return null;
                }
                StringBuilder args = new StringBuilder();
                for (int i = 0; i < eFunction.argsList.Count; i++) {
                    EArg eArg = eFunction.argsList.ElementAt(i);
                    if (eArg.argType == ArgType.Body) args.Append("body ");
                    else if (eArg.argType == ArgType.OptionalParameter) args.Append("optional ");
                    else if (eArg.argType == ArgType.MandatoryParameter) args.Append("mandatory ");
                    else if (eArg.argType == ArgType.Segment) args.Append("segment ");
                    args.Append(eArg.typeName);
                    args.Append(" ");
                    args.Append(eArg.name);
                    if (i < (eFunction.argsList.Count - 1)) args.Append(", ");
                }
                Logger.LogInfoIfDebugLevel(DebugLevels.Functions|DebugLevels.All, "\t\t" +eFunction.functionName );
                return eFunction;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return null;
        }
        #endregion

        #region GetVersion
        static private int GetVersion(string file, int lineNumber, string txt) {
            try {
                var regexPost = new Regex("RestInPeacePost\\((\\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline, TimeSpan.FromMilliseconds(1000));
                Match postMatch = regexPost.Match(txt);
                if (postMatch.Success) return Convert.ToInt32(postMatch.Groups[1].Value);

                var regexPut = new Regex("RestInPeacePut\\((\\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline, TimeSpan.FromMilliseconds(1000));
                Match putMatch = regexPut.Match(txt);
                if (putMatch.Success) return Convert.ToInt32(putMatch.Groups[1].Value);

                var regexGet = new Regex("RestInPeaceGet\\((\\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline, TimeSpan.FromMilliseconds(1000));
                Match getMatch = regexGet.Match(txt);
                if (getMatch.Success) return Convert.ToInt32(getMatch.Groups[1].Value);

                var regexDelete = new Regex("RestInPeaceDelete\\((\\d+)", RegexOptions.IgnoreCase | RegexOptions.Singleline, TimeSpan.FromMilliseconds(1000));
                Match deleteMatch = regexDelete.Match(txt);
                if (deleteMatch.Success) return Convert.ToInt32(deleteMatch.Groups[1].Value);
                Logger.LogError($"Unable to find version in file {file} at lineNumber {lineNumber}");
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return -1;
        }
        #endregion

        #region GetFunctionNameAndReturnType
        //Line Ex: [RestInPeacePost(1, "ECaminhao GetByID(segment number id, mandatory number age, optional string bla=0, body string oi)")]
        //txt=ECaminhao GetByID(segment number id, mandatory number age, optional string bla=0, body string oi)
        static private bool GetFunctionNameAndReturnType(string file, int lineNumber, string txt, ref EFunction eFunction) {
            try {
                int end=txt.IndexOf('(');
                if (end == -1 ) {
                    Logger.LogError("unable to find '(' in file "+file+" at line number "+lineNumber.ToString()+": "+txt);
                    return false;
                }
                string functionNameWithReturn = txt.Substring(0, end).Trim();
                string[] words = functionNameWithReturn.Split(' ');
                if (words.Length != 2) {
                    Logger.LogError("unable to find return type and function name in file "+file+" at line number "+lineNumber.ToString()+": "+txt);
                    return false;
                }
                eFunction.returnTypeName = words[0].Trim();
                eFunction.functionName=words[1].Trim();
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region GetUrlSegments
        //Line Ex: [RestInPeacePost(1, "ECaminhao GetByID(segment number id, mandatory number age, optional string bla=0, body string oi)")]
        //txt=ECaminhao GetByID(segment number id, mandatory number age, optional string bla=0, body string oi)
        static private bool GetFunctionArguments(string file, int lineNumber, string txt, ref EFunction eFunction) {
            try {
                int start=txt.IndexOf('(');
                int end = txt.IndexOf(')');
                if (start == -1 || end == -1) {
                    Logger.LogError("unable to find '(' or ')' in file "+file+" at line number "+lineNumber.ToString()+": "+txt);
                    return false;
                }
                string arguments = txt.Substring(start+1, end-start-1);
                string[] splittedContent = arguments.Split(',');
                foreach(string arg in splittedContent) {
                    if (!GetArgument(file,lineNumber, arg.Trim(), ref eFunction)) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region GetArgument
        static private bool GetArgument(string file, int lineNumber, string txt, ref EFunction eFunction) {
            try {
                EArg eArg=new EArg();
                if (txt.Trim().Length == 0) return true;//no arguments
                var regex = new Regex("(\\w+)\\s+(\\w+\\[?\\]?)\\s+(\\w+)\\s*=?\\s*(.+)?", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000));
                Match match = regex.Match(txt);
                if (!match.Success) {
                    Logger.LogError("unable to identify argument in file "+file+" at line number "+lineNumber.ToString()+": "+txt);
                    return false;
                }
                string argType = match.Groups[1].Value.ToLower().Trim();
                eArg.typeName = match.Groups[2].Value.Trim();
                eArg.name = match.Groups[3].Value.Trim();
                if (match.Groups[4].Success) {
                    eArg.defaultValue = match.Groups[4].Value.Trim();
                }
                if (argType.Contains("mandatory")) {
                    eArg.argType =ArgType.MandatoryParameter;
                    eFunction.argsList.Add(eArg);
                }else if (argType.Contains("optional")) {
                    eArg.argType = ArgType.OptionalParameter;
                    eFunction.argsList.Add(eArg);
                }else if (argType.Contains("segment")) {
                    eArg.argType = ArgType.Segment;
                    eFunction.argsList.Add(eArg);
                }else if (argType.Contains("form")) {
                    eArg.argType=ArgType.FormData;
                    eFunction.argsList.Add(eArg);
                    if (eFunction.functionType == FunctionType.GET || eFunction.functionType == FunctionType.DELETE) {
                        Logger.LogError("Get/Delete function can not have form in file " + file + " at line number " + lineNumber.ToString() + ": " + txt);
                        return false;
                    }
                }else if (argType.Contains("body")) {
                    eArg.argType = ArgType.Body;
                    eFunction.argsList.Add(eArg);
                    if (eFunction.functionType == FunctionType.GET || eFunction.functionType == FunctionType.DELETE) {
                        Logger.LogError("Get/Delete function can not have body in file " + file + " at line number " + lineNumber.ToString() + ": " + txt);
                        return false;
                    }
                } else {
                    Logger.LogError("unable to idenfity argument type in file " + file + " at line number " + lineNumber.ToString() + ": " + txt);
                    return false;
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