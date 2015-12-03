//Mohammed Inoue CS300 Group: Carcaju
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



///<Summary>
///This acts as an adapter to interact with the Bings map API to retrieve route data for the EV to see on his machine
///</Summary>
namespace BATSystem
{
    class BingMapAdapter
    {
        /// <summary>
        /// Please don't steal my API key
        /// </summary>
        private string API_KEY = "AuVQiitO79AZBg1X9M_4YyQUX9U78gzQPyLNFQbMRiexfwUUJRQcx4YRoHgIYTyl";

        /// <summary>
        /// This allows us to change the map size easily
        /// </summary>
        /// <param name="h"> height</param>
        /// <param name="w"> width</param>
        public void setMapSize(int h,int w)
        {
            height = h;
            width = w;
        }
        /// <summary>
        /// Get image interacts with the Bing Map API to generate a map to display to the EV, using its location and the destination
        /// </summary>
        /// <param name="origin"> This is the EVs location</param>
        /// <param name="destination">This is the destination</param>
        /// <returns></returns>
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
