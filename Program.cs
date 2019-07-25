#region Imports
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using RestInPeace.Codes;
using RestInPeace.Codes.BlazorFrontend;
using RestInPeace.Codes.FlutterFrontend;
using RestInPeace.Codes.QMLTypescriptFrontend;
using RestInPeace.Entities;
using RestInPeace.Enums;
#endregion

namespace RestInPeace {
    class Program {
        #region Main
        static void Main(string[] args) {
            try {
                Globals.debugLevel = DebugLevels.All;
                List<string> backendRootFolders = new List<string>();
                CommandLineApplication commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);
                CommandOption frontendTypeOption = commandLineApplication.Option("-$|-t | --type <type>", "Frontend Type (qml, qmlv2, qmltypescript, angular, dotnet, flutter).", CommandOptionType.SingleValue);
                CommandOption backendOption = commandLineApplication.Option("-b |--backend <directory>", "Backend source directory", CommandOptionType.SingleValue);
                CommandOption frontendOption = commandLineApplication.Option("-f | --frontend <directory>", "Frontend app directory.", CommandOptionType.SingleValue);
                CommandOption generateEntitiesOption = commandLineApplication.Option("-g | --generate-entities <value>", "Set true to create all entities.", CommandOptionType.SingleValue);
                CommandOption additionalEntityFolder = commandLineApplication.Option("-a | --additional-entities-folder <path>", "Set this to read entities from a library", CommandOptionType.SingleValue);
                CommandOption additionalEntityFolder2 = commandLineApplication.Option("-a | --additional-entities-folder-2 <path>", "Set this to read entities from a library", CommandOptionType.SingleValue);
                CommandOption restInPeaceVersion = commandLineApplication.Option("-r |--rest-in-peace-version <version>", "Rest in peace version", CommandOptionType.SingleValue);
                CommandOption renameToMJSOption = commandLineApplication.Option("--rename-to-mjs <directory>", "Rename to mjs", CommandOptionType.SingleValue);
                CommandOption excludeMJSOption = commandLineApplication.Option("--exclude <files>", "Do not rename these files", CommandOptionType.SingleValue);
                commandLineApplication.HelpOption("-? | -h | --help");
                commandLineApplication.OnExecute(() => {
                    if (renameToMJSOption.HasValue()) {
                        Helper.RenameJSToMJS(renameToMJSOption.Value(), excludeMJSOption.Value());
                        return 0;
                    }
                    if (!frontendTypeOption.HasValue()) {
                        Logger.LogError("Missing --type");
                        return 0;
                    }
                    if (!backendOption.HasValue()) {
                        Logger.LogError("Missing --backend");
                        return 0;
                    }
                    if (!frontendOption.HasValue()) {
                        Logger.LogError("Missing --frontend");
                        return 0;
                    }
                    if (generateEntitiesOption.HasValue() && generateEntitiesOption.Value() != "true" && generateEntitiesOption.Value() != "false") {
                        Logger.LogError("--generate-entities must be true or false");
                        return 0;
                    }
                    string frontendType = frontendTypeOption.Value();
                    Logger.LogInfo("Executing RestInPeace... Frontend type:" + frontendType);
                    if (frontendType != "qml" && frontendType != "qmlv2" && frontendType != "angular" && frontendType != "dotnet" && frontendType != "flutter" && frontendType != "qmltypescript" && frontendType != "blazor") {
                        Logger.LogError("--type must be qml, qmltypescript, angular, dotnet, blazor or flutter");
                        return 0;
                    }
                    bool generateEntities = false;
                    backendRootFolders.Add(backendOption.Value()); //we read all entities defined in the backend                    
                    if (generateEntitiesOption.HasValue()) generateEntities = generateEntitiesOption.Value() == "true";
                    if (generateEntities || frontendType == "qmltypescript" || frontendType == "flutter" || frontendType == "blazor" || frontendType == "dotnet") {
                        if (additionalEntityFolder.HasValue()) backendRootFolders.Add(additionalEntityFolder.Value()); //we read all entities defined in an optional library
                        if (additionalEntityFolder2.HasValue()) backendRootFolders.Add(additionalEntityFolder2.Value()); //we read all entities defined in an optional library
                        if (!CSharpEntityReader.AnalyseFiles(backendRootFolders)) {
                            Console.ReadKey();
                            return 0;
                        }
                        if (!CSharpEnumReader.AnalyseFiles(backendRootFolders)) {
                            Console.ReadKey();
                            return 0;
                        }
                    }
                    if (!CSharpFunctionReader.AnalyseFiles(backendRootFolders)) {
                        Console.ReadKey();
                        return 0;
                    }
                    bool ok = WriteFrontendFiles(frontendType, generateEntitiesOption.HasValue(), frontendOption.Value(), restInPeaceVersion.Value());
                    if (ok) Logger.LogInfo("=====================SUCCESS=====================");
                    //Console.ReadKey();
                    return 0;
                });
                commandLineApplication.Execute(args);
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        #endregion

        #region WriteFrontendFiles
        static bool WriteFrontendFiles(string frontendType, bool generateEntities, string frontendPath, string restInPeaceVersion) {
            try {
                bool ok = false;
                switch (frontendType) {
                    case "angular": {
                        var obj = new AngularWatcher(frontendPath, generateEntities, restInPeaceVersion);
                        break;
                    }
                    case "qml": {
                        if (generateEntities) {
                            if (!QMLEntityWriter.WriteFiles(CSharpEntityReader.fileList, frontendPath)) {
                                Console.ReadKey();
                                return false;
                            }
                        }
                        ok = QmlJSFunctionWriter.WriteFiles(frontendPath + "/Scripts");
                        break;
                    }
                    case "qmlv2": {
                        if (generateEntities) {
                            if (!QMLEntityWriter.WriteFiles(CSharpEntityReader.fileList, frontendPath)) {
                                Console.ReadKey();
                                return false;
                            }
                        }
                        ok = QmlJSFunctionWriter.WriteFiles(frontendPath + "/Scripts", true);
                        break;
                    }
                    case "qmltypescript": {
                        var obj = new QmlTSWatcher(frontendPath);
                        break;
                    }
                    case "blazor": {
                        var obj = new BlazorWatcher(frontendPath);
                        break;
                    }
                    case "flutter": {
                        var obj = new FlutterWatcher(frontendPath);
                        break;
                    }
                    case "dotnet": {
                        var obj = new DotnetWatcher(frontendPath);
                        break;
                    }
                    case "qmlweb": {
                        break;
                    }
                }
                return ok;
            } catch (Exception ex) {
                Logger.LogError(ex);
            }
            return false;
        }
        #endregion
    }
}