#region Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DotnetBase.Codes;
using RestInPeace.Entities;
using RestInPeace.Enums;
#endregion

namespace RestInPeace.Codes.QMLTypescriptFrontend {
    public class DTSWriter {
        
        #region GetResumedTypeName
        static public string GetResumedTypeName(string completeTypeName) {
            //ex: retorna MySimpleEvent a partir de MySimpleEvent<void>
            int start=completeTypeName.IndexOf("<");
            if (start < 0) return completeTypeName;
            int end = completeTypeName.IndexOf(">");
            var result = completeTypeName.Substring(0, start );
            return result;
        }
        #endregion

        #region WriteTypescriptEventHandler
        static public bool WriteTypescriptEventHandler() {
            try {
                string mySimpleEvent = "import {Subscribable} from './MySubscribable'\n"+
                                       "import {MySimpleEventHandler} from './MySimpleEventHandler'\n\n"+
                                       "export interface MySimpleEvent<TArgs> extends Subscribable<MySimpleEventHandler<TArgs>> {\n"+
                                       "}";

                string mySimpleEventHandler = @"export declare interface MySimpleEventHandler<TArgs> {

}";

                string mySubscribable = @"export declare interface Subscribable<THandlerType>{

    connect(fn: THandlerType): void;
    connectWithThis(currentObj:any, fn:THandlerType):void;
}";
                
                File.WriteAllText(Globals.frontendRestInPeaceFolder+"/MySimpleEvent.d.ts",mySimpleEvent);
                File.WriteAllText(Globals.frontendRestInPeaceFolder+"/MySimpleEventHandler.d.ts",mySimpleEventHandler);
                File.WriteAllText(Globals.frontendRestInPeaceFolder+"/MySubscribable.d.ts",mySubscribable);
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion        
        
        #region WriteTypescriptImports
        static public  void WriteTypescriptImports(EQmlFile eQmlFormFile, ref StringBuilder fileContent) {
            try {
                List<string> importFileTypes=new List<string>();
                if (eQmlFormFile.signalList.Count > 0) {    
                    fileContent.Append("import {MySimpleEvent} from './MySimpleEvent';\n");
                    importFileTypes.Add("MySimpleEvent");
                }
                foreach (EImport eImport in eQmlFormFile.typescriptImports) {
                    if (!importFileTypes.Contains(eImport.className)) {
                        string resumedClassName = GetResumedTypeName(eImport.className);
                        if (resumedClassName == "TQuickItem") continue;
                        fileContent.Append("import {" + resumedClassName + "} from '" + eImport.path + resumedClassName+ "';\n");
                        importFileTypes.Add(resumedClassName);
                    }
                }
                
                //todo talvez deixar de usar o formato abaixo e usar apenas via typescriptImports acima
                foreach (EQMLFormProperty eqmlFormProperty in eQmlFormFile.propertyList) {
                    if (Helper.IsBasicTypescriptType(eqmlFormProperty.type)) continue;
                    if (!importFileTypes.Contains(eqmlFormProperty.type)) {
                        string resumedClassName = GetResumedTypeName(eqmlFormProperty.type);
                        if (eQmlFormFile.name == resumedClassName) continue;//we can not import the current file
                        fileContent.Append("import {" + resumedClassName + "} from './" + resumedClassName + "';\n");
                        importFileTypes.Add(resumedClassName);
                    }
                }
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        #endregion

        #region WriteTypescriptDefinitionToFrontEnd
        static public bool WriteTypescriptDefinitionToFrontEnd(EQmlFile eQmlFile) {
            try {
                string tsComponentFileName = eQmlFile.name + ".d.ts";
                string completeFilePath = Globals.frontendRestInPeaceFolder + "/" + tsComponentFileName;
                Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"\t" + tsComponentFileName);
                StringBuilder fileContent = new StringBuilder();
                fileContent.Append("\n");
                WriteTypescriptImports(eQmlFile, ref fileContent);
                fileContent.Append("\n");
                if(!string.IsNullOrEmpty(eQmlFile.extendsElementName))fileContent.Append("export declare class " + eQmlFile.name + " extends "+eQmlFile.extendsElementName+" {\n");
                else fileContent.Append("export declare class " + eQmlFile.name + " {\n");
                if(eQmlFile.propertyList.Any())fileContent.Append("\t//region fields\n");
                foreach (EQMLFormProperty eQmlFormProperty in eQmlFile.propertyList) {
                    string st = "";
                    if (eQmlFormProperty.isStatic) st = "static ";                    
                    if (eQmlFormProperty.isReadOnly) st = " readonly ";
                    fileContent.Append("\t" +st+ eQmlFormProperty.name + ": " + eQmlFormProperty.type + ";\n");
                }
                if(eQmlFile.propertyList.Any())fileContent.Append("\t//endregion\n");
                fileContent.Append("\n");
                //let's write the enums
                if(eQmlFile.enumList.Any())fileContent.Append("\t//region enums\n");
                foreach (EEnumFile eEnumFile in eQmlFile.enumList) {
                    foreach (EEnumValue eEnumValue in eEnumFile.valueList) {
                        fileContent.Append($"\tstatic readonly {eEnumValue.name};\n");                        
                    }
                }
                if(eQmlFile.enumList.Any())fileContent.Append("\t//endregion\n");
                fileContent.Append("\n");                
                foreach (EQmlSignal eQmlSignal in eQmlFile.signalList) {
                    if(eQmlSignal.argTypeName!="void" && (!string.IsNullOrEmpty(eQmlSignal.argTypeName)))fileContent.Append("\t" + eQmlSignal.name + ": MySimpleEvent<" + eQmlSignal.argTypeName + ">;\n");
                    else fileContent.Append("\t" + eQmlSignal.name + ": MySimpleEvent<void>;\n");
                    
                }
                foreach (ETypescriptFunction eTypescriptFunction in eQmlFile.functionList) {
                    string st = "";
                    if (eTypescriptFunction.isStatic) st = "static ";
                    fileContent.Append("\t"+st + eTypescriptFunction.signature + ": " + eTypescriptFunction.returnType+";\n");
                }
                fileContent.Append("}");
                //Logger.LogInfo(newFileContent.ToString());
                string oldContent = "";
                if (File.Exists(completeFilePath)) oldContent = File.ReadAllText(completeFilePath);
                if (fileContent.ToString() != oldContent) File.WriteAllText(completeFilePath, fileContent.ToString());
                return true;
            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
        }
        #endregion
        
    }
}