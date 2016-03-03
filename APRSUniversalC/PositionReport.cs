using hamradio.ke4pjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace APRSUniversalC
{
    class PositionReport
    {
        public PositionReport()
        {
            this.NormalizedAnchorPoint = new Point(0.5, 0.5);
            this.ImageSourceUri = new Uri("ms-appx:///Assets/plus.png");
        }
        public string DisplayName { get; set; }
        public APRS APRSPacket { get; set; }
        public Uri ImageSourceUri { get; set; }
        //public Geopoint Location
        //{
        //    get
        //    {
        //        return new Geopoint(new BasicGeoposition() { Latitude = Convert.ToDouble(this.APRSPacket.Latitude), Longitude = Convert.ToDouble(this.APRSPacket.Longitude) });
        //    }
        //}
        public Geopoint Location { get; set; }

        public Point NormalizedAnchorPoint { get; set; }

    }
}
