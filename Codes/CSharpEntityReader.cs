#region Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using RestInPeace.Entities;
using RestInPeace.Enums;
#endregion

namespace RestInPeace.Codes {
    public class CSharpEntityReader {

        static public List<EEntityFile> fileList = new List<EEntityFile>();

        #region AnalyseFiles
        static public bool AnalyseFiles(List<string> entitiesRootFolders) {
            try {
                fileList.Clear();
                foreach (string entitiesFolder in entitiesRootFolders) {
                    Logger.LogInfoIfDebugLevel(DebugLevels.Basic|DebugLevels.Files|DebugLevels.Functions|DebugLevels.All, "Reading entity files from backend: " + entitiesFolder);
                    if (!Directory.Exists(entitiesFolder)) {
                        Logger.LogError($"Directory {entitiesFolder} does not exist");
                        return false;
                    }
                    IEnumerable<string> files = Directory.EnumerateFiles(entitiesFolder, "*.cs", SearchOption.AllDirectories);
                    foreach (string file in files) {
                        if (file.Contains("\\obj\\")) continue;
                        if (!AnalyseFile(file)) return false;
                    }
                    if (!SetTypescriptPropertyTypes()) return false;
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
                string fullContent = File.ReadAllText(path);
                IEnumerable<string> linesList = fullContent.Split('\n');
                EEntityFile eEntityFile = new EEntityFile();
                eEntityFile.className = fileInfo.Name.Replace(".cs", String.Empty);
                eEntityFile.propertyList = new List<EEntityProperty>();
                if (fullContent.Contains("[RestInPeaceEntity]") || fullContent.Contains("[RestInPeaceEntityImport]")) {
                    Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.All, "\t"+eEntityFile.className);
                }
                for (int l = 0; l < linesList.Count(); l++) {
                    string lineContent = linesList.ElementAt(l);
                    if(lineContent.Contains("[RestInPeaceEntityImport]")) {//significa que nao vamos compiar o conteudo do entity. Apenas vamos adicionar o import no Angular service onde ele é usado
                        fileList.Add(eEntityFile);
                        eEntityFile.onlyImport = true;
                        return true;
                    }
                    if (!lineContent.Contains("[RestInPeaceEntity]")) continue;
                    if (!AnalyseEntity(ref eEntityFile, fullContent)) return false;
                    if (!AnalyseListsInEntity(ref eEntityFile, fullContent)) return false;
                    //por enquanto suporta só um entity por arquivo
                    break;
                }
                if (eEntityFile.propertyList.Count > 0) fileList.Add(eEntityFile);
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region AnalyseEntity
        static private bool AnalyseEntity(ref EEntityFile eEntityFile, string fileContent) {
            try {
                var regex = new Regex("public\\s+(\\w+\\?*)\\s+(\\w+)\\s+{\\s*get", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
                Match match = regex.Match(fileContent);
                if (!match.Success) return true;
                for (Int32 i = 0; match.Success; i++, match = match.NextMatch()) {
                    EEntityProperty eEntityProperty = new EEntityProperty();
                    eEntityProperty.csharpTypeName = match.Groups[1].Value;
                    eEntityProperty.name = match.Groups[2].Value;
                    eEntityFile.propertyList.Add(eEntityProperty);
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region AnalyseListsInEntity
        static private bool AnalyseListsInEntity(ref EEntityFile eEntityFile, string fileContent) {
            try {
                var regex = new Regex("public\\s+List<(\\w+)>\\s+(\\w+)\\s+{\\s*get", RegexOptions.None, TimeSpan.FromMilliseconds(1000));
                Match match = regex.Match(fileContent);
                if (!match.Success) return true;//no list found. Ex: public List<EPieReportDataset> datasets { get; set; }
                for (Int32 i = 0; match.Success; i++, match = match.NextMatch()) {
                    EEntityProperty eEntityProperty = new EEntityProperty();
                    eEntityProperty.isList = true;
                    eEntityProperty.csharpTypeName = match.Groups[1].Value;
                    eEntityProperty.name = match.Groups[2].Value;
                    eEntityFile.propertyList.Add(eEntityProperty);
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region SetTypescriptPropertyTypes
        static private bool SetTypescriptPropertyTypes() {
            try {
                foreach (EEntityFile eEntityFile in fileList) {
                    for (int j = 0; j < eEntityFile.propertyList.Count; j++) {
                        EEntityProperty eEntityProperty = eEntityFile.propertyList[j];
                        if (!GetTypescriptPropertyTypeName(ref eEntityProperty)) {
                            Logger.LogError("unable to analyse property in file " + eEntityFile.className + " named " + eEntityProperty.csharpTypeName + " " + eEntityProperty.name);
                            return false;
                        }
                    }
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region GetTypescriptPropertyTypeName
        static private bool GetTypescriptPropertyTypeName(ref EEntityProperty eEntityProperty) {
            try {
                //Logger.LogInfo(eEntityProperty.csharpTypeName);
                eEntityProperty.typescriptPropName = eEntityProperty.name;
                if (eEntityProperty.csharpTypeName.Contains("Int32") ||
                    eEntityProperty.csharpTypeName.Contains("Int64") ||
                    eEntityProperty.csharpTypeName.Contains("int") ||
                    eEntityProperty.csharpTypeName.Contains("Decimal") ||
                    eEntityProperty.csharpTypeName.Contains("decimal") ||
                    eEntityProperty.csharpTypeName.Contains("float") ||
                    eEntityProperty.csharpTypeName.Contains("double")) eEntityProperty.typescriptTypeName = "number";
                else if (eEntityProperty.csharpTypeName.Contains("string")) eEntityProperty.typescriptTypeName = "string";
                else if (eEntityProperty.csharpTypeName.Contains("DateTime")) eEntityProperty.typescriptTypeName = "string";
                else if (eEntityProperty.csharpTypeName.Contains("TimeSpan")) eEntityProperty.typescriptTypeName = "string";
                else if (eEntityProperty.csharpTypeName.Contains("bool")) eEntityProperty.typescriptTypeName = "boolean";
                else eEntityProperty.typescriptTypeName = eEntityProperty.csharpTypeName;
                if (!string.IsNullOrEmpty(eEntityProperty.typescriptTypeName) && eEntityProperty.csharpTypeName.Contains('?')) {
                    eEntityProperty.typescriptPropName += "?";
                    return true;
                }
                /*foreach (EEntityFile eEntityFile in fileList) {
                    foreach(EEntityProperty e in eEntityFile.propertyList){

                    }
                }*/
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
        
        static public bool IsEntity(string typeName) {
            foreach (EEntityFile eEntityFile in fileList) {
                if (eEntityFile.className == typeName) return true;
            }
            return false;
        }
    }
}