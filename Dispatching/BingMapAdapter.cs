using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BATSystem.ImageryService;


namespace BATSystem
{
    class BingMapAdapter
    {
        private string API_KEY = "ENTER KEY HERE";

        public void setMapSize(int h,int w)
        {
            height = h;
            width = w;
        }
        public string getImage(EmergencyVehicle EV, Dispatcher D)
        {
          string origin      = EV.getLocation();
          string destination = D.getDestination();
           String imageURL = "http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/Routes?wp.0=" + origin + "&wp.1=" + destination + "&mapSize=" + width + "," + height +  "&key=" + API_KEY;
           Console.WriteLine(imageURL);
           return imageURL;
        }

        int height = 600;
        int width = 700;
    }
}
