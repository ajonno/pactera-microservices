#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"

#load "environmentVariables.csx"

using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("Invoking GetWeatherWebhook...");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    if (data.city == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Please pass 'city' property in the json req. body object"
        });
    }
    
    string OPENWEATHER_KEY = GetEnvironmentVariable("OPENWEATHER_KEY");
    string OPENWEATHER_BASEURL = GetEnvironmentVariable("OPENWEATHER_BASEURL");

    string URL = $"{OPENWEATHER_BASEURL}?q={data.city}&units=metric&appid={OPENWEATHER_KEY}";

    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri(URL);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    HttpResponseMessage response = client.GetAsync(URL).Result;

    var responseData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult().ToString();
    OpenWeather weatherResponse = JsonConvert.DeserializeObject<OpenWeather>(responseData);

    log.Info(weatherResponse.name); //<-- melb
    //{DateTime.Now}  Thursday 11:00 AM
    //Weather   Mostly Cloudy   weather[0].main
    log.Info(weatherResponse.weather[0].main); //<-- melb
    
    //Temperature  9°C
    log.Info(weatherResponse.main.temp); //<

    //Wind 32km/h
  //  log.Info(weatherResponse.wind.speed.ToString()); //


    return req.CreateResponse(HttpStatusCode.OK, new {
        weatherData = weatherResponse
    });
}



public class OpenWeather
{
    public List<Weather> weather { get; set; }
    public string @base {get; set;}
    public WeatherMain main {get; set;}
    public int visibility {get; set;}
    public WeatherWind wind {get; set;}
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

public class WeatherMain
{
    public string temp { get; set; }
    public string pressure { get; set; }
    public string humidity { get; set; }
    public string temp_min { get; set; }
    public string temp_max { get; set; }
    
}

public class WeatherWind
{
    public double speed { get; set; }
    public int deg { get; set; }
}


public class Coord
{
    public double lon { get; set; }
    public double lat { get; set; }
}

