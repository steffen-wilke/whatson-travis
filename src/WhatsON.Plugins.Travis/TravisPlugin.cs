namespace WhatsON.Plugins.Travis
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Soloplan.WhatsON.Composition;
  using Soloplan.WhatsON.Configuration;
  using Soloplan.WhatsON.Model;

  public class TravisPlugin : ConnectorPlugin
  {
    public TravisPlugin()
      : base(typeof(TravisConnector))
    {
    }

    public override void Configure(Project project, IConfigurationItemProvider configurationItemsSupport, string serverAddress)
    {
      throw new System.NotImplementedException();
    }

    public override Connector CreateNew(ConnectorConfiguration configuration)
    {
      throw new System.NotImplementedException();
    }

    public override async Task<IList<Project>> GetProjects(string address)
    {
      var jobs = await TravisAPI.GetTravisJobs(address);
      var projects = new List<Project>();
      foreach (var job in jobs)
      {
        projects.Add(new Project() { Name = job.Name, Address = job.Url });
      }

      return projects;
    }

    public override void OnStartup(string[] args)
    {
      if (args.Length == 0)
      {
        return;
      }

      for (var i = 0; i < args.Length; i++)
      {
        var arg = args[i];
        if (string.IsNullOrWhiteSpace(arg))
        {
          continue;
        }

        // start WhatsON like: Soloplan.WhatsON.GUI.exe -env:TRAVIS_AUTH_TOKEN=YOUR_TRAVIS_TOKEN to set the authentication from the start parameters
        // otherwise, the TravisAPI will try to fetch this informations from the environment variables
        if (arg.StartsWith($"-env:{TravisAPI.ENV_TRAVIS_AUTH_TOKEN}=", StringComparison.InvariantCultureIgnoreCase))
        {
          TravisAPI.TRAVIS_AUTH_TOKEN_FROM_ARGS = arg.Split('=')[1];
          break;
        }
      }

      base.OnStartup(args);
    }
  }
}
