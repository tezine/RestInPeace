#region Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DotnetBase.Codes;
using RestInPeace.Entities;
using RestInPeace.Enums;
#endregion

namespace RestInPeace.Codes {
    public class QmlMJSFunctionWriter {
        
        #region WriteFiles
        static public bool WriteFiles() {  
            try {
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic|DebugLevels.Files|DebugLevels.Functions|DebugLevels.All, "Writing MJS scripts to " + Globals.frontendRestInPeaceFolder );
                //string javascriptFileName = "";
                //string javascriptClassName = "";
                foreach (EFunctionFile eFile in Globals.backendControllerFiles) {
                    //if (eFile.csharpFileName.StartsWith("S")) javascriptFileName = eFile.csharpFileName.Remove(0, 1);
                    if (eFile.csharpFileName.Contains(".service")) {
                        eFile.frontendClassName = eFile.csharpFileName.Remove(0, 1);
                        eFile.frontendClassName = StringHelper.RemoveString(eFile.frontendClassName, ".service");
                        eFile.frontendClassName = "J"+eFile.frontendClassName + "Service";
                        eFile.frontendFileName = Globals.frontendRestInPeaceFolder + "/" + eFile.frontendClassName + ".mjs";
                    } else {
                        eFile.frontendClassName = "J"+eFile.csharpFileName.Remove(0, 1) + "Service";
                        eFile.frontendFileName = Globals.frontendRestInPeaceFolder + "/" + eFile.csharpFileName.Remove(0, 1) + "Service.mjs";
                    }
                    
                    
                    //javascriptClassName = "J" + javascriptFileName;
                    //javascriptFileName = "J" + javascriptFileName + ".mjs";
                    //string completeJavascriptFilePath = Globals.frontendRestInPeaceFolder + "/" + javascriptFileName;
                    Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.Functions|DebugLevels.All, eFile.frontendFileName );
                    if (!WriteFunctions(eFile)) return false;
                    if (!WriteFile(eFile.frontendClassName, eFile.frontendFileName, eFile)) return false;
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
                Logger.LogInfoIfDebugLevel(DebugLevels.Functions|DebugLevels.All, "\tFunction " + eFunction.functionName);
                string functionName = Char.ToLowerInvariant(eFunction.functionName[0]) + eFunction.functionName.Substring(1);
                string operation = Helper.GetRESTOperation(eFile, eFunction);
                if (string.IsNullOrEmpty(operation)) return false;
                if (!GetFunctionArgs(eFunction, ref argsStringBuilder, out allArgsList, out segmentsList, out mandatoryList, out optionalList, out formArgList, out bodyArg)) {
                    Logger.LogError("unable to get funcion parameters to write file " + eFile.frontendFileName + ": " + eFunction.functionName);
                    return false;
                }
                result.Append("\tstatic "+functionName);
                string args = argsStringBuilder.ToString();
                if (args.Length > 0) result.Append("(base, progressBar, ");
                else result.Append("(base, progressBar"); //nao tem argumento
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
                bodyTxt.Append("\t\tif(progressBar)progressBar.visible=true;\n");
                bodyTxt.Append("\t\tlet rest=base.createRestObj();\n");
                bodyTxt.Append("\t\trest.clear();\n");
                bodyTxt.Append("\t\trest.setBaseUrl(base.baseURL);\n");
                var cSharpClassName = "S"+eFile.frontendClassName.Remove(0, 1);
                bodyTxt.Append("\t\trest.setRoute('/v" + eFunction.version.ToString() + "/" + cSharpClassName + "/" + eFunction.functionName + "');\n");
                bodyTxt.Append("\t\trest.setToken(base.token);\n");
                foreach (EArg eArgSegment in segmentsList) {
                    bodyTxt.Append("\t\trest.appendUrlSegment(" + eArgSegment.name + ");\n");
                }
                foreach (EArg eArgParameter in parametersList) {
                    bodyTxt.Append("\t\trest.appendParameter('" + eArgParameter.name + "'," + eArgParameter.name + ");\n");
                }
                if (bodyArg != null) {
                    bodyTxt.Append("\t\trest.setBody(JSON.stringify(" + bodyArg.name + "));\n");
                }
                bodyTxt.Append("\t\treturn rest." + Helper.GetRESTOperationForQML(eFile, eFunction,true) + "(progressBar);\n");
                bodyTxt.Append("\t}");
                return bodyTxt.ToString();
            } catch (Exception e) {
                Logger.LogError(e);
            }

            return "";
        }
        #endregion

        #region WriteFile
        static private bool WriteFile(string mJSClassName, string completeJavascriptFilename, EFunctionFile eFile) {
            try {
                StringBuilder newFileContent = new StringBuilder();
                newFileContent.Append("//author Bruno Tezine\n\n");
                newFileContent.Append("export class "+mJSClassName+" {\n");
                newFileContent.Append("\n");
                foreach (EFunction eFunction in eFile.functionList) {
                    newFileContent.Append(eFunction.qmlFunctionContent);
                    newFileContent.Append("\n\n");
                }
                newFileContent.Append("}\n");
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