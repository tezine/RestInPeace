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

namespace RestInPeace.Codes {
    public class Helper {

        #region GetRESTOperation
        static public string GetRESTOperation(EFunctionFile eFile, EFunction eFunction) {
            try {
                string operation = "";
                if (eFunction.functionType == FunctionType.GET) operation = "get";
                else if (eFunction.functionType == FunctionType.POST) operation = "post";
                else if (eFunction.functionType == FunctionType.PUT) operation = "put";
                else if (eFunction.functionType == FunctionType.DELETE) operation = "delete";
                else {
                    Logger.LogError("unable to identify function operation to write file " + eFile.frontendFileName + ": " + eFunction.functionName);
                    return "";
                }
                return operation;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return "";
        }
        #endregion
        
        #region GetRESTOperationForQML
        static public string GetRESTOperationForQML(EFunctionFile eFile, EFunction eFunction, bool isForTypescriptApp=false) {
            try {
                if (!isForTypescriptApp) {
                    if (eFunction.functionType == FunctionType.GET) return "get";
                    if (eFunction.functionType == FunctionType.POST)return "post";
                    if (eFunction.functionType == FunctionType.PUT)return "put";
                    if (eFunction.functionType == FunctionType.DELETE)return "del";
                } else {//for typescript app
                    if (eFunction.functionType == FunctionType.GET) return "getForTypescript";
                    if (eFunction.functionType == FunctionType.POST)return "postForTypescript";
                    if (eFunction.functionType == FunctionType.PUT)return "putForTypescript";
                    if (eFunction.functionType == FunctionType.DELETE)return "delForTypescript";
                }
                Logger.LogError("unable to identify function operation to write file " + eFile.frontendFileName + ": " + eFunction.functionName);
                return "";
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return "";
        }
        #endregion

        #region GetBodyArg
        static public EArg GetBodyArg(List<EArg> argsList) {
            try {
                foreach (EArg eArg in argsList) {
                    if (eArg.argType == ArgType.Body) return eArg;
                }
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return null;
        }
        #endregion

        #region GetSegmentsArgs
        static public List<EArg> GetSegmentsArgs(List<EArg> argsList) {
            try {
                List<EArg> segmentsList=new List<EArg>();
                foreach (EArg eArg in argsList) {
                    if (eArg.argType == ArgType.Segment)segmentsList.Add(eArg);
                }
                return segmentsList;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return null;
        }
        #endregion

        #region GetMandatoryArgs
        static public List<EArg> GetMandatoryArgs(List<EArg> argsList) {
            try {
                List<EArg> mandatoryList=new List<EArg>();
                foreach (EArg eArg in argsList) {
                    if (eArg.argType == ArgType.MandatoryParameter)mandatoryList.Add(eArg);
                }
                return mandatoryList;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return null;
        }
        #endregion

        #region GetOptionalArgs
        static public List<EArg> GetOptionalArgs(List<EArg> argsList) {
            try {
                List<EArg> optionalList=new List<EArg>();
                foreach (EArg eArg in argsList) {
                    if (eArg.argType == ArgType.OptionalParameter)optionalList.Add(eArg);
                }
                return optionalList;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return null;
        }
        #endregion

        #region GetFormArgs
        static public List<EArg> GetFormArgs(List<EArg> argsList) {
            try {
                List<EArg> formArgList=new List<EArg>();
                foreach (EArg eArg in argsList) {
                    if (eArg.argType == ArgType.FormData)formArgList.Add(eArg);
                }
                return formArgList;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return null;
        }
        #endregion

        #region isBasicDotnetType
        static public bool isBasicDotnetType(string typeName) {
            switch (typeName) {
                case "void":
                case "int":
                case "Int64":
                case "double":
                case "decimal":
                case "Decimal":
                case "string":
                case "bool":
                case "float":
                    return true;                    
            }
            return false;
        }
        #endregion

        #region ConvertQmlBasicTypeToTypescriptBasicType
        static public string ConvertQmlBasicTypeToTypescriptBasicType(string typeName) {
            switch (typeName) {
                case "int": 
                case "Int64":
                case "double":
                case "decimal":
                case "float":     
                case "var":
                case "variant":
                    return "any";
                case "real":
                case "Decimal":
                    return "number";                    
                case "bool":
                    return "boolean";             
                case "url":
                    return "URL";
                case "date":
                    return "Date";
            }
            return typeName;
        }
        #endregion

        #region IsBasicTypescriptType
        static public bool IsBasicTypescriptType(string typeName) {
            switch (typeName) {
                case "number":
                case "any":
                case "variant":
                case "boolean":
                case "Date":
                case "string":
                case "URL":
                    return true;
            }
            return false;
        }
        #endregion

        #region RenameJSToMJS
        static public void RenameJSToMJS(string folder, string excludeFiles) {
            DirectoryInfo d = new DirectoryInfo(folder);
            string[] excludedFilesnames = excludeFiles.Split(";");
            FileInfo[] infos = d.GetFiles("*.js", SearchOption.AllDirectories);
            FileInfo[] dtsList = d.GetFiles("*.d.ts", SearchOption.AllDirectories);
            //let's rename import { JUsers } from "../RestInPeace/JUsers"; to import { JUsers } from "../RestInPeace/JUsers.mjs"; so qt can handle it
            foreach (FileInfo info in infos) {
                if (!CanRenameToMJS(info.Name,excludedFilesnames)) continue;
                var replaced = Regex.Replace(File.ReadAllText(info.FullName), "import { (\\w+) } from \"(.*)\";", "import { $1 } from '$2.mjs';");
                foreach (FileInfo dtsFileInfo in dtsList) {
                    string dtsClassName = StringHelper.RemoveString(dtsFileInfo.Name, ".d.ts");                    
                    if(replaced.Contains(dtsClassName))RemoveDTSImport(info.Name, ref replaced,dtsFileInfo, dtsClassName);
                }
                File.WriteAllText(info.FullName, replaced);
            }
            foreach(FileInfo f in infos) {
                if (f.Extension != ".js") continue;
                if (!CanRenameToMJS(f.Name,excludedFilesnames)) continue;
                string destFilename = f.FullName.Replace(".js", ".mjs");
                if (File.Exists(destFilename)) File.Delete(destFilename);
                File.Move(f.FullName, destFilename);
                Logger.LogInfo($"Renamed {f.Name}");
            }
        }
        #endregion

        #region CanRenameToMJS
        static private bool CanRenameToMJS(string fileName,string[] excludedFilesnames) {
            foreach (string excluded in excludedFilesnames) {
                if (fileName == excluded.Trim()) return false;
            }
            return true;
        }
        #endregion
        
        #region ReadKeyIfnotOk
        static public bool ReadKeyIfNotOk(bool ok) {
            if (ok) return true;
            Console.ReadKey();
            return false;
        }
        #endregion

        //todo ver se dá para o typescript nao fazer o import de d.ts
        static private void RemoveDTSImport(string mjsFileName, ref string mjsContent, FileInfo dtsFileInfo, string dtsClassName) {
            string pattern = "import\\s*{\\s*" + dtsClassName + "\\s*}\\s*from\\s*.*;";
            Match match= Regex.Match(mjsContent, pattern);
            if (!match.Success) return;
            //se tiver o arquivo .mjs do arquivo d.ts, nao removemos o import
            if (File.Exists(dtsFileInfo.Directory + "/" + dtsClassName + ".mjs")) return;
            mjsContent=Regex.Replace(mjsContent, pattern,"");            
            Logger.LogInfo($"Removed import {dtsClassName} from {mjsFileName}");
        }

    }
}