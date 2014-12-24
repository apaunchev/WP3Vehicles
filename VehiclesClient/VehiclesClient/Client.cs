using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace VehiclesClient
{
    class Client
    {
        static void Main(string[] args)
        {
            try
            {
                // Initialise a VehicleServiceClient object.
                VehicleReference.VehicleServiceClient vc = new VehicleReference.VehicleServiceClient();

                // Get the student ID, which is hard-coded on the server.
                Console.WriteLine("Student ID: " + vc.getStudentId());

                String menu = "\nEnter:\n" +
                    "1 to print student ID;\n" +
                    "2 to get the number of vehicles in stock;\n" +
                    "3 to get all vehicle listings;\n" +
                    "4 to get vehicle listings by price range;\n" +
                    "5 to get vehicle listings by colour;\n" +
                    "6 to mark a vehicle listing as sold;\n" +
                    "7 to create a new vehicle listing;\n" +
                    "8 to delete a vehicle listing;\n" +
                    "0 to quit.";

                // Print the menu.
                Console.WriteLine(menu);
                Console.Write("> ");

                // Prompt for user input. Will throw FormatException if not int.
                int entry = int.Parse(Console.ReadLine());

                // Initialise variables for later use.
                List<VehicleReference.VehicleListing> vehicles;
                int vehicleId;
                string lineFormat;

                // Quit program on 0.
                while (entry != 0)
                {
                    switch (entry)
                    {
                        case 1:
                            // Print student ID.
                            Console.WriteLine("Student ID: " + vc.getStudentId());

                            break;

                        case 2:
                            // Print number of vehicles in the database.
                            Console.WriteLine("Number of vehicles currently in stock: " + vc.getListingsCount());
                            
                            break;

                        case 3:
                            // Get all vehicle listings, convert them to a list and store them in the vehicles variable.
                            vehicles = vc.getAllListings().ToList();

                            // Display message if no vehicles are found.
                            if (vehicles.Count == 0)
                            {
                                Console.WriteLine("No listings found.");
                            }
                            else
                            {
                                // Format the output line by specifying the width of each string.
                                // Not flexible, but works for the purposes of the coursework.
                                lineFormat = "{0,2} {1,10} {2,10} {3,10} {4,5} {5,15} {6,15} {7,15}";
                                Console.WriteLine(String.Format(lineFormat, "ID", "Make", "Model", "Price", "Sold", "Colour", "VehicleType", "WheelType"));

                                // For each listing print its fields.
                                // Include additional fields from relevant tables such as colour, vehicle type and wheel type.
                                // Price is rounded to 2 decimal places for brevity.
                                foreach (VehicleReference.VehicleListing vehicle in vehicles)
                                {
                                    Console.WriteLine(String.Format(lineFormat, vehicle.ID, vehicle.Make, vehicle.Model, decimal.Round(vehicle.Price, 2, MidpointRounding.AwayFromZero), vehicle.Sold, vehicle.Colour, vehicle.VehicleType, vehicle.WheelType));
                                }
                            }

                            break;

                        case 4:
                            Console.Write("Enter price range (two integers separated by a space, e.g. 2999 9999), or 0 to cancel: ");

                            // Read user input, split it on any spaces and store it in the ranges array.
                            string[] ranges = Console.ReadLine().Split(' ');

                            // If user entered "0", break out of the case.
                            if (ranges[0] == "0")
                            {
                                break;
                            }
                            // Throw exception if operand count does not match the required number.
                            else if (ranges.Length != 2)
                            {
                                throw new FormatException();
                            }
                            else
                            {
                                // Validate input. Will throw FormatException if not integers.
                                int[] op = new int[2];
                                for (int i = 0; i < 2; i++)
                                {
                                    op[i] = int.Parse(ranges[i]);
                                }

                                // Get all listings within the price range, convert them to a list and store them in the vehicles variable.
                                vehicles = vc.getListingsByPriceRange(op[0], op[1]).ToList();

                                // Display message if no vehicles matching the criteria are found.
                                if (vehicles.Count == 0)
                                {
                                    Console.WriteLine("No listings found within the given price range.");
                                }
                                else
                                {
                                    // Format the output line by specifying the width of each string.
                                    lineFormat = "{0,2} {1,10} {2,10} {3,10} {4,5}";
                                    Console.WriteLine(String.Format(lineFormat, "ID", "Make", "Model", "Price", "Sold"));

                                    // For each listing print its fields.
                                    foreach (VehicleReference.VehicleListing vehicle in vehicles)
                                    {
                                        Console.WriteLine(String.Format(lineFormat, vehicle.ID, vehicle.Make, vehicle.Model, vehicle.Price, vehicle.Sold));
                                    }
                                }
                            }

                            break;

                        case 5:
                            Console.Write("Enter colour (a string, e.g. blue), or 0 to cancel: ");
                            
                            // Read user input and store it in the colour variable.
                            string colour = Console.ReadLine();

                            // If user enters "0", break out of the case.
                            if (colour == "0")
                            {
                                break;
                            }
                            else
                            {
                                // Get all listings that match the colour string, convert them to a list and store them in the vehicles variable.
                                vehicles = vc.getListingsByColour(colour).ToList();

                                // Display message if no vehicles matching the criteria are found.
                                if (vehicles.Count == 0)
                                {
                                    Console.WriteLine("No listings found for the given colour.");
                                }
                                else
                                {
                                    // Format the output line by specifying the width of each string.
                                    // Also display colour as it is relevant for this query.
                                    lineFormat = "{0,2} {1,10} {2,10} {3,10} {4,5} {5,15}";
                                    Console.WriteLine(String.Format(lineFormat, "ID", "Make", "Model", "Price", "Sold", "Colour"));

                                    // For each listing print its fields.
                                    foreach (VehicleReference.VehicleListing vehicle in vehicles)
                                    {
                                        Console.WriteLine(String.Format(lineFormat, vehicle.ID, vehicle.Make, vehicle.Model, vehicle.Price, vehicle.Sold, vehicle.Colour));
                                    }
                                }
                            }

                            break;

                        case 6:
                            Console.Write("Enter vehicle ID (an integer, e.g. 2), or 0 to cancel: ");

                            // Read user input, parse it to int and store it in the vehicleId variable.
                            vehicleId = int.Parse(Console.ReadLine());

                            // If user entered 0, break out of the case.
                            if (vehicleId == 0)
                            {
                                break;
                            }
                            else
                            {
                                try
                                {
                                    // Attempt to toggle the listing's sold status.
                                    vc.toggleListingSold(vehicleId);

                                    Console.WriteLine("Listing sold status updated.");
                                }
                                catch (FaultException ex)
                                {
                                    // Catch any exceptions thrown by the service and display a user-friendly message.
                                    Console.WriteLine("{0}: {1}", ex.Code.Name, ex.Reason);
                                }
                            }

                            break;

                        case 7:
                            Console.Write("Enter vehicle make, model, price, colour ID, vehicle type ID, and wheel type ID separated by spaces (e.g. Ford Mustang 23000 2 3 1), or 0 to cancel: ");

                            // Read user input, split it on any spaces and store it in the details array.
                            string[] details = Console.ReadLine().Split(' ');

                            // Throw exception if operand count does not match the required number.
                            if (details.Length != 6)
                            {
                                // Additionally, check if the user entered "0" and if so, break out of the case.
                                if (details[0] == "0") break;

                                throw new FormatException();
                            }
                            else
                            {
                                // Create a Dictionary and populate it with user input.
                                // Not essential, but makes it easier to reference the input by keys in the service code.
                                Dictionary<string, string> detailsDict = new Dictionary<string, string>();
                                detailsDict.Add("Make", details[0]);
                                detailsDict.Add("Model", details[1]);
                                detailsDict.Add("Price", details[2]);
                                detailsDict.Add("ColourId", details[3]);
                                detailsDict.Add("VehicleTypeId", details[4]);
                                detailsDict.Add("WheelTypeId", details[5]);

                                try
                                {
                                    // Attempt to create the new listing
                                    vc.createListing(detailsDict);

                                    Console.WriteLine("Listing created.");
                                }
                                catch (FaultException ex)
                                {
                                    // Catch any exceptions thrown by the service and display a user-friendly message.
                                    Console.WriteLine("{0}: {1}", ex.Code.Name, ex.Reason);
                                }
                            }

                            break;

                        case 8:
                            Console.Write("Enter vehicle ID (an integer, e.g. 16), or 0 to cancel: ");

                            // Read user input, parse it to int and store it in the vehicleId variable.
                            vehicleId = int.Parse(Console.ReadLine());

                            // If user entered 0, break out of the case.
                            if (vehicleId == 0)
                            {
                                break;
                            }
                            else
                            {
                                try
                                {
                                    // Attempt to delete the vehicle listing.
                                    vc.deleteListing(vehicleId);

                                    Console.WriteLine("Listing deleted.");
                                }
                                catch (FaultException ex)
                                {
                                    // Catch any exceptions thrown by the service and display a user-friendly message.
                                    Console.WriteLine("{0}: {1}", ex.Code.Name, ex.Reason);
                                }
                            }

                            break;

                        default:
                            // Fallback message if the user has entered an invalid menu option number.
                            Console.WriteLine("Error: unrecognised menu option.");

                            break;
                    }

                    // Print the menu.
                    Console.WriteLine(menu);
                    Console.Write("> ");

                    // Prompt for user input. Will throw FormatException if not int.
                    entry = int.Parse(Console.ReadLine());
                } // while
            }
            catch (FormatException)
            {
                Console.WriteLine("FormatException: Invalid operand format or check operand count.");
            }
            catch (Exception)
            {
                Console.WriteLine("Exception: Unidentified system exception; check server running?");
            }
        } // Main
    } // class Client
}
