using System;
using System.Collections.Generic;
using System.Linq;

namespace Gridnine.FlightCodingTest
{
    public class FlightBuilder
    {
        private DateTime _threeDaysFromNow;

        public FlightBuilder()
        {
            _threeDaysFromNow = DateTime.Now.AddDays(3);
        }

        public IList<Flight> GetFlights()
        {
            return new List<Flight>
                       {
                           //A normal flight with two hour duration
			               CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2)),

                           //A normal multi segment flight
			               CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2), _threeDaysFromNow.AddHours(3), _threeDaysFromNow.AddHours(5)),
                           
                           //A flight departing in the past
                           CreateFlight(_threeDaysFromNow.AddDays(-6), _threeDaysFromNow),

                           //A flight that departs before it arrives
                           CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(-6)),

                           //A flight with more than two hours ground time
                           CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2), _threeDaysFromNow.AddHours(5), _threeDaysFromNow.AddHours(6)),

                            //Another flight with more than two hours ground time
                           CreateFlight(_threeDaysFromNow, _threeDaysFromNow.AddHours(2), _threeDaysFromNow.AddHours(3), _threeDaysFromNow.AddHours(4), _threeDaysFromNow.AddHours(6), _threeDaysFromNow.AddHours(7))
                       };
        }

        private static Flight CreateFlight(params DateTime[] dates)
        {
            if (dates.Length % 2 != 0) throw new ArgumentException("You must pass an even number of dates,", "dates");

            var departureDates = dates.Where((date, index) => index % 2 == 0);
            var arrivalDates = dates.Where((date, index) => index % 2 == 1);

            var segments = departureDates.Zip(arrivalDates,
                                              (departureDate, arrivalDate) =>
                                              new Segment { DepartureDate = departureDate, ArrivalDate = arrivalDate }).ToList();

            return new Flight { Segments = segments };
        }
    }

    public class Flight
    {
        public IList<Segment> Segments { get; set; }
    }

    public class Segment
    {
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
    }

    public class SortFlights
    {
        public void BeforeNow()
        {
            FlightBuilder airTravels = new FlightBuilder();
            IList<Flight> travels = new List<Flight> { };
            travels = airTravels.GetFlights();
            Console.WriteLine("Рейсы с вылетом после текущего момента времени:");
            var selectedBefore = from schedule in travels
                                 from sch in schedule.Segments
                                 where sch.DepartureDate.CompareTo(DateTime.Now)>0
                                 select sch;
            foreach (Segment time in selectedBefore)
            {
                Console.WriteLine($"{time.DepartureDate}-{time.ArrivalDate}");
            }
            Console.WriteLine("-------------------------------------------");
        }

        public void ArriveBeforeDeparture()
        {
            FlightBuilder airTravels = new FlightBuilder();
            IList<Flight> travels = new List<Flight> { };
            travels = airTravels.GetFlights();
            Console.WriteLine("Сегменты с датой прилёта не раньше даты вылета:");
            var selectedAD = from schedule in travels
                                 from sch in schedule.Segments
                                 where sch.ArrivalDate >= sch.DepartureDate
                                 select sch;
            foreach (Segment time in selectedAD)
            {
                Console.WriteLine($"{time.DepartureDate}-{time.ArrivalDate}");
            }
            Console.WriteLine("-------------------------------------------");
        }

        public void GroundTime()
        {
            FlightBuilder airTravels = new FlightBuilder();
            IList<Flight> travels = new List<Flight> { };
            travels = airTravels.GetFlights();
            Console.WriteLine("Общее время на земле меньше двух часов:");

            for (int i = 0; i < travels.Count; i++)
            {
                if (travels.ElementAt(i).Segments.Count>1)
                {
                    TimeSpan transfer= new TimeSpan();
                    for (int j = 0; j < travels.ElementAt(i).Segments.Count-1;j++)
                    {
                        transfer+=travels.ElementAt(i).Segments.ElementAt(j + 1).ArrivalDate.Subtract(travels.ElementAt(i).Segments.ElementAt(j).DepartureDate);
                    }
                    if (transfer.TotalHours>2)
                    {
                        travels.RemoveAt(i);
                    }
                }
            }
            var selectAll = from schedule in travels
                            from sch in schedule.Segments
                            select sch;
            foreach (Segment time in selectAll)
            {
                Console.WriteLine($"{time.DepartureDate}-{time.ArrivalDate}");
            }

            Console.WriteLine("-------------------------------------------");
        }
        public void ShowAll()
        {
            FlightBuilder airTravels = new FlightBuilder();
            IList<Flight> travels = new List<Flight> { };
            travels = airTravels.GetFlights();
            Console.WriteLine("Все сегменты:");
            var selectAll = from schedule in travels
                                 from sch in schedule.Segments
                                 select sch;
            foreach (Segment time in selectAll)
            {
                Console.WriteLine($"{time.DepartureDate}-{time.ArrivalDate}");
            }
            Console.WriteLine("-------------------------------------------");
        }
    }
    

    class Program
    {
        static void Main(string[] args)
        {
            SortFlights testing = new SortFlights();
            testing.ShowAll();
            testing.BeforeNow();
            testing.ArriveBeforeDeparture();
            testing.GroundTime();
            Console.ReadKey();
        }
    }

}


