
using PartnerAPI;

using CallOptions = PartnerAPI.CallOptions;
using LoginResult = PartnerAPI.LoginResult;
using SessionHeader = PartnerAPI.SessionHeader;
using System.ServiceModel;
using Microsoft.AspNetCore.Server.Kestrel;
using MetadataAPI;
using System.IO.Compression;
using CustomObjects.Models;
using System.Drawing.Printing;

namespace CustomObjects.Services
{
    public class PartnerLogin
    {

        private readonly IConfiguration _configuration;

        public PartnerLogin(IConfiguration configuration)

        {
            _configuration = configuration;

        }
        /// Demonstrates how to set the LoginScopeHeader values.
        public static async void login(LogInInfo info, List<HubSpotModel> data, string nameOfObject)
        {

            string username = info.Username;
            string password = info.Password;
            string token = info.SecurityToken;


            SoapClient sc = new SoapClient(SoapClient.EndpointConfiguration.Soap);

            CallOptions callOptions = new CallOptions();

            loginResponse lr = await sc.loginAsync(null, callOptions, username, password + token);
            EndpointAddress PartnerEndpointAddress = new EndpointAddress(lr.result.metadataServerUrl);

            LoginScopeHeader lsh = new LoginScopeHeader();
            lsh.portalId = "";
            lsh.organizationId = lr.result.userInfo.organizationId;


            SessionHeader PartnerSessionHeader = new SessionHeader();
            PartnerSessionHeader.sessionId = lr.result.sessionId;
            sc.Close();
            SoapClient.EndpointConfiguration endpoint = SoapClient.EndpointConfiguration.Soap;
            SoapClient PartnerSoapClient = new SoapClient(endpoint, PartnerEndpointAddress.ToString());

            List<Metadata> objects = new List<Metadata>();

            // This could've been put in a separate function, but alas no time was had since I didn't manage to make CustomFields
            CustomObject customObject = new CustomObject();
            customObject.deploymentStatus = DeploymentStatus.Deployed;
            customObject.deploymentStatusSpecified = true;
            customObject.description = nameOfObject;
            customObject.fullName = $"{nameOfObject}__c";
            customObject.label = nameOfObject;
            customObject.pluralLabel = $"{nameOfObject}s"; ;
            customObject.sharingModel = SharingModel.ReadWrite;
            customObject.sharingModelSpecified = true;
            customObject.enableActivities = true;

            customObject.nameField = new CustomField();
            customObject.nameField.label = nameOfObject;
            customObject.nameField.length = 100;
            customObject.nameField.lengthSpecified = true;
            customObject.nameField.typeSpecified = true;

            List<CustomField> customFields = new List<CustomField>();

            foreach (HubSpotModel point in data)
            {
                CustomField field = new CustomField();


                field.unique = true;
                field.label = point.Name;
                field.length = 100;
                field.lengthSpecified = true;
                field.typeSpecified = true;
                field.description = point.Description;
                field.fullName = point.InternalName;
                field.required = false;
                field.customDataType = point.Type;
                field.externalDeveloperName = point.InternalName;

                customFields.Add(field);
            }
            
            // I've read somewhere that you can only create 10 fields at a time (that's why this is here) (it was true for CustomObjects)
            // This should add the CustomFields created above, however they do not appear on Object Manager in Salesforce
            // The only one that appears is the customObject.nameField, however I haven't found a way to make more of the fields in this manner
            customObject.fields = customFields.Take(10).ToArray();
            


            MetadataAPI.CallOptions options = new MetadataAPI.CallOptions();
            MetadataAPI.SessionHeader sessionHeader = new MetadataAPI.SessionHeader();
            MetadataAPI.AllOrNoneHeader allOrNone = new MetadataAPI.AllOrNoneHeader();

            options.client = PartnerSoapClient.ToString();
            allOrNone.allOrNone = false;

            sessionHeader.sessionId = PartnerSessionHeader.sessionId;

            MetadataPortTypeClient metadataservice = new MetadataPortTypeClient(MetadataPortTypeClient.EndpointConfiguration.Metadata, PartnerEndpointAddress); 

            dynamic results = metadataservice.createMetadata(sessionHeader, options, allOrNone, new Metadata[] { customObject });


        }



    }
}
