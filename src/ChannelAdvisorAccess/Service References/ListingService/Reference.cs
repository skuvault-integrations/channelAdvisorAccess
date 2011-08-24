﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.468
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChannelAdvisorAccess.ListingService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://api.channeladvisor.com/webservices/", ConfigurationName="ListingService.ListingServiceSoap")]
    public interface ListingServiceSoap {
        
        // CODEGEN: Generating message contract since message WithdrawListingsRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://api.channeladvisor.com/webservices/WithdrawListings", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        ChannelAdvisorAccess.ListingService.WithdrawListingsResponse WithdrawListings(ChannelAdvisorAccess.ListingService.WithdrawListingsRequest request);
        
        // CODEGEN: Generating message contract since message PingRequest has headers
        [System.ServiceModel.OperationContractAttribute(Action="http://api.channeladvisor.com/webservices/Ping", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        ChannelAdvisorAccess.ListingService.PingResponse Ping(ChannelAdvisorAccess.ListingService.PingRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.channeladvisor.com/webservices/")]
    public enum ResultStatus {
        
        /// <remarks/>
        Success,
        
        /// <remarks/>
        Failure,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.channeladvisor.com/webservices/")]
    public partial class APIResultOfInt32 : object, System.ComponentModel.INotifyPropertyChanged {
        
        private ResultStatus statusField;
        
        private int messageCodeField;
        
        private string messageField;
        
        private string dataField;
        
        private int resultDataField;
        
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
        public int ResultData {
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://api.channeladvisor.com/webservices/")]
    public enum WithdrawReason {
        
        /// <remarks/>
        ItemWasLostOrBroken,
        
        /// <remarks/>
        ItemNoLongerAvailableForSale,
        
        /// <remarks/>
        ErrorInMinimumBidOrReserve,
        
        /// <remarks/>
        ErrorInTheListing,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="WithdrawListings", WrapperNamespace="http://api.channeladvisor.com/webservices/", IsWrapped=true)]
    public partial class WithdrawListingsRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://api.channeladvisor.com/webservices/")]
        public ChannelAdvisorAccess.ListingService.APICredentials APICredentials;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.channeladvisor.com/webservices/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string accountID;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.channeladvisor.com/webservices/", Order=1)]
        public string[] skuList;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.channeladvisor.com/webservices/", Order=2)]
        public string[] listingIDList;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.channeladvisor.com/webservices/", Order=3)]
        public ChannelAdvisorAccess.ListingService.WithdrawReason withdrawReason;
        
        public WithdrawListingsRequest() {
        }
        
        public WithdrawListingsRequest(ChannelAdvisorAccess.ListingService.APICredentials APICredentials, string accountID, string[] skuList, string[] listingIDList, ChannelAdvisorAccess.ListingService.WithdrawReason withdrawReason) {
            this.APICredentials = APICredentials;
            this.accountID = accountID;
            this.skuList = skuList;
            this.listingIDList = listingIDList;
            this.withdrawReason = withdrawReason;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="WithdrawListingsResponse", WrapperNamespace="http://api.channeladvisor.com/webservices/", IsWrapped=true)]
    public partial class WithdrawListingsResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.channeladvisor.com/webservices/", Order=0)]
        public ChannelAdvisorAccess.ListingService.APIResultOfInt32 WithdrawListingsResult;
        
        public WithdrawListingsResponse() {
        }
        
        public WithdrawListingsResponse(ChannelAdvisorAccess.ListingService.APIResultOfInt32 WithdrawListingsResult) {
            this.WithdrawListingsResult = WithdrawListingsResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="Ping", WrapperNamespace="http://api.channeladvisor.com/webservices/", IsWrapped=true)]
    public partial class PingRequest {
        
        [System.ServiceModel.MessageHeaderAttribute(Namespace="http://api.channeladvisor.com/webservices/")]
        public ChannelAdvisorAccess.ListingService.APICredentials APICredentials;
        
        public PingRequest() {
        }
        
        public PingRequest(ChannelAdvisorAccess.ListingService.APICredentials APICredentials) {
            this.APICredentials = APICredentials;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="PingResponse", WrapperNamespace="http://api.channeladvisor.com/webservices/", IsWrapped=true)]
    public partial class PingResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://api.channeladvisor.com/webservices/", Order=0)]
        public ChannelAdvisorAccess.ListingService.APIResultOfString PingResult;
        
        public PingResponse() {
        }
        
        public PingResponse(ChannelAdvisorAccess.ListingService.APIResultOfString PingResult) {
            this.PingResult = PingResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ListingServiceSoapChannel : ChannelAdvisorAccess.ListingService.ListingServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ListingServiceSoapClient : System.ServiceModel.ClientBase<ChannelAdvisorAccess.ListingService.ListingServiceSoap>, ChannelAdvisorAccess.ListingService.ListingServiceSoap {
        
        public ListingServiceSoapClient() {
        }
        
        public ListingServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ListingServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ListingServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ListingServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ChannelAdvisorAccess.ListingService.WithdrawListingsResponse ChannelAdvisorAccess.ListingService.ListingServiceSoap.WithdrawListings(ChannelAdvisorAccess.ListingService.WithdrawListingsRequest request) {
            return base.Channel.WithdrawListings(request);
        }
        
        public ChannelAdvisorAccess.ListingService.APIResultOfInt32 WithdrawListings(ChannelAdvisorAccess.ListingService.APICredentials APICredentials, string accountID, string[] skuList, string[] listingIDList, ChannelAdvisorAccess.ListingService.WithdrawReason withdrawReason) {
            ChannelAdvisorAccess.ListingService.WithdrawListingsRequest inValue = new ChannelAdvisorAccess.ListingService.WithdrawListingsRequest();
            inValue.APICredentials = APICredentials;
            inValue.accountID = accountID;
            inValue.skuList = skuList;
            inValue.listingIDList = listingIDList;
            inValue.withdrawReason = withdrawReason;
            ChannelAdvisorAccess.ListingService.WithdrawListingsResponse retVal = ((ChannelAdvisorAccess.ListingService.ListingServiceSoap)(this)).WithdrawListings(inValue);
            return retVal.WithdrawListingsResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        ChannelAdvisorAccess.ListingService.PingResponse ChannelAdvisorAccess.ListingService.ListingServiceSoap.Ping(ChannelAdvisorAccess.ListingService.PingRequest request) {
            return base.Channel.Ping(request);
        }
        
        public ChannelAdvisorAccess.ListingService.APIResultOfString Ping(ChannelAdvisorAccess.ListingService.APICredentials APICredentials) {
            ChannelAdvisorAccess.ListingService.PingRequest inValue = new ChannelAdvisorAccess.ListingService.PingRequest();
            inValue.APICredentials = APICredentials;
            ChannelAdvisorAccess.ListingService.PingResponse retVal = ((ChannelAdvisorAccess.ListingService.ListingServiceSoap)(this)).Ping(inValue);
            return retVal.PingResult;
        }
    }
}
