// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 3.3.0.20459
//    <NameSpace>OSGeo.MapGuide.ObjectModels.Common</NameSpace><Collection>BindingList</Collection><codeType>CSharp</codeType><EnableDataBinding>True</EnableDataBinding><EnableLasyLoading>False</EnableLasyLoading><HidePrivateFieldInIDE>True</HidePrivateFieldInIDE><EnableSummaryComment>True</EnableSummaryComment><IncludeSerializeMethod>True</IncludeSerializeMethod><UseBaseClass>False</UseBaseClass><GenerateCloneMethod>True</GenerateCloneMethod><GenerateDataContracts>False</GenerateDataContracts><CodeBaseTag>Net20</CodeBaseTag><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><GenerateXMLAttributes>True</GenerateXMLAttributes><AutomaticProperties>False</AutomaticProperties><DisableDebug>False</DisableDebug><CustomUsings></CustomUsings><ExcludeIncludedTypes>True</ExcludeIncludedTypes><EnableInitializeFields>True</EnableInitializeFields>
//  </auto-generated>
// ------------------------------------------------------------------------------
namespace OSGeo.MapGuide.ObjectModels.Common {
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;
    using System.IO;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2Code", "3.3.0.20460")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=true)]
    public partial class ResourceSecurityType : System.ComponentModel.INotifyPropertyChanged {
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool inheritedField;
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        private ResourceSecurityTypeUsers usersField;
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        private ResourceSecurityTypeGroups groupsField;
        
        private static System.Xml.Serialization.XmlSerializer serializer;
        
        /// <summary>
        /// ResourceSecurityType class constructor
        /// </summary>
        public ResourceSecurityType() {
            this.groupsField = new ResourceSecurityTypeGroups();
            this.usersField = new ResourceSecurityTypeUsers();
        }
        
        public bool Inherited {
            get {
                return this.inheritedField;
            }
            set {
                if ((inheritedField.Equals(value) != true)) {
                    this.inheritedField = value;
                    this.OnPropertyChanged("Inherited");
                }
            }
        }
        
        public ResourceSecurityTypeUsers Users {
            get {
                return this.usersField;
            }
            set {
                if ((this.usersField != null)) {
                    if ((usersField.Equals(value) != true)) {
                        this.usersField = value;
                        this.OnPropertyChanged("Users");
                    }
                }
                else {
                    this.usersField = value;
                    this.OnPropertyChanged("Users");
                }
            }
        }
        
        public ResourceSecurityTypeGroups Groups {
            get {
                return this.groupsField;
            }
            set {
                if ((this.groupsField != null)) {
                    if ((groupsField.Equals(value) != true)) {
                        this.groupsField = value;
                        this.OnPropertyChanged("Groups");
                    }
                }
                else {
                    this.groupsField = value;
                    this.OnPropertyChanged("Groups");
                }
            }
        }
        
        private static System.Xml.Serialization.XmlSerializer Serializer {
            get {
                if ((serializer == null)) {
                    serializer = new System.Xml.Serialization.XmlSerializer(typeof(ResourceSecurityType));
                }
                return serializer;
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        public virtual void OnPropertyChanged(string info) {
            System.ComponentModel.PropertyChangedEventHandler handler = this.PropertyChanged;
            if ((handler != null)) {
                handler(this, new System.ComponentModel.PropertyChangedEventArgs(info));
            }
        }
        
        #region Serialize/Deserialize
        /// <summary>
        /// Serializes current ResourceSecurityType object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public virtual string Serialize() {
            System.IO.StreamReader streamReader = null;
            System.IO.MemoryStream memoryStream = null;
            try {
                memoryStream = new System.IO.MemoryStream();
                Serializer.Serialize(memoryStream, this);
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                streamReader = new System.IO.StreamReader(memoryStream);
                return streamReader.ReadToEnd();
            }
            finally {
                if ((streamReader != null)) {
                    streamReader.Dispose();
                }
                if ((memoryStream != null)) {
                    memoryStream.Dispose();
                }
            }
        }
        
        /// <summary>
        /// Deserializes workflow markup into an ResourceSecurityType object
        /// </summary>
        /// <param name="xml">string workflow markup to deserialize</param>
        /// <param name="obj">Output ResourceSecurityType object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string xml, out ResourceSecurityType obj, out System.Exception exception) {
            exception = null;
            obj = default(ResourceSecurityType);
            try {
                obj = Deserialize(xml);
                return true;
            }
            catch (System.Exception ex) {
                exception = ex;
                return false;
            }
        }
        
        public static bool Deserialize(string xml, out ResourceSecurityType obj) {
            System.Exception exception = null;
            return Deserialize(xml, out obj, out exception);
        }
        
        public static ResourceSecurityType Deserialize(string xml) {
            System.IO.StringReader stringReader = null;
            try {
                stringReader = new System.IO.StringReader(xml);
                return ((ResourceSecurityType)(Serializer.Deserialize(System.Xml.XmlReader.Create(stringReader))));
            }
            finally {
                if ((stringReader != null)) {
                    stringReader.Dispose();
                }
            }
        }
        
        /// <summary>
        /// Serializes current ResourceSecurityType object into file
        /// </summary>
        /// <param name="fileName">full path of outupt xml file</param>
        /// <param name="exception">output Exception value if failed</param>
        /// <returns>true if can serialize and save into file; otherwise, false</returns>
        public virtual bool SaveToFile(string fileName, out System.Exception exception) {
            exception = null;
            try {
                SaveToFile(fileName);
                return true;
            }
            catch (System.Exception e) {
                exception = e;
                return false;
            }
        }
        
        public virtual void SaveToFile(string fileName) {
            System.IO.StreamWriter streamWriter = null;
            try {
                string xmlString = Serialize();
                System.IO.FileInfo xmlFile = new System.IO.FileInfo(fileName);
                streamWriter = xmlFile.CreateText();
                streamWriter.WriteLine(xmlString);
                streamWriter.Close();
            }
            finally {
                if ((streamWriter != null)) {
                    streamWriter.Dispose();
                }
            }
        }
        
        /// <summary>
        /// Deserializes xml markup from file into an ResourceSecurityType object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output ResourceSecurityType object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this XmlSerializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, out ResourceSecurityType obj, out System.Exception exception) {
            exception = null;
            obj = default(ResourceSecurityType);
            try {
                obj = LoadFromFile(fileName);
                return true;
            }
            catch (System.Exception ex) {
                exception = ex;
                return false;
            }
        }
        
        public static bool LoadFromFile(string fileName, out ResourceSecurityType obj) {
            System.Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }
        
        public static ResourceSecurityType LoadFromFile(string fileName) {
            System.IO.FileStream file = null;
            System.IO.StreamReader sr = null;
            try {
                file = new System.IO.FileStream(fileName, FileMode.Open, FileAccess.Read);
                sr = new System.IO.StreamReader(file);
                string xmlString = sr.ReadToEnd();
                sr.Close();
                file.Close();
                return Deserialize(xmlString);
            }
            finally {
                if ((file != null)) {
                    file.Dispose();
                }
                if ((sr != null)) {
                    sr.Dispose();
                }
            }
        }
        #endregion
        
        #region Clone method
        /// <summary>
        /// Create a clone of this ResourceSecurityType object
        /// </summary>
        public virtual ResourceSecurityType Clone() {
            return ((ResourceSecurityType)(this.MemberwiseClone()));
        }
        #endregion
    }
}