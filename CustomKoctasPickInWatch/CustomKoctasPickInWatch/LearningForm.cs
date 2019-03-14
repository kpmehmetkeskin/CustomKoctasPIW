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
    public partial class LearningForm : Form
    {
        MotionDetector detector = new MotionDetector(
            new TwoFramesDifferenceDetector(),
            new BlobCountingObjectsProcessing()
            { HighlightColor = Color.Red, MinObjectsHeight = 50, MinObjectsWidth = 50, HighlightMotionRegions = true });

        CustomBarcodeReader customBarcodeReader = null;
        VideoCaptureDevice FinalVideoSource;
        FilterInfoCollection VideoCaptuerDevices;

        public LearningForm()
        {
            InitializeComponent();
        }

        private void LearningForm_Load(object sender, EventArgs e)
        {
            InitializeCamera();
            customBarcodeReader = new CustomBarcodeReader();
        }

        public void InitializeCamera()
        {
            VideoCaptuerDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            FinalVideoSource = new VideoCaptureDevice(VideoCaptuerDevices[1].MonikerString);
            FinalVideoSource.NewFrame += FinalVideoSource_NewFrame;
            FinalVideoSource.Start();
        }

        private void FinalVideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            String barcode = customBarcodeReader.getBarcodeFromCamera(image);

            if (!barcode.Equals("") && !StaticVariables.objectBarcodeList.Contains(barcode) && barcode.Length > 7)
            {
                StaticVariables.objectBarcodeList.Add(barcode);
                setBarcodeToListbox(barcode);
            }

            if (detector.ProcessFrame(image) > 1)
                pictureBox1.Image = image;
            pictureBox1.Image = image;

           
            
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (StaticVariables.objectBarcodeList.Count < 1 )
                return;

            for (int i = 0; i < StaticVariables.objectBarcodeList.Count; i++)
            {
                int rectangleCount = StaticVariables.objectBarcodeList.Count;
                int rectangleWidth = Constants.width / rectangleCount;
                int rectangleXLocation = (i * rectangleWidth);

                Rectangle ee = new Rectangle(rectangleXLocation, 0, rectangleWidth, 400);
                using (Pen pen = new Pen(Color.Blue, 4))
                {
                    e.Graphics.DrawRectangle(pen, ee);
                }
                using (Font myFont = new Font("Arial", 14))
                {
                    e.Graphics.DrawString(StaticVariables.objectBarcodeList[i].ToString(), myFont, Brushes.Red, new Point(rectangleXLocation, 380));
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String barcodeItem = listBox1.GetItemText(listBox1.SelectedItem);
            listBox1.Items.Remove(barcodeItem);
            StaticVariables.objectBarcodeList.Remove(barcodeItem);
        }

        private void setBarcodeToListbox(String barcode)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                listBox1.Items.Add(barcode);
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FinalVideoSource.Stop();
            this.Dispose();
            this.Close();
            ExecutingForm ef = new ExecutingForm();
            ef.Show();
        }

        private void LearningForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
