#region Imports
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using RestInPeace.Entities;
#endregion

namespace RestInPeace.Codes {
    public class QMLEntityWriter {

        #region WriteFiles
        static public bool WriteFiles(List<EEntityFile> filesList, string appDirectory) {
            try {
                Logger.LogInfo("\n\nWriting entity "+filesList.Count.ToString()+" files to " + appDirectory + "========================================================");
                string entityDirectory = appDirectory + "/Entities";
                foreach (EEntityFile eEntityFile in filesList) {
                    if (eEntityFile.onlyImport) continue;
                    if (!WriteQMLEntityFile(eEntityFile, entityDirectory)) return false;
                }
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region WriteQMLEntityFile
        static private bool WriteQMLEntityFile(EEntityFile eEntityFile, string entityDirectory) {
            try {
                Directory.CreateDirectory(entityDirectory);
                string qmlFileName = eEntityFile.className + ".qml";
                string completeQMLFilePath = entityDirectory + "/" + qmlFileName;
                Logger.LogInfo("Writing " + qmlFileName);
                StringBuilder newFileContent = new StringBuilder();
                newFileContent.Append("import QtQuick 2.6\n");
                newFileContent.Append("import com.tezine.base 1.0\n\n");
                newFileContent.Append("//author Bruno Tezine\n");
                newFileContent.Append("//Se for tipo primitivo nullable, usar var\n");
                newFileContent.Append("QtObject {");
                foreach (EEntityProperty eEntityProperty in eEntityFile.propertyList) {
                    string propertyLine = "";
                    if (!GetPropertyLine(eEntityProperty, out propertyLine)) {
                        Logger.LogError("unable to write property in file " + qmlFileName);
                        return false;
                    }
                    newFileContent.Append(propertyLine);
                }
                newFileContent.Append("\n}");
                //Logger.LogInfo(newFileContent.ToString());
                string oldContent = "";
                if (File.Exists(completeQMLFilePath)) oldContent = File.ReadAllText(completeQMLFilePath);
                if (newFileContent.ToString() != oldContent) File.WriteAllText(completeQMLFilePath, newFileContent.ToString());
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion

        #region GetPropertyLine
        static private bool GetPropertyLine(EEntityProperty eEntityProperty, out string propertyLine) {
            propertyLine = "";
            try {
                if (eEntityProperty.csharpTypeName.Contains("?")) {
                    propertyLine = "\n\tproperty var " + eEntityProperty.name;
                    return true;
                }
                if (eEntityProperty.csharpTypeName.Contains("Int32") || eEntityProperty.csharpTypeName.Contains("Int64") ||eEntityProperty.csharpTypeName.Contains("int"))propertyLine = "\n\tproperty int " + eEntityProperty.name;
                else if(eEntityProperty.csharpTypeName.Contains("double") || eEntityProperty.csharpTypeName.Contains("Decimal") ||eEntityProperty.csharpTypeName.Contains("decimal"))propertyLine="\n\tproperty double "+eEntityProperty.name;
                else if (eEntityProperty.csharpTypeName.Contains("float")) propertyLine = "\n\tproperty float " + eEntityProperty.name;
                else if (eEntityProperty.csharpTypeName.Contains("bool")) propertyLine = "\n\tproperty bool " + eEntityProperty.name;
                else if (eEntityProperty.csharpTypeName.Contains("string")) propertyLine = "\n\tproperty string " + eEntityProperty.name;
                else propertyLine="\n\tproperty var " + eEntityProperty.name;
                return true;
            } catch (Exception e) {
                Logger.LogError(e);
            }
            return false;
        }
        #endregion
    }
}