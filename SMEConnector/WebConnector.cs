using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.IO;
using System.Text;
using System.Windows.Forms;
using log4net;

namespace SMEConnector
{
    public class WebConnector
    {
        private static ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public WebConnector()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);

        }

        public void SendXml(string ssid, string strxml, StringBuilder result)
        {
            //string ssid = GetSessionId();
            try
            {
                PostRequest(ssid, strxml, result);
                _log.Info(string.Format("SME response: {0}", result.ToString()));
            }
            catch (FaultException soapEx)
            {
                //keep log
                _log.Error("sendXML error", soapEx);
                if (soapEx.Message.Contains("Session ID timeout or invalid."))
                {
                    ssid = GetSessionId(); // get sessionId again
                    PostRequest(ssid, strxml, result);
                }
            }
        }

        public string GetSessionId()
        {
            string sessionId = string.Empty;
            SMEServiceReferenceLogin.AuthenticateSoapClient auth = new SMEServiceReferenceLogin.AuthenticateSoapClient();
            SMEServiceReferenceLogin.LoginInput input = new SMEServiceReferenceLogin.LoginInput
            {
                UserName = GetUserPwd("username"),
                PassWord = GetUserPwd("password")
            };
            input.UserName = "G_WSFDI";
            input.PassWord = "123";
            SMEServiceReferenceLogin.LoginOutput output = auth.Login(input);

            if (output.Success)
                sessionId = output.SessionId;

            return sessionId;
        }

        private void PostRequest(string sessionId, string xml, StringBuilder result)
        {
            SMEServiceReferenceFDI.FDISoapClient client = new SMEServiceReferenceFDI.FDISoapClient();
            using (new OperationContextScope(client.InnerChannel))
            {
                // Embeds the extracted cookie in the next web service request
                // Note that we manually have to create the request object since
                // since it doesn't exist yet at this stage
                HttpRequestMessageProperty request = new HttpRequestMessageProperty();
                request.Headers["Cookie"] = "ASP.NET_SessionId=" + sessionId.Trim();
                OperationContext.Current.OutgoingMessageProperties[
                    HttpRequestMessageProperty.Name] = request;

                SMEServiceReferenceFDI.FdiResult rsp = client.FdiIrT(xml.Trim());
                //result = rsp.Result;
                result.Append(rsp.Result);
            }
        }

        private string GetUserPwd(string key)
        {
            XmlDocument doc = new XmlDocument();
            if (File.Exists("username.config"))
            {
                doc.Load("username.config");

                XmlNodeList nodelist = doc.GetElementsByTagName("UserPwd");
                foreach (XmlNode node in nodelist)
                {
                    XmlElement xe = (XmlElement)node; //将子节点类型转换为xmlelement类型
                    if (!xe.GetAttribute(key).Equals(""))
                    {
                        return xe.GetAttribute(key);
                    }
                }
            }
            return "";
        }
    }
}
