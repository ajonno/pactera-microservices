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

    //time
    DateTime time = DateTime.Now;         
    var formattedTime = time.ToString("dddd HH:mm tt");  //eg. Monday 05:30 AM

    //convert epoch time from data response payload, to AEST
    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    var final = epoch.AddSeconds(weatherResponse.dt);
    TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
    DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(final, cstZone);
    var formattedcstTime = cstTime.ToString("dddd HH:mm tt");
    
    //creating an anonymous type to hold the required payload/field data
    var payload = new object[] {
        new {field = "City", val = weatherResponse.name},
        new {field = "Updated Time", val = formattedcstTime},
        new {field  = "Weather", val = weatherResponse.weather[0].main},
        new {field = "Temperature", val = weatherResponse.main.temp + " " + DEGREES_CELCIUS},
        new {field = "Wind", val = weatherResponse.wind.speed + " " + KM_PER_HR}
    };  

    return req.CreateResponse(HttpStatusCode.OK, new {
        payload                     
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
    public long dt {get; set;}
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
    public string speed { get; set; }
    public double deg { get; set; }
}


public class Coord
{
    public double lon { get; set; }
    public double lat { get; set; }
}

