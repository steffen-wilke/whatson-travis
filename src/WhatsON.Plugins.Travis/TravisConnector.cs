

namespace WhatsON.Plugins.Travis
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Soloplan.WhatsON.Composition;
  using Soloplan.WhatsON.Configuration;
  using Soloplan.WhatsON.Model;
  using WhatsON.Plugins.Travis.Model;

  [ConnectorType("Travis-CI", Description = "Retrieve the current status of a Travis project.")]
  [ConfigurationItem(nameof(Owner), typeof(string), Optional = false, Priority = 300)]
  [ConfigurationItem(nameof(Repository), typeof(string), Optional = false, Priority = 400)]
  [ConfigurationItem(nameof(Branch), typeof(string), Optional = false, Priority = 500)]
  [NotificationConfigurationItem(NotificationsVisbility, typeof(ConnectorNotificationConfiguration), Priority = 1600000000)]
  public class TravisConnector : Connector
  {
    public TravisConnector(ConnectorConfiguration configuration)
      : base(configuration)
    {
    }

    public string Owner
    {
      get => this.ConnectorConfiguration.GetConfigurationByKey(nameof(Owner)).Value;
      set => this.ConnectorConfiguration.GetConfigurationByKey(nameof(Owner)).Value = value;
    }

    public string Repository
    {
      get => this.ConnectorConfiguration.GetConfigurationByKey(nameof(Repository)).Value;
      set => this.ConnectorConfiguration.GetConfigurationByKey(nameof(Repository)).Value = value;
    }

    public string Branch
    {
      get => this.ConnectorConfiguration.GetConfigurationByKey(nameof(Branch)).Value;
      set => this.ConnectorConfiguration.GetConfigurationByKey(nameof(Branch)).Value = value;
    }

    protected override async Task ExecuteQuery(CancellationToken cancellationToken, params string[] args)
    {
      var job = await TravisAPI.GetJob(this.Owner, this.Repository, this.Branch);
      if (job?.LastBuild?.Id == 0)
      {
        return;
      }

      var latestBuild = await TravisAPI.GetBuild(job.LastBuild.Id);
      this.CurrentStatus = CreateStatus(latestBuild);
      if (this.Snapshots.Count == 0 && job.LastBuild.BuildNumber > 1)
      {
        foreach (var build in await TravisAPI.GetBuilds(this.Owner, this.Repository, this.Branch, MaxSnapshots))
        {
          var buildStatus = CreateStatus(build);
          this.AddSnapshot(buildStatus);
        }
      }
    }

    private Status CreateStatus(TravisBuild build)
    {
      var status = new TravisStatus()
      {
        Name = build.Commit?.Sha,
        Building = build.Finished == default,
        State = build.GetState(),
        BuildNumber = build.BuildNumber,
        Time = build.Started,
        Duration = new TimeSpan(0, 0, build.Duration),
      };

      return status;
    }
  }
}
