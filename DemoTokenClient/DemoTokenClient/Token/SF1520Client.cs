using System;
using System.IdentityModel.Tokens;
using System.Net;
using System.Net.Security;
using System.ServiceModel;
using DemoTokenClient.SF1520;

namespace DemoTokenClient.Token
{
    public class SF1520Client
    {
        public PersonLookupResponse CallPersonLookupWithToken(string cpr10, string endpointUrl = null)
        {
            // Security protocols supported by Serviceplatformen
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Optional: only for local/self-signed scenarios (do NOT use in production)
            // ServicePointManager.ServerCertificateValidationCallback += (se, cert, chain, sslerror) => true;

            var token = TokenFetcher.IssueToken(ConfigVariables.ServiceEntityId);

            // SF1520 expects PNR as 10 digits (no dash)
            var request = new PersonLookupRequest
            {
                PersonLookupRequest1 = new PersonLookupRequestType
                {
                    PNR = cpr10,
                    CallContext = GetCallContext()
                }
            };

            var channel = CreateChannel(token, endpointUrl);

            // NOTE: method name is PersonLookup on the SF1520 port
            var response = channel.PersonLookup(request);

            return response;
        }

        private PersonBaseDataExtendedPortType CreateChannel(SecurityToken token, string endpointUrl)
        {
            var client = new PersonBaseDataExtendedPortTypeClient();

            // If you need to disable revocation checks (not recommended)
            // client.ClientCredentials.ServiceCertificate.Authentication.RevocationMode =
            //     System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;

            // Important: Identity must match STS/service expected DNS identity (your ConfigVariables alias)
            EndpointIdentity identity = EndpointIdentity.CreateDnsIdentity(ConfigVariables.ServiceCertificateAlias);

            EndpointAddress endpointAddress =
                endpointUrl != null
                    ? new EndpointAddress(new Uri(endpointUrl), identity)
                    : new EndpointAddress(client.Endpoint.Address.Uri, identity);

            client.Endpoint.Address = endpointAddress;

            var certificate = CertificateLoader.LoadCertificate(
                ConfigVariables.ClientCertificateStoreName,
                ConfigVariables.ClientCertificateStoreLocation,
                ConfigVariables.ClientCertificateThumbprint);

            client.ClientCredentials.ClientCertificate.Certificate = certificate;

            // Keep signing only
            client.Endpoint.Contract.ProtectionLevel = ProtectionLevel.Sign;

            return client.ChannelFactory.CreateChannelWithIssuedToken(token);
        }

        private static CallContextType GetCallContext()
        {
            return new CallContextType()
            {
                AccountingInfo = ConfigVariables.AccountingInfo,
                OnBehalfOfUser = ConfigVariables.OnBehalfOfUser,
                CallersServiceCallIdentifier = ConfigVariables.CallersServiceCallIdentifier
            };
        }
    }
}