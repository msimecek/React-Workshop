using System.Net;
using System.Net.Http.Headers;
using System.Text;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{   
    var image = await req.Content.ReadAsStreamAsync();
    
    var emotions = await GetEmotions(image, log);

    if (string.IsNullOrEmpty(emotions)) {
        return req.CreateResponse(HttpStatusCode.BadRequest);
    }

    var res = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
    res.Content = new StringContent(emotions, Encoding.UTF8, "application/json");

    return res;
}

static async Task<string> GetEmotions(Stream image, TraceWriter log) 
{
    using (var client = new HttpClient())
    {
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("EmotionApiKey"));

        var content = new StreamContent(image);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        var httpResponse = await client.PostAsync("https://api.projectoxford.ai/emotion/v1.0/recognize", content);
 
        if (httpResponse.StatusCode == HttpStatusCode.OK){
            return await httpResponse.Content.ReadAsStringAsync();
        }
        else {
            log.Error(await httpResponse.Content.ReadAsStringAsync());
        }
    }

    return null;
}
