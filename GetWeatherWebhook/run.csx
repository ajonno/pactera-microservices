#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"
using Microsoft.WindowsAzure.Storage.Table;

using System;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    if (data.city == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Please pass 'city' property in the json req. body object"
        });
    }
    
    string OPENWEATHER_KEY = GetEnvironmentVariable("OPENWEATHER_KEY");
    string OPENWEATHER_BASEURL = GetEnvironmentVariable("OPENWEATHER_BASEURL");

//http://api.openweathermap.org/data/2.5/weather
    string URL = $"{OPENWEATHER_BASEURL}?q={data.city}&appid={OPENWEATHER_KEY}";
    log.Info($"url = {URL}");

    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri(URL);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    HttpResponseMessage response = client.GetAsync(URL).Result;

    var responseData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult().ToString();

    OpenWeather weatherResponse = JsonConvert.DeserializeObject<OpenWeather>(responseData);

    return req.CreateResponse(HttpStatusCode.OK, new {
        weatherData = weatherResponse
    });
}


public class Coord
{
    public double lon { get; set; }
    public double lat { get; set; }
}

public class OpenWeather
{
    public List<Weather> weather { get; set; }
    public string @base {get; set;}
    public object main {get; set;}
    public int visibility {get; set;}
    public object wind {get; set;}
    public object clouds {get; set;}
    public object sys {get; set;}
    public string name {get; set;}

}
public class Weather
{
    public int id { get; set; }
    public string main { get; set; }
    public string description { get; set; }
    public string icon { get; set; }
}

public static string GetEnvironmentVariable(string name)
{
    return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
}