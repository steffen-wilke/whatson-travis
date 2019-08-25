using Newtonsoft.Json;
using Soloplan.WhatsON.Model;
using System;

namespace WhatsON.Plugins.Travis.Model
{
  public class TravisBuild
  {
    public int Id { get; set; }

    [JsonProperty("number")]
    public int BuildNumber { get; set; }

    /// <summary>
    /// Used for later conversion to WhatsON ObservationState.
    /// </summary>
    public string State { get; set; }

    [JsonProperty("previous_state")]
    public string PreviousState { get; set; }

    [JsonProperty("started_at")]
    public DateTime Started { get; set; }

    [JsonProperty("finished_at")]
    public DateTime Finished { get; set; }


    [JsonProperty("@href")]
    public string Url { get; set; }

    /// <summary>
    /// The duration in seconds.
    /// </summary>
    public int Duration { get; set; }

    public TravisRepository Repository { get; set; }

    [JsonProperty("branch")]
    public TravisJob Job { get; set; }

    [JsonProperty("created_by")]
    public User Culprit { get; set; }

    public TravisCommit Commit { get; set; }

    public ObservationState GetState()
    {
      if (string.IsNullOrWhiteSpace(this.State))
      {
        return this.Finished != null ? ObservationState.Running : ObservationState.Unknown;
      }

      if (this.State.Equals("passed", StringComparison.InvariantCultureIgnoreCase))
      {
        return ObservationState.Success;
      }

      return ObservationState.Unknown;
    }
  }
}
