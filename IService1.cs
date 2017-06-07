using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ApiAiSDK.Model;
using Newtonsoft.Json.Linq;


namespace AiChatService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        //[OperationContract]
        //[WebGet(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ValidateUser/{sUserName}/{sPassword}")]
        //List<ValidateUser> ValidateUser(string sUserName, string sPassword);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ValidateUser")]
        List<ValidateUser> ValidateUser(AIResponse ai);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "GetTestResponse")]
        AiResult GetTestResponse(AIResponse ai);
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class ValidateUser
    {
        Int32 id = 0;


        [DataMember]
        public Int32 ID
        {
            get { return id; }
            set { id = value; }
        }


    }
      # region AI
    public class AiResult
    {
        string Speech = string.Empty;
        string DisplayText = string.Empty;
        string Source = string.Empty;

        public string speech
        {
            get { return Speech; }
            set { Speech = value; }
        }
        public string displayText
        {
            get { return DisplayText; }
            set { DisplayText = value; }
        }
        public string source
        {
            get { return Source; }
            set { Source = value; }
        }
        public object data
        {
            get;
            set;
        }
        public object[] contextOut
        {
            get;
            set;
        }
    }

    public class AIResponse
    {
        public string Id { get; set; }
        public DateTime timestamp { get; set; }
        public string lang { get; set; }
        public Result result { get; set; }
        public Status status { get; set; }
        public string sessionId { get; set; }
        public Fulfillment fulfillment { get; set; }
        public bool IsError
        {
            get
            {
                if (status != null && status.code.HasValue && status.code >= 400)
                {
                    return true;
                }

                return false;
            }
        }
    }
    public class Status
    {
        public int? code { get; set; }
        public string errorType { get; set; }
        public string errorDetails { get; set; }
        public string errorID { get; set; }
        public Status()
        {
        }
    }


    public class Result
    {
        String action;
        public Boolean ActionIncomplete { get; set; }
        public String Action
        {
            get
            {
                if (string.IsNullOrEmpty(action))
                {
                    return string.Empty;
                }
                return action;
            }
            set
            {
                action = value;
            }
        }

        public Dictionary<string, object> parameters { get; set; }
        public AIOutputContext[] contexts { get; set; }
        public Metadata Metadata { get; set; }
        public String resolvedQuery { get; set; }
        public Fulfillment fulfillment { get; set; }
        public string source { get; set; }
        public float score { get; set; }
        public bool hasParameters
        {
            get
            {
                return parameters != null && parameters.Count > 0;
            }
        }

        public string GetStringParameter(string name, string defaultValue = "")
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (parameters.ContainsKey(name))
            {
                return parameters[name].ToString();
            }

            return defaultValue;
        }

        public int GetIntParameter(string name, int defaultValue = 0)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (parameters.ContainsKey(name))
            {
                var parameterValue = parameters[name].ToString();
                int result;
                if (int.TryParse(parameterValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }

                float floatResult;
                if (float.TryParse(parameterValue, NumberStyles.Float, CultureInfo.InvariantCulture, out floatResult))
                {
                    result = Convert.ToInt32(floatResult);
                    return result;
                }
            }

            return defaultValue;
        }

        public float GetFloatParameter(string name, float defaultValue = 0)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (parameters.ContainsKey(name))
            {
                var parameterValue = parameters[name].ToString();
                float result;
                if (float.TryParse(parameterValue, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }
            }

            return defaultValue;
        }

        public JObject GetJsonParameter(string name, JObject defaultValue = null)
        {
            if (string.IsNullOrEmpty("name"))
            {
                throw new ArgumentNullException(name);
            }

            if (parameters.ContainsKey(name))
            {
                var parameter = parameters[name] as JObject;
                if (parameter != null)
                {
                    return parameter;
                }
            }

            return defaultValue;
        }

        public AIOutputContext GetContext(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name must be not empty", name);
            }

            return contexts.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.CurrentCultureIgnoreCase));
        }

        public Result()
        {
        }
    }

    #endregion


   
}
