namespace WhatsON.Plugins.Travis.Model
{
  using Newtonsoft.Json;

  public class TravisRepository
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public string Slug { get; set; }

    [JsonProperty("@href")]
    public string Url { get; set; }
  }
}
