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

namespace RestInPeace.Codes.QMLTypescriptFrontend {
    static public class QmlTSFormsReader {                

        #region AnalyseFiles
        static public bool AnalyseFiles(string frontendPath) {
            try {
                Globals.eQMLFormFiles.Clear();
                IEnumerable<string> files = Directory.EnumerateFiles(frontendPath, "*.qml", SearchOption.AllDirectories);
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic|DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"Reading qml files from frontend: " + frontendPath );
                foreach (string qmlFile in files) {
                    if (!File.Exists(qmlFile)) continue;
                    FileInfo fileInfo = new FileInfo(qmlFile);
                    Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"\t"+fileInfo.Name);
                    EQmlFile eQMLFormFile = new EQmlFile {
                        completeFilePath = qmlFile,
                        name = fileInfo.Name.Replace(".qml", String.Empty).Replace(".ui.qml", String.Empty),
                        content = File.ReadAllText(qmlFile)
                    };
                    if (!StringHelper.ContainsAnyInString(eQMLFormFile.content, "TQuickItem","[RestInPeaceQmlComponent]"))continue;//nao precisa mais colocar RestInPeaceQmlUI
                    if (eQMLFormFile.content.Contains("TQuickItem")) eQMLFormFile.qmlFileType = QmlFileType.AppForm;
                    if (eQMLFormFile.content.Contains("[RestInPeaceQmlComponent]")) {
                        eQMLFormFile.qmlFileType = QmlFileType.AppComponent;
                    }
                    //if (eQMLFormFile.qmlFileType == QmlFileType.AppComponent) {
                        if (!GetAllQmlSignalsFromFile(eQMLFormFile)) return false;
                    //}
                    GetAllQmlElementsFromFile(ref eQMLFormFile);
                    Globals.qmlElements.AddRange(eQMLFormFile.elementsList);
                    if (!AnalyseFile(eQMLFormFile)) return false;
                    Globals.eQMLFormFiles.Add(eQMLFormFile);
                    //if (!SetTypescriptPropertyTypes()) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region GetAllQmlElementsFromFile - Usar sempre ids únicos!
        static private void GetAllQmlElementsFromFile(ref EQmlFile eQmlFile) {
            try {                
                var regex = new Regex("(\\w+)\\s*{\\s*\\n\\s*\\t*(id):\\s*(\\w+)", RegexOptions.Multiline);
                MatchCollection matches = regex.Matches(eQmlFile.content);
                foreach (Match match in matches) {
                    EQMLElement eqmlElement = new EQMLElement {
                        typeName = match.Groups[1].Value,
                        id = match.Groups[3].Value
                    };
                    eQmlFile.elementsList.Add(eqmlElement);
                }                
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        #endregion

        #region GetAllQmlSignalsFromFile
        static private bool GetAllQmlSignalsFromFile(EQmlFile eQmlFile) {
            var regex = new Regex("signal\\s+(\\w+)\\s*\\((.*)\\)", RegexOptions.Multiline);
            MatchCollection matches = regex.Matches(eQmlFile.content);
            foreach (Match match in matches) {
                string signalName = match.Groups[1].Value;
                string signalArguments = match.Groups[2].Value;
                if (signalArguments.Contains(",")) {
                    Logger.LogError($"Signal can not have multiple arg types yet. File {eQmlFile.name}");
                    return false;
                }
                Logger.LogInfoIfDebugLevel(DebugLevels.Functions|DebugLevels.All,"\t\tsignal "+signalName);
                //string[] args= signalArguments.Split(",");
                string signalArgName = signalArguments.Trim();
                if(string.IsNullOrEmpty(signalArgName))signalArgName = "void";
                else {
                    var argRegex = new Regex("(\\w+)\\s+(\\w+)", RegexOptions.Singleline);
                    var argMatch = argRegex.Match(signalArguments.Trim());
                    if (!argMatch.Success) {
                        Logger.LogError($"Invalid signal argument in  signal {signalName}. File {eQmlFile.name}");
                        return false;
                    }
                    signalArgName = Helper.ConvertQmlBasicTypeToTypescriptBasicType(argMatch.Groups[1].Value);                    
                }
                eQmlFile.signalList.Add(new EQmlSignal {
                    name = signalName,
                    argTypeName = signalArgName
                });
            }
            return true;
        }
        #endregion

        #region GetAllJsFunctionsFromFile
        static private void GetAllJsFunctionsFromFile(EQmlFile eqmlFile) {        
            var regex = new Regex("function\\s+(\\w+)\\s*\\((.*)\\)", RegexOptions.Multiline);
            MatchCollection matches = regex.Matches(eqmlFile.content);
            foreach (Match match in matches) {
                string functionBaseName = match.Groups[1].Value;
                string functionArguments = match.Groups[2].Value;
                string[] args= functionArguments.Split(",");
                string completeSignature = functionBaseName + "(";
                for (int i = 0; i < args.Length; i++) {
                    string argName = args.ElementAt(i);
                    if (string.IsNullOrEmpty(argName)) continue;
                    if (i == (args.Length - 1)) completeSignature += argName+": any";
                    else completeSignature += argName + ": any, ";
                }
                completeSignature += ")";
                eqmlFile.functionList.Add(new ETypescriptFunction {
                    signature = completeSignature,
                    returnType = "any"
                });
            }
        }
        #endregion

        #region AnalyseFile
        static private bool AnalyseFile(EQmlFile eQMLFormFile) {
            try {
                IEnumerable<string> linesList = eQMLFormFile.content.Split('\n');
                var regex = new Regex("(\\w+)\\s*{(.*)}", RegexOptions.Multiline | RegexOptions.Singleline, TimeSpan.FromMilliseconds(5000));
                Match putMatch = regex.Match(eQMLFormFile.content);
                if (!putMatch.Success) {
                    Logger.LogError("unable to read qml file " + eQMLFormFile.completeFilePath);
                    return false;
                }
                string rootElement = putMatch.Groups[1].Value;
                string rootElementContent = putMatch.Groups[2].Value;
                //let's extend the rootElement
                eQMLFormFile.extendsElementName = rootElement;
                eQMLFormFile.typescriptImports.Add(new EImport {className = rootElement ,path = "./"});
                GetAllJsFunctionsFromFile(eQMLFormFile);
                //let's get all properties ending with : something
                regex = new Regex("\\s*(property)\\s*(alias)?\\s*(\\w+)\\s*(\\w+)?\\s*(?=:\\s*(.*))?", RegexOptions.Multiline);
                MatchCollection matches = regex.Matches(rootElementContent);
                foreach (Match match in matches) {
                    bool isAlias = match.Groups[2].Value == "alias";
                    string propertyName = "";
                    string propertyType = "";
                    string propertyValue = ""; 
                    if (isAlias) {//ex:     property alias inverted: false
                        propertyName = match.Groups[3].Value;
                    } else {//ex:     property bool inverted: false
                        propertyType = match.Groups[3].Value;
                        propertyName = match.Groups[4].Value;
                    }
                    propertyValue = match.Groups[5].Value;//it might be something like txtField.maximumLength
                    if (!string.IsNullOrEmpty(propertyValue)) {
                        propertyValue = match.Groups[5].Value.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                        if (isAlias) {
                            var found=FindAliasInSameQmlFile(eQMLFormFile, propertyValue, ref propertyType);
                            if (!found) {
                                if (!FindPropertyReferenceTypeName(propertyValue, ref propertyType)) {
                                    Logger.LogError($"(QmlTSFormReader)unable to find qml property {propertyValue} as reference type. File:" + eQMLFormFile.completeFilePath + ".Line:" + match.Groups[0].Value);
                                    return false;
                                }
                            }
                        }                                     
                    }
                    EQMLFormProperty eqmlFormProperty = new EQMLFormProperty {name = propertyName, type = Helper.ConvertQmlBasicTypeToTypescriptBasicType(propertyType)};
                    eQMLFormFile.propertyList.Add(eqmlFormProperty);
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region FindPropertyReferenceTypeName
        //todo talvez remover essa funcao
        static private bool FindPropertyReferenceTypeName(string referenceName, ref string typeName) {
            typeName = "";
            try {
                //let's check if it's something like txtField.maximumLength, where txtField is a Element  defined in this file
                var regex = new Regex("(\\w+)\\.(\\w+)", RegexOptions.Singleline);
                Match match = regex.Match(referenceName);
                string subReference = "";
                if (match.Success) {
                    referenceName = match.Groups[1].Value;
                    subReference = match.Groups[2].Value;
                }                
                foreach (EQMLElement eqmlElement in Globals.qmlElements) {
                    if (eqmlElement.id == referenceName) {
                        Console.WriteLine("");
                        typeName = eqmlElement.typeName;
                        return true;
                    }
                }
                //List<EQmlFile> qtBasicComponents=QmlBasicComponents.GetQtBasicComponents();
                
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
        
        
        #region FindAliasInSameQmlFile
        static private bool FindAliasInSameQmlFile(EQmlFile eQmlFile, string referenceName, ref string typeName) {
            typeName = "";
            try {                
                //Console.WriteLine($"{eQmlFile.name} procurando o alias {referenceName}. ");
                //let's check if it's something like txtField.maximumLength, where txtField is a Element  defined in this file
                var regex = new Regex("(\\w+)\\.(\\w+)", RegexOptions.Singleline);
                Match match = regex.Match(referenceName);
                string subReference = "";
                if (match.Success) {
                    referenceName = match.Groups[1].Value;//ex: txtField
                    subReference = match.Groups[2].Value;//ex: maximumLength
                }      
                foreach (EQMLElement eqmlElement in eQmlFile.elementsList) {
                    if (eqmlElement.id == referenceName) {
                        Console.WriteLine("");
                        typeName = eqmlElement.typeName;
                        typeName = QmlBasicComponents.FindPropertyType(typeName, subReference);
                        if (typeName != null) return true;
                    }
                }
                
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
        
        
        
    }
}