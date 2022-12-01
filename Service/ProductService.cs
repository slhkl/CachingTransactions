using Data.Model;
using System.Xml.Linq;

namespace Service
{
    public class ProductService
    {
        private static List<Product> _productList = new List<Product>()
        {
                new Product() { Id = 1, Name = "Salih", Description = "açıklama işte"},
                new Product() { Id = 2, Name = "Kenan", Description = "işte açıklama"}
        };

        public List<Product> GetProductList() { return _productList; }

        public void AddProduct(Product product) { _productList.Add(product); }
    }
}