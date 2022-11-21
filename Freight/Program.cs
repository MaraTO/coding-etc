using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freight
{
    public class FlightDetails
    {
        public string Flight { get; set; }
        public string Deptarture { get; set; }
        public string Arrival { get; set; }
        public int Capacity { get; set; } = 20;
        public int Current { get; set; } = 0;

        public override string ToString()
        {
            return string.Format("Flight: {0}, departure: {1}, arrival: {2}", Flight, Deptarture, Arrival);
        }

    }

    public struct OrderDetails
    {
        public string destination;
    }
    

    // Orders to be accessed by order#
    interface IOrders
    {
        Dictionary<string, OrderDetails> Orders { get; set; }
    }

    public class OrdersList : IOrders
    {
        public Dictionary<string, OrderDetails> Orders { get; set; }
    }


    interface IFlightsSchedule
    {
        (FlightDetails, int) GetFlight(string Arrival);
        void Reset();
    }

    class Flights : IFlightsSchedule
    {
        public Dictionary<int, Dictionary<string, FlightDetails>> DayToFlights { get; set; }

        public (FlightDetails, int) GetFlight(string Arrival)
        {
            foreach (var day in DayToFlights.Keys)
            {
                var dayFlights = DayToFlights[day];
                var flight = dayFlights.FirstOrDefault(p => (p.Value.Arrival == Arrival && p.Value.Current < p.Value.Capacity)).Value;
                if (flight != null)
                {
                    flight.Current++;
                    return (flight, day);
                }
            }
            return (null, 0);
        }

        public void Reset()
        {
            foreach (var day in DayToFlights.Values)
            {
                foreach (var flight in day.Values)
                {
                    flight.Current = 0;
                }
            }
        }

    }

    class Program
    {
        static void PrintSchedule(Flights flights)
        {
            foreach (var day in flights.DayToFlights.Keys)
            {
                var dayToFlights = flights.DayToFlights[day];
                foreach (var flight in dayToFlights.Values)
                {
                    Console.WriteLine("{0}, day: {1}", flight.ToString(), day);
                }
            }
        }

        static void ProcessOrders(IFlightsSchedule flights, IOrders ordersList)
        {
            foreach (var orderNumber in ordersList.Orders.Keys)
            {
                var some = flights.GetFlight(ordersList.Orders[orderNumber].destination);
                if (some.Item1 != null)
                {
                    Console.WriteLine("order: {0}, flightNumber: {1}, departure: {2}, arrival: {3}, day: {4}", orderNumber, some.Item1.Flight, some.Item1.Deptarture, some.Item1.Arrival, some.Item2);
                }
                else
                {
                    Console.WriteLine("order: {0}, flightNumber: not scheduled", orderNumber);
                }
            }
        }

        static void Main(string[] args)
        {
            var flights = new Flights
            {
                DayToFlights = new Dictionary<int, Dictionary<string, FlightDetails>>()
            };
            // Day 1
            flights.DayToFlights.Add(1, new Dictionary<string, FlightDetails>());
            flights.DayToFlights[1].Add("Flight 1", new FlightDetails { Arrival = "YYZ", Deptarture = "YUL", Flight = "Flight 1" });
            flights.DayToFlights[1].Add("Flight 2", new FlightDetails { Arrival = "YYC", Deptarture = "YUL", Flight = "Flight 2" });
            flights.DayToFlights[1].Add("Flight 3", new FlightDetails { Arrival = "YVR", Deptarture = "YUL", Flight = "Flight 3" });
            // Day 2
            flights.DayToFlights.Add(2, new Dictionary<string, FlightDetails>());
            flights.DayToFlights[2].Add("Flight 4", new FlightDetails { Arrival = "YYZ", Deptarture = "YUL", Flight = "Flight 4" });
            flights.DayToFlights[2].Add("Flight 5", new FlightDetails { Arrival = "YYC", Deptarture = "YUL", Flight = "Flight 5" });
            flights.DayToFlights[2].Add("Flight 6", new FlightDetails { Arrival = "YVR", Deptarture = "YUL", Flight = "Flight 6" });
            // Orders
            var content = File.ReadAllText("coding-assigment-orders.json");
            var ordersList = new OrdersList { Orders = JsonConvert.DeserializeObject<Dictionary<string, OrderDetails>>(content) };
            // Search "YYZ"(34 hits in 1 file of 1 searched)
            // Search "YYC"(22 hits in 1 file of 1 searched)
            // Search "YYE"(3 hits in 1 file of 1 searched)
            // Search "YVR" (37 hits in 1 file of 1 searched)

            // USER STORY #1
            Console.WriteLine("USER STORY #1");
            PrintSchedule(flights);

            // USER STORY #2
            Console.WriteLine("USER STORY #2");
            ProcessOrders(flights, ordersList);

            Console.ReadKey();
        }
    }
}
