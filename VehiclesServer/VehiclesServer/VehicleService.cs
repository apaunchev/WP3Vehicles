using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using VehiclesServer.Models;

namespace VehiclesServer
{
    class VehicleService : IVehicleService
    {
        // Define database object.
        VehicleSalesContext db;

        public VehicleService()
        {
            // Initialise database object in the class constructor to be used later.
            db = new VehicleSalesContext();
        }

        public string getStudentId()
        {
            // Hard-coded value for my student ID.
            string studentId = "S1105400";

            return studentId;
        }

        public int getListingsCount()
        {
            // Count records in VehicleStockItems table.
            int count = db.VehicleStockItems.Count();

            return count;
        }

        public List<VehicleListing> getAllListings()
        {
            // Use a LINQ query to get all vehicle listings.
            // Include other fields from relevant tables (Colour, VehicleType, WheelType).
            IEnumerable<VehicleListing> vehicles =
                from v in db.VehicleStockItems
                join c in db.Colours on v.ColourID equals c.ColourID
                join vt in db.VehicleTypes on v.VehicleTypeID equals vt.VehicleTypeID
                join wt in db.WheelTypes on v.WheelTypeID equals wt.WheelTypeID
                select new VehicleListing
                {
                    ID = v.VehicleStockItemID,
                    Make = v.Make,
                    Model = v.Model,
                    Price = v.Price,
                    Sold = v.Sold,
                    Colour = c.ColourName,
                    VehicleType = vt.VehicleTypeName,
                    WheelType = wt.WheelTypeName
                };

            // Convert the IEnumberable of VehicleListing objects to a List.
            List<VehicleListing> vehicleList = vehicles.ToList();

            return vehicleList;
        } 

        public List<VehicleListing> getListingsByPriceRange(int rangeLow, int rangeHigh)
        {
            // Use a LINQ query to get all vehicle listings in the specified price range.
            // Only fields from the VehicleStockItems table are displayed as they are the most relevant for the query.
            IEnumerable<VehicleListing> vehicles =
                from v in db.VehicleStockItems
                where v.Price >= rangeLow && v.Price <= rangeHigh
                select new VehicleListing
                {
                    ID = v.VehicleStockItemID,
                    Make = v.Make,
                    Model = v.Model,
                    Price = v.Price,
                    Sold = v.Sold
                };

            // Convert the IEnumberable of VehicleListing objects to a List.
            List<VehicleListing> vehicleList = vehicles.ToList();

            return vehicleList;
        }

        public List<VehicleListing> getListingsByColour(string colour)
        {
            // Use a LINQ query to get all vehicle listings with the specified colour.
            // The query looks for any colour records with names which contain the user specified string.
            // e.g. "bl" would return all records containing "blue" (but also "black").
            IEnumerable<VehicleListing> vehicles =
                from v in db.VehicleStockItems
                join c in db.Colours on v.ColourID equals c.ColourID
                where c.ColourName.ToLower().Contains(colour)
                select new VehicleListing
                {
                    ID = v.VehicleStockItemID,
                    Make = v.Make,
                    Model = v.Model,
                    Price = v.Price,
                    Sold = v.Sold,
                    Colour = c.ColourName
                };

            // Convert the IEnumberable of VehicleListing objects to a List.
            List<VehicleListing> vehicleList = vehicles.ToList();

            return vehicleList;
        }

        public void toggleListingSold(int listingId)
        {
            try
            {
                // Find the VehicleStockItem with ID matching the user input.
                VehicleStockItem vehicle = db.VehicleStockItems.First(v => v.VehicleStockItemID == listingId);

                // Toggle the Sold property from True to False or vice versa.
                vehicle.Sold = !vehicle.Sold;

                // Save the changes to the database.
                db.SaveChanges();
            }
            catch (InvalidOperationException ex)
            {
                // Throw a FaultException to the client if the record with the entered ID does not exist.
                if (ex.Message == "Sequence contains no elements")
                    throw new FaultException(
                        new FaultReason("Vehicle listing with that ID does not exist."),
                        new FaultCode("Invalid Operation Error"));
                else
                    // Throw a generic FaultException.
                    throw new FaultException(
                        new FaultReason("Cause unknown."),
                        new FaultCode("Invalid Operation Error"));
            }
        }

        public void deleteListing(int listingId)
        {
            try
            {
                // Find the VehicleStockItem with ID matching the user input.
                VehicleStockItem vehicle = db.VehicleStockItems.First(v => v.VehicleStockItemID == listingId);

                // Remove the vehicle from the table.
                db.VehicleStockItems.Remove(vehicle);

                // Save the changes to the database.
                db.SaveChanges();
            }
            catch (InvalidOperationException ex)
            {
                // Throw a FaultException to the client if the record with the entered ID does not exist.
                if (ex.Message == "Sequence contains no elements")
                    throw new FaultException(
                        new FaultReason("Vehicle listing with that ID does not exist."),
                        new FaultCode("Invalid Operation Error"));
                else
                    // Throw a generic FaultException.
                    throw new FaultException(
                        new FaultReason("Cause unknown."),
                        new FaultCode("Invalid Operation Error"));
            }
        }

        public void createListing(Dictionary<string,string> args)
        {
            try
            {
                // Create a new VehicleStockItem object and populate some of its fields with user input...
                VehicleStockItem vehicle = new VehicleStockItem();
                vehicle.Make = args["Make"];
                vehicle.Model = args["Model"];
                vehicle.Price = int.Parse(args["Price"]);
                vehicle.ColourID = int.Parse(args["ColourId"]);
                vehicle.VehicleTypeID = int.Parse(args["VehicleTypeId"]);
                vehicle.WheelTypeID = int.Parse(args["WheelTypeId"]);

                // ... then fill in the rest of the required fields with sample data.
                // Done so in order to simplify user input.
                vehicle.Registration = "AA1AAAA";
                vehicle.Capacity = 1500;
                vehicle.Automatic = false;
                vehicle.Sold = false;
                vehicle.StockEntryDate = DateTime.Now;
                vehicle.DateNew = Convert.ToDateTime("11/11/2003");

                // Add the new vehicle object to the table.
                db.VehicleStockItems.Add(vehicle);

                // Save the changes to the database.
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                // Throw FaultException to the client in case there is a failure when updating the database.
                throw new FaultException(
                    new FaultReason("Database update failed, check input."),
                    new FaultCode("Database Update Error"));
            }
            catch (FormatException)
            {
                // Throw a FaultException to the client if the input format is invalid.
                throw new FaultException(
                    new FaultReason("Input format invalid, check dates and numbers."),
                    new FaultCode("Format Error"));
            }
        }
    }
}
