using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VehiclesServer
{
    // Define the interface that is published to the client.
    [ServiceContract(Namespace = "http://services.gcal.ac.uk")]
    public interface IVehicleService
    {
        // Define the methods that are made accessible to the client.

        // Method for retrieving the student ID.
        [OperationContract]
        string getStudentId();

        // Method for getting the number of vehicle listings in the database.
        [OperationContract]
        int getListingsCount();

        // Method for getting all vehicle listings as a list of objects.
        [OperationContract]
        List<VehicleListing> getAllListings();

        // Method for getting all vehicle listings matching a price range as a list of objects.
        [OperationContract]
        List<VehicleListing> getListingsByPriceRange(int rangeLow, int rangeHigh);

        // Method for getting al vehicle listings matching a colour as a list of objects.
        [OperationContract]
        List<VehicleListing> getListingsByColour(string colour);

        // Method for toggling the sold status of a vehicle listing.
        [OperationContract]
        void toggleListingSold(int listingId);

        // Method for deleting a vehicle listing from the database.
        [OperationContract]
        void deleteListing(int listingId);

        // Method for creating a new vehicle listing in the database.
        [OperationContract]
        void createListing(Dictionary<string,string> args);
    }

    // Define data class.
    [DataContract]
    public class VehicleListing
    {
        int id;
        string make;
        string model;
        decimal price;
        bool sold;
        string colour;
        string vehicleType;
        string wheelType;

        // Define properties of each instance of the class.
        [DataMember]
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string Make
        {
            get { return make; }
            set { make = value; }
        }

        [DataMember]
        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        [DataMember]
        public decimal Price
        {
            get { return price; }
            set { price = value; }
        }

        [DataMember]
        public bool Sold
        {
            get { return sold; }
            set { sold = value; }
        }

        [DataMember]
        public string Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        [DataMember]
        public string VehicleType
        {
            get { return vehicleType; }
            set { vehicleType = value; }
        }

        [DataMember]
        public string WheelType
        {
            get { return wheelType; }
            set { wheelType = value; }
        }
    }
}
