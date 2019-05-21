using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomKoctasPickInWatch.DTO
{
    class ProductDTO
    {
        public ProductDTO()
        {

        }

        public ProductDTO(String barcode)
        {
            this.barcode = barcode;
            this.status = ProductStatus.NOT_AVAILABLE;
        }

        public String barcode { get; set; }
        public ProductStatus status { get; set; }
    }
}
