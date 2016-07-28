using System;
using System.IO;
using System.Xml.Serialization;

namespace DAF.GetPrice
{
    public class EmailHelper
    {
        /// <exception cref="System.ArgumentNullException">Se templateStream for nulo</exception>
        public static EMailData GetEMailData(Stream templateStream, params Tuple<String, String>[] aParams)
        {
            if (((Object)templateStream) == null) throw new ArgumentNullException("templateStream");

            var vEMailData = (EMailData)new XmlSerializer(typeof(EMailData)).Deserialize(templateStream);

            // substituo cada tupla passada por parâmetro no template informado por parâmetro
            foreach (var tuple in aParams)
            {
                if (String.IsNullOrEmpty(tuple.Item1) || String.IsNullOrEmpty(tuple.Item2))
                    throw new Exception("Tuple cannot have an empty value");
                vEMailData.Subject = vEMailData.Subject.Replace("<%" + tuple.Item1.ToUpper() + "%>", tuple.Item2);
                vEMailData.Body = vEMailData.Body.Replace("<%" + tuple.Item1.ToUpper() + "%>", tuple.Item2);
            }
            return vEMailData;
        }
    }
}
