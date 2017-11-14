using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Rakuten.Controllers
{
    public class RakutenProductSearchController : Controller
    {

        private const string _RakutenUsername = "YOUR_RAKUTEN_USERNAME";
        private const string _RakutenPassword = "YOUR_RAKUTEN_PASSWORD";
        private const string _RakutenSiteId = "YOUR_RAKUTEN_SITEID";
        private const string _RakutenAccessKey = "RAKUTEN_ACCESS_KEY_GENERATED_FROM_WEBSERVICE_PAGE";
        private const string _AccessTokenUrl = "https://api.rakutenmarketing.com/token?grant_type=password&username={0}&password={1}&scope={2}";
        private const string _ProductSearchUrl = "https://api.rakutenmarketing.com/productsearch/1.0?keyword={0}";
       
        #region "Rakuten"

        public ProductCatalogRakuten ProductSearch(string keyword)
        {
            var url = string.Format(_ProductSearchUrl, keyword);
            var authorizationToken = GetAccessToken();
            var responseText = GetResponse(url, "GET", authorizationToken);

            ProductCatalogRakuten result = new ProductCatalogRakuten();

            XmlSerializer serializer = new XmlSerializer(typeof(ProductCatalogRakuten));
            using (TextReader reader = new StringReader(@responseText))
            {
                result = (ProductCatalogRakuten)serializer.Deserialize(reader);
            }

            return result;
        }

        private string GetAccessToken()
        {
            var url = string.Format(_AccessTokenUrl, _RakutenUsername, _RakutenPassword, _RakutenSiteId);
            var result = GetResponse(url, "POST", _RakutenAccessKey, "application/x-www-form-urlencoded");

            var tokenObject = JsonConvert.DeserializeObject<AccessToken>(result);

            return tokenObject.Token;
        }
        #endregion

        #region "Generic Call"

        private string GetResponse(string url, string method, string authorizationToken, string contentType = "")
        {
            WebRequest req = WebRequest.Create(@url);
            req.Method = method;
            req.Headers["Authorization"] = authorizationToken;
            if (!string.IsNullOrEmpty(contentType))
            {
                req.ContentType = contentType;
            }

            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            WebHeaderCollection header = resp.Headers;

            var encoding = ASCIIEncoding.ASCII;
            string responseText = string.Empty;
            using (var reader = new StreamReader(resp.GetResponseStream(), encoding))
            {
                responseText = reader.ReadToEnd();
            }
            resp.Close();
            return responseText;
        }

        #endregion
    }

    public class AccessToken
    {
        public string token_type { get; set; }

        public string expires_in { get; set; }

        public string refresh_token { get; set; }

        public string access_token { get; set; }

        public string Token
        {
            get
            {
                return string.Format("Bearer {0}", access_token);
            }
        }
    }

    [XmlRoot("result")]
    public class ProductCatalogRakuten
    {
        [XmlElement("TotalMatches")]
        public string TotalMatches { get; set; }

        [XmlElement("TotalPages")]
        public string TotalPages { get; set; }

        [XmlElement("PageNumber")]
        public string PageNumber { get; set; }

        [XmlElement("item")]
        public List<RakutenProducts> Products { get; set; }


    }

    public class RakutenProducts
    {
        [XmlElement("mid")]
        public string Mid { get; set; }

        [XmlElement("merchantname")]
        public string MerchantName { get; set; }

        [XmlElement("linkid")]
        public string LinkId { get; set; }

        [XmlElement("createdon")]
        public string CreatedOn { get; set; }

        [XmlElement("sku")]
        public string Sku { get; set; }

        [XmlElement("productname")]
        public string ProductName { get; set; }

        [XmlElement("category")]
        public ProductCatagory Catagory { get; set; }

        [XmlElement("price")]
        public string Price { get; set; }

        [XmlElement("saleprice")]
        public string SalePrice { get; set; }

        [XmlElement("description")]
        public Description Description { get; set; }

        [XmlElement("linkurl")]
        public string LinkUrl { get; set; }

        [XmlElement("imageurl")]
        public string ImageUrl { get; set; }
    }

    public class ProductCatagory
    {
        [XmlElement("primary")]
        public string Primary { get; set; }

        [XmlElement("secondary")]
        public string Secondary { get; set; }
    }

    public class Description
    {
        [XmlElement("short")]
        public string Short { get; set; }

        [XmlElement("long")]
        public string Long { get; set; }
    }
}