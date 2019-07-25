using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RestInPeace.Entities;
using RestInPeace.Enums;

namespace RestInPeace.Codes.QMLTypescriptFrontend {
    public class CppComponents {
        
        static private List<EQmlFile> cppComponents=new List<EQmlFile>();

        #region GetQtBasicComponents
        static public List<EQmlFile> GetCppComponents() {
            if (cppComponents.Any()) return cppComponents;
            AddEBase();
            AddEFirebase();
            AddQt();
            AddQtInputMethod();          
            return cppComponents;
        }
        #endregion
                
        
        #region AddEBase (CTezine)
        static private void AddEBase() {
            EQmlFile textField = new EQmlFile {
                name = "EBase",
                qmlFileType = QmlFileType.CppComponent,
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "screenHeight",type = "number"},
                    new EQMLFormProperty{name = "screenWidth",type = "number"},
                    new EQMLFormProperty{name = "qmlBaseURL",type = "string"},
                    new EQMLFormProperty{name = "baseURL",type = "string"},
                    new EQMLFormProperty{name = "token",type = "string"},
                    new EQMLFormProperty{name = "hotReloadEnabled",type = "boolean"},
                    new EQMLFormProperty{name = "listCount",type = "number"},
                    new EQMLFormProperty{name = "appVersion",type = "string"},
                },                
                functionList = new List<ETypescriptFunction>() {
                    new ETypescriptFunction{signature = "quit()",returnType = "void"},
                    new ETypescriptFunction{signature = "getIpAddress()",returnType = "string"},
                    new ETypescriptFunction{signature = "getOSVersion()",returnType = "string"},
                    new ETypescriptFunction{signature = "openUrl(url: string)",returnType = "void"},
                    new ETypescriptFunction{signature = "closeDialogs(obj: any)",returnType = "void"},
                    new ETypescriptFunction{signature = "createGUID()",returnType = "void"},
                    new ETypescriptFunction{signature = "isDesktop()",returnType = "boolean"},
                    new ETypescriptFunction{signature = "tr(txt: string)",returnType = "string"},
                    new ETypescriptFunction{signature = "setScreenOrientationMask(orientation: any)",returnType = "void"},
                    new ETypescriptFunction{signature = "setValue(key: any, value: any)",returnType = "void"},
                    new ETypescriptFunction{signature = "getValue(key: any)",returnType = "any"},
                    new ETypescriptFunction{signature = "containsKey(key: any)",returnType = "boolean"},
                    new ETypescriptFunction{signature = "removeKey(key: any)",returnType = "void"},
                    new ETypescriptFunction{signature = "readFileInBase64(completeFilePath: string)",returnType = "string"},
                }
            };
            cppComponents.Add(textField);
        }
        #endregion
        
        #region AddEFirebase (FireBase)
        static private void AddEFirebase() {
            EQmlFile eQmlFile = new EQmlFile {
                name = "EFirebase",
                qmlFileType = QmlFileType.CppComponent,
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "token",type = "string"},
                },                                
                signalList = new List<EQmlSignal>() {
                                    new EQmlSignal{name = "messageReceived",argTypeName = "any"},
                                    new EQmlSignal{name = "messageReceivedWithObject",argTypeName = "any"},
                                    new EQmlSignal{name = "tokenChanged",argTypeName = "string"},
                },
                functionList = new List<ETypescriptFunction>() {
                    new ETypescriptFunction{signature = "containsKey(key: string)",returnType = "boolean"},
                    new ETypescriptFunction{signature = "getValue(key: string)",returnType = "string"},
                    new ETypescriptFunction{signature = "clearHash()",returnType = "void"},
                    new ETypescriptFunction{signature = "setQmlObject(qmlObject:any)",returnType = "void"},
                }
            };
            cppComponents.Add(eQmlFile);
        }
        #endregion
        
        #region AddQt
        static private void AddQt() {
            EQmlFile textField = new EQmlFile {
                name = "Qt",
                qmlFileType = QmlFileType.CppComponent,
                typescriptImports = {
                    new EImport{className = "QInputMethod",path = "./"}
                },
                propertyList = {
                    new EQMLFormProperty{name = "inputMethod",type = "QInputMethod",isStatic = true},              
                },                
                enumList= {
                    new EEnumFile{enumName = "Qt",valueList = new List<EEnumValue>() {
                        new EEnumValue{name = "PrimaryOrientation",},
                        new EEnumValue{name = "LandscapeOrientation"},
                        new EEnumValue{name = "PortraitOrientation"},
                    }}
                },
                functionList = new List<ETypescriptFunction>() {
                    new ETypescriptFunction{signature = "resolvedUrl(url: string)",returnType = "string", isStatic = true},
                    new ETypescriptFunction{signature = "quit()",returnType = "void", isStatic = true},
                    new ETypescriptFunction{signature = "exit(retCode: number)",returnType = "void", isStatic = true},
                    new ETypescriptFunction{signature = "size(width: number, height: number)",returnType = "any", isStatic = true},//ver return
                    new ETypescriptFunction{signature = "incluse(url: string, callback: any)",returnType = "void", isStatic = true},
                    new ETypescriptFunction{signature = "isQtObject(object)",returnType = "boolean", isStatic = true},
                    new ETypescriptFunction{signature = "qsTr(txt: string)",returnType = "string", isStatic = true},
                    new ETypescriptFunction{signature = "openUrlExternally(target: string)",returnType = "boolean", isStatic = true},
                    new ETypescriptFunction{signature = "createComponent(url: string, mode: any, parent: any)",returnType = "any", isStatic = true},//ver paramers types e returntype
                    new ETypescriptFunction{signature = "createQmlObject(qml: string, parent: object, filePath: string)",returnType = "any", isStatic = true},//ver return type
                    new ETypescriptFunction{signature = "formatDate(dt: any, format: string)",returnType = "string", isStatic = true},
                    new ETypescriptFunction{signature = "formatDateTime(dtTime: any, format: string)",returnType = "string", isStatic = true},
                    new ETypescriptFunction{signature = "formatTime(dtTime: any, format: string)",returnType = "string", isStatic = true},
                    new ETypescriptFunction{signature = "md5(txt: string)",returnType = "string", isStatic = true},
                }
            };
            cppComponents.Add(textField);
        }
        #endregion
        
        #region AddQtInputMethod
        static private void AddQtInputMethod() {
            EQmlFile textField = new EQmlFile {
                name = "QInputMethod",
                qmlFileType = QmlFileType.CppComponent,
                functionList = new List<ETypescriptFunction>() {
                    new ETypescriptFunction{signature = "hide()",returnType = "void"},
                    new ETypescriptFunction{signature = "commit()",returnType = "void"},
                }
            };
            cppComponents.Add(textField);
        }
        #endregion

        #region WriteTypescriptDefinitionsToFrontend
        static public bool WriteTypescriptDefinitionsToFrontend() {            
            var list=GetCppComponents();
            Logger.LogInfoIfDebugLevel(DebugLevels.Basic| DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"Writing Qt Typescript definition files to " + Globals.frontendRestInPeaceFolder );
            foreach (var basicElement in list) {
                if (!DTSWriter.WriteTypescriptDefinitionToFrontEnd(basicElement)) return false;
            }
            return true;
        }
        #endregion

        #region WriteTypescriptDefinitionToFrontEnd //todo mudar para DTSWRiter WriteTypescriptDefinitions....
        static private bool WriteTypescriptDefinitionToFrontEnd(EQmlFile eQmlFile) {
            try {
                string tsComponentFileName = eQmlFile.name + ".d.ts";
                string completeFilePath = Globals.frontendRestInPeaceFolder + "/" + tsComponentFileName;
                Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"\t" + tsComponentFileName);
                StringBuilder fileContent = new StringBuilder();
                //WriteTypescriptImports(eQmlFormFile, ref fileContent);
                //fileContent.Append("\n");
                foreach (EImport eImport in eQmlFile.typescriptImports) {
                    fileContent.Append("import {"+eImport.className+"} from '"+eImport.path+eImport.className+"';\n");    
                }
                fileContent.Append("\n");
                fileContent.Append("export declare class " + eQmlFile.name + " {\n");
                if(eQmlFile.propertyList.Any())fileContent.Append("\t//region fields\n");
                foreach (EQMLFormProperty eQmlFormProperty in eQmlFile.propertyList) {
                    string st = "";
                    if (eQmlFormProperty.isStatic) st = "static ";
                    fileContent.Append("\t"+st + eQmlFormProperty.name + ": " + eQmlFormProperty.type + ";\n");
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
                    fileContent.Append("\t"+st +eTypescriptFunction.signature + ": " + eTypescriptFunction.returnType+";\n");
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