using System.Collections.Generic;

namespace Web.Models
{
    public class ServiceCenter
    {
        public int Id { get; set; }
        public string PlaceName { get; set; } 
        public int Index { get; set; } 
        public double GeoLong { get; set; } 
        public double GeoLat { get; set; } 
    }

    public class ServiceCenters
    {
        private ServiceCenters()
        {
            GetServiceCenterList = new List<ServiceCenter>()
            {
                new ServiceCenter()
                {
                    Id = 0, PlaceName = "ДП «КиївГазЕнерджи»  вул. Кіквідзе, 4-Б ",
                    Index = 01103, GeoLong = 50.4173374, GeoLat = 30.5478991
                },
                new ServiceCenter()
                {
                    Id = 1, PlaceName = "вул. М. Василенка, 5",
                    Index = 03124, GeoLong = 50.451115, GeoLat = 30.418265
                },
                new ServiceCenter()
                {
                    Id = 2, PlaceName = "вул. Володимирська, 49А, секція 7, оф. 415",
                    Index = 01054, GeoLong = 50.4457306, GeoLat = 30.513276
                },
                new ServiceCenter()
                {
                    Id = 3, PlaceName = "вул. Білицька, 45",
                    Index = 04078, GeoLong = 50.491723, GeoLat = 30.434101
                },
                new ServiceCenter()
                {
                    Id = 4, PlaceName = "пров. Червоноармійський, 18",
                    Index = 03039, GeoLong = 50.409344, GeoLat = 30.519973
                },
                new ServiceCenter()
                {
                    Id = 5, PlaceName = "вул. Електротехнічна, 9",
                    Index = 02217, GeoLong = 50.503999, GeoLat = 30.617512
                },
                new ServiceCenter()
                {
                    Id = 6, PlaceName = "вул. Раїси Окіпної, 4",
                    Index = 02002, GeoLong = 50.450061, GeoLat = 30.595562
                }
            };
        }

        public static ServiceCenters GetInstance()
        {
            if(_instance == null)
                lock (_obj)
                    if(_instance == null)
                        _instance = new ServiceCenters();

            return _instance;
        }

        public List<ServiceCenter> GetServiceCenterList { get; }


        private static readonly object _obj = new object();
        private static ServiceCenters _instance;
    }
}