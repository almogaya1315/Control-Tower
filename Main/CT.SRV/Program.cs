using CT.Contracts.SimOperationContracts;
using CT.SVC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace CT.SRV
{
    public class Program
    {
        static void Main(string[] args)
        {
            string SimServiceAdress = "http://localhost:4767/Services/SimService.svc";

            using (var SimServicesHost = new ServiceHost(typeof(SimService), new Uri(SimServiceAdress)))
            {
                try
                {
                    SimServicesHost.AddServiceEndpoint(typeof(ISimService), new WSDualHttpBinding(), SimServiceAdress);
                    SimServicesHost.Open();
                    Console.WriteLine("Simulation Service is running...");
                    Console.ReadKey();
                }
                catch (Exception e)
                {
                    ServiceEndpoint sep = SimServicesHost.Description.Endpoints.ElementAt(0);
                    string st = SimServicesHost.Description.ServiceType.ToString();
                    string error = " failed to open due to " + e.Message;
                    SimServicesHost.Abort();
                    throw new FaultException(st + " at " + sep + error);
                }
            }
        }
    }
}
