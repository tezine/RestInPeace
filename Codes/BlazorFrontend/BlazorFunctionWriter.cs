using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DotnetBase.Codes;
using RestInPeace.Codes.FlutterFrontend;
using RestInPeace.Entities;
using RestInPeace.Enums;

namespace RestInPeace.Codes.BlazorFrontend {
    public class BlazorFunctionWriter {
        #region WriteFiles
        static public bool WriteFiles() {
            try {
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic | DebugLevels.Files | DebugLevels.Functions | DebugLevels.All, "Writing blazor services to " + Globals.blazorServicesFolder);
                foreach (EFunctionFile eFile in Globals.backendControllerFiles) {
                    if (!eFile.csharpFileName.StartsWith("S")) continue;
                    if (eFile.csharpFileName.Contains(".service")) {
                        eFile.frontendClassName = eFile.csharpFileName.Remove(0, 1);
                        eFile.frontendClassName = StringHelper.RemoveString(eFile.frontendClassName, ".service");
                        eFile.frontendClassName = eFile.frontendClassName + "Service";
                        eFile.frontendFileName = Globals.blazorServicesFolder + "/" + eFile.frontendClassName + ".cs";
                    } else {
                        eFile.frontendClassName = eFile.csharpFileName.Remove(0, 1) + "Service";
                        eFile.frontendFileName = Globals.blazorServicesFolder + "/" + eFile.csharpFileName.Remove(0, 1) + "Service.cs";
                    }

                    Logger.LogInfoIfDebugLevel(DebugLevels.Files | DebugLevels.Functions | DebugLevels.All, "\t" + eFile.frontendClassName);
                    if (!WriteBlazorFile(eFile)) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region WriteBlazorFile
        static private bool WriteBlazorFile(EFunctionFile eFile) {
            try {
                StringBuilder newFileContent = new StringBuilder();
                newFileContent.Append("#region Imports\n");
                //newFileContent.Append("//author Bruno Tezine\n");
                newFileContent.Append("using Newtonsoft.Json;\n");
                newFileContent.Append("using SharedLib.Entities;\n");
                newFileContent.Append("using " + Globals.blazorNamespaceName + ".Codes;\n");
                newFileContent.Append("using " + Globals.blazorNamespaceName + ".Entities.RestInPeace;\n");
                newFileContent.Append("using System;\n");
                newFileContent.Append("using System.Text;\n");
                newFileContent.Append("using System.Collections.Generic;\n");
                newFileContent.Append("using System.Linq;\n");
                newFileContent.Append("using System.Net.Http;\n");
                newFileContent.Append("using System.Threading.Tasks;\n");
                newFileContent.Append("#endregion\n");
                //imports above            
                newFileContent.Append("\nnamespace Frontend.Services {\n");
                newFileContent.Append("\tpublic class " + eFile.frontendClassName + " {\n");
                newFileContent.Append("\t\tHttpClient httpClient;\n\n");
                newFileContent.Append("\t\tpublic " + eFile.frontendClassName + "(HttpClient client) {\n");
                newFileContent.Append("\t\t\thttpClient = client;\n");
                newFileContent.Append("\t\t}\n\n");
                foreach (EFunction eFunction in eFile.functionList) {
                    if (!WriteFunction(eFile, eFunction)) {
                        Logger.LogError("unable get function " + eFunction.functionName + " from file " + eFile.csharpFileName);
                        return false;
                    }
                    newFileContent.Append(eFunction.frontendFunctionContent);
                    newFileContent.Append("\n\n");
                }
                newFileContent.Append("\t}\n");
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
                eFunction.frontendReturnTypeName = eFunction.returnTypeName;
                string operation = Helper.GetRESTOperation(eFile, eFunction);
                if (string.IsNullOrEmpty(operation)) return false;
                if (!GetFunctionArgs(eFunction, ref argsStringBuilder, out allArgsList, out segmentsList, out mandatoryList, out optionalList, out formArgList, out bodyArg)) {
                    Logger.LogError("unable to get funcion parameters to write file " + eFile.frontendFileName + ": " + eFunction.functionName);
                    return false;
                }
                result.Append("\t\tpublic async Task<" + eFunction.frontendReturnTypeName + "> " + eFunction.functionName + "(" + argsStringBuilder.ToString() + ")  {");
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
                for (int i = 0; i < allArgsList.Count; i++) {
                    EArg eArg = allArgsList.ElementAt(i);
                    bool hasDefaultValue = !string.IsNullOrEmpty(eArg.defaultValue);
                    argsStringBuilder.Append(eArg.typeName + " " + eArg.name);
                    if (hasDefaultValue) {
                        argsStringBuilder.Append("=" + eArg.defaultValue.Replace("'", "\""));
                    }
                    if (i < (allArgsList.Count - 1)) argsStringBuilder.Append(", ");
                }
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

                string operationName = "";
                switch (eFunction.functionType) {
                    case FunctionType.GET:
                        operationName = "HttpMethod.Get";
                        break;
                    case FunctionType.PUT:
                        operationName = "HttpMethod.Put";
                        break;
                    case FunctionType.POST:
                        operationName = "HttpMethod.Post";
                        break;
                    case FunctionType.DELETE:
                        operationName = "HttpMethod.Delete";
                        break;
                }
                bodyTxt.Append("\n\t\t\tGlobals.loading = true;");
                bodyTxt.Append("\n\t\t\tHttpRequestMessage req = new HttpRequestMessage(" + operationName + ", $\"api/v" + eFunction.version + "/S" + eFile.frontendClassName + "/" + eFunction.functionName);
                if (segmentsList.Count == 0&& optionalList.Count==0) bodyTxt.Append("\");");
                foreach (EArg eArgSegment in segmentsList) {
                    bodyTxt.Append("/{" + eArgSegment.name + "}");
                }
                if (optionalList.Any()) bodyTxt.Append("?");
                for(int i=0;i<optionalList.Count;i++) {
                    var eArgOptional = optionalList.ElementAt(i);
                    bodyTxt.Append(eArgOptional.name+"={"+eArgOptional.name+"}");
                    if (i < (optionalList.Count - 1)) bodyTxt.Append("&");
                }
                if (segmentsList.Any() || optionalList.Any()) bodyTxt.Append("\");");
                bodyTxt.Append("\n");
                bodyTxt.Append("\t\t\treq.Headers.Add(\"Authorization\", $\"bearer {Globals.jwtToken}\");\n");
                if (bodyArg != null) bodyTxt.Append("\t\t\treq.Content = new StringContent(JsonConvert.SerializeObject(" + bodyArg.name + "), Encoding.UTF8, \"application/json\");\n");
                bodyTxt.Append("\t\t\tvar response = await httpClient.SendAsync(req);\n");
                bodyTxt.Append("\t\t\tGlobals.loading = false;\n");
                bodyTxt.Append("\t\t\tif (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)throw new UnauthorizedAccessException();\n");
                bodyTxt.Append("\t\t\tresponse.EnsureSuccessStatusCode();\n");
                if (eFunction.frontendReturnTypeName != "void") {
                    bodyTxt.Append("\t\t\tstring responseBody = await response.Content.ReadAsStringAsync();\n");
                    switch (eFunction.frontendReturnTypeName) {
                        case "string":
                            bodyTxt.Append("\t\t\treturn responseBody;\n"); 
                            break;
                        case "Int64":
                            bodyTxt.Append("\t\t\treturn Int64.Parse(responseBody);\n");
                            break;
                        case "bool":
                            bodyTxt.Append("\t\t\treturn bool.Parse(responseBody);\n");
                            break;
                        case "Decimal":
                        case "decimal":
                            bodyTxt.Append("\t\t\treturn decimal.Parse(responseBody);\n");
                            break;
                        default:
                            bodyTxt.Append("\t\t\t" + eFunction.frontendReturnTypeName + " result = JsonConvert.DeserializeObject<" + eFunction.frontendReturnTypeName + ">(responseBody);\n");
                            bodyTxt.Append("\t\t\treturn result;\n");                            
                            break;
                    }
                }
                bodyTxt.Append("\t\t}");

//                
//                
//                
//                
//                
//                
//                bodyTxt.Append("\n\t\tfinal response = await http." + operation + "(Defines.RestBaseURL + '/v" + eFunction.version.ToString() + "/" + eFile.csharpFileName + "/" + eFunction.functionName);
//                if (segmentsList.Count == 0) bodyTxt.Append("'");
//                foreach (EArg eArgSegment in segmentsList) {
//                    bodyTxt.Append("/${" + eArgSegment.name + "}");
//                }
//                if (segmentsList.Count > 0) bodyTxt.Append("'");
//                bodyTxt.Append(",\n\t\t\t\theaders: {\"Content-Type\": \"application/json\", \"Authorization\": \"Bearer \"+Defines.JwtToken},");
//                if (bodyArg != null) {
//                    switch (bodyArg.typeName) {
//                        //cshartptype
//                        case "string":
//                            bodyTxt.Append("\n\t\t\t\tbody: json.encode(" + bodyArg.name + "));\n\n");
//                            break;
//                        default:
//                            bodyTxt.Append("\n\t\t\t\tbody: json.encode(" + bodyArg.name + ".toJson()));\n\n");
//                            break;
//                    }
//                } else bodyTxt.Append(");\n\n");
//
//                bodyTxt.Append("\t\tif (response.statusCode != 200) print('(" + eFile.frontendClassName + ")" + eFunction.functionName + " error. Status code: ${response.statusCode}');\n");
//                if (eFunction.frontendReturnTypeName != "void") {
//                    bodyTxt.Append("\t\treturn ");
//                    if (eFunction.frontendReturnTypeName == "int") bodyTxt.Append(eFunction.frontendReturnTypeName + ".parse(response.body);\n");
//                    else if (eFunction.frontendReturnTypeName == "bool") bodyTxt.Append("BoolHelper.convertStringToBool(response.body);\n");
//                    else if (eFunction.frontendReturnTypeName.Contains("List<")) {
//                        string nameWithoutList = StringHelper.RemoveString(eFunction.returnTypeName, "List<");
//                        nameWithoutList = StringHelper.RemoveString(nameWithoutList, ">");
//                        bodyTxt.Append("(json.decode(response.body) as List).map((e) => new " + nameWithoutList + ".fromJson(e)).toList();\n");
//                    } else if (eFunction.frontendReturnTypeName == "String") bodyTxt.Append("json.decode(response.body);\n");
//                    else bodyTxt.Append(eFunction.frontendReturnTypeName + ".fromJson(json.decode(response.body));\n");
//                }

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