using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace BATSystem
{
    class BingMapAdapter
    {
        private string API_KEY = "AuVQiitO79AZBg1X9M_4YyQUX9U78gzQPyLNFQbMRiexfwUUJRQcx4YRoHgIYTyl";

        public void setMapSize(int h,int w)
        {
            height = h;
            width = w;
        }
        public string getImage(string origin, string destination)
        {
           String imageURL = "http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/Routes?wp.0=" + origin + "&wp.1=" + destination + "&mapSize=" + width + "," + height +  "&key=" + API_KEY;
           Console.WriteLine(imageURL);
           return imageURL;
        }

        int height = 200;
        int width = 300;
    }
}
