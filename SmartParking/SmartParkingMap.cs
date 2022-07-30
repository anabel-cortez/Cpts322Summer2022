using Firebase.Database;
using Firebase.Database.Query;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace SmartParking
{
    public partial class ParkingLot : Form
    {
        FirebaseClient client = new FirebaseClient("https://heymotocarro-1a1d4.firebaseio.com/");
        //FirebaseClient client = new FirebaseClient("https://cpts322summer2022-default-rtdb.firebaseio.com/");
        Graphics G;
        Beacons beacons;
        Sensors sensors;
        ParkingLotData parkingLotData;
        int[] map = new int[4];
        Rectangle[] rect = new Rectangle[13];
        SolidBrush myBrush = new SolidBrush(Color.SkyBlue);
        SolidBrush myBrush2 = new SolidBrush(Color.YellowGreen);

        public ParkingLot()
        {
            InitializeComponent();
           // WindowState = FormWindowState.Maximized;
           
    

        }
        private void ParkingLot_Load(object sender, EventArgs e)
        {
           // G = this.CreateGraphics();
            sensors = new Sensors(4);
          
            sensors.data[0].setCordinaties(0,5);
            sensors.data[1].setCordinaties(9,5);
            sensors.data[2].setCordinaties(0,0);
            sensors.data[3].setCordinaties(9,0);

            map[0] = 0;
            map[1] = 0;

            //Slots = new Slot(4)
        }
        private void loadBtn_Click_1(object sender, EventArgs e)
        {
            G = this.CreateGraphics();
            parkingLotData = new ParkingLotData(12, G);
            getPopulationAsync();
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
        
            

        }
        private async void getPopulationAsync() // grabs population from database 
        {
            
    
            //******************** Get initial list of beacons ***********************//
           beacons = await client
               .Child("Beacons/")//Prospect list
               .OnceSingleAsync<Beacons>();

           // getBeacons(beacons);

            //******************** Get changes on beacons ***********************//
            onChildChanged();


        }


        private void getBeacons(Beacons beacons) // Selects the individuals to vaccinate
        {
            foreach (var beacon in beacons.data)
            {
                Console.WriteLine($"beacon id: { beacon.Id} [{ beacon.D1}]");//dont need
                //create data for every becon 
               
            }

        }

        private static async Task sendData(double key)
        {
            FormUrlEncodedContent content;
            HttpResponseMessage response;
            HttpClient httpclient = new HttpClient();
            string responseString;
            int companyId = 0;

            var dictionary = new Dictionary<string, string>{
                            { "key",key.ToString()},
                            { "companyId = 0",companyId.ToString()}
                        };

            content = new FormUrlEncodedContent(dictionary);
            response = await httpclient.PostAsync("https://us-central1-heymotocarro-1a1d4.cloudfunctions.net/sendData", content);
            responseString = await response.Content.ReadAsStringAsync();
            Response data = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(responseString);
            //Response message
            Console.WriteLine(data.key);
            Console.WriteLine(data.companyId);

        }



        private void onChildChanged() // Waits for data base to start with variable
        {

            var child = client.Child("Beacons/data");
            var observable = child.AsObservable<Beacon>();
            var subscription = observable
                .Subscribe(x =>
                {
                    int index = 0;
                    int key = Int32.Parse(x.Key);
                   beacons.data[key].update(x.Object);
                   
                    Point p = beacons.data[key].getxy(sensors);
                    // Console.WriteLine($"x ,y value: { p.x } { p.y }");
                    //int i = Int32.Parse(x.Key);
                    //int index;
                    // beacons.data[i].update(x.Object);
                    //Point p = beacons.data[i].getxy(sensors);
                    //int add = 0;
                    //if (p.y >= 3 && p.y <= 5) { add = 0; }
                    //if (p.y >= 0 && p.y <= 2) { add = 6; }


                    //index = (int)Math.Floor(p.x / 1.5) + add;
                    //if (map[i] != index)
                    //{
                    //    G.FillRectangle(myBrush, rect[map[i]]);
                    //    G.FillRectangle(myBrush2, rect[index]);
                    //    //parkingLotData.slots[i].ColorR(G);
                    //    sendData();
                    //}
                    //map[i] = index;

                    //funtion to find the index
                    // beacons.data[key].setSlotindex(index);


                    //parkingLotData.slots[7].Blue(G);
                    //int companyId = 0;
                    Font drawFont = new Font("Arial", 16);
                    SolidBrush drawBrush = new SolidBrush(Color.Black);
                    StringFormat drawFormat = new StringFormat();

                    if (p.y >= 0 && p.y <= 2)
                    {
                        index = (int)Math.Floor(p.x / 1.5) + 6;
                        if(beacons.data[key].index != index){
                            parkingLotData.slots[beacons.data[key].index].ColorB(G);
                            parkingLotData.slots[index].ColorR(G);
                            beacons.data[key].index = index;
                        
                        }
                        //G.DrawString(index.ToString(), drawFont, drawBrush, 100 * 100 + 50, 300 + 100, drawFormat);

                        sendData(index);

                    }
                    if (p.y >= 3 && p.y <= 5)
                    {
                        index = (int)Math.Floor(p.x / 1.5);
                        if (beacons.data[key].index != index)
                        {
                            parkingLotData.slots[beacons.data[key].index].ColorB(G);
                            parkingLotData.slots[index].ColorR(G);
                            beacons.data[key].index = index;

                        }
                        //G.DrawString(index.ToString(), drawFont, drawBrush, 100 * 100 + 50, 300 + 100, drawFormat);
                        //Slot.DrawR(G)
                        beacons.data[key].index = index;
                        sendData(index);
                    }
                    //Slot.DrawR(G);

                    //call function Run().Wait()

                });
           
        }
       
        public void DrawStringFloatFormat(String drawString, float x, float y)
        {

     
            // Create font and brush.
            Font drawFont = new Font("Arial", 16);
           SolidBrush drawBrush = new SolidBrush(Color.Black);
             

          // Set format of string.
            StringFormat drawFormat = new StringFormat();
          // drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

            // Draw string to screen.
            G.DrawString(drawString, drawFont, drawBrush, x, y, drawFormat);
        }

        private void ParkingLot_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
