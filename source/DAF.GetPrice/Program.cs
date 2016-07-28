using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
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
                StoreName = "Submarino",
                ProductName = "iPhone SE 64GB Cinza Espacial",
                URL = "http://www.submarino.com.br/produto/126546738/iphone-se-64gb-cinza-desbloqueado-ios-3g-4g-wi-fi-camera-12mp-apple",
                Pattern = "<span class=\"card-price\">(.*?)<\\/span>"
            });

            sites.Add(new Item()
            {
                StoreName = "Americanas",
                ProductName = "iPhone SE 64GB Cinza Espacial",
                URL = "http://www.americanas.com.br/produto/126546738/iphone-se-64gb-cinza-desbloqueado-ios-3g-4g-wi-fi-camera-12mp-apple",
                Pattern = "<span itemprop=\"price/salesPrice\">(.*?)<\\/span>"
            });

            sites.Add(new Item()
            {
                StoreName = "Saraiva",
                ProductName = "iPhone SE 64GB Cinza Espacial",
                URL = "http://www.saraiva.com.br/iphone-se-64gb-cinza-espacial-apple-9320591.html",
                Pattern = "<span class=\"final-price\">(.*?)<\\/span>"
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
                URL = "http://www.fastshop.com.br/loja/celular-e-telefone/smartphone/iphone/iphone-se-cinza-espacial-64gb-mlm62bz-a-fast?cm_re=FASTSHOP%3aSub-departamento%3aiPhone-_-Vitrine+24-_-AEMLM62BZACNZ",
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
                Pattern = "<strong itemprop=\"price\">(.*?)<\\/strong>"
            });

            Execute(sites);
            Console.ReadLine();
        }

        static void PrepareEmail(string text)
        {
            var dtNow = DateTime.Now;

            var paramList = new List<Tuple<string, string>>
                        {
                            new Tuple<string, string>("PRM_ASSUNTO", "GPoP"),
                            new Tuple<string, string>("PRM_DATAHORA", string.Format("{0} {1}", dtNow.ToShortDateString(), dtNow.ToShortTimeString())),
                            new Tuple<string, string>("PRM_TEXTO", text)
                        };

            var emailTemplate = "DAF.GetPrice.Template.Teste.xml";
            var templateStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(emailTemplate);

            var emailData = EmailHelper.GetEMailData(templateStream, paramList.ToArray());

            try
            {
                //SendEmail(emailData);
            }
            catch (System.Net.Mail.SmtpFailedRecipientException ex)
            {

            }
        }

        static void SendEmail(EMailData emailData)
        {
            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("", ""),
                Timeout = 30000
            };

            var mail = new MailMessage { From = new MailAddress("") };

            mail.To.Add(new MailAddress(""));
            mail.Subject = emailData.Subject;
            mail.IsBodyHtml = true;
            mail.Body = emailData.Body;

            //ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            smtpClient.Send(mail);
        }

        static void Execute(List<Item> sites)
        {
            while (true)
            {
                var dtNow = DateTime.Now;
                Console.WriteLine();
                Console.WriteLine(" --- Started at {0} {1} ---", dtNow.ToShortDateString(), dtNow.ToShortTimeString());
                Console.WriteLine();

                foreach (var i in sites)
                {
                    Console.WriteLine();
                    Console.WriteLine("------------------------------");
                    Console.WriteLine("StoreName:  {0}", i.StoreName);
                    Console.WriteLine("ProductName:  {0}", i.ProductName);

                    try
                    {
                        var request = (HttpWebRequest)WebRequest.Create(i.URL);
                        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
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
                                {
                                    //Console.BackgroundColor = ConsoleColor.Blue;
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Inner DIV: {0}", m.Groups[1]);
                                    Console.ResetColor();
                                    PrepareEmail(m.Groups[1].Value);
                                }
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

                dtNow = DateTime.Now;
                Console.WriteLine();
                Console.WriteLine(" --- Finished at {0} {1} ---", dtNow.ToShortDateString(), dtNow.ToShortTimeString());
                Console.WriteLine();
                Console.WriteLine(" --- Waiting 10 minutes ---");
                Console.WriteLine();
                System.Threading.Thread.Sleep(600000); // 10 minutes - 600000
                //System.Threading.Thread.Sleep(1800000); // 30 minutes - 1800000
            }
        }
    }
}
