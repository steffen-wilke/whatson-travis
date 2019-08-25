namespace WhatsON.Plugins.Travis.Model
{
  using Newtonsoft.Json;
  using System;

  public class TravisCommit
  {
    public int Id { get; set; }
    public string Sha { get; set; }

    public string Message { get; set; }

    [JsonProperty("compare_url")]
    public string DiffUrl { get; set; }

    [JsonProperty("committed_at")]
    public DateTime CommittedAt { get; set; }
  }
}
