
namespace WhatsON.Plugins.Travis
{
  using Soloplan.WhatsON;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Web;
  using WhatsON.Plugins.Travis.Model;

  public static class TravisAPI
  {
    public const string OpenSourceAPI_Url = "https://api.travis-ci.com/v3/";
    public const string OpenSourceUrl = "https://travis-ci.com/";
    public const string ENV_TRAVIS_AUTH_TOKEN = "TRAVIS_AUTH_TOKEN";

    /// <summary>
    /// In order to perform the API calls against the real Travis CI server, you need to have the environment variable TRAVIS_AUTH_TOKEN set for your user to a valid authentication token.
    /// For details on how the tokens work, please see https://developer.travis-ci.com/authentication.
    /// </summary>
    /// <returns></returns>
    public static string FetchTravisToken()
    {
      var token = Environment.GetEnvironmentVariable(ENV_TRAVIS_AUTH_TOKEN, EnvironmentVariableTarget.User);
      if (string.IsNullOrWhiteSpace(token))
      {
        throw new ArgumentException($"Could not fetch the Travis CI token from the environment variables of the user. Make sure that the environment variable {ENV_TRAVIS_AUTH_TOKEN} is set to a valid Travis CI API token (see https://developer.travis-ci.com/authentication).");
      }

      return token;
    }

    public static async Task<TravisJob> GetTravisJob(string slugOrId, string branch)
    {
      return null;
    }

    /// <summary>
    /// This method filters out only jobs that still exist on GitHub.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async Task<IList<TravisJob>> GetTravisJobs(string address)
    {
      if (string.IsNullOrWhiteSpace(address))
      {
        return null;
      }

      if (!address.StartsWith(OpenSourceUrl))
      {
        return null;
      }

      var slug = address.Substring(OpenSourceUrl.Length);
      if (!slug.Contains("/"))
      {
        throw new ArgumentException($"Invalid Travis CI Url format. Make sure to include the organization and project! Accepted format: {OpenSourceUrl}/ORGANIZATION/PROJECT");
      }

      var split = slug.Split('/');
      var encodedSlug = HttpUtility.UrlEncode($"{split[0]}/{split[1]}");

      var url = $"{OpenSourceAPI_Url}repo/{encodedSlug}/branches";
      var jobs = await SerializationHelper.GetJsonModel<TravisJobs>(url, default, (request) => ApplyAuthorizationHeader(request, FetchTravisToken()));
      if (jobs == null || jobs.Jobs == null)
      {
        return new List<TravisJob>();
      }

      return jobs.Jobs.Where(x => x.Exists).ToList();
    }

    private static void ApplyAuthorizationHeader(WebRequest request, string token)
    {
      request.Headers.Add(HttpRequestHeader.Authorization, $"token {token}");
    }
  }
}
