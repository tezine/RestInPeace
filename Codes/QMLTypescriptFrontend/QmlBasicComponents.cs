using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RestInPeace.Entities;
using RestInPeace.Enums;

namespace RestInPeace.Codes.QMLTypescriptFrontend {
    public class QmlBasicComponents {

        static private List<EQmlFile> qtBasicComponents=new List<EQmlFile>();

        #region GetQtBasicComponents
        static public List<EQmlFile> GetQtBasicComponents() {
            if (qtBasicComponents.Any()) return qtBasicComponents;
            AddItem();
            AddRectangle();
            AddControl();
            AddTextInput();
            AddTextField();
            AddAbstractButton();
            AddButton();
            AddImage();
            AddStackView();
            AddMouseArea();
            AddProgressBar();
            AddSettings();
            AddCheckBox();
            AddFocusScope();
            AddListView();
            AddFlickable();
            AddListModel();
            AddLabel();
            AddText();
            AddColumnLayout();
            AddMenu();
            AddMenuItem();
            AddPopup();
            AddComboBox();
            AddDialog();
            AddFont();
            AddAnchors();
            AddMap();
            AddQtPositioning();
            AddRouteModel();
            AddRouteQuery();
            AddMapRoute();
            AddRoute();
            AddTimer();
            AddDrawer();
            AddTextEdit();
            AddTextArea();
            AddPositionSource();
            AddCamera();
            return qtBasicComponents;
        }
        #endregion

