using System;
using DemoTokenClient.SF1520;

namespace DemoTokenClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Enter CPR (PNR) (10 digits), or 'q' to exit:");
                var pnr = Console.ReadLine();
                if (pnr == null || pnr.Trim().Equals("q", StringComparison.OrdinalIgnoreCase))
                    return;

                pnr = pnr.Trim();

                // IMPORTANT: endpoint name must match the <endpoint name="..."> in App.config
                var client = new PersonBaseDataExtendedPortTypeClient("PersonBaseDataExtended_v5");

                var req = new PersonLookupRequestType
                {
                    PNR = pnr
                };

                var res = client.PersonLookup(req);

                Console.WriteLine("OK - SOAP call succeeded.");
                Console.WriteLine("Response type: " + (res?.GetType().FullName ?? "null"));

                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
        }
    }
}