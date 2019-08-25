namespace WhatsON.Plugins.Travis
{
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
      foreach(var job in jobs)
      {
        projects.Add(new Project() {Name = job.Name, Address = job.Url });
      }

      return projects;
    }
  }
}
