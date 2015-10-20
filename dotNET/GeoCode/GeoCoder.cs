using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucuma.Libs;


namespace Lucuma
{

    public class Location
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string Accuracy { get; set; }

        public Location()
        {
            Name = String.Empty;
            Latitude = 0;
            Longitude = 0;
            Type = String.Empty;
            Address = String.Empty;
            Accuracy = String.Empty;
        }
    }

    public class GeoCodeResult : Lucuma.IGeoCodeResult
    {
        public bool HasValue { get { return Count > 0; }  }


        public double Latitude
        {
            get
            {
                if (_locations.Count() > 0)
                {
                    return _locations[0].Latitude;
                }
                return 0;
            }
            set
            {
              
            }
        
        }
        public double Longitude
        {
            get
            {
                if (_locations.Count() > 0)
                {
                    return _locations[0].Longitude;
                }
                return 0;
            }
            set
            {
               
            }

        }
        public string Error { get; set; }
        public int Count
        {
            get
            {
                return _locations.Count();
            }
        }

        public string Library { get; set; }
        private List<Location> _locations;

        public List<Location> Locations { get { return _locations; } }
        
        public GeoCodeResult()
        {
            
            Latitude = 0;
            Longitude = 0;
            Error = string.Empty;
            
            Library = string.Empty;
            this._locations = new List<Location>();
        }
    }

    public class GeoCoder
    {
        private List<IGeoProvider> providers;

        public  GeoCoder()
        {

            providers = new List<IGeoProvider>();
        }
        
        public GeoCoder(IGeoProvider provider): this()
        {
            AddProvider(provider);

        }

        private void SetupDefaults()
        {
            providers.Add(new GoogleGmap());
        }

        public GeoCoder AddProvider(IGeoProvider provider) {
            providers.Add(provider);
            return this;
        }

      
        public IGeoCodeResult GetCoordinates(string search)
        {
            if (providers.Count == 0) SetupDefaults();

            IGeoCodeResult gResult = new GeoCodeResult();

            foreach (IGeoProvider provider in providers)
            {
                try
                {
                    gResult = provider.GetCoordinates(search);
                } catch (Exception ex)
                {
                    throw (ex);
                }
                
                if (gResult.HasValue)
                {
                    break;
                }

            }

            return gResult;
        }
    }
}
