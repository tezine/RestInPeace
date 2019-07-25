#region imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RestInPeace.Entities;
using RestInPeace.Enums;
#endregion

namespace RestInPeace.Codes.QMLTypescriptFrontend {
    public class QmlTSUIWriter {
        
        #region WriteFiles
        static public bool WriteFiles(string frontendQmlPagesFolder) {
            try {
                Logger.LogInfoIfDebugLevel(DebugLevels.Basic| DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"Writing Typescript uis to " + frontendQmlPagesFolder );
                foreach (EQmlFile eqmlFormFile in Globals.eQMLFormFiles) {
                    if (eqmlFormFile.qmlFileType != QmlFileType.AppForm) continue;
                    if (!WriteTypescriptFormFile(eqmlFormFile, frontendQmlPagesFolder)) return false;
                }
                return true;   
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
        
        #region WriteTypescriptFormFile
        static private bool WriteTypescriptFormFile(EQmlFile eQmlFormFile, string frontendQmlPagesFolder) {
            try {
                string typescriptFormFileName = eQmlFormFile.name + ".ts";
                string completeFilePath = frontendQmlPagesFolder + "/" + typescriptFormFileName;
                if (File.Exists(completeFilePath)) return true;                
                Logger.LogInfoIfDebugLevel(DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"\t" + typescriptFormFileName);
                StringBuilder fileContent = new StringBuilder(); 
                fileContent.Append("//region imports\n");
                fileContent.Append("import {"+eQmlFormFile.name+"Form"+ "} from '../RestInPeace/" + eQmlFormFile.name + "Form';\n");
                //fileContent.Append("import {ECppObjects} from '../RestInPeace/ECppObjects';\n");
                fileContent.Append("import {EBase} from '../RestInPeace/EBase';\n");
                fileContent.Append("//endregion\n");
                fileContent.Append("\n");
                fileContent.Append("export class " + eQmlFormFile.name + " {\n");
                //fileContent.Append("\n");
//                fileContent.Append("\t//region fields\n");
//                fileContent.Append("\tform: " + eQmlFormFile.name + "Form;\n");
//                fileContent.Append("\teCppObjects: ECppObjects;\n");
//                fileContent.Append("\tbase: EBase;\n");
//                fileContent.Append("\t//endregion\n");
                fileContent.Append("\n");
//                fileContent.Append("\tsetup(eCppObjects: ECppObjects, form: " + eQmlFormFile.name + "Form): void{\n");
//                fileContent.Append("\t\tthis.form=form;\n");
//                fileContent.Append("\t\tthis.eCppObjects=eCppObjects;\n");
//                fileContent.Append("\t\tthis.base=eCppObjects.base;\n");                
//                fileContent.Append("\t}\n");
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