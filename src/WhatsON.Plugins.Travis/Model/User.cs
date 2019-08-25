namespace WhatsON.Plugins.Travis.Model
{
  using Newtonsoft.Json;

  public class User
  {
    public int Id { get; set; }

    [JsonProperty("login")]
    public string Name { get; set; }

    [JsonProperty("@href")]
    public string Url { get; set; }
  }
}
