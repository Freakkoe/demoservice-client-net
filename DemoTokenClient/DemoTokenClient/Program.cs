using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using DemoTokenClient.Token;

namespace DemoTokenClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Force TLS 1.2 (Serviceplatformen)
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            try
            {
                // Default SF1520 endpoint (exttest)
                string endpointUrl = "https://exttest.serviceplatformen.dk/service/CPR/PersonBaseDataExtended/5";

                string cpr10;
                bool pressButtonForExit = true;

                // Args usage:
                //   1 arg: CPR
                //   2 args: CPR + endpointUrl
                if (args.Length == 1 || args.Length == 2)
                {
                    cpr10 = args[0];
                    pressButtonForExit = false;

                    if (args.Length == 2)
                        endpointUrl = args[1];
                }
                else
                {
                    Console.WriteLine("Enter CPR (10 digits, no dash), e.g. 2905690000:");
                    cpr10 = Console.ReadLine();
                }

                // Normalize CPR input
                cpr10 = (cpr10 ?? "").Trim().Replace("-", "");
                if (cpr10.Length != 10)
                {
                    Console.WriteLine("CPR must be exactly 10 digits (no dash).");
                    ExitIfNeeded(pressButtonForExit);
                    return;
                }

                Console.WriteLine();
                Console.WriteLine("Calling SF1520 PersonBaseDataExtended (v5) with token...");
                Console.WriteLine("Endpoint: " + endpointUrl);
                Console.WriteLine();

                var client = new SF1520Client();
                var response = client.CallPersonLookupWithToken(cpr10, endpointUrl);

                Console.WriteLine("✅ Call succeeded. Response object received.");

                // Serialize to XML
                string xml = SerializeToXml(response);

                // Save full XML to file (much easier to inspect than console spam)
                string outFile = Path.Combine(Environment.CurrentDirectory, "sf1520-response.xml");
                File.WriteAllText(outFile, xml, Encoding.UTF8);

                Console.WriteLine("✅ Saved full response to: " + outFile);

                // Print a short preview to console (first 80 lines)
                Console.WriteLine();
                Console.WriteLine("--- XML PREVIEW (first lines) ---");

                PrintFirstLines(xml, 80);

                Console.WriteLine("--- END PREVIEW ---");

                ExitIfNeeded(pressButtonForExit);
            }
            catch (Exception exc)
            {
                Console.WriteLine("❌ Error:");
                Console.WriteLine(exc);

                // Help hint for the common test error
                if (exc.Message != null && exc.Message.IndexOf("PNR not found", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Tip: Use a CPR that exists in the exttest dataset (CPR 'arketyper').");
                }

                ExitIfNeeded(true);
            }
        }

        private static void ExitIfNeeded(bool pressButtonForExit)
        {
            if (!pressButtonForExit) return;

            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static string SerializeToXml<T>(T obj)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var sw = new StringWriter())
                {
                    serializer.Serialize(sw, obj);
                    return sw.ToString();
                }
            }
            catch (Exception ex)
            {
                return "Unable to serialize: " + ex.Message;
            }
        }

        private static void PrintFirstLines(string text, int maxLines)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("(empty)");
                return;
            }

            using (var sr = new StringReader(text))
            {
                string line;
                int count = 0;

                while (count < maxLines && (line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                    count++;
                }

                if (sr.ReadLine() != null)
                {
                    Console.WriteLine("... (truncated, see sf1520-response.xml for full output)");
                }
            }
        }
    }
}