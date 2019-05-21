using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CustomKoctasPickInWatch
{
    class DownloadVideoService
    {
        public void downloadVideoFromCdn(String barcode)
        {
            WebClient client = new WebClient();
            client.DownloadFile("https://cdnh.koctas.com.tr/video/SenseIT_Project/" + barcode + ".mp4", @"C:\\SenseIT_Videos\\" + barcode + ".mp4");
        }

        

        

    }
}
