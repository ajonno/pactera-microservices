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
    log.Info($"Webhook to get Weather data was just triggered..");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    //{trigger: true} body will ensure the function is run & calls Imagine's API
    if (data.city == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Please pass 'city' property in the json req. body object"
        });
    }
    
    log.Info($"Making POST request to openweatherap.org..");

    string URL = $"http://api.openweathermap.org/data/2.5/weather?q={data.city},uk&appid=c444b38cb798426821ad20f4391f2eb2";
    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri(URL);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    HttpResponseMessage response = client.GetAsync(URL).Result;

    var responseData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult().ToString();
    log.Info($"{DateTime.Now} ===> {responseData}");

    string json = @"{
      'Name': 'Bad Boys',
      'ReleaseDate': '1995-4-7T00:00:00',
      'Genres': [
        'Action',
        'Comedy'
      ]
    }";

    Movie m = JsonConvert.DeserializeObject<Movie>(json);

    OpenWeather w = JsonConvert.DeserializeObject<OpenWeather>(responseData);



    return req.CreateResponse(HttpStatusCode.OK, new {
        reply = w
        //$"{responseData}"
    });
}

public class Movie {
    public string Name {get; set;}
}


public class Coord
{
    public double lon { get; set; }
    public double lat { get; set; }
}

public class OpenWeather
{
    //public Coord coord { get; set; }
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
