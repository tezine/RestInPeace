#region Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DotnetBase.Codes;
using RestInPeace.Entities; 
#endregion

namespace RestInPeace.Codes
{
    class DotnetFunctionWriter{

        #region WriteFiles
        static public bool WriteFiles( ) {
            try {
                Logger.LogInfo("\n\nWriting files to " + Globals.dotnetServiceFolder + "========================================================");
                foreach (EFunctionFile eFile in Globals.backendControllerFiles) {
                    if (eFile.csharpFileName.Contains(".service")) {
                        eFile.frontendClassName = eFile.csharpFileName.Remove(0, 1);
                        eFile.frontendClassName = StringHelper.RemoveString(eFile.frontendClassName, ".service");
                        eFile.frontendClassName = eFile.frontendClassName + "Service";
                        eFile.frontendFileName = Globals.blazorServicesFolder + "/" + eFile.frontendClassName + ".cs";
                    } else {
                        eFile.frontendClassName = eFile.csharpFileName.Remove(0, 1) + "Service";
                        eFile.frontendFileName = Globals.blazorServicesFolder + "/" + eFile.csharpFileName.Remove(0, 1) + "Service.cs";
                    }
                    
                    string serviceFileName = Globals.dotnetServiceFolder + "/" + eFile.frontendClassName + ".cs";
                    Logger.LogInfo(eFile.frontendClassName + ".cs" + "============================");
                    if (!WriteFunctionsFromFile(eFile)) return false;
                    if (!WriteFile(new List<string>(), serviceFileName, eFile)) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region WriteFunctionsFromFile
        static private bool WriteFunctionsFromFile(EFunctionFile eFile) {
            try {
                foreach (EFunction eFunction in eFile.functionList) {
                    if (!WriteFunction(eFile, eFunction)) {
                        Logger.LogError("unable write function " + eFunction.functionName + " from file " + eFile.csharpFileName);
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
                string functionName = Char.ToUpperInvariant(eFunction.functionName[0]) + eFunction.functionName.Substring(1);
                string operation = Helper.GetRESTOperation(eFile, eFunction);
                if (string.IsNullOrEmpty(operation)) return false;
                if (!GetFunctionArgs(eFunction, ref argsStringBuilder, out allArgsList, out segmentsList, out mandatoryList, out optionalList, out formArgList, out bodyArg)) {
                    Logger.LogError("unable to get funcion parameters to write file " + eFile.frontendFileName + ": " + eFunction.functionName);
                    return false;
                }
                result.Append("\n\t\t#region " + eFunction.functionName+"\n");
                result.Append("\t\tstatic public async ");
                if (eFunction.returnTypeName != "void") result.Append("Task<" + eFunction.returnTypeName + "> ");
                else result.Append("void ");
                result.Append(functionName);
                result.Append("(");
                result.Append(argsStringBuilder.ToString());
                result.Append("){\n");
                string functionBody = GetFunctionBody(eFile, eFunction, operation, segmentsList, mandatoryList, optionalList, formArgList, bodyArg);
                result.Append(functionBody);
                result.Append("\t\t#endregion");
                eFunction.frontendFunctionContent = result.ToString();
                //Logger.LogInfo("Conteudo:"+eFunction.frontendFunctionContent);
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
                    argsStringBuilder.Append(eArg.typeName);
                    argsStringBuilder.Append(" ");
                    argsStringBuilder.Append(eArg.name);
                    if (!string.IsNullOrEmpty(eArg.defaultValue)) {
                        eArg.defaultValue= eArg.defaultValue.Replace("\'", "\"");
                        //if(eArg.defaultValue=="''")argsStringBuilder.Append("=\"\"");
                        argsStringBuilder.Append("=" + eArg.defaultValue);
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
                bodyTxt.Append("\t\t\ttry{\n");
                if (eFunction.returnTypeName != "void") bodyTxt.Append("\t\t\t\t" + eFunction.returnTypeName + " result = await (Defines.RestBaseURL+\"/v" + eFunction.version.ToString() + "/S" + eFile.frontendClassName + "/" + eFunction.functionName + "\")\n");
                else bodyTxt.Append("\t\t\t\t await (Defines.RestBaseURL+\"/v" + eFunction.version.ToString() + "/" + eFile.csharpFileName + "/" + eFunction.functionName + "\")\n");
                foreach (EArg eArgSegment in segmentsList) {
                    bodyTxt.Append("\t\t\t\t\t.AppendPathSegment("+eArgSegment.name+")\n");
                }
                foreach (EArg mandatoryArg in mandatoryList) {
                    bodyTxt.Append("\t\t\t\t\t.SetQueryParam(\"" + mandatoryArg.name + "\", " + mandatoryArg.name + ")\n");
                }
                foreach (EArg optionalArg in optionalList) {
                    bodyTxt.Append("\t\t\t\t\t.SetQueryParam(\"" + optionalArg.name + "\", " + optionalArg.name + ")\n");
                }
                bodyTxt.Append("\t\t\t\t\t.WithHeader(\"Authorization\",\"Bearer \"+Defines.Token)\n");
                if (eFunction.functionType == FunctionType.GET || eFunction.functionType==FunctionType.DELETE) {
                    if (eFunction.returnTypeName != "void") bodyTxt.Append("\t\t\t\t\t.GetJsonAsync<" + eFunction.returnTypeName + ">();\n");
                    else bodyTxt.Append("\t\t\t\t\t.GetAsync();\n");
                }else if (eFunction.functionType == FunctionType.POST) {
                    if(bodyArg!=null) bodyTxt.Append("\t\t\t\t\t.PostJsonAsync(JsonConvert.SerializeObject(" + bodyArg.name + "))\n");
                    else bodyTxt.Append("\t\t\t\t\t.PostAsync(null)\n");
                    if (eFunction.returnTypeName != "void") bodyTxt.Append("\t\t\t\t\t.ReceiveJson<"+eFunction.returnTypeName+">();\n");
                }else if (eFunction.functionType == FunctionType.PUT) {
                    if (bodyArg != null) bodyTxt.Append("\t\t\t\t\t.PutJsonAsync(JsonConvert.SerializeObject(" + bodyArg.name + "))\n");
                    else bodyTxt.Append("\t\t\t\t\t.PutAsync(null)\n");
                    if (eFunction.returnTypeName != "void") bodyTxt.Append("\t\t\t\t\t.ReceiveJson<"+eFunction.returnTypeName+">();\n");
                }
                if(eFunction.returnTypeName!="void") bodyTxt.Append("\t\t\t\treturn result;\n");
                bodyTxt.Append("\t\t\t}catch(Exception ex){\n");
                bodyTxt.Append("\t\t\t\tLogger.LogError(ex);\n");
                bodyTxt.Append("\t\t\t}\n");
                if (eFunction.returnTypeName != "void") {
                    if(eFunction.returnTypeName=="bool")bodyTxt.Append("\t\t\treturn false;\n");
                    else if(eFunction.returnTypeName=="int" || eFunction.returnTypeName=="Int64" || eFunction.returnTypeName=="Int32" || eFunction.returnTypeName=="double" || eFunction.returnTypeName=="float")bodyTxt.Append("\t\t\treturn -1;\n");
                    else if (eFunction.returnTypeName == "TimeSpan") bodyTxt.Append("\t\t\treturn new TimeSpan();\n");
                    else bodyTxt.Append("\t\t\treturn null;\n");
                }
                bodyTxt.Append("\t\t}\n");
                return bodyTxt.ToString();
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return "";
        }
        #endregion

        #region WriteFile
        static private bool WriteFile(List<string> entitiesList, string serviceFileName, EFunctionFile eFile) {
            try {
                string newFileContent = 
@"#region Imports
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using SharedLib.Entities;
using Mobile.Codes;
using Mobile.Entities.RestInPeace;
#endregion

namespace Mobile.Services{
    class "+eFile.frontendClassName+@"{
        #region Singleton
        private static "+eFile.frontendClassName+@" instance;
        private "+eFile.frontendClassName+@"() { }
        public static "+eFile.frontendClassName+@" Obj {
            get {
                if (instance == null) instance = new "+eFile.frontendClassName+@"();
                return instance;
            }
        } 
        #endregion
";
                
                foreach (EFunction eFunction in eFile.functionList) {
                    newFileContent+=eFunction.frontendFunctionContent;
                    newFileContent+="\n";
                }
                newFileContent += "\t}\n";
                newFileContent += "}\n";
                newFileContent += System.Environment.NewLine;
                string oldContent = "";
                if (File.Exists(serviceFileName)) oldContent = File.ReadAllText(serviceFileName);
                if (newFileContent != oldContent) File.WriteAllText(serviceFileName, newFileContent.ToString());
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
    }
}
