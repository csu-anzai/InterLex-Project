namespace Interlex.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AkomaNtosoXml.Xslt.Core.Classes.Resolver;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class AknConvertService
    {
        private static readonly String convertApiUrl = "http://techno.eucases.eu:2698/api/convert/akomantoso/interlex";

        static AknConvertService()
        {
            AkomaNtosoPreProcessor.ReThrowExceptions = true;
        }

        public async Task<String> ConvertToHtmlAsync(int documentType /* 1 - case; 2 - meta */, String interlexJsonContent)
        {
            if (String.IsNullOrEmpty(interlexJsonContent))
            {
                return null;
            }

            using (var httpClient = new HttpClient())
            {
                var httpResponse = await httpClient.PostAsJsonAsync(convertApiUrl, new { documentType = documentType, jsonContent = interlexJsonContent });
                var jsonStr = await httpResponse.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
                var jsonDict = (JObject)JsonConvert.DeserializeObject(jsonStr);

                var aknXml = jsonDict["akomaNtosoXml"].Value<String>();
                var html = AkomaNtosoPreProcessor.ConvertToFragments(aknXml, new AkomaNtosoPreProcessorConfig()).OverAll;

                return html;
            }
        }
    }
}
