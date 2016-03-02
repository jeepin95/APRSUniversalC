using hamradio.ke4pjw;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace APRSUniversalC
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SocketClient client = new SocketClient();
        public MainPage()
        {
            this.InitializeComponent();
        }

        StreamSocket socket = new StreamSocket();
        
        private void button_Click(object sender, RoutedEventArgs e)
        {
            test2();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            login();
        }

        private async void login() {
            DataWriter writer = new DataWriter(socket.OutputStream);
//            writer.WriteString("user KD7ZYW pass -1 filter r/45/-122/500\r\n" + Environment.NewLine);

            writer.WriteString("user KD7ZYW pass -1\r\n" + Environment.NewLine);

            await writer.StoreAsync();
        }

        private async void test2()
        {
            socket.Control.KeepAlive = false;
            HostName hostName = new HostName("rotate.aprs.net");
            //string serviceName = "14580";
            string serviceName = "23";
            DataWriter writer;

            await socket.ConnectAsync(hostName, serviceName, SocketProtectionLevel.PlainSocket);

            //writer = new DataWriter(socket.OutputStream);
            //string loginString = "user KD7ZYW pass 0 " + Environment.NewLine;
            //writer.WriteString(loginString);
            //try
            //{
            //    await writer.StoreAsync();
            //    textBlock.Text = "Connecting...";
            //    waitForData(socket);
            //}
            //catch (Exception ex)
            //{
            //    textBlock.Text = ex.Message;

            //}
            while (true)
            {
                using (DataReader inputStream = new DataReader(this.socket.InputStream))
                {
                    inputStream.InputStreamOptions = InputStreamOptions.Partial;
                    DataReaderLoadOperation loadOperation = inputStream.LoadAsync(2500);
                    await loadOperation;
                    if (loadOperation.Status != AsyncStatus.Completed)
                    {
                        //this.();  //insert your handler here
                        return;
                    }

                    //read complete message
                    uint byteCount = inputStream.UnconsumedBufferLength;

                    byte[] bytes = new byte[byteCount];
                    inputStream.ReadBytes(bytes);



                    string text = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                    this.handleServerMessage(text);  //insert your handler here

                    //detach stream so that it won't be closed when the datareader is disposed later
                    inputStream.DetachStream();
                }
            }
            
        }
        RandomAccessStreamReference mapIconStreamReference;
        int locCount = 0;

        private void handleServerMessage(string bytes)
        {
            StringReader sr = new StringReader(bytes);
            while(true)
            {
                string t = sr.ReadLine();
                if(t != null)
                {
                    APRS p = new APRS();
                    p.Parse(t);
                    if(p.PacketType == "Location")
                    {
                        locCount++;
                        txtCount.Text = String.Format("{0}", locCount);
                        //textBlock.Text += p.RawData;
                        //textBlock.Text += String.Format("{0} ({1},{2}){3}",p.Callsign,p.Latitude,p.Longitude,Environment.NewLine);
                        mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/mappin.png"));
                        MapIcon mapIcon1 = new MapIcon();
                        BasicGeoposition bgp = new BasicGeoposition() { Latitude = Convert.ToDouble(p.Latitude), Longitude = Convert.ToDouble(p.Longitude)};
                        Geopoint gp = new Geopoint(bgp);


                        mapIcon1.Location = gp;
                        mapIcon1.NormalizedAnchorPoint = new Point(0.5, 0.5);
                        mapIcon1.Title = p.Callsign;
                        mapIcon1.Image = mapIconStreamReference;
                        mapIcon1.ZIndex = 0;
                        myMap.MapElements.Add(mapIcon1);

                    }

                }
                else { break; }
            }



            //throw new NotImplementedException();
        }

       

        private async void waitForData(StreamSocket socket)
        {
            DataReader dr = new DataReader(socket.InputStream);
            dr.InputStreamOptions = InputStreamOptions.Partial;
            string text = String.Empty;
            try
            {
                int length = dr.ReadInt32();
                uint numStrBytes = await dr.LoadAsync((uint)length);
                text = dr.ReadString(numStrBytes);
                textBlock.Text = text;
                waitForData(socket);

            }
            catch (Exception ex)
            {
                textBlock.Text = ex.Message;
            }

        }


    }
}
