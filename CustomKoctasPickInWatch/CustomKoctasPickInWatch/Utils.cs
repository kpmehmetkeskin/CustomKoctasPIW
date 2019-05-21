using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomKoctasPickInWatch
{
    public class Utils
    {
        public void DeleteFileIfExist(String barcode)
        {
            if (File.Exists(@"C:\\SenseIT_Videos\\" + barcode + ".mp4"))
            {
                File.Delete(@"C:\\SenseIT_Videos\\" + barcode + ".mp4");
            }
        }
    }
}
