using System;
namespace Lucuma.Libs
{
    public interface IGeoProvider
    {
        Lucuma.IGeoCodeResult GetCoordinates(string search);
        string BuildSearchQuery(string search);
         IGeoProviderConfig Config { get; set; }
    }

    
}
