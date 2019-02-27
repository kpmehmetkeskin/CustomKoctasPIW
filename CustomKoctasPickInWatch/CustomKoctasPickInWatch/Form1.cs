using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomKoctasPickInWatch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            KameralariGetir();
        }

        public void KameralariGetir()
        {
            VideoCaptureDevice FinalVideoSource;
            FilterInfoCollection VideoCaptuerDevices;
            VideoCaptuerDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            FinalVideoSource = new VideoCaptureDevice(VideoCaptuerDevices[1].MonikerString);
            FinalVideoSource.NewFrame += FinalVideoSource_NewFrame;
            FinalVideoSource.DesiredFrameRate = 500;
            FinalVideoSource.DesiredFrameSize = new Size(1, 500);
            FinalVideoSource.Start();
        }

        MotionDetector detector = new MotionDetector(
            new TwoFramesDifferenceDetector(),
            new BlobCountingObjectsProcessing()
            { HighlightColor = Color.Red, MinObjectsHeight = 75, MinObjectsWidth = 75, HighlightMotionRegions = true });

        int SumX = 0;
        int SumY = 0;

        private void FinalVideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();

            detector.ProcessFrame(image);
                pictureBox1.Image = image;
            pictureBox1.Image = image;

            int X = 0;
            int Y = 0;

            try
            {
                BlobCountingObjectsProcessing countingDetector = (BlobCountingObjectsProcessing)detector.MotionProcessingAlgorithm;

                foreach (Rectangle rect in countingDetector.ObjectRectangles)
                {
                    X += rect.X;
                    Y += rect.Y;
                }

                SumX = X / countingDetector.ObjectRectangles.Length;
                SumY = Y / countingDetector.ObjectRectangles.Length;

            }
            catch (Exception e)
            {
                
                Console.WriteLine(e.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = SumX.ToString() + "------------" + SumY.ToString();
        }
    }
}