        #region AddItem
        static private void AddItem() {
            EQmlFile element = new EQmlFile {
                name = "Item",                
                qmlFileType = QmlFileType.QtComponent,
                typescriptImports = {
                    new EImport{className = "Anchors",path = "./"}, 
                },                
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "activeFocus",type = "boolean"},
                    new EQMLFormProperty{name = "activeFocusOnTab",type = "boolean"},
                    new EQMLFormProperty{name = "anchors",type = "Anchors"},
                    new EQMLFormProperty{name = "antialising",type = "boolean"},
                    new EQMLFormProperty{name = "baselineOffset",type = "number"},
                    new EQMLFormProperty{name = "children",type = "any"},//ver
                    new EQMLFormProperty{name = "childrenRect",type = "any"},//ver
                    new EQMLFormProperty{name = "clip",type = "boolean"},
                    new EQMLFormProperty{name = "containmentMask",type = "any"},//ver
                    new EQMLFormProperty{name = "data",type = "any"},//ver
                    new EQMLFormProperty{name = "enabled",type = "boolean"},
                    new EQMLFormProperty{name = "focus",type = "boolean"},
                    new EQMLFormProperty{name = "height",type = "number"},
                    new EQMLFormProperty{name = "implicitHeight",type = "number"},
                    new EQMLFormProperty{name = "implicitWidth",type = "number"},
                    //adicionar layers
                    new EQMLFormProperty{name = "opacity",type = "number"},
                    new EQMLFormProperty{name = "parent",type = "Item"},
                    new EQMLFormProperty{name = "resources",type = "any"},
                    new EQMLFormProperty{name = "rotation",type = "number"},
                    new EQMLFormProperty{name = "scale",type = "number"},
                    new EQMLFormProperty{name = "smooth",type = "boolean"},
                    new EQMLFormProperty{name = "state",type = "string"},
                    new EQMLFormProperty{name = "states",type = "any"},//ver
                    new EQMLFormProperty{name = "transform",type = "any"},//ver
                    new EQMLFormProperty{name = "transformOrigin",type = "any"},//ver
                    new EQMLFormProperty{name = "transitions",type = "any"},//ver
                    new EQMLFormProperty{name = "visible",type = "boolean"},
                    new EQMLFormProperty{name = "visibleChildren",type = "any"},//ver
                    new EQMLFormProperty{name = "width",type = "number"},
                    new EQMLFormProperty{name = "x",type = "number"},
                    new EQMLFormProperty{name = "y",type = "number"},
                    new EQMLFormProperty{name = "z",type = "number"},
                },
                functionList = new List<ETypescriptFunction>() {
                    new ETypescriptFunction{signature = "forceActiveFocus()",returnType = "void"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddRectangle
        static private void AddRectangle() {
            EQmlFile element = new EQmlFile {
                name = "Rectangle",                
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "Item",
                typescriptImports = new List<EImport>() {
                    new EImport{className = "Item",path = "./"} 
                },
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "color",type = "string"},                    
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddControl
        static private void AddControl() {
            EQmlFile element = new EQmlFile {
                name = "Control",                
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "Item",
                typescriptImports = new List<EImport>() {
                    new EImport{className = "Item",path = "./"} 
                },  
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "availableHeight",type = "number"},                    
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion

        #region AddTextInput
        static private void AddTextInput() {
            EQmlFile textField = new EQmlFile {
                name = "TextInput",
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "Item",
                typescriptImports = new List<EImport>() {
                    new EImport{className = "Item",path = "./"},
                    new EImport{className = "Font",path = "./"} 
                },
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "acceptableInput",type = "boolean"},
                    new EQMLFormProperty{name = "activeFocusOnPress",type = "boolean"},
                    new EQMLFormProperty{name = "autoScroll",type = "boolean"},
                    new EQMLFormProperty{name = "bottomPadding",type = "number"},
                    new EQMLFormProperty{name = "canPaste",type = "boolean"},
                    new EQMLFormProperty{name = "canRedo",type = "boolean"},
                    new EQMLFormProperty{name = "canUndo",type = "boolean"},
                    new EQMLFormProperty{name = "color",type = "any"},//ver
                    new EQMLFormProperty{name = "contentHeight",type = "number"},
                    new EQMLFormProperty{name = "contentWidth",type = "number"},
                    new EQMLFormProperty{name = "cursorDelegate",type = "any"},//ver
                    new EQMLFormProperty{name = "cursorPosition",type = "number"},
                    new EQMLFormProperty{name = "cursorRectangle",type = "any"},//ver
                    new EQMLFormProperty{name = "cursorVisible",type = "boolean"},
                    new EQMLFormProperty{name = "displayText",type = "string"},
                    new EQMLFormProperty{name = "echoMode",type = "any"},//ver
                    new EQMLFormProperty{name = "effectiveHorizontalAlignment",type = "any"},//ver
                    new EQMLFormProperty{name = "font",type = "Font"},
                    new EQMLFormProperty{name = "horizontalAlignment",type = "any"},
                    new EQMLFormProperty{name = "inputMask",type = "string"},
                    new EQMLFormProperty{name = "inputMethodComposing",type = "boolean"},
                    new EQMLFormProperty{name = "inputMethodHints",type = "any"},//ver
                    new EQMLFormProperty{name = "leftPadding",type = "number"},
                    new EQMLFormProperty{name = "length",type = "number"},
                    new EQMLFormProperty{name = "maximumLength",type = "number"},
                    new EQMLFormProperty{name = "mouseSelectionMode",type = "any"},//ver
                    new EQMLFormProperty{name = "overwriteMode",type = "boolean"},
                    new EQMLFormProperty{name = "padding",type = "number"},
                    new EQMLFormProperty{name = "passwordCharacter",type = "string"},
                    new EQMLFormProperty{name = "passwordMaskDelay",type = "number"},
                    new EQMLFormProperty{name = "persistentSelection",type = "boolean"},
                    new EQMLFormProperty{name = "preeditText",type = "string"},
                    new EQMLFormProperty{name = "readOnly",type = "boolean"},
                    new EQMLFormProperty{name = "renderType",type = "any"},//ver
                    new EQMLFormProperty{name = "rightPadding",type = "number"},
                    new EQMLFormProperty{name = "selectByMouse",type = "boolean"},
                    new EQMLFormProperty{name = "selectedText",type = "string"},
                    new EQMLFormProperty{name = "selectedTextColor",type = "string"},
                    new EQMLFormProperty{name = "selectionColor",type = "string"},
                    new EQMLFormProperty{name = "selectionEnd",type = "number"},
                    new EQMLFormProperty{name = "selectionStart",type = "number"},
                    new EQMLFormProperty{name = "text",type = "string"},
                    new EQMLFormProperty{name = "topPadding",type = "number"},
                    new EQMLFormProperty{name = "validator",type = "any"},//ver
                    new EQMLFormProperty{name = "verticalAlignment",type = "number"},
                    new EQMLFormProperty{name = "wrapMode",type = "number"},
                },
                functionList = {
                    new ETypescriptFunction{signature = "clear()",returnType = "void"},
                    new ETypescriptFunction{signature = "copy()",returnType = "void"},
                    new ETypescriptFunction{signature = "cut()",returnType = "void"},    
                    new ETypescriptFunction{signature = "deselect()",returnType = "void"},
                    new ETypescriptFunction{signature = "ensureVisible(position: number)",returnType = "void"},
                    new ETypescriptFunction{signature = "getText(start: number, end: number)",returnType = "string"},
                    new ETypescriptFunction{signature = "insert(position: number, text: string)",returnType = "void"},
                    new ETypescriptFunction{signature = "isRightToLeft(start: number, end: number)",returnType = "boolean"},
                    new ETypescriptFunction{signature = "moveCursorSelection(position: number, mode?: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "paste()",returnType = "void"},
                    new ETypescriptFunction{signature = "positionAt(x: number, y: number, position?: any)",returnType = "number"},//ver
                    new ETypescriptFunction{signature = "positionToRectangle(pos: number)",returnType = "any"},//ver
                    new ETypescriptFunction{signature = "redo()",returnType = "void"},
                    new ETypescriptFunction{signature = "remove(start: number, end: number)",returnType = "void"},
                    new ETypescriptFunction{signature = "select(start: number, end: number)",returnType = "void"},
                    new ETypescriptFunction{signature = "selectAll()",returnType = "void"},
                    new ETypescriptFunction{signature = "selectWord()",returnType = "void"},
                    new ETypescriptFunction{signature = "undo()",returnType = "void"},
                }
            };
            qtBasicComponents.Add(textField);
        }
        #endregion
        
        #region AddTextField
        static private void AddTextField() {
            EQmlFile textField = new EQmlFile {
                name = "TextField",
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "TextInput",
                typescriptImports = new List<EImport>() {
                    new EImport{className = "TextInput",path = "./"} 
                },                
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "leftInset",type = "number"},                    
                }
            };
            qtBasicComponents.Add(textField);
        }
        #endregion

        #region AddAbstractButton
        static private void AddAbstractButton() {
            EQmlFile element = new EQmlFile {
                name = "AbstractButton",
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "Control",
                typescriptImports = new List<EImport>() {
                    new EImport{className = "Control",path = "./"} 
                },     
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "text",type = "string"},
                    new EQMLFormProperty{name = "checked",type = "boolean"},
                    new EQMLFormProperty{name = "onClicked",type = "MySimpleEvent<void>"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion

        #region AddButton
        static private void AddButton() {
            EQmlFile element = new EQmlFile {
                name = "Button",                
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "AbstractButton",
                typescriptImports = {
                    new EImport{className = "AbstractButton",path = "./"} 
                },                
                propertyList = {
                    new EQMLFormProperty{name = "flat",type = "boolean"},                    
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddStackView -  USAR ESSE COMO REFERENCIA
        static private void AddStackView() {
            EQmlFile element = new EQmlFile {
                name = "StackView",                
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "FocusScope",
                typescriptImports = {
                    new EImport{className = "FocusScope",path = "./"} 
                },                
                propertyList = {
                    new EQMLFormProperty{name = "busy",type = "boolean"},
                    new EQMLFormProperty{name = "currentItem",type = "Item"},
                    new EQMLFormProperty{name = "depth",type = "number"},
                    new EQMLFormProperty{name = "initialItem",type = "any"},
                },
                functionList = {
                    new ETypescriptFunction{signature = "clear()",returnType = "void"},
                    new ETypescriptFunction{signature = "push(item: any)",returnType = "any"},
                    new ETypescriptFunction{signature = "pop(item: any|undefined)",returnType = "any"},
//                    new ETypescriptFunction{signature = "push(item: any, properties?: any, operation?: any)",returnType = "any"},
//                    new ETypescriptFunction{signature = "pop(item?: any, operation?: any)",returnType = "any"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddComboBox 
        static private void AddComboBox() {
            EQmlFile element = new EQmlFile {
                name = "ComboBox",                
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "Control",
                typescriptImports = {
                    new EImport{className = "Control",path = "./"} 
                },                
                propertyList = {
                    new EQMLFormProperty{name = "flat",type = "boolean"},
                },
                functionList = {
                    new ETypescriptFunction{signature = "selectAll()",returnType = "void"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddMenu 
        static private void AddMenu() {
            EQmlFile element = new EQmlFile {
                name = "Menu",                
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "Popup",
                typescriptImports = {
                    new EImport{className = "Popup",path = "./"} 
                },                
                propertyList = {
                    new EQMLFormProperty{name = "title",type = "string"},
                    new EQMLFormProperty{name = "cascade",type = "boolean"},
                },
                functionList = {
                    new ETypescriptFunction{signature = "dismiss()",returnType = "void"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddMenuItem 
        static private void AddMenuItem() {
            EQmlFile element = new EQmlFile {
                name = "MenuItem",                
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "AbstractButton",
                typescriptImports = {
                    new EImport{className = "AbstractButton",path = "./"} 
                },                
                propertyList = {
                    new EQMLFormProperty{name = "menu",type = "Menu"},
                },
                functionList = {
                    new ETypescriptFunction{signature = "triggered()",returnType = "void"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddPopup 
        static private void AddPopup() {
            EQmlFile element = new EQmlFile {
                name = "Popup",                
                qmlFileType = QmlFileType.QtComponent,
                propertyList = {
                    new EQMLFormProperty{name = "enabled",type = "boolean"},
                },
                functionList = {
                    new ETypescriptFunction{signature = "open()",returnType = "void"},
                    new ETypescriptFunction{signature = "close()",returnType = "void"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddColumnLayout 
        static private void AddColumnLayout() {
            EQmlFile element = new EQmlFile {
                name = "ColumnLayout",                
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "Item",
                typescriptImports = {
                    new EImport{className = "Item",path = "./"} 
                },                
                propertyList = {
                    new EQMLFormProperty{name = "spacing",type = "number"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddText
        static private void AddText() {
            EQmlFile element = new EQmlFile {
                name = "Text",
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "Item",
                typescriptImports = {
                    new EImport{className = "Item",path = "./"} 
                },
                propertyList = {
                    new EQMLFormProperty{name = "text",type = "string"},                    
                }
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddLabel
        static private void AddLabel() {
            EQmlFile element = new EQmlFile {
                name = "Label",
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "Text",
                typescriptImports = {
                    new EImport{className = "Text",path = "./"} 
                },
                propertyList = {
                    new EQMLFormProperty{name = "background",type = "Item"},                    
                }
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddFlickable
        static private void AddFlickable() {
            EQmlFile element = new EQmlFile {
                name = "Flickable",
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "Item",
                typescriptImports = {
                    new EImport{className = "Item",path = "./"} 
                },
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "flicking",type = "boolean"},                    
                }
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddListView
        static private void AddListView() {
            EQmlFile element = new EQmlFile {
                name = "ListView",
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "Flickable",
                typescriptImports = {
                    new EImport{className = "Flickable",path = "./"} 
                },
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "section",type = "string"},                    
                }
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddListModel
        static private void AddListModel() {
            EQmlFile element = new EQmlFile {
                name = "ListModel",
                qmlFileType = QmlFileType.QtComponent,
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "count",type = "number"},                    
                },
                functionList = {
                    new ETypescriptFunction{signature = "clear()",returnType = "void"},
                    new ETypescriptFunction{signature = "get(index: number)",returnType = "any"},
                    new ETypescriptFunction{signature = "setProperty(index: number, property: string, value: any)",returnType = "void"},
                    new ETypescriptFunction{signature = "remove(index: number, count?: number)",returnType = "void"},
                }
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddFocusScope
        static private void AddFocusScope() {
            EQmlFile element = new EQmlFile {
                name = "FocusScope",                
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "Item",
                typescriptImports = {
                    new EImport{className = "Item",path = "./"} 
                },                
                propertyList = {                  
                },
                functionList = {
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddImage
        static private void AddImage() {
            EQmlFile element = new EQmlFile {
                name = "Image",
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "Item",
                typescriptImports = {
                    new EImport{className = "Item",path = "./"} 
                },                
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "source",type = "URL"},
                }
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddMouseArea
        static private void AddMouseArea() {
            EQmlFile element = new EQmlFile {
                name = "MouseArea",
                qmlFileType = QmlFileType.QtComponent,
                propertyList = {
                    new EQMLFormProperty{name = "source",type = "URL"},              
                    new EQMLFormProperty{name = "onClicked",type = "MySimpleEvent<void>"},
                }
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddProgressBar
        static private void AddProgressBar() {
            EQmlFile element = new EQmlFile {
                name = "ProgressBar",
                qmlFileType = QmlFileType.QtComponent,
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "value",type = "number"},                    
                }
            };
            qtBasicComponents.Add(element);
        }
        #endregion       
        
        #region AddSettings
        static private void AddSettings() {
            EQmlFile element = new EQmlFile {
                name = "Settings",  
                qmlFileType = QmlFileType.QtComponent,
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "category",type = "string"},                    
                },
                functionList = new List<ETypescriptFunction>() {
                    new ETypescriptFunction{signature = "setValue(key:any, value: any)",returnType = "void"},
                    new ETypescriptFunction{signature = "value(key:any)",returnType = "any"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddCheckBox
        static private void AddCheckBox() {
            EQmlFile element = new EQmlFile {
                name = "CheckBox",
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "AbstractButton",
                typescriptImports = new List<EImport>() {
                    new EImport{className = "AbstractButton",path = "./"} 
                },                
                propertyList = new List<EQMLFormProperty>(){
                    new EQMLFormProperty{name = "tristate",type = "boolean"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddDialog 
        static private void AddDialog() {
            EQmlFile element = new EQmlFile {
                name = "Dialog",                
                qmlFileType = QmlFileType.QtComponent,
                extendsElementName = "Popup",
                typescriptImports = {
                    new EImport{className = "Popup",path = "./"} 
                },                
                propertyList = {
                    new EQMLFormProperty{name = "result",type = "number"},
                    new EQMLFormProperty{name = "title",type = "string"},                    
                },
                functionList = {
                    new ETypescriptFunction{signature = "accept()",returnType = "void"},
                    new ETypescriptFunction{signature = "reject()",returnType = "void"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddFont 
        static private void AddFont() {
            EQmlFile element = new EQmlFile {
                name = "Font",                
                qmlFileType = QmlFileType.QtComponent,
                propertyList = {
                    new EQMLFormProperty{name = "bold",type = "boolean"},
                    new EQMLFormProperty{name = "capitalization",type = "any"},//ver
                    new EQMLFormProperty{name = "family",type = "string"},
                    new EQMLFormProperty{name = "hintingPreference",type = "any"},//ver
                    new EQMLFormProperty{name = "italic",type = "boolean"},
                    new EQMLFormProperty{name = "kerning",type = "boolean"},
                    new EQMLFormProperty{name = "letterSpacing",type = "number"},
                    new EQMLFormProperty{name = "pixelSize",type = "number"},
                    new EQMLFormProperty{name = "pointSize",type = "number"},
                    new EQMLFormProperty{name = "preferShaping",type = "boolean"},
                    new EQMLFormProperty{name = "strikeout",type = "boolean"},
                    new EQMLFormProperty{name = "styleName",type = "string"},
                    new EQMLFormProperty{name = "underline",type = "boolean"},
                    new EQMLFormProperty{name = "weight",type = "any"},//ver
                    new EQMLFormProperty{name = "wordSpacing",type = "number"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddAnchors 
        static private void AddAnchors() {
            EQmlFile element = new EQmlFile {
                name = "Anchors",                
                qmlFileType = QmlFileType.QtComponent,
                typescriptImports = {
                    new EImport{className = "Item",path = "./"} 
                },                
                propertyList = {
                    new EQMLFormProperty{name = "top",type = "any"},//ver
                    new EQMLFormProperty{name = "bottom",type = "any"},//ver
                    new EQMLFormProperty{name = "left",type = "any"},//ver
                    new EQMLFormProperty{name = "right",type = "any"},//ver
                    new EQMLFormProperty{name = "horizontalCenter",type = "any"},//ver
                    new EQMLFormProperty{name = "verticalCenter",type = "any"},//ver
                    new EQMLFormProperty{name = "baseline",type = "any"},//ver
                    new EQMLFormProperty{name = "fill",type = "Item"},
                    new EQMLFormProperty{name = "centerIn",type = "Item"},
                    new EQMLFormProperty{name = "margins",type = "number"},
                    new EQMLFormProperty{name = "topMargin",type = "number"},
                    new EQMLFormProperty{name = "bottomMargin",type = "number"},
                    new EQMLFormProperty{name = "leftMargin",type = "number"},
                    new EQMLFormProperty{name = "rightMargin",type = "number"},
                    new EQMLFormProperty{name = "horizontalCenterOffset",type = "number"},
                    new EQMLFormProperty{name = "verticalCenterOffset",type = "number"},
                    new EQMLFormProperty{name = "baselineOffset",type = "number"},
                    new EQMLFormProperty{name = "alignWhenCentered",type = "boolean"},

                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddMap 
        static private void AddMap() {
            EQmlFile element = new EQmlFile {
                name = "Map",                
                qmlFileType = QmlFileType.QtComponent,             
                propertyList = {
                    new EQMLFormProperty{name = "activeMapType",type = "any"},//ver
                    new EQMLFormProperty{name = "bearing",type = "any"},
                    new EQMLFormProperty{name = "center",type = "any"},//ver
                    new EQMLFormProperty{name = "color",type = "any"},//ver
                    new EQMLFormProperty{name = "copyrightsVisible",type = "boolean"},
                    new EQMLFormProperty{name = "error",type = "any"},//ver
                    new EQMLFormProperty{name = "errorString",type = "string"},
                    new EQMLFormProperty{name = "fieldOfView",type = "number"},
                    new EQMLFormProperty{name = "gesture",type = "any"},//ver
                    new EQMLFormProperty{name = "mapItens",type = "any"},
                    new EQMLFormProperty{name = "mapParameters",type = "any"},
                    new EQMLFormProperty{name = "mapReady",type = "boolean"},
                    new EQMLFormProperty{name = "maximumFieldOfView",type = "number"},
                    new EQMLFormProperty{name = "maximumTilt",type = "number"},
                    new EQMLFormProperty{name = "maximumZoomLevel",type = "number"},
                    new EQMLFormProperty{name = "minimumFieldOfView",type = "number"},
                    new EQMLFormProperty{name = "minimumTilt",type = "number"},
                    new EQMLFormProperty{name = "minimumZoomLevel",type = "number"},
                    new EQMLFormProperty{name = "plugin",type = "any"},//ver
                    new EQMLFormProperty{name = "supportedMapTypes",type = "any"},//ver
                    new EQMLFormProperty{name = "tilt",type = "number"},
                    new EQMLFormProperty{name = "visibleArea",type = "any"},//ver
                    new EQMLFormProperty{name = "visibleRegion",type = "any"},//ver
                    new EQMLFormProperty{name = "zoomLevel",type = "number"},
                },
                functionList = {
                    new ETypescriptFunction{signature = "addMapItem(item: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "addMapItemGroup(itemGroup: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "addMapItemView(itemView: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "addMapParameter(parameter: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "alignCoordinateToPoint(coordinate: any, point: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "clearData()",returnType = "void"},
                    new ETypescriptFunction{signature = "clearMapItems()",returnType = "void"},
                    new ETypescriptFunction{signature = "clearMapParameters()",returnType = "void"},
                    new ETypescriptFunction{signature = "fitViewportToVisibleMapItems()",returnType = "void"},
                    new ETypescriptFunction{signature = "fromCoordinate(coordinate: any, clipToViewport: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "pan(dx: number, dy: number)",returnType = "void"},
                    new ETypescriptFunction{signature = "prefechData()",returnType = "void"},
                    new ETypescriptFunction{signature = "removeMapItem(item: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "removeMapItemGroup(itemGroup: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "removeMapItemView(itemView: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "removeMapParameter(parameter: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "setBearing(bearing: number, coordinate: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "toCoordinate(position: any, clipToViewPort: boolean)",returnType = "void"},//ver
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddQtPositioning 
        static private void AddQtPositioning() {
            EQmlFile element = new EQmlFile {
                name = "QtPositioning",                
                qmlFileType = QmlFileType.QtComponent,                
                functionList = {
                    new ETypescriptFunction{signature = "coordinate(latitude: number, longitude: number, altitude?: number)",returnType = "any", isStatic = true},                    
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddRouteModel 
        static private void AddRouteModel() {
            EQmlFile element = new EQmlFile {
                name = "RouteModel",                
                qmlFileType = QmlFileType.QtComponent, 
                typescriptImports = {
                    new EImport{className = "Route",path = "./"} 
                },                
                propertyList = {
                    new EQMLFormProperty{name = "autoUpdate",type = "boolean"},
                    new EQMLFormProperty{name = "count",type = "number"},
                    new EQMLFormProperty{name = "error",type = "number"},//ver
                    new EQMLFormProperty{name = "errorString",type = "string"},
                    new EQMLFormProperty{name = "measurementSystem",type = "any"},//ver
                    new EQMLFormProperty{name = "plugin",type = "any"},//ver
                    new EQMLFormProperty{name = "query",type = "RouteQuery"},
                    new EQMLFormProperty{name = "status",type = "number"},//ver
                },
                functionList = {
                    new ETypescriptFunction{signature = "cancel()",returnType = "void"},
                    new ETypescriptFunction{signature = "get(index: number)",returnType = "Route"},
                    new ETypescriptFunction{signature = "reset()",returnType = "void"},
                    new ETypescriptFunction{signature = "update()",returnType = "void"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
                
        #region AddRouteQuery 
        static private void AddRouteQuery() {
            EQmlFile element = new EQmlFile {
                name = "RouteQuery",                
                qmlFileType = QmlFileType.QtComponent, 
                propertyList = {
                    new EQMLFormProperty{name = "excludedAreas",type = "any"},//ver
                    new EQMLFormProperty{name = "extraParameters",type = "any"},//ver
                    new EQMLFormProperty{name = "featureTypes",type = "any"},//ver
                    new EQMLFormProperty{name = "maneuverDetail",type = "number"},//ver
                    new EQMLFormProperty{name = "numberAlternativeRoutes",type = "number"},
                    new EQMLFormProperty{name = "routeOptimizations",type = "number"},//ver
                    new EQMLFormProperty{name = "segmentDetail",type = "number"},//ver
                    new EQMLFormProperty{name = "travelModes",type = "number"},//ver
                    new EQMLFormProperty{name = "waypoints",type = "any"},//ver
                },
                functionList = {
                    new ETypescriptFunction{signature = "addExcludedArea(georectangle: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "addWaypoint(coordinate: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "clearExcludedAreas()",returnType = "void"},
                    new ETypescriptFunction{signature = "clearWaypoints()",returnType = "void"},
                    new ETypescriptFunction{signature = "featureWeight(featureType: any)",returnType = "any"},//ver
                    new ETypescriptFunction{signature = "removeExcludedArea(georectangle: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "removeWaypoint(coordinate: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "resetFeatureWeights()",returnType = "void"},
                    new ETypescriptFunction{signature = "setFeatureWeight(featureType: any, featureWeight: any)",returnType = "void"},//ver
                    new ETypescriptFunction{signature = "waypointObjects()",returnType = "any"},//ver
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion        
        
        #region AddMapRoute
        static private void AddMapRoute() {
            EQmlFile element = new EQmlFile {
                name = "MapRoute",                
                qmlFileType = QmlFileType.QtComponent,
                propertyList = {
                    new EQMLFormProperty{name = "route",type = "Route"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddRoute
        static private void AddRoute() {
            EQmlFile element = new EQmlFile {
                name = "Route",                
                qmlFileType = QmlFileType.QtComponent,
                propertyList = {
                    new EQMLFormProperty{name = "bounds",type = "any"},//ver
                    new EQMLFormProperty{name = "distance",type = "number"},
                    new EQMLFormProperty{name = "legs",type = "any"},//ver
                    new EQMLFormProperty{name = "path",type = "any"},//ver
                    new EQMLFormProperty{name = "routeQuery",type = "RouteQuery"},
                    new EQMLFormProperty{name = "segments",type = "any"},//ver
                    new EQMLFormProperty{name = "travelTime",type = "number"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddTimer 
        static private void AddTimer() {
            EQmlFile element = new EQmlFile {
                name = "Timer",                
                qmlFileType = QmlFileType.QtComponent,
                propertyList = {
                    new EQMLFormProperty{name = "interval",type = "number"},
                    new EQMLFormProperty{name = "repeat",type = "boolean"},
                    new EQMLFormProperty{name = "running",type = "boolean"},
                    new EQMLFormProperty{name = "triggeredOnStart",type = "boolean"},
                },
                signalList = {
                    new EQmlSignal{name = "triggered",argTypeName = ""}, 
                },
                functionList = {
                    new ETypescriptFunction{signature = "restart()",returnType = "void"},
                    new ETypescriptFunction{signature = "start()",returnType = "void"},
                    new ETypescriptFunction{signature = "stop()",returnType = "void"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddDrawer 
        static private void AddDrawer() {
            EQmlFile element = new EQmlFile {
                name = "Drawer",                
                qmlFileType = QmlFileType.QtComponent,    
                extendsElementName = "Popup",
                typescriptImports = {
                    new EImport{className = "Popup",path = "./"} 
                },                
                propertyList = {
                    new EQMLFormProperty{name = "dragMargin",type = "number"},
                    new EQMLFormProperty{name = "edge",type = "number"},//ver
                    new EQMLFormProperty{name = "interactive",type = "boolean"},
                    new EQMLFormProperty{name = "position",type = "number"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddTextEdit 
        static private void AddTextEdit() {
            EQmlFile element = new EQmlFile {
                name = "TextEdit",                
                qmlFileType = QmlFileType.QtComponent,    
                extendsElementName = "Item",
                typescriptImports = {
                    new EImport{className = "Item",path = "./"} 
                },                
                propertyList = {
                    new EQMLFormProperty{name = "text",type = "string"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddTextArea 
        static private void AddTextArea() {
            EQmlFile element = new EQmlFile {
                name = "TextArea",                
                qmlFileType = QmlFileType.QtComponent,    
                extendsElementName = "TextEdit",
                typescriptImports = {
                    new EImport{className = "TextEdit",path = "./"} 
                },                
                propertyList = {
                    new EQMLFormProperty{name = "background",type = "Item"},
                },
                functionList = {
                    new ETypescriptFunction{signature = "clear()",returnType = "void"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddPositionSource 
        static private void AddPositionSource() {
            EQmlFile element = new EQmlFile {
                name = "PositionSource",                
                qmlFileType = QmlFileType.QtComponent,    
                propertyList = {
                    new EQMLFormProperty{name = "active",type = "boolean"},
                    new EQMLFormProperty{name = "nmeaSource",type = "string"},
                    new EQMLFormProperty{name = "position",type = "any"},//ver
                    new EQMLFormProperty{name = "preferredPositioningMethods",type = "any"},//ver
                    new EQMLFormProperty{name = "sourceError",type = "any"},//ver
                    new EQMLFormProperty{name = "supportedPositioningMethods",type = "any"},//ver
                    new EQMLFormProperty{name = "updateInterval",type = "number"},
                    new EQMLFormProperty{name = "valid",type = "boolean"},
                },
                signalList = {
                    new EQmlSignal{name = "updateTimeout",argTypeName = ""}, 
                },
                functionList = {
                    new ETypescriptFunction{signature = "start()",returnType = "void"},
                    new ETypescriptFunction{signature = "stop()",returnType = "void"},
                    new ETypescriptFunction{signature = "update()",returnType = "void"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        #region AddCamera
        static private void AddCamera() {
            EQmlFile element = new EQmlFile {
                name = "Camera",                
                qmlFileType = QmlFileType.QtComponent,
                propertyList = {
                    new EQMLFormProperty{name = "availability",type = "number"},
                },
                functionList = {
                    new ETypescriptFunction{signature = "start()",returnType = "void"},
                    new ETypescriptFunction{signature = "stop()",returnType = "void"},
                },
            };
            qtBasicComponents.Add(element);
        }
        #endregion
        
        //============================QML COMPONENTS ABOVE===================
        
        #region IsQtBasicElement
        static public bool IsQtBasicElement(string typeName) {
            GetQtBasicComponents();
            foreach (EQmlFile eQmlFile in qtBasicComponents) {
                if (eQmlFile.name == typeName) return true;
            }
            return false;
        }
        #endregion


        static public string FindPropertyType(string typeName, string propertyName) {//ex: typeName=TextField, 
            List<EQmlFile> qtBasicComponents=QmlBasicComponents.GetQtBasicComponents();
            foreach (EQmlFile qtBasic in qtBasicComponents) {
                if (qtBasic.name != typeName) continue;
                EQMLFormProperty propertyFound=qtBasic.propertyList.FirstOrDefault(x => x.name ==propertyName);
                if (propertyFound != null) {
                    return propertyFound.type;
                }
                //por exemplo, se nao achamos color no TextField, devemos procurar em TextInput
                EQmlFile subFile= qtBasicComponents.FirstOrDefault(x => x.name == qtBasic.extendsElementName);
                return FindPropertyType(qtBasic.extendsElementName, propertyName);
            }
            return null;
        }
        
        static public bool WriteTypescriptDefinitionsToFrontend() {            
            var list=GetQtBasicComponents();
            Logger.LogInfoIfDebugLevel(DebugLevels.Basic| DebugLevels.Files|DebugLevels.Functions|DebugLevels.All,"Writing Qt Typescript definition files to " + Globals.frontendRestInPeaceFolder );
            WriteQtEnums();
            foreach (var basicElement in list) {
                if (!DTSWriter.WriteTypescriptDefinitionToFrontEnd(basicElement)) return false;
            }
            return true;
        }

        

        static private void WriteQtEnums() {            
//            foreach (EEnumFile enumFile in eEnumFiles) {
//                QmlTSEnumWriter.WriteTypescriptEnumFile(enumFile, Globals.frontendRestInPeaceFolder,false);
//            }
        }
        
    }
}