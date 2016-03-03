using hamradio.ke4pjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace APRSUniversalC
{
    class PositionReportManager
    {
        private List<PositionReport> positionreports = new List<PositionReport>();
        public List<PositionReport> getPositionReports(BasicGeoposition center)
        {
            return positionreports;
        }

        public void addAPRS(APRS packet)
        {
            positionreports.Add(new PositionReport() {
                Location = new Geopoint(new BasicGeoposition() { Latitude = Convert.ToDouble(packet.Latitude), Longitude = Convert.ToDouble(packet.Longitude) }),
                DisplayName = packet.Callsign
        });
        }
    }
}
