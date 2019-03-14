using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace CustomKoctasPickInWatch
{
    class CustomBarcodeReader
    {
        public String getBarcodeFromCamera(Bitmap bitmap)
        {
            String barcode = "";
            IBarcodeReader reader = new BarcodeReader();
            var barcodeBitmap = bitmap;
            var result = reader.Decode(barcodeBitmap);
            if (result != null)
            {
                barcode = result.Text;
            }
            return barcode;
        }
    }
}

