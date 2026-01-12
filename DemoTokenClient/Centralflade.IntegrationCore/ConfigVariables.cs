using System.Security.Cryptography.X509Certificates;

namespace Centralflade.IntegrationCore
{
    // TO_BE_MODIFIED
    // All these constants can be modified to configure the client.
    // See the document "Programmers Guide - Serviceplatformen" for details.
    static class ConfigVariables
    {
        // The alias used for Serviceplatformen endpoint identity check.
        
        public const string ServiceCertificateAlias = "SP_EXTTEST_Signing_1";
        public const string ClientCertificateThumbprint = "639b5ff8e53246dd448bab496b6bbc2fb5e485fc";
                                    

        public const StoreLocation ClientCertificateStoreLocation = StoreLocation.CurrentUser;  

        public const StoreName ClientCertificateStoreName = StoreName.My;

        // Entity ID for the Serviceplatform service to fetch token for and call 
        // This ID can be found in the service contract package from the Serviceplatform as 'service.entityID' inside /sp/service.properties.
        public const string ServiceEntityId = "http://cpr.serviceplatformen.dk/service/personbasedataextended/5";

        // The STS issuer for token requests.
        public const string StsIssuer = "https://adgangsstyring.eksterntest-stoettesystemerne.dk/";
        
        // The endpoint of the STS (Secure Token Service).
        public const string StsEndpoint = "https://n2adgangsstyring.eksterntest-stoettesystemerne.dk/runtime/services/kombittrust/14/certificatemixed";

        // The alias used for STS endpoint identity check.
	    public const string StsCertificateAlias = "ADG_EXTTEST_Adgangsstyring_1";


        // SHA-1 thumbprint of the certificate used for signing by STS.
        public const string StsCertificateThumbprint = "0A:A7:A1:93:F1:8D:09:5F:7E:2C:E0:9D:89:21:78:C9:68:2B:79:24";
        
        public const StoreLocation StsCertificateStoreLocation = StoreLocation.CurrentUser;

        public const StoreName StsCertificateStoreName = StoreName.TrustedPeople;

        // The CVR of the municipality involved in the service agreement.
        // Used in the token request to STS.
        public const string Cvr = "29188459";

        // Below are some of the optional values that can be used in the CallContext.
        public const string AccountingInfo = ".Net DemoService sample call";
        public const string OnBehalfOfUser = "Centralflade";
        public const string CallersServiceCallIdentifier = "123456789-Token";
    }
}
