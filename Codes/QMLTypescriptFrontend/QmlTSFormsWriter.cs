using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RestInPeace.Entities;
using RestInPeace.Enums;

namespace RestInPeace.Codes.QMLTypescriptFrontend {
    static public class QmlTSFormsWriter {
        #region WriteFiles
        static public bool WriteFiles() {
            try {
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic| DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"Writing Typescript forms to " + Globals.frontendRestInPeaceFolder );
                foreach (EQmlFile eqmlFormFile in Globals.eQMLFormFiles) {
                    if (eqmlFormFile.qmlFileType==QmlFileType.AppForm) {
                        if (!WriteTypescriptFormFile(eqmlFormFile)) return false;
                    } else {
                        if (!WriteTypescriptComponentFile(eqmlFormFile)) return false;
                    }
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region WriteTypescriptFormFile
        static private bool WriteTypescriptFormFile(EQmlFile eQmlFormFile) {
            try {
                string typescriptFormFileName = eQmlFormFile.name + "Form.d.ts";
                string completeFilePath = Globals.frontendRestInPeaceFolder + "/" + typescriptFormFileName;
                Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"\t" + typescriptFormFileName);
                StringBuilder fileContent = new StringBuilder();
                fileContent.Append("\n");
                DTSWriter.WriteTypescriptImports(eQmlFormFile, ref fileContent);
                fileContent.Append("\n");
                fileContent.Append("export declare class " + eQmlFormFile.name + "Form {\n");
                foreach (EQMLFormProperty eQmlFormProperty in eQmlFormFile.propertyList) {
                    fileContent.Append("\t" + eQmlFormProperty.name + ": " + eQmlFormProperty.type + ";\n");
                }
                foreach (EQmlSignal eQmlSignal in eQmlFormFile.signalList) {
                    fileContent.Append("\t" + eQmlSignal.name + ": MySimpleEvent<" + eQmlSignal.argTypeName + ">;\n");
                }
                foreach (ETypescriptFunction eTypescriptFunction in eQmlFormFile.functionList) {
                    fileContent.Append("\t" + eTypescriptFunction.signature + ": " + eTypescriptFunction.returnType+";\n");
                }
                fileContent.Append("}");
                //Logger.LogInfo(newFileContent.ToString());
                string oldContent = "";
                if (File.Exists(completeFilePath)) oldContent = File.ReadAllText(completeFilePath);
                if (fileContent.ToString() != oldContent) File.WriteAllText(completeFilePath, fileContent.ToString());
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
        
        #region WriteTypescriptComponentFile //todo passar a usar o DTSWriter
        static private bool WriteTypescriptComponentFile(EQmlFile eQmlFormFile) {
            try {
                string tsComponentFileName = eQmlFormFile.name + ".d.ts";
                string completeFilePath = Globals.frontendRestInPeaceFolder + "/" + tsComponentFileName;
                Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"\t" + tsComponentFileName);
                StringBuilder fileContent = new StringBuilder();
                fileContent.Append("\n");
                DTSWriter.WriteTypescriptImports(eQmlFormFile, ref fileContent);
                fileContent.Append("\n");
                if(!string.IsNullOrEmpty(eQmlFormFile.extendsElementName))fileContent.Append("export declare class " + eQmlFormFile.name + " extends "+eQmlFormFile.extendsElementName+" {\n");
                else fileContent.Append("export declare class " + eQmlFormFile.name + " {\n");
                foreach (EQMLFormProperty eQmlFormProperty in eQmlFormFile.propertyList) {
                    fileContent.Append("\t" + eQmlFormProperty.name + ": " + eQmlFormProperty.type + ";\n");
                }
                fileContent.Append("\n");
                foreach (EQmlSignal eQmlSignal in eQmlFormFile.signalList) {
                    fileContent.Append("\t" + eQmlSignal.name + ": MySimpleEvent<" + eQmlSignal.argTypeName + ">;\n");
                }
                foreach (ETypescriptFunction eTypescriptFunction in eQmlFormFile.functionList) {
                    fileContent.Append("\t" + eTypescriptFunction.signature + ": " + eTypescriptFunction.returnType+";\n");
                }
                fileContent.Append("}");
                //Logger.LogInfo(newFileContent.ToString());
                string oldContent = "";
                if (File.Exists(completeFilePath)) oldContent = File.ReadAllText(completeFilePath);
                if (fileContent.ToString() != oldContent) File.WriteAllText(completeFilePath, fileContent.ToString());
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
        


    }
}