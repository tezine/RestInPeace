using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DotnetBase.Codes;
using RestInPeace.Entities;
using RestInPeace.Enums;

namespace RestInPeace.Codes.FlutterFrontend {
    public class FlutterFunctionWriter {
        #region WriteFiles
        static public bool WriteFiles() {
            try {
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic | DebugLevels.Files | DebugLevels.Functions | DebugLevels.All, "Writing flutter services to " + Globals.flutterServicesFolder);
                foreach (EFunctionFile eFile in Globals.backendControllerFiles) {
                    if (!eFile.csharpFileName.StartsWith("S")) continue;
                    eFile.frontendClassName = eFile.csharpFileName.Remove(0, 1) + "Service";
                    eFile.frontendFileName = Globals.flutterServicesFolder + "/" + eFile.csharpFileName.Remove(0, 1).ToLower() + "_service.dart";
                    ;
                    Logger.LogInfoIfDebugLevel(DebugLevels.Files | DebugLevels.Functions | DebugLevels.All, "\t" + eFile.frontendClassName);
                    if (!WriteFlutterFile(eFile)) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region WriteFlutterFile
        static private bool WriteFlutterFile(EFunctionFile eFile) {
            try {
                StringBuilder newFileContent = new StringBuilder();
                newFileContent.Append("//region imports\n");
                newFileContent.Append("//author Bruno Tezine\n");
                newFileContent.Append("import \'dart:convert\';\n");
                newFileContent.Append("import \'package:http/http.dart\' as http;\n");                
                newFileContent.Append("import \'package:flutter_base/codes/bool_helper.dart\';\n");
                newFileContent.Append($"import \'package:{Globals.flutterPackageName}/codes/defines.dart\';\n");
                //let's insert all entity imports
                List<string> importList = new List<string>();
                List<string> allFunctionArgTypes = GetFunctionArgumentTypeNames(eFile.functionList);
                List<string> allReturnTypes = GetAllReturnTypeNames(eFile.functionList);
                foreach (string argType in allFunctionArgTypes) {
                    string completeImportLine = "";
                    if (CSharpEntityReader.IsEntity(argType)) completeImportLine = "import 'package:" + Globals.flutterPackageName + "/entities/restinpeace/" + argType.ToLower() + ".dart';\n";
                    else if (CSharpEnumReader.IsEnum(argType)) completeImportLine = "import 'package:" + Globals.flutterPackageName + "/enums/" + argType.ToLower() + ".dart';\n";
                    else if (!Helper.isBasicDotnetType(argType)) {
                        Logger.LogError($"Unknown arg type {argType} used in file {eFile.csharpFileName}!!!");
                        continue;
                    }
                    if (importList.Contains(completeImportLine)) continue;
                    importList.Add(completeImportLine);
                }
                foreach (string returnType in allReturnTypes) {
                    string completeImportLine = "";
                    if (CSharpEntityReader.IsEntity(returnType)) completeImportLine = "import 'package:" + Globals.flutterPackageName + "/entities/restinpeace/" + returnType.ToLower() + ".dart';\n";
                    else if (CSharpEnumReader.IsEnum(returnType)) completeImportLine = "import 'package:" + Globals.flutterPackageName + "/enums/" + returnType.ToLower() + ".dart';\n";
                    else if (!Helper.isBasicDotnetType(returnType)) {
                        Logger.LogError($"Unknown return type {returnType} used in file {eFile.csharpFileName}!!!");
                        continue;
                    }
                    if (importList.Contains(completeImportLine)) continue;
                    importList.Add(completeImportLine);
                }
                foreach (string importLine in importList) {
                    newFileContent.Append(importLine);
                }
                newFileContent.Append("//endregion\n\n");
                //imports above                

                newFileContent.Append($"class {eFile.frontendClassName}" + "{\n");
//                foreach (EFunction eFunction in eFile.functionList) {
//                    string flutterTypeName=FlutterWatcher.GetFlutterType(eFunction.returnTypeName, out bool isAnotherEntityImport);//antes era GetFlutterFuctionReturnType
//                    if (string.IsNullOrEmpty(flutterTypeName)) continue;
//                }                                
                newFileContent.Append("\n");
                foreach (EFunction eFunction in eFile.functionList) {
                    if (!WriteFunction(eFile, eFunction)) {
                        Logger.LogError("unable get function " + eFunction.functionName + " from file " + eFile.csharpFileName);
                        return false;
                    }
                    newFileContent.Append(eFunction.frontendFunctionContent);
                    newFileContent.Append("\n\n");
                }
                newFileContent.Append("}\n");
                string oldContent = "";
                if (File.Exists(eFile.frontendFileName)) oldContent = File.ReadAllText(eFile.frontendFileName);
                if (newFileContent.ToString() != oldContent) File.WriteAllText(eFile.frontendFileName, newFileContent.ToString());
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region WriteFunction
        static private bool WriteFunction(EFunctionFile eFile, EFunction eFunction) {
            try {
                StringBuilder result = new StringBuilder();
                StringBuilder argsStringBuilder = new StringBuilder();
                List<EArg> segmentsList, mandatoryList, optionalList, formArgList, allArgsList;
                EArg bodyArg;
                Logger.LogInfoIfDebugLevel(DebugLevels.Functions | DebugLevels.All, "\tFunction " + eFunction.functionName);
                string functionName = Char.ToLowerInvariant(eFunction.functionName[0]) + eFunction.functionName.Substring(1);
                eFunction.frontendReturnTypeName = FlutterWatcher.GetFlutterType(eFunction.returnTypeName, out bool isAnotherEntityImport); //antes era GetFlutterFuctionReturnType                
                string operation = Helper.GetRESTOperation(eFile, eFunction);
                if (string.IsNullOrEmpty(operation)) return false;
                if (!GetFunctionArgs(eFunction, ref argsStringBuilder, out allArgsList, out segmentsList, out mandatoryList, out optionalList, out formArgList, out bodyArg)) {
                    Logger.LogError("unable to get funcion parameters to write file " + eFile.frontendFileName + ": " + eFunction.functionName);
                    return false;
                }
                result.Append("\tstatic Future<" + eFunction.frontendReturnTypeName + "> " + functionName + "(" + argsStringBuilder.ToString() + ") async {");
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
                bool optionalArgsStarted = false;
                for (int i = 0; i < allArgsList.Count; i++) {
                    EArg eArg = allArgsList.ElementAt(i);
                    bool hasDefaultValue=!string.IsNullOrEmpty(eArg.defaultValue);
                    if ( hasDefaultValue && (!optionalArgsStarted)) {
                        argsStringBuilder.Append("{");
                        optionalArgsStarted = true;
                    }
                    string flutterTypeName = FlutterWatcher.GetFlutterType(eArg.typeName, out bool isAnotherEntityImport);
                    argsStringBuilder.Append(flutterTypeName + " " + eArg.name);
                    if(hasDefaultValue)argsStringBuilder.Append("=" + eArg.defaultValue);
                    if (i < (allArgsList.Count - 1)) argsStringBuilder.Append(", ");
                }
                if(optionalArgsStarted)argsStringBuilder.Append("}");
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
//                if (formArgList.Any()) {
//                    bodyTxt.Append("\t\tlet formData = new FormData();\n");
//                    foreach (EArg formArg in formArgList) {
//                        if (formArg.typeName == "any") bodyTxt.Append("\t\tformData.append('" + formArg.name + "', " + formArg.name + ");\n");
//                        else bodyTxt.Append("\t\tformData.append('" + formArg.name + "', " + formArg.name + ".toString());\n");
//                    }
//                    bodyTxt.Append("\t\tconst response = await this.authHttp." + operation + "(Defines.RestBaseURL + '/v" + eFunction.version.ToString() + "/" + eFile.csharpFileName + "/" + eFunction.functionName + "'");
//                    if (string.IsNullOrEmpty(AngularFunctionWriter.restInPeaceVersion)) bodyTxt.Append(", formData).timeout(timeoutSeconds*1000).toPromise().catch(Helper.handleError);\n");
//                    else bodyTxt.Append(", formData).toPromise().catch(Helper.handleError);\n");
//                    bodyTxt.Append("\t\treturn response.json();\n");
//                    bodyTxt.Append("\t}");
//                    return bodyTxt.ToString();
//                }
//                if (mandatoryList.Count > 0 || optionalList.Count > 0) {
//                    bodyTxt.Append("\t\tlet params: HttpParams = new HttpParams();\n");
//                    foreach (EArg mandatoryArg in mandatoryList) {
//                        bodyTxt.Append("\t\tparams= params.set('" + mandatoryArg.name + "', " + mandatoryArg.name + ".toString());\n");
//                    }
//                    foreach (EArg optionalArg in optionalList) {
//                        bodyTxt.Append("\t\tif(" + optionalArg.name + "!=null) params= params.set('" + optionalArg.name + "', " + optionalArg.name + ".toString());\n");
//                    }
//                }
                bodyTxt.Append("\n\t\tfinal response = await http." + operation + "(Defines.RestBaseURL + '/v" + eFunction.version.ToString() + "/" + eFile.csharpFileName + "/" + eFunction.functionName );  
                if (segmentsList.Count == 0) bodyTxt.Append("'");
                foreach (EArg eArgSegment in segmentsList) {
                    bodyTxt.Append("/${"+eArgSegment.name+"}");
                }
                if(segmentsList.Count>0)bodyTxt.Append("'");
                bodyTxt.Append(",\n\t\t\t\theaders: {\"Content-Type\": \"application/json\", \"Authorization\": \"Bearer \"+Defines.JwtToken},");
                if (bodyArg!=null) {
                    switch (bodyArg.typeName) {//cshartptype
                        case "string":
                            bodyTxt.Append("\n\t\t\t\tbody: json.encode("+bodyArg.name+"));\n\n");
                            break;
                        default:
                            bodyTxt.Append("\n\t\t\t\tbody: json.encode("+bodyArg.name+".toJson()));\n\n");
                            break;
                    }                                           
                }else bodyTxt.Append(");\n\n"); 
//                if (string.IsNullOrEmpty(AngularFunctionWriter.restInPeaceVersion)) {
//                    if (eFunction.functionType == FunctionType.PUT || eFunction.functionType == FunctionType.POST) {
//                        bodyTxt.Append(", { headers: Defines.Headers }");
//                    }
//                }
//                if (mandatoryList.Count > 0 || optionalList.Count > 0) {
//                    bodyTxt.Append(", {params: params}");
//                }
                
                bodyTxt.Append("\t\tif (response.statusCode != 200) print('("+eFile.frontendClassName+")"+eFunction.functionName+" error. Status code: ${response.statusCode}');\n");
                if (eFunction.frontendReturnTypeName!="void") {
                    bodyTxt.Append("\t\treturn ");
                    if (eFunction.frontendReturnTypeName=="int")  bodyTxt.Append(eFunction.frontendReturnTypeName+ ".parse(response.body);\n");
                    else if (eFunction.frontendReturnTypeName == "bool") bodyTxt.Append("BoolHelper.convertStringToBool(response.body);\n");
                    else if (eFunction.frontendReturnTypeName.Contains("List<")) {
                        string nameWithoutList = StringHelper.RemoveString(eFunction.returnTypeName, "List<");
                        nameWithoutList = StringHelper.RemoveString(nameWithoutList, ">");
                        bodyTxt.Append("(json.decode(response.body) as List).map((e) => new " + nameWithoutList + ".fromJson(e)).toList();\n");
                    }else if (eFunction.frontendReturnTypeName == "String") bodyTxt.Append("json.decode(response.body);\n");
                    else bodyTxt.Append(eFunction.frontendReturnTypeName+".fromJson(json.decode(response.body));\n");
                }                
                bodyTxt.Append("\t}");
                return bodyTxt.ToString();
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return "";
        }
        #endregion

        #region GetFunctionArgumentTypeNames
        static private List<string> GetFunctionArgumentTypeNames(List<EFunction> functionList) {
            List<string> argList = new List<string>();
            foreach (EFunction eFunction in functionList) {
                foreach (EArg eArg in eFunction.argsList) {
                    if (argList.Any(x => x == eArg.typeName)) continue; //para fazer a comparacao exata. nao remover
                    argList.Add(eArg.typeName);
                }
            }
            return argList;
        }
        #endregion

        #region GetAllReturnTypeNames
        static private List<string> GetAllReturnTypeNames(List<EFunction> functionList) {
            List<string> returnList = new List<string>();
            foreach (EFunction eFunction in functionList) {
                string nameWithoutList = StringHelper.RemoveString(eFunction.returnTypeName, "List<");
                nameWithoutList = StringHelper.RemoveString(nameWithoutList, ">");
                if (returnList.Contains(nameWithoutList)) continue;
                returnList.Add(nameWithoutList);
            }
            return returnList;
        }
        #endregion
    }
}