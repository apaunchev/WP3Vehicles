using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using VehiclesServer.Models;

namespace VehiclesServer
{
    class Server
    {
        static void Main(string[] args)
        {
            using (ServiceHost serviceHost = new ServiceHost(typeof(VehicleService)))
            {
                try
                {
                    // Open the ServiceHost to start listening for messages.
                    serviceHost.Open();

                    // List the available endpoints for info.
                    foreach (ServiceEndpoint se in serviceHost.Description.Endpoints)
                        Console.WriteLine("A: {0}, B: {1}, C: {2}", se.Address, se.Binding.Name, se.Contract.Name);

                    // The service can now be accessed.
                    Console.WriteLine("The service is ready.");
                    Console.WriteLine("Press <ENTER> to terminate service.");
                    Console.ReadLine();

                    // Close the ServiceHost.
                    serviceHost.Close();
                }
                catch (TimeoutException timeProblem)
                {
                    Console.WriteLine(timeProblem.Message);
                }
                catch (CommunicationException commProblem)
                {
                    Console.WriteLine(commProblem.Message);
                }
            }
        }
    }
}
