﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChannelAdvisorAccess.AdminService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://api.channeladvisor.com/webservices/", ConfigurationName="AdminService.AdminServiceSoap")]
    public interface AdminServiceSoap {
        
        // CODEGEN: Generating message contract since message GetAuthorizationListRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://api.channeladvisor.com/webservices/GetAuthorizationList", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        ChannelAdvisorAccess.AdminService.GetAuthorizationListResponse GetAuthorizationList(ChannelAdvisorAccess.AdminService.GetAuthorizationListRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://api.channeladvisor.com/webservices/GetAuthorizationList", ReplyAction="*")]
        System.Threading.Tasks.Task<ChannelAdvisorAccess.AdminService.GetAuthorizationListResponse> GetAuthorizationListAsync(ChannelAdvisorAccess.AdminService.GetAuthorizationListRequest request);
        
        // CODEGEN: Generating message contract since message RequestAccessRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://api.channeladvisor.com/webservices/RequestAccess", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        ChannelAdvisorAccess.AdminService.RequestAccessResponse RequestAccess(ChannelAdvisorAccess.AdminService.RequestAccessRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://api.channeladvisor.com/webservices/RequestAccess", ReplyAction="*")]
        System.Threading.Tasks.Task<ChannelAdvisorAccess.AdminService.RequestAccessResponse> RequestAccessAsync(ChannelAdvisorAccess.AdminService.RequestAccessRequest request);
        
        // CODEGEN: Generating message contract since message PingRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://api.channeladvisor.com/webservices/Ping", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        ChannelAdvisorAccess.AdminService.PingResponse Ping(ChannelAdvisorAccess.AdminService.PingRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://api.channeladvisor.com/webservices/Ping", ReplyAction="*")]
        System.Threading.Tasks.Task<ChannelAdvisorAccess.AdminService.PingResponse> PingAsync(ChannelAdvisorAccess.AdminService.PingRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.channeladvisor.com/webservices/")]
    public partial class APICredentials : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string developerKeyField;
        
        private string passwordField;
        
        private System.Xml.XmlAttribute[] anyAttrField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string DeveloperKey {
            get {
                return this.developerKeyField;
            }
            set {
                this.developerKeyField = value;
                this.RaisePropertyChanged("DeveloperKey");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string Password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
                this.RaisePropertyChanged("Password");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr {
            get {
                return this.anyAttrField;
            }
            set {
                this.anyAttrField = value;
                this.RaisePropertyChanged("AnyAttr");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.channeladvisor.com/webservices/")]
    public partial class APIResultOfString : object, System.ComponentModel.INotifyPropertyChanged {
        
        private ResultStatus statusField;
        
        private int messageCodeField;
        
        private string messageField;
        
        private string dataField;
        
        private string resultDataField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public ResultStatus Status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
                this.RaisePropertyChanged("Status");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public int MessageCode {
            get {
                return this.messageCodeField;
            }
            set {
                this.messageCodeField = value;
                this.RaisePropertyChanged("MessageCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string Message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
                this.RaisePropertyChanged("Message");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string Data {
            get {
                return this.dataField;
            }
            set {
                this.dataField = value;
                this.RaisePropertyChanged("Data");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string ResultData {
            get {
                return this.resultDataField;
            }
            set {
                this.resultDataField = value;
                this.RaisePropertyChanged("ResultData");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.channeladvisor.com/webservices/")]
    public enum ResultStatus {
        
        /// <remarks/>
        Success,
        
        /// <remarks/>
        Failure,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.channeladvisor.com/webservices/")]
    public partial class APIResultOfBoolean : object, System.ComponentModel.INotifyPropertyChanged {
        
        private ResultStatus statusField;
        
        private int messageCodeField;
        
        private string messageField;
        
        private string dataField;
        
        private bool resultDataField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public ResultStatus Status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
                this.RaisePropertyChanged("Status");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public int MessageCode {
            get {
                return this.messageCodeField;
            }
            set {
                this.messageCodeField = value;
                this.RaisePropertyChanged("MessageCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string Message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
                this.RaisePropertyChanged("Message");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string Data {
            get {
                return this.dataField;
            }
            set {
                this.dataField = value;
                this.RaisePropertyChanged("Data");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public bool ResultData {
            get {
                return this.resultDataField;
            }
            set {
                this.resultDataField = value;
                this.RaisePropertyChanged("ResultData");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.channeladvisor.com/webservices/")]
    public partial class AuthorizationResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string accountIDField;
        
        private int localIDField;
        
        private string accountNameField;
        
        private string accountTypeField;
        
        private string resourceNameField;
        
        private string statusField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string AccountID {
            get {
                return this.accountIDField;
            }
            set {
                this.accountIDField = value;
                this.RaisePropertyChanged("AccountID");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public int LocalID {
            get {
                return this.localIDField;
            }
            set {
                this.localIDField = value;
                this.RaisePropertyChanged("LocalID");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string AccountName {
            get {
                return this.accountNameField;
            }
            set {
                this.accountNameField = value;
                this.RaisePropertyChanged("AccountName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string AccountType {
            get {
                return this.accountTypeField;
            }
            set {
                this.accountTypeField = value;
                this.RaisePropertyChanged("AccountType");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string ResourceName {
            get {
                return this.resourceNameField;
            }
            set {
                this.resourceNameField = value;
                this.RaisePropertyChanged("ResourceName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string Status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
                this.RaisePropertyChanged("Status");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.channeladvisor.com/webservices/")]
    public partial class APIResultOfArrayOfAuthorizationResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private ResultStatus statusField;
        
        private int messageCodeField;
        
        private string messageField;
        
        private string dataField;
        
        private AuthorizationResponse[] resultDataField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public ResultStatus Status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
                this.RaisePropertyChanged("Status");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public int MessageCode {
            get {
                return this.messageCodeField;
            }
            set {
                this.messageCodeField = value;
                this.RaisePropertyChanged("MessageCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string Message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
                this.RaisePropertyChanged("Message");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string Data {
            get {
                return this.dataField;
            }
            set {
                this.dataField = value;
                this.RaisePropertyChanged("Data");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Order=4)]
        public AuthorizationResponse[] ResultData {
            get {
                return this.resultDataField;
            }
            set {
                this.resultDataField = value;
                this.RaisePropertyChanged("ResultData");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetAuthorizationList", WrapperNamespace="http://api.channeladvisor.com/webservices/", IsWrapped=true)]
    public partial class GetAuthorizationListRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://api.channeladvisor.com/webservices/")]
        public ChannelAdvisorAccess.AdminService.APICredentials APICredentials;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.channeladvisor.com/webservices/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string localID;
        
        public GetAuthorizationListRequest() {
        }
        
        public GetAuthorizationListRequest(ChannelAdvisorAccess.AdminService.APICredentials APICredentials, string localID) {
            this.APICredentials = APICredentials;
            this.localID = localID;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="GetAuthorizationListResponse", WrapperNamespace="http://api.channeladvisor.com/webservices/", IsWrapped=true)]
    public partial class GetAuthorizationListResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.channeladvisor.com/webservices/", Order=0)]
        public ChannelAdvisorAccess.AdminService.APIResultOfArrayOfAuthorizationResponse GetAuthorizationListResult;
        
        public GetAuthorizationListResponse() {
        }
        
        public GetAuthorizationListResponse(ChannelAdvisorAccess.AdminService.APIResultOfArrayOfAuthorizationResponse GetAuthorizationListResult) {
            this.GetAuthorizationListResult = GetAuthorizationListResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RequestAccess", WrapperNamespace="http://api.channeladvisor.com/webservices/", IsWrapped=true)]
    public partial class RequestAccessRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://api.channeladvisor.com/webservices/")]
        public ChannelAdvisorAccess.AdminService.APICredentials APICredentials;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.channeladvisor.com/webservices/", Order=0)]
        public int localID;
        
        public RequestAccessRequest() {
        }
        
        public RequestAccessRequest(ChannelAdvisorAccess.AdminService.APICredentials APICredentials, int localID) {
            this.APICredentials = APICredentials;
            this.localID = localID;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="RequestAccessResponse", WrapperNamespace="http://api.channeladvisor.com/webservices/", IsWrapped=true)]
    public partial class RequestAccessResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.channeladvisor.com/webservices/", Order=0)]
        public ChannelAdvisorAccess.AdminService.APIResultOfBoolean RequestAccessResult;
        
        public RequestAccessResponse() {
        }
        
        public RequestAccessResponse(ChannelAdvisorAccess.AdminService.APIResultOfBoolean RequestAccessResult) {
            this.RequestAccessResult = RequestAccessResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Ping", WrapperNamespace="http://api.channeladvisor.com/webservices/", IsWrapped=true)]
    public partial class PingRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://api.channeladvisor.com/webservices/")]
        public ChannelAdvisorAccess.AdminService.APICredentials APICredentials;
        
        public PingRequest() {
        }
        
        public PingRequest(ChannelAdvisorAccess.AdminService.APICredentials APICredentials) {
            this.APICredentials = APICredentials;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="PingResponse", WrapperNamespace="http://api.channeladvisor.com/webservices/", IsWrapped=true)]
    public partial class PingResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.channeladvisor.com/webservices/", Order=0)]
        public ChannelAdvisorAccess.AdminService.APIResultOfString PingResult;
        
        public PingResponse() {
        }
        
        public PingResponse(ChannelAdvisorAccess.AdminService.APIResultOfString PingResult) {
            this.PingResult = PingResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface AdminServiceSoapChannel : ChannelAdvisorAccess.AdminService.AdminServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AdminServiceSoapClient : System.ServiceModel.ClientBase<ChannelAdvisorAccess.AdminService.AdminServiceSoap>, ChannelAdvisorAccess.AdminService.AdminServiceSoap {
        
        public AdminServiceSoapClient() {
        }
        
        public AdminServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AdminServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AdminServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AdminServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ChannelAdvisorAccess.AdminService.GetAuthorizationListResponse ChannelAdvisorAccess.AdminService.AdminServiceSoap.GetAuthorizationList(ChannelAdvisorAccess.AdminService.GetAuthorizationListRequest request) {
            return base.Channel.GetAuthorizationList(request);
        }
        
        public ChannelAdvisorAccess.AdminService.APIResultOfArrayOfAuthorizationResponse GetAuthorizationList(ChannelAdvisorAccess.AdminService.APICredentials APICredentials, string localID) {
            ChannelAdvisorAccess.AdminService.GetAuthorizationListRequest inValue = new ChannelAdvisorAccess.AdminService.GetAuthorizationListRequest();
            inValue.APICredentials = APICredentials;
            inValue.localID = localID;
            ChannelAdvisorAccess.AdminService.GetAuthorizationListResponse retVal = ((ChannelAdvisorAccess.AdminService.AdminServiceSoap)(this)).GetAuthorizationList(inValue);
            return retVal.GetAuthorizationListResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ChannelAdvisorAccess.AdminService.GetAuthorizationListResponse> ChannelAdvisorAccess.AdminService.AdminServiceSoap.GetAuthorizationListAsync(ChannelAdvisorAccess.AdminService.GetAuthorizationListRequest request) {
            return base.Channel.GetAuthorizationListAsync(request);
        }
        
        public System.Threading.Tasks.Task<ChannelAdvisorAccess.AdminService.GetAuthorizationListResponse> GetAuthorizationListAsync(ChannelAdvisorAccess.AdminService.APICredentials APICredentials, string localID) {
            ChannelAdvisorAccess.AdminService.GetAuthorizationListRequest inValue = new ChannelAdvisorAccess.AdminService.GetAuthorizationListRequest();
            inValue.APICredentials = APICredentials;
            inValue.localID = localID;
            return ((ChannelAdvisorAccess.AdminService.AdminServiceSoap)(this)).GetAuthorizationListAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ChannelAdvisorAccess.AdminService.RequestAccessResponse ChannelAdvisorAccess.AdminService.AdminServiceSoap.RequestAccess(ChannelAdvisorAccess.AdminService.RequestAccessRequest request) {
            return base.Channel.RequestAccess(request);
        }
        
        public ChannelAdvisorAccess.AdminService.APIResultOfBoolean RequestAccess(ChannelAdvisorAccess.AdminService.APICredentials APICredentials, int localID) {
            ChannelAdvisorAccess.AdminService.RequestAccessRequest inValue = new ChannelAdvisorAccess.AdminService.RequestAccessRequest();
            inValue.APICredentials = APICredentials;
            inValue.localID = localID;
            ChannelAdvisorAccess.AdminService.RequestAccessResponse retVal = ((ChannelAdvisorAccess.AdminService.AdminServiceSoap)(this)).RequestAccess(inValue);
            return retVal.RequestAccessResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ChannelAdvisorAccess.AdminService.RequestAccessResponse> ChannelAdvisorAccess.AdminService.AdminServiceSoap.RequestAccessAsync(ChannelAdvisorAccess.AdminService.RequestAccessRequest request) {
            return base.Channel.RequestAccessAsync(request);
        }
        
        public System.Threading.Tasks.Task<ChannelAdvisorAccess.AdminService.RequestAccessResponse> RequestAccessAsync(ChannelAdvisorAccess.AdminService.APICredentials APICredentials, int localID) {
            ChannelAdvisorAccess.AdminService.RequestAccessRequest inValue = new ChannelAdvisorAccess.AdminService.RequestAccessRequest();
            inValue.APICredentials = APICredentials;
            inValue.localID = localID;
            return ((ChannelAdvisorAccess.AdminService.AdminServiceSoap)(this)).RequestAccessAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ChannelAdvisorAccess.AdminService.PingResponse ChannelAdvisorAccess.AdminService.AdminServiceSoap.Ping(ChannelAdvisorAccess.AdminService.PingRequest request) {
            return base.Channel.Ping(request);
        }
        
        public ChannelAdvisorAccess.AdminService.APIResultOfString Ping(ChannelAdvisorAccess.AdminService.APICredentials APICredentials) {
            ChannelAdvisorAccess.AdminService.PingRequest inValue = new ChannelAdvisorAccess.AdminService.PingRequest();
            inValue.APICredentials = APICredentials;
            ChannelAdvisorAccess.AdminService.PingResponse retVal = ((ChannelAdvisorAccess.AdminService.AdminServiceSoap)(this)).Ping(inValue);
            return retVal.PingResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<ChannelAdvisorAccess.AdminService.PingResponse> ChannelAdvisorAccess.AdminService.AdminServiceSoap.PingAsync(ChannelAdvisorAccess.AdminService.PingRequest request) {
            return base.Channel.PingAsync(request);
        }
        
        public System.Threading.Tasks.Task<ChannelAdvisorAccess.AdminService.PingResponse> PingAsync(ChannelAdvisorAccess.AdminService.APICredentials APICredentials) {
            ChannelAdvisorAccess.AdminService.PingRequest inValue = new ChannelAdvisorAccess.AdminService.PingRequest();
            inValue.APICredentials = APICredentials;
            return ((ChannelAdvisorAccess.AdminService.AdminServiceSoap)(this)).PingAsync(inValue);
        }
    }
}
