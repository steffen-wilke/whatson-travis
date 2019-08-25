

namespace WhatsON.Plugins.Travis
{
  using System.Threading;
  using System.Threading.Tasks;
  using Soloplan.WhatsON.Composition;
  using Soloplan.WhatsON.Configuration;
  using Soloplan.WhatsON.Model;


  [ConnectorType("Travis", Description = "Retrieve the current status of a Travis project.")]
  public class TravisConnector : Connector
  {
    public TravisConnector(ConnectorConfiguration configuration)
      : base(configuration)
    {
    }

    protected override Task ExecuteQuery(CancellationToken cancellationToken, params string[] args)
    {
      throw new System.NotImplementedException();
    }
  }
}
