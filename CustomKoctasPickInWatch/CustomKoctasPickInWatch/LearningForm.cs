using AForge.Video.DirectShow;
using AForge.Vision.Motion;
using CustomKoctasPickInWatch.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
        CheckVideoFromCDNService checkVideoFromCDNService;
        Utils utils;

        public LearningForm()
        {
            InitializeComponent();
        }

        private void LearningForm_Load(object sender, EventArgs e)
        {
            InitializeCamera();
            customBarcodeReader = new CustomBarcodeReader();
            checkVideoFromCDNService = new CheckVideoFromCDNService();
            utils = new Utils();
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

            if (barcode != null && 
                !barcode.Equals("") && 
                !StaticVariables.productDTOList.Any(x => x.barcode.Equals(barcode)))
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    button1.Enabled = false;
                }));
                StaticVariables.productDTOList.Add(new DTO.ProductDTO(barcode));
                setBarcodeToListbox(barcode);
            }

            if (detector.ProcessFrame(image) > 1)
                pictureBox1.Image = image;
            pictureBox1.Image = image;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (StaticVariables.productDTOList.Count < 1 )
                return;

            for (int i = 0; i < StaticVariables.productDTOList.Count; i++)
            {
                int rectangleCount = StaticVariables.productDTOList.Count;
                int rectangleWidth = Constants.width / rectangleCount;
                int rectangleXLocation = (i * rectangleWidth);

                Rectangle ee = new Rectangle(rectangleXLocation, 0, rectangleWidth, 400);
                using (Pen pen = new Pen(Color.Blue, 4))
                {
                    e.Graphics.DrawRectangle(pen, ee);
                }
                using (Font myFont = new Font("Arial", 14))
                {
                    e.Graphics.DrawString(StaticVariables.productDTOList[i].barcode, myFont, Brushes.Red, new Point(rectangleXLocation, 380));
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            String barcodeItem = listBox1.GetItemText(listBox1.SelectedItem);
            listBox1.Items.Remove(barcodeItem);
            StaticVariables.productDTOList.RemoveAll(x => x.barcode.Equals(barcodeItem));
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
            this.Close();
            ExecutingForm ef = new ExecutingForm();
            ef.Show();
        }

        private void LearningForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FinalVideoSource.Stop();
            this.Dispose();
            this.Close();
        }

        //  --------------------------------------------------------------------------------------------------

        private void downloadVideo(String barcode)
        {
            try
            {
                lbl_barcode.Text = barcode + ".mp4";
                string url = "https://cdnh.koctas.com.tr/video/SenseIT_Project/" + barcode + ".mp4";
                string filename = @"C:\\SenseIT_Videos\\" + barcode + ".mp4";

                using (WebClient wc = new WebClient())
                {
                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(new Uri(url), filename);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Video indirilirken bir hata oluştu!", "UYARI");
                deleteBarcodeFromEverywhere(barcode);
            }
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            String downloadingProductBarcode = StaticVariables.productDTOList.Where(x => x.status.Equals(ProductStatus.DOWNLOADING)).FirstOrDefault().barcode;
            progressBar1.Value = 0;
            lbl_barcode.Text = "..";
            if (e.Cancelled)
            {
                deleteBarcodeFromEverywhere(downloadingProductBarcode);
                MessageBox.Show("Video indirmesi iptal edildi!", "UYARI");
                return;
            }
            if (e.Error != null) // We have an error! Retry a few times, then abort.
            {
                deleteBarcodeFromEverywhere(downloadingProductBarcode);
                MessageBox.Show("Video indirilirken bir hata oluştu!", "UYARI");
                return;
            }
            StaticVariables.productDTOList.Where(x => x.status.Equals(ProductStatus.DOWNLOADING)).FirstOrDefault().status = ProductStatus.AVAILABLE_ON_DISK;
        }

        private void DownloadVideo_timer1_Tick(object sender, EventArgs e)
        {
            if (!StaticVariables.productDTOList.Any(x => x.status.Equals(ProductStatus.DOWNLOADING)) && StaticVariables.productDTOList.Any(x => x.status.Equals(ProductStatus.NOT_AVAILABLE)))
            {
                String toBeDownloadProductBarcode = StaticVariables.productDTOList.Where(x => x.status.Equals(ProductStatus.NOT_AVAILABLE)).FirstOrDefault().barcode;
                if (File.Exists(@"C:\\SenseIT_Videos\\" + toBeDownloadProductBarcode + ".mp4"))
                {
                    StaticVariables.productDTOList.Where(x => x.barcode.Equals(toBeDownloadProductBarcode)).FirstOrDefault().status = ProductStatus.AVAILABLE_ON_DISK;
                    return;
                }

                if (checkVideoFromCDNService.isAvailableVideoOnCDN(toBeDownloadProductBarcode))
                {
                    StaticVariables.productDTOList.Where(x => x.barcode.Equals(toBeDownloadProductBarcode)).FirstOrDefault().status = ProductStatus.DOWNLOADING;
                    downloadVideo(toBeDownloadProductBarcode);
                }
                else
                {
                    deleteBarcodeFromEverywhere(toBeDownloadProductBarcode);
                    MessageBox.Show(toBeDownloadProductBarcode + " barkodlu ürünün videosu CDN üzerinde mevcut değil!", "UYARI");
                }
            }

            if (StaticVariables.productDTOList.Count > 0 && StaticVariables.productDTOList.Count == StaticVariables.productDTOList.Where(x => x.status.Equals(ProductStatus.AVAILABLE_ON_DISK)).ToList().Count)
            {
                button1.Enabled = true;
            }
        }

        private void deleteBarcodeFromEverywhere(String barcode)
        {
            try
            {
                StaticVariables.productDTOList.RemoveAll(x => x.barcode.Equals(barcode));
                listBox1.Items.Remove(barcode);
                utils.DeleteFileIfExist(barcode);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
