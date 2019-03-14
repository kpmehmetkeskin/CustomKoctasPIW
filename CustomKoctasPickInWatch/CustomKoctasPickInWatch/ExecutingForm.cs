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
    public partial class ExecutingForm : Form
    {
        public ExecutingForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeCamera();
            axWindowsMediaPlayer1.URL = "";
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            axWindowsMediaPlayer1.Width = this.Width;
            axWindowsMediaPlayer1.Height = this.Height;
        }

        public void InitializeCamera()
        {
            VideoCaptureDevice FinalVideoSource;
            FilterInfoCollection VideoCaptuerDevices;
            VideoCaptuerDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            FinalVideoSource = new VideoCaptureDevice(VideoCaptuerDevices[1].MonikerString);
            FinalVideoSource.NewFrame += FinalVideoSource_NewFrame2;
            FinalVideoSource.DesiredFrameRate = 500;
            FinalVideoSource.DesiredFrameSize = new Size(1, 500);
            FinalVideoSource.Start();
        }

        MotionDetector detector = new MotionDetector(
            new TwoFramesDifferenceDetector(),
            new BlobCountingObjectsProcessing()
            { HighlightColor = Color.Red, MinObjectsHeight = 50, MinObjectsWidth = 50, HighlightMotionRegions = true });

        int SumX = 0;
        int SumY = 0;

        private void FinalVideoSource_NewFrame2(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();

            if(detector.ProcessFrame(image) > 1)
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

        private String currentVideo = "";

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < StaticVariables.objectBarcodeList.Count; i++)
            {
                int rectangleCount = StaticVariables.objectBarcodeList.Count;
                int rectangleWidth = Constants.width / rectangleCount;
                int rectangleXLocation = (i * rectangleWidth);
                int rectangleXLocationWithWidth = rectangleXLocation + rectangleWidth;

                if (SumX > rectangleXLocation && SumX <= rectangleXLocationWithWidth && !currentVideo.Equals(StaticVariables.objectBarcodeList[i].ToString()))
                {
                    currentVideo = currentVideo = StaticVariables.objectBarcodeList[i].ToString();
                    axWindowsMediaPlayer1.URL = "C://PIWVideos//" + StaticVariables.objectBarcodeList[i].ToString() + ".mp4";
                    axWindowsMediaPlayer1.Ctlcontrols.stop();
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                    axWindowsMediaPlayer1.Width = this.Width;
                    axWindowsMediaPlayer1.Height = this.Height;
                    break;
                }
            }









            //this.Text = SumX.ToString() + "------------" + SumY.ToString();

            //if (SumX < 300 && SumX > 1)
            //{
            //    label1.Text = "B Reklamı";

            //    if (key == 1 || key == 0)
            //    {
            //        axWindowsMediaPlayer1.URL = "C://1.mp4";
            //        axWindowsMediaPlayer1.Ctlcontrols.stop();
            //        axWindowsMediaPlayer1.Ctlcontrols.play();
            //        key = 2;
            //    }
            //}
            //else if (SumX >= 300)
            //{
            //    label1.Text = "A Reklamı";

            //    if (key == 2 || key == 0)
            //    {
            //        axWindowsMediaPlayer1.URL = "C://2.mp4";
            //        axWindowsMediaPlayer1.Ctlcontrols.stop();
            //        axWindowsMediaPlayer1.Ctlcontrols.play();
            //        key = 1;
            //    }
            //}

        }
    }
}
