using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace DAF.GetPrice
{
    public class Item
    {
        public string StoreName { get; set; }
        public string ProductName { get; set; }
        public string URL { get; set; }
        public string Pattern { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var sites = new List<Item>();

            sites.Add(new Item()
            {
                StoreName = "Walmart",
                ProductName = "Fone de Ouvido Sony Estéreo Intra-auricular MDR-EX15LP",
                URL = "https://www.walmart.com.br/produto/3079697/pr",
                Pattern = "<span class=\"payment-price\">(.*?)<\\/span>"
            });

            sites.Add(new Item()
            {
                StoreName = "Submarino",
                ProductName = "iPhone SE 64GB Cinza Espacial",
                URL = "http://www.submarino.com.br/produto/126546738/iphone-se-64gb-cinza-desbloqueado-ios-3g-4g-wi-fi-camera-12mp-apple",
                Pattern = "<span class=\"card-price\">(.*?)<\\/span>"
            });

            sites.Add(new Item()
            {
                StoreName = "Magazine Luiza",
                ProductName = "iPhone SE 64GB Cinza Espacial",
                URL = "http://www.magazineluiza.com.br/iphone-se-apple-64gb-cinza-espacial-4g-tela-4-retina-cam.-12mp-ios-9-proc.-chip-a9-touch-id/p/2162213/te/ipse/",
                //Pattern = "<span class=\"right-price\">(.*?)<\\/span>"
                Pattern = "<strong class=\"js-price-value\">(.*?)<\\/strong>"
            });

            sites.Add(new Item()
            {
                StoreName = "Fast Shop",
                ProductName = "iPhone SE 64GB Cinza Espacial",
                URL = "http://www.fastshop.com.br/loja/apple-iphone-ipad-ipod/apple-iphone-se/iphone-se-cinza-espacial-64gb-mlm62bz-a-fast",
                Pattern = "<span id=\"bestPhonePrice\">(.*?)<\\/span>"
            });

            sites.Add(new Item()
            {
                StoreName = "Casas bahia",
                ProductName = "iPhone SE 64GB Cinza Espacial",
                URL = "http://www.casasbahia.com.br/TelefoneseCelulares/Smartphones/iOSiPhone/iPhone-SE-Apple-com-64GB-Tela-4-iOS-9-Sensor-de-Impressao-Digital-Camera-iSight-12MP-Wi-Fi-3G-4G-GPS-MP3-Bluetooth-e-NFC-Cinza-Espacial-7990221.html",
                Pattern = "<i class=\"sale price\">(.*?)<\\/i>"
            });

            sites.Add(new Item()
            {
                StoreName = "Pontofrio",
                ProductName = "iPhone SE 64GB Cinza Espacial",
                URL = "http://www.pontofrio.com.br/TelefoneseCelulares/Smartphones/iOSiPhone/iPhone-SE-Apple-com-64GB-Tela-4-iOS-9-Sensor-de-Impressao-Digital-Camera-iSight-12MP-Wi-Fi-3G-4G-GPS-MP3-Bluetooth-e-NFC-Cinza-Espacial-7990221.html",
                Pattern = "<i class=\"sale price\">(.*?)<\\/i>"
            });

            sites.Add(new Item()
            {
                StoreName = "Girafa",
                ProductName = "iPhone SE 64GB Cinza Espacial",
                URL = "http://www.girafa.com.br/Telefonia/apple/iphone-apple-cinza-espacial-64gb-ios-9-a9-c-mera-12mp-e-touch-id.htm",
                Pattern = "<span class=\"price\">(.*?)<\\/span>"
            });

            Console.WriteLine("Start");
            Run2(sites);
            Console.WriteLine("Finish");
            Console.ReadLine();
        }

        static void Run2(List<Item> sites)
        {
            foreach (var i in sites)
            {
                Console.WriteLine();
                Console.WriteLine("------------------------------");
                Console.WriteLine("StoreName:  {0}", i.StoreName);
                Console.WriteLine("ProductName:  {0}", i.ProductName);

                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(i.URL);
                    var response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var receiveStream = response.GetResponseStream();
                        StreamReader readStream = null;

                        if (response.CharacterSet == null)
                            readStream = new StreamReader(receiveStream);
                        else
                            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                        string data = readStream.ReadToEnd();

                        var matches = Regex.Matches(data, i.Pattern);
                        Console.WriteLine("Matches found: {0}", matches.Count);

                        if (matches.Count > 0)
                        {
                            Console.WriteLine();

                            foreach (Match m in matches)
                                Console.WriteLine("Inner DIV: {0}", m.Groups[1]);
                        }

                        response.Close();
                        readStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex);
                }
                finally
                {

                }

                Console.WriteLine("------------------------------");
                Console.WriteLine();
            }
        }
    }
}
