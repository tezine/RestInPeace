#region Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DotnetBase.Codes;
using RestInPeace.Entities;
#endregion

namespace RestInPeace.Codes {
    public class AngularFunctionWriter {

        static private string restInPeaceVersion = "";
        
        #region WriteFiles
        static public bool WriteFiles( List<EEntityFile> entityFilesList, string restInPeaceVersion) {
            try {
                Logger.LogInfo("\n\nWriting files to " + Globals.angularServicesFolder + "========================================================");
                AngularFunctionWriter.restInPeaceVersion = restInPeaceVersion;
                foreach (EFunctionFile eFile in Globals.backendControllerFiles) {
                    if (eFile.csharpFileName.StartsWith("S")) eFile.frontendFileName = eFile.csharpFileName.Remove(0, 1);
                    if (eFile.csharpFileName.Contains(".service")) {
                        eFile.frontendClassName = eFile.csharpFileName.Remove(0, 1);
                        eFile.frontendClassName = StringHelper.RemoveString(eFile.frontendClassName, ".service");
                        eFile.frontendFileName = Globals.blazorServicesFolder + "/" + eFile.frontendClassName.ToLower() + ".service.ts";                        
                        eFile.frontendClassName = eFile.frontendClassName + "Service";
                    } else {
                        eFile.frontendClassName = eFile.csharpFileName.Remove(0, 1) + "Service";
                        eFile.frontendFileName = Globals.blazorServicesFolder + "/" + eFile.csharpFileName.Remove(0, 1).ToLower() + ".service.ts";
                    }
                    string serviceFileName = Globals.angularServicesFolder + "/" + eFile.frontendFileName;
                    Logger.LogInfo(eFile.frontendFileName + "============================");
                    List<string> entitiesUsedList = GetProjectEntitiesUsedInFile(entityFilesList, eFile);
                    if (entitiesUsedList != null) {
                        entitiesUsedList = entitiesUsedList.Distinct().ToList();
                        Logger.LogInfo("\tEntity Imports: " + string.Join(",", entitiesUsedList.ToArray()));
                    }
                    if (!GetTypescriptFunctions(eFile)) return false;
                    if (entitiesUsedList == null) return false;
                    if (!WriteTypescriptFile(entitiesUsedList, serviceFileName, eFile)) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region GetTypescriptFunctions
        static private bool GetTypescriptFunctions(EFunctionFile eFile) {
            try {
                //Logger.LogInfo("Writing functions of file " + eFile.fileName);
                foreach (EFunction eFunction in eFile.functionList) {
                    if (!WriteFunction(eFile, eFunction)) {
                        Logger.LogError("unable get typescript function " + eFunction.functionName + " from file " + eFile.csharpFileName);
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

        #region WriteFunction
        //Ex from Typescript: [RestInPeacePost("ECaminhao GetByID(segment number id, mandatory number age, optional string bla, body string oi)")]
        static private bool WriteFunction(EFunctionFile eFile, EFunction eFunction) {
            try {
                StringBuilder result = new StringBuilder();
                StringBuilder argsStringBuilder = new StringBuilder();
                List<EArg> segmentsList, mandatoryList, optionalList, formArgList, allArgsList;
                EArg bodyArg;
                Logger.LogInfo("\tFunction " + eFunction.functionName);
                string functionName = Char.ToLowerInvariant(eFunction.functionName[0]) + eFunction.functionName.Substring(1);
                string operation = Helper.GetRESTOperation(eFile, eFunction);
                if (string.IsNullOrEmpty(operation)) return false;
                if (!GetFunctionArgs(eFunction, ref argsStringBuilder, out allArgsList, out segmentsList, out mandatoryList, out optionalList, out formArgList, out bodyArg)) {
                    Logger.LogError("unable to get funcion parameters to write file " + eFile.frontendFileName + ": " + eFunction.functionName);
                    return false;
                }
                result.Append("\tasync ");
                result.Append(functionName);
                result.Append("(");
                result.Append(argsStringBuilder.ToString());
                result.Append("):Promise<");
                result.Append(AngularWatcher.ConvertToAngularTypeName(eFunction.returnTypeName));
                result.Append(">{\n");
                string functionBody = GetFunctionBody(eFile, eFunction, operation, segmentsList, mandatoryList, optionalList, formArgList, bodyArg);
                result.Append(functionBody);
                eFunction.frontendFunctionContent = result.ToString();
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region GetFunctionBody
        static private string GetFunctionBody(EFunctionFile eFile, EFunction eFunction, string operation, List<EArg> segmentsList, List<EArg> mandatoryList, List<EArg> optionalList, List<EArg> formArgList, EArg bodyArg) {
            try {
                StringBuilder bodyTxt = new StringBuilder();
                if (formArgList.Any()) {
                    bodyTxt.Append("\t\tlet formData = new FormData();\n");
                    foreach (EArg formArg in formArgList) {
                        if(formArg.typeName=="any")bodyTxt.Append("\t\tformData.append('" + formArg.name + "', " + formArg.name + ");\n");
                        else bodyTxt.Append("\t\tformData.append('" + formArg.name + "', " + formArg.name + ".toString());\n");
                    }
                    bodyTxt.Append("\t\tconst response = await this.authHttp." + operation + "(Defines.RestBaseURL + '/v" + eFunction.version.ToString() + "/S" + eFile.frontendClassName + "/" + eFunction.functionName + "'");
                    if(string.IsNullOrEmpty(AngularFunctionWriter.restInPeaceVersion))bodyTxt.Append(", formData).timeout(timeoutSeconds*1000).toPromise().catch(Helper.handleError);\n");
                    else bodyTxt.Append(", formData).toPromise().catch(Helper.handleError);\n");
                    bodyTxt.Append("\t\treturn response;\n");
                    bodyTxt.Append("\t}");
                    return bodyTxt.ToString();
                }
                if (mandatoryList.Count > 0 || optionalList.Count > 0) {
                    bodyTxt.Append("\t\tlet params: HttpParams = new HttpParams();\n");
                    foreach (EArg mandatoryArg in mandatoryList) {
                        bodyTxt.Append("\t\tparams= params.set('" + mandatoryArg.name + "', " + mandatoryArg.name + ".toString());\n");
                    }
                    foreach (EArg optionalArg in optionalList) {
                        bodyTxt.Append("\t\tif(" + optionalArg.name + "!=null) params= params.set('" + optionalArg.name + "', " + optionalArg.name + ".toString());\n");
                    }
                }
                bodyTxt.Append("\t\tconst response = await this.authHttp." + operation + "(Defines.RestBaseURL + '/v" + eFunction.version.ToString() + "/S" + eFile.frontendClassName + "/" + eFunction.functionName + "'");
                foreach (EArg eArgSegment in segmentsList) {
                    bodyTxt.Append("+'/'+" + eArgSegment.name);
                }
                if (bodyArg != null) {                    
                    if (string.IsNullOrEmpty(AngularFunctionWriter.restInPeaceVersion)) bodyTxt.Append(", JSON.stringify(" + bodyArg.name + ")");
                        else bodyTxt.Append(", JSON.stringify(" + bodyArg.name + "), Defines.httpOptions");                     
                } else {
                    if ((!string.IsNullOrEmpty(AngularFunctionWriter.restInPeaceVersion)) && (eFunction.functionType == FunctionType.POST || eFunction.functionType==FunctionType.PUT)) {
                        bodyTxt.Append(", Defines.httpOptions");
                    }
                }
                if (string.IsNullOrEmpty(AngularFunctionWriter.restInPeaceVersion)) {
                    if (eFunction.functionType == FunctionType.PUT || eFunction.functionType == FunctionType.POST) {
                        bodyTxt.Append(", { headers: Defines.Headers }");
                    }
                }
                if (mandatoryList.Count > 0 || optionalList.Count > 0) {
                    bodyTxt.Append(", {params: params}");
                }
                if(string.IsNullOrEmpty(AngularFunctionWriter.restInPeaceVersion))bodyTxt.Append(").timeout(timeoutSeconds*1000).toPromise().catch(Helper.handleError);\n");
                else bodyTxt.Append(").toPromise().catch(Helper.handleError);\n");
                if(string.IsNullOrEmpty(AngularFunctionWriter.restInPeaceVersion))bodyTxt.Append("\t\treturn response.json();\n");
                else bodyTxt.Append("\t\treturn response;\n");
                bodyTxt.Append("\t}");
                return bodyTxt.ToString();
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return "";
        }
        #endregion

        #region GetFunctionArgs
        static private bool GetFunctionArgs(EFunction eFunction, ref StringBuilder argsStringBuilder, out List<EArg> allArgsList, out List<EArg> segmentsList, out List<EArg> mandatoryList, out List<EArg> optionalList, out List<EArg> formArgList, out EArg bodyArg) {
            segmentsList = null;
            mandatoryList = null;
            optionalList = null;
            formArgList = null;
            allArgsList = new List<EArg>();
            bodyArg = null;
            try {
                //orderby then to linq nao funciona  para mesmo campo
                bodyArg = Helper.GetBodyArg(eFunction.argsList);
                segmentsList = Helper.GetSegmentsArgs(eFunction.argsList);
                mandatoryList = Helper.GetMandatoryArgs(eFunction.argsList);
                optionalList = Helper.GetOptionalArgs(eFunction.argsList);
                formArgList = Helper.GetFormArgs(eFunction.argsList);
                if (bodyArg != null) allArgsList.Add(bodyArg);
                if (segmentsList.Any()) allArgsList.AddRange(segmentsList);
                if (mandatoryList.Any()) allArgsList.AddRange(mandatoryList);
                if (optionalList.Any()) allArgsList.AddRange(optionalList);
                if (formArgList.Any()) allArgsList.AddRange(formArgList);
                for (int i = 0; i < allArgsList.Count; i++) {
                    EArg eArg = allArgsList.ElementAt(i);
                    argsStringBuilder.Append(eArg.name);
                    argsStringBuilder.Append(":");
                    argsStringBuilder.Append(AngularWatcher.ConvertToAngularTypeName( eArg.typeName));
                    if (!string.IsNullOrEmpty(eArg.defaultValue)) argsStringBuilder.Append("=" + eArg.defaultValue);
                    if (i < (allArgsList.Count - 1)) argsStringBuilder.Append(", ");
                }
                if (argsStringBuilder.Length > 0) argsStringBuilder.Append(", timeoutSeconds:number=20000");
                else argsStringBuilder.Append("timeoutSeconds:number=20000");
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region WriteTypescriptFile
        static private bool WriteTypescriptFile(List<string> entitiesList, string serviceFileName, EFunctionFile eFile) {
            try {
                StringBuilder newFileContent = new StringBuilder();
                string imports = GetImports(entitiesList, eFile);
                if (string.IsNullOrEmpty(imports)) return false;
                newFileContent.Append("//author Bruno Vacare Tezine\n");
                newFileContent.Append(imports);
                newFileContent.Append("@Injectable()\n");
                newFileContent.Append("export class " + eFile.frontendClassName + " {\n\n");
                if(string.IsNullOrEmpty(AngularFunctionWriter.restInPeaceVersion)) newFileContent.Append("\tconstructor(private authHttp: AuthHttp) {}\n\n");
                else newFileContent.Append("\tconstructor(private authHttp: HttpClient) {}\n\n");
                foreach (EFunction eFunction in eFile.functionList) {
                    newFileContent.Append(eFunction.frontendFunctionContent);
                    newFileContent.Append("\n\n");
                }
                newFileContent.Append("\n}");
                string oldContent = "";
                if (File.Exists(serviceFileName)) oldContent = File.ReadAllText(serviceFileName);
                if (newFileContent.ToString() != oldContent) File.WriteAllText(serviceFileName, newFileContent.ToString());
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region GetImports
        static private string GetImports(List<string> entitiesList, EFunctionFile eFile) {
            try {
                StringBuilder imports = new StringBuilder();
                imports.Append("//#region Imports\n");
                imports.Append("import { Injectable } from '@angular/core'\n");
                imports.Append("import {Http, Response, URLSearchParams} from '@angular/http'\n");
                if (string.IsNullOrEmpty(AngularFunctionWriter.restInPeaceVersion)) {
                    imports.Append("import {AuthHttp} from 'angular2-jwt'\n");
                    imports.Append("import {Observable} from 'rxjs/Rx';\n");
                } else {
                    imports.Append("import {HttpClient, HttpParams} from '@angular/common/http';\n");
                }
                imports.Append("import {Defines} from '../../codes/defines';\n");
                imports.Append("import {Helper} from '../../codes/helper';\n");
                //Logger.LogInfo("entidade usadas:"+entitiesList.Count.ToString());
                List<EEnumFile> restInPeaceEnums = CSharpEnumReader.fileList;
                
                foreach (string entityName in entitiesList) {
                    if(restInPeaceEnums.Any(x=>String.Equals(x.enumName, entityName, StringComparison.CurrentCultureIgnoreCase))) imports.Append("import { " + entityName + " } from '../../enums/restinpeace/" + entityName.ToLower() + "';\n");
                    else imports.Append("import { " + entityName + " } from '../../entities/restinpeace/" + entityName.ToLower() + "';\n"); //import { ECaminhao } from "../entities/ecaminhao";
                }
                //let's find external(not entities) imports used in file
                foreach (EFunction eFunction in eFile.functionList) {
                    GetImportFromType(ref imports, eFunction.returnTypeName);
                    foreach (EArg eArg in eFunction.argsList) {
                        GetImportFromType(ref imports, eArg.typeName);
                    }
                }
                imports.Append("//#endregion\n\n");
                return imports.ToString();
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return "";
        }
        #endregion

        #region GetImportFromType - External imports. Not entity imports
        static private void GetImportFromType(ref StringBuilder imports, string typeName) {
            try {
                if (typeName.Contains("TimeSpan") && (!imports.ToString().Contains(typeName)))imports.Append("import {TimeSpan} from 'typescript-dotnet-amd/System/Time/TimeSpan'\n");
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        #endregion

        #region GetProjectEntitiesUsedInFile
        static private List<string> GetProjectEntitiesUsedInFile(List<EEntityFile> entityFilesList, EFunctionFile eFunctionFile) {
            try {
                List<string> entitiesUsedList = new List<string>();
                foreach (EFunction eFunction in eFunctionFile.functionList) {
                    string name = StringHelper.RemoveString(eFunction.returnTypeName, "List<").Trim();
                    name = StringHelper.RemoveString(name, ">");
                    if(IsEntity(entityFilesList, name))entitiesUsedList.Add(name);
                    foreach (EArg eArg in eFunction.argsList) {
                        string name1 = StringHelper.RemoveString(eArg.typeName, "List<").Trim();
                        name1 = StringHelper.RemoveString(name1, ">");
                        if(IsEntity(entityFilesList, name1))entitiesUsedList.Add(name1);
                    }
                }
                return entitiesUsedList;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return null;
        }
        #endregion

        #region IsEntity
        static public bool IsEntity(List<EEntityFile> entityFilesList, string name) {
            try {
                foreach (EEntityFile eEntityFile in entityFilesList) {
                    if (name== eEntityFile.className) return true;
                }
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
    }
}