using Newtonsoft.Json;
using System.Collections.Generic;

namespace WhatsON.Plugins.Travis.Model
{
  public class TravisJobs
  {
    [JsonProperty("branches")]
    public IList<TravisJob> Jobs { get; set; }
  }
}
