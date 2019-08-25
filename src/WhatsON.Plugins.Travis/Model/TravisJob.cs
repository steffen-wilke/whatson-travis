namespace WhatsON.Plugins.Travis.Model
{
  using Newtonsoft.Json;

  /// <summary>
  /// In Travis, the WhatsON concept of "Jobs" is represented by branches. Actual Travis "Jobs" are something more fine granular that is not supported by the tool.
  /// Model created from the documentation: https://developer.travis-ci.com/explore/repo/${IDorSlug}/branch/master
  /// </summary>
  public class TravisJob
  {
    public string Name { get; set; }

    [JsonProperty("@href")]
    public string Url { get; set; }

    [JsonProperty("default_branch")]
    public bool IsDefault { get; set; }

    public TravisRepository Repository { get; set; }

    [JsonProperty("last_build")]
    public TravisBuild LastBuild { get; set; }
  }
}
