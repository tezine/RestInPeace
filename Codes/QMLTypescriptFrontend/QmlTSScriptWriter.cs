#region Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DotnetBase.Codes;
using RestInPeace.Entities;
using RestInPeace.Enums;
using RestInPeace.Entities;
#endregion

namespace RestInPeace.Codes.QMLTypescriptFrontend {
    public static class QmlTSScriptWriter {
        
        
        #region WriteFiles
        static public bool WriteFiles() {
            try {
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic|DebugLevels.Files|DebugLevels.Functions|DebugLevels.All, "Writing Typescript scripts to " + Globals.frontendRestInPeaceFolder);               
                //string typescriptFileName = "";
                foreach (EFunctionFile eFile in Globals.backendControllerFiles) {
                    //if (eFile.csharpFileName.StartsWith("S")) typescriptFileName = eFile.csharpFileName.Remove(0, 1);
                    
                                        
                    if (eFile.csharpFileName.Contains(".service")) {
                        eFile.frontendClassName = eFile.csharpFileName.Remove(0, 1);
                        eFile.frontendClassName = StringHelper.RemoveString(eFile.frontendClassName, ".service");
                        eFile.frontendClassName = "J"+eFile.frontendClassName + "Service";
                        eFile.frontendFileName = Globals.frontendRestInPeaceFolder + "/" + eFile.frontendClassName + ".d.ts";
                    } else {
                        eFile.frontendClassName = "J"+eFile.csharpFileName.Remove(0, 1) + "Service";
                        eFile.frontendFileName = Globals.frontendRestInPeaceFolder + "/" + eFile.csharpFileName.Remove(0, 1) + "Service.d.ts";
                    }

                    
                    //typescriptFileName = "J" + typescriptFileName + ".d.ts";
                    //string completeTypescriptFilePath = Globals.frontendRestInPeaceFolder + "/" + typescriptFileName;
                    Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.Functions|DebugLevels.All, eFile.frontendFileName);
                    if (!WriteTypescriptFile(eFile.frontendFileName, eFile)) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion  
                
        #region WriteTypescriptFile
        static private bool WriteTypescriptFile(string completeTypescriptFilename, EFunctionFile eFile) {
            try {
                //string typescriptClassName = "";
                //if (eFile.csharpFileName.StartsWith("S")) typescriptClassName = eFile.csharpFileName.Remove(0, 1);
                StringBuilder newFileContent = new StringBuilder();
                newFileContent.Append("//author Bruno Tezine\n");
                //let's insert all entity imports
                List<string> importList=new List<string>();
                List<string> allFunctionArgTypes = GetFunctionArgumentTypeNames(eFile.functionList);
                List<string> allReturnTypes = GetAllReturnTypeNames(eFile.functionList);
                newFileContent.Append("//region imports\n");
                foreach (string argType in allFunctionArgTypes) {
                    string completeImportLine = "";
                    if(CSharpEntityReader.IsEntity(argType))completeImportLine="import {" + argType + "} from \"./" + argType + "\";\n";
                    else if(CSharpEnumReader.IsEnum(argType))completeImportLine="import {" + argType + "} from \"./" + argType + "\";\n";
                    else if (!Helper.isBasicDotnetType(argType)){
                        Logger.LogError($"Unknown arg type {argType} used in file {eFile.csharpFileName}!!!");
                        continue;
                    }
                    if (importList.Contains(completeImportLine)) continue;
                    importList.Add(completeImportLine);
                }
                foreach (string returnType in allReturnTypes) {
                    string completeImportLine = "";
                    if(CSharpEntityReader.IsEntity(returnType))completeImportLine="import {" + returnType + "} from \"./" + returnType + "\";\n";
                    else if(CSharpEnumReader.IsEnum(returnType))completeImportLine="import {" + returnType + "} from \"./" + returnType + "\";\n";
                    else if (!Helper.isBasicDotnetType(returnType)){
                        Logger.LogError($"Unknown return type {returnType} used in file {eFile.csharpFileName}!!!");
                        continue;
                    }
                    if (importList.Contains(completeImportLine)) continue;
                    importList.Add(completeImportLine);
                }
                foreach (string importLine in importList) {
                    newFileContent.Append(importLine);
                }
//                newFileContent.Append("import {MySimpleEvent} from \"@QtTyped/Codes/MySimpleEvent\";\n");
                newFileContent.Append("import {MySimpleEvent} from \"./MySimpleEvent\";\n");
                newFileContent.Append("//endregion\n\n");
                //imports above
                                
                foreach (EFunction eFunction in eFile.functionList) {
                    GetTypeScriptFuctionReturnType(eFunction, out string userTypeName);
                    if (string.IsNullOrEmpty(userTypeName)) continue;
                }                
                
                newFileContent.Append("export declare class " + eFile.frontendClassName + " {\n");
                newFileContent.Append("\n");

                foreach (EFunction eFunction in eFile.functionList) {
                    Logger.LogInfoIfDebugLevel(DebugLevels.Functions|DebugLevels.All, "\tFunction " + eFunction.functionName);                    
                    string functionName = Char.ToLowerInvariant(eFunction.functionName[0]) + eFunction.functionName.Substring(1);
                    newFileContent.Append("\tstatic " + functionName + "(" + GetTypeScriptFuctionArguments(eFunction) + ")" + GetTypeScriptFuctionReturnType(eFunction, out string userTypeName));
                    newFileContent.Append("\n");
                }
                newFileContent.Append("}\n");
                string oldContent = "";
                if (File.Exists(completeTypescriptFilename)) oldContent = File.ReadAllText(completeTypescriptFilename);
                if (newFileContent.ToString() != oldContent) File.WriteAllText(completeTypescriptFilename, newFileContent.ToString());
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region GetTypeScriptFuctionArguments
        static private string GetTypeScriptFuctionArguments(EFunction eFunction) {
            List<string> argumentsList = new List<string>();
            argumentsList.Add("base: any");
            argumentsList.Add("progressBar: any");
            try {
                foreach (EArg eArg in eFunction.argsList) {
                    switch (eArg.typeName) {
                        case "string":
                            argumentsList.Add(" " + eArg.name + ": " + eArg.typeName);
                            break;
                        case "bool":
                            argumentsList.Add(" " + eArg.name + ": boolean");
                            break;
                        case "Int32":
                        case "Int64":
                        case "int":
                        case "double":
                        case "float":
                        case "decimal":
                        case "Decimal":
                            argumentsList.Add(" " + eArg.name + ": number");
                            break;
                        default:
                            argumentsList.Add(" " + eArg.name + ": " + eArg.typeName);
                            break;
                    }
                }
                return string.Join(",", argumentsList.ToArray());
            } catch (Exception e) {
                Logger.LogError(e);
            }

            return "";
        }
        #endregion

        #region GetFunctionArgumentTypeNames
        static private List<string> GetFunctionArgumentTypeNames( List<EFunction> functionList) {
            List<string> argList=new List<string>();
            foreach (EFunction eFunction in functionList) {
                foreach (EArg eArg in eFunction.argsList) {
                    if (argList.Any(x=>x== eArg.typeName)) continue;//para fazer a comparacao exata. nao remover
                    argList.Add(eArg.typeName);
                }
            }
            return argList;
        }
        #endregion
        
        #region GetAllReturnTypeNames
        static private List<string> GetAllReturnTypeNames( List<EFunction> functionList) {
            List<string> returnList=new List<string>();
            foreach (EFunction eFunction in functionList) {
                string nameWithoutList=StringHelper.RemoveString(eFunction.returnTypeName, "List<");
                nameWithoutList= StringHelper.RemoveString(nameWithoutList, ">");
                if (returnList.Contains(nameWithoutList)) continue;
                returnList.Add(nameWithoutList);
            }
            return returnList;
        }
        #endregion

        #region GetTypeScriptFuctionReturnType
        static private string GetTypeScriptFuctionReturnType(EFunction eFunction,out string userTypeName) {
            userTypeName = "";
            try {
                switch (eFunction.returnTypeName) {
                    case "void":
                        return "{}";
                    case "bool":
                        return ":MySimpleEvent<boolean>;";
                    case "Int64":
                    case "int":
                    case "double":
                    case "decimal":
                    case "Decimal":
                        return ":MySimpleEvent<number>;";
                    default: {
                        if (eFunction.returnTypeName.Contains("List")) {
                            string ret = StringHelper.RemoveString(eFunction.returnTypeName, "List<");
                            ret = StringHelper.RemoveString(ret, ">");
                            userTypeName = ret;
                            ret += "[]";
                            return ":MySimpleEvent<" + ret + ">;";
                        } else userTypeName = eFunction.returnTypeName;
                        return ":MySimpleEvent<" + eFunction.returnTypeName + ">;";
                    }
                }
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return "";
        }
        #endregion
    }
}