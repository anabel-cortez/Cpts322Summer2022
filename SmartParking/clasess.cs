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
namespace SmartParking
{


    public class Beacon
    {
        public double D1 { get; set; }
        public double D2 { get; set; }
        public double D3 { get; set; }
        public double D4 { get; set; }
        public int index { get; set; }
        public long Id { get; set; }
        public long Time { get; set; }
        public void update(Beacon data)
        {
            D1 = data.D1;
            D2 = data.D2;
            D3 = data.D3;
            D4 = data.D4;
            Time = data.Time;
        }
        public Point getxy(Sensors s)
        {

            Point P = new Point(0,0);
            //Point p1 = s.data[0].location;
            double x1 = s.data[0].location.x;
            double y1 = s.data[0].location.y;
            double x2 = s.data[1].location.x;
            double y2 = s.data[1].location.y;
            double x3 = s.data[2].location.x;
            double y3 = s.data[2].location.y;
            double x4 = s.data[3].location.x;
            double y4 = s.data[3].location.y;

            double A = 2 * x2 - 2 * x1;
            double B = 2 * y2 - 2 * y1;
            double C = Math.Pow(D1, 2) - Math.Pow(D2, 2) - Math.Pow(x1, 2) + Math.Pow(x2, 2) - Math.Pow(y1, 2) + Math.Pow(y2, 2);
            double D = 2 * x3 - 2 * x2;
            double E = 2 * y3 - 2 * y2;
            double F = Math.Pow(D2, 2) - Math.Pow(D3, 2) - Math.Pow(x2, 2) + Math.Pow(x3, 2) - Math.Pow(y2, 2) + Math.Pow(y3, 2);
            P.x = (C * E - F * B) / (E * A - B * D);
            P.y = (C * D - A * F) / (B * D - A * E);

            Console.WriteLine($"(x,y) = {P.x},{P.y}");
          

            return P;
        }

        public int slotId { get; set; }

    }
    public class Beacons
    {
        public int Total { get; set; }
        public int free { get; set; }
        public Beacon[] data { get; set; }
        //public setSlotindex(index)
        //{

        //}


    }
   
    public class Response

    {
        public bool success { get; set; }
        public int index { get; set; }
        public string message { get; set; }
        public string received { get; set; }
        public string companyId { get; set; }
        public string color { get; set; }
        public int[] infected { get; set; }
        public string key { get; set; }

    }

    public class ParkingLotData
    {
       
        public Slot[] slots
        {
            get; set;
        }
        //constructor that create new slots #12
        public int Total { get; set; }
        public int free { get; set; }
        public int identifySlot { 
           get;set; 
        }
        //contructor
        public ParkingLotData(int size, Graphics G){
            slots = new Slot[size];
            for (int i = 0; i < size; i++)
            {
                slots[i] = new Slot(i,G);
            }

        }
   
}

    public class Slot
    {
        private int spotNum;
        public Point[] coordinates = new Point[4];
        Rectangle rect;


        Font drawFont = new Font("Arial", 16);
        SolidBrush drawBrush = new SolidBrush(Color.Black);


        // Set format of string.
        StringFormat drawFormat = new StringFormat();

        Pen blackPen = new Pen(Color.Black, 0);
        SolidBrush myBrush = new SolidBrush(Color.SkyBlue);
        SolidBrush myBrushB = new SolidBrush(Color.Blue);
        SolidBrush myBrushR = new SolidBrush(Color.Red);

        public Slot(int i, Graphics G)
        {
            spotNum = i + 1;
            int b = i < 6 ? 0 : 2;
            coordinates[0] = new Point(1.5 * i, b);
            coordinates[1] = new Point((i + 1) * 1.5, b);
            coordinates[2] = new Point(1.5 * i, b+2);
            coordinates[3] = new Point((i + 1) * 1.5, b+2);

            DrawR(G);

        }
        public bool isInside(Point p)   // change to bool
        {
            //within slot coordinates
            //coordinates[0].x < p.x;
            //coordinates[1].x > p.x;
            //coordinates[0].y < p.y;
            //coordinates[3].y > p.y;
            return true;

        }
        public void ColorR(Graphics G)
        {
            
            //G.FillRectangle(myBrushB, rect);
            G.FillRectangle(myBrushR, rect);
            G.DrawRectangle(blackPen, rect);
            //G.DrawString(spotNum.ToString(), drawFont, drawBrush, 100 * a + 50, b + 100, drawFormat);

        }
        public void ColorB(Graphics G)
        {

            //G.FillRectangle(myBrushB, rect);
            G.FillRectangle(myBrush, rect);
            G.DrawRectangle(blackPen, rect);
            //G.DrawString(spotNum.ToString(), drawFont, drawBrush, 100 * a + 50, b + 100, drawFormat);

        }
        public void DrawR(Graphics G)
        {
            int a = 0, b = 0;
            if (spotNum < 7)
            {
                a = spotNum-1;
                b = 100;
            }
            else {
                a = spotNum-7;
                b = 300;
            }

            rect = new Rectangle(100 * a,b, 100, 200);
          
            G.FillRectangle(myBrush, rect);
            G.DrawRectangle(blackPen, rect);
            G.DrawString(spotNum.ToString(), drawFont, drawBrush, 100 * a + 50, b+100, drawFormat);

        }

    }





    public class Sensors
    {
        public int total { set; get; }
        public Sensor[] data { get; set; }
        public Sensors(int size)
        {
            data = new Sensor[size];
            data[0] = new Sensor();
            data[1] = new Sensor();
            data[2] = new Sensor();
            data[3] = new Sensor();
        }

    }
    public class Sensor//goes inside the parking lot 
    {
        public Point location { get; set; }
        public Sensor()
        {
            location = new Point(0,0);
        }
        public void setCordinaties(double x, double y)
        {
            location.x = x;
            location.y = y;
        }
    }


    public class Point
    {
        public double x { get; set; }
        public double y { get; set; }

        public Point(double x, double y) {
            x = x;
            y = y;
        }
    }

    //public void DrawString(String drawString, float x, float y)
    //{


    //    // Create font and brush.
     
    //    //drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;

    //    // Draw string to screen.
    //   // G.DrawString(drawString, drawFont, drawBrush, x, y, drawFormat);
    //}
}

