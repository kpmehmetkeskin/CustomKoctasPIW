using System;
using System.Net;

namespace CustomKoctasPickInWatch
{
    class CheckVideoFromCDNService
    {
        public Boolean isAvailableVideoOnCDN(String barcode)
        {
            try
            {
                string urlAddress = "https://cdnh.koctas.com.tr/video/SenseIT_Project/" + barcode + ".mp4";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
