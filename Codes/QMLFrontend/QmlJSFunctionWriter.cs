#region Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RestInPeace.Entities;
#endregion

namespace RestInPeace.Codes {
    public static class QmlJSFunctionWriter {
        
        static private bool isForTypescriptApp=false;
        
        #region WriteFiles
        static public bool WriteFiles(string frontendScriptDirectory, bool forTypescriptApp=false) {  
            try {
                isForTypescriptApp = forTypescriptApp;
                Logger.LogInfo("\n\nWriting Javascript scripts to " + frontendScriptDirectory + "========================================================");
                Directory.CreateDirectory(frontendScriptDirectory);
                string javascriptFileName = "";
                foreach (EFunctionFile eFile in Globals.backendControllerFiles) {
                    if (eFile.csharpFileName.StartsWith("S")) javascriptFileName = eFile.csharpFileName.Remove(0, 1);
                    javascriptFileName = "J" + javascriptFileName + ".js";
                    string completeJavascriptFilePath = frontendScriptDirectory + "/" + javascriptFileName;
                    Logger.LogInfo(javascriptFileName + "============================");
                    if (!WriteFunctions(eFile)) return false;
                    if (!WriteFile(completeJavascriptFilePath, eFile)) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }

            return false;
        }
        #endregion

        #region WriteFunctions
        static private bool WriteFunctions(EFunctionFile eFile) {
            try {
                //Logger.LogInfo("Writing functions of file " + eFile.fileName);
                foreach (EFunction eFunction in eFile.functionList) {
                    if (!WriteFunction(eFile, eFunction)) {
                        Logger.LogError("unable get qml function " + eFunction.functionName + " from file " + eFile.csharpFileName);
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

                result.Append("function ");
                result.Append(functionName);
                string args = argsStringBuilder.ToString();
                if (!isForTypescriptApp) {
                    if (args.Length > 0) result.Append("(progressBar, callback, ");
                    else result.Append("(progressBar, callback"); //nao tem argumento
                } else {
                    if (args.Length > 0) result.Append("(progressBar, ");
                    else result.Append("(progressBar"); //nao tem argumento
                }
                result.Append(args);
                result.Append(") {\n");
                string functionBody = GetFunctionBody(eFile, eFunction, operation, segmentsList, mandatoryList, optionalList, formArgList, bodyArg);
                result.Append(functionBody);
                eFunction.qmlFunctionContent = result.ToString();
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
                    argsStringBuilder.Append(eArg.name);
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
                List<EArg> parametersList = mandatoryList;
                parametersList.AddRange(optionalList);
                StringBuilder bodyTxt = new StringBuilder();
                bodyTxt.Append("\tif(progressBar)progressBar.visible=true;\n");
                bodyTxt.Append("\tvar rest=base.createRestObj();\n");
                bodyTxt.Append("\trest.clear();\n");
                bodyTxt.Append("\trest.setBaseUrl(dstore.baseURL);\n");
                bodyTxt.Append("\trest.setRoute('/v" + eFunction.version.ToString() + "/" + eFile.csharpFileName + "/" + eFunction.functionName + "');\n");
                bodyTxt.Append("\trest.setToken(dstore.token);\n");
                foreach (EArg eArgSegment in segmentsList) {
                    bodyTxt.Append("\trest.appendUrlSegment(" + eArgSegment.name + ");\n");
                }
                foreach (EArg eArgParameter in parametersList) {
                    bodyTxt.Append("\trest.appendParameter('" + eArgParameter.name + "'," + eArgParameter.name + ");\n");
                }
                if (bodyArg != null) {
                    bodyTxt.Append("\trest.setBody(JSON.stringify(" + bodyArg.name + "));\n");
                }
                if (!isForTypescriptApp) {
                    bodyTxt.Append("\trest." + Helper.GetRESTOperationForQML(eFile, eFunction, isForTypescriptApp) + "(function(response){\n");
                    bodyTxt.Append("\t\tif(progressBar)progressBar.visible=false;\n");
                    bodyTxt.Append("\t\tvar result=JSON.parse(response);\n");
                    bodyTxt.Append("\t\tcallback(result);\n");
                    bodyTxt.Append("\t});\n");
                } else {//for typescript app
                    bodyTxt.Append("\treturn rest." + Helper.GetRESTOperationForQML(eFile, eFunction, isForTypescriptApp) + "(progressBar);\n");
                }
                bodyTxt.Append("}");
                return bodyTxt.ToString();
            } catch (Exception e) {
                Logger.LogError(e);
            }

            return "";
        }
        #endregion

        #region WriteFile
        static private bool WriteFile(string completeJavascriptFilename, EFunctionFile eFile) {
            try {
                StringBuilder newFileContent = new StringBuilder();
                newFileContent.Append("//author Bruno Tezine\n");
                newFileContent.Append("//.pragma library nao suporta variavel de contexto\n");
                newFileContent.Append(".import com.tezine.basesingletons 1.0 as BaseSingletons\n");
                newFileContent.Append(".import com.tezine.base 1.0 as Base\n");
                newFileContent.Append(".import 'qrc:/Scripts/JFunctions.js' as JFunctions\n");
                newFileContent.Append("\n");
                foreach (EFunction eFunction in eFile.functionList) {
                    newFileContent.Append(eFunction.qmlFunctionContent);
                    newFileContent.Append("\n\n");
                }
                string oldContent = "";
                if (File.Exists(completeJavascriptFilename)) oldContent = File.ReadAllText(completeJavascriptFilename);
                if (newFileContent.ToString() != oldContent) File.WriteAllText(completeJavascriptFilename, newFileContent.ToString());
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }

            return false;
        }
        #endregion
    }
}