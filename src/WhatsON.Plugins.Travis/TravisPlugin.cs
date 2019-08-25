namespace WhatsON.Plugins.Travis
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
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
      configurationItemsSupport.GetConfigurationByKey(Connector.ServerAddress).Value = TravisAPI.OpenSourceUrl;
      configurationItemsSupport.GetConfigurationByKey(nameof(TravisConnector.Owner)).Value = TravisAPI.GetOwnerName(TravisAPI.GetSlug(serverAddress));
      configurationItemsSupport.GetConfigurationByKey(nameof(TravisConnector.Repository)).Value = TravisAPI.GetRepositoryName(TravisAPI.GetSlug(serverAddress));
      configurationItemsSupport.GetConfigurationByKey(nameof(TravisConnector.Branch)).Value = project.Name;
    }

    public override Connector CreateNew(ConnectorConfiguration configuration)
    {
      return new TravisConnector(configuration);
    }

    public override async Task<IList<Project>> GetProjects(string address)
    {
      var projects = new List<Project>();
      var owner = TravisAPI.GetOwnerName(TravisAPI.GetSlug(address));
      if (string.IsNullOrEmpty(owner))
      {
        return projects;
      }

      var repositoryFromAddress = TravisAPI.GetRepositoryName(TravisAPI.GetSlug(address));
      if (string.IsNullOrWhiteSpace(repositoryFromAddress))
      {
        // try to fetch all repositories for an organization
        var repositories = await TravisAPI.GetRepositories(address);
        foreach (var repository in repositories)
        {
          if (repository == null || string.IsNullOrWhiteSpace(repository.Name))
          {
            continue;
          }

          projects.Add(await AddProjects(owner, repository.Name));
        }
      }
      else
      {
        projects.Add(await AddProjects(owner, repositoryFromAddress));
      }

      return projects;
    }

    private static async Task<Project> AddProjects(string owner, string repository)
    {
      var jobs = await TravisAPI.GetJobs(owner, repository);
      if (!jobs.Any())
      {
        return null;
      }

      var parent = new Project() { Name = jobs.FirstOrDefault().Repository?.Name, Address = jobs.FirstOrDefault().Repository.Url };

      foreach (var job in jobs)
      {
        parent.Children.Add(new Project() { Name = job.Name, Address = job.Url });
      }

      return parent;
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
