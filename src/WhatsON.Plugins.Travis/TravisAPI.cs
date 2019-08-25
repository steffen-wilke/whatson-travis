
namespace WhatsON.Plugins.Travis
{
  using Soloplan.WhatsON;
  using Soloplan.WhatsON.Model;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Threading.Tasks;
  using System.Web;
  using WhatsON.Plugins.Travis.Model;

  public static class TravisAPI
  {
    public const string OpenSourceAPI_Url = "https://api.travis-ci.com/v3/";
    public const string OpenSourceUrl = "https://travis-ci.com/";
    public const string ENV_TRAVIS_AUTH_TOKEN = "TRAVIS_AUTH_TOKEN";

    public static string TRAVIS_AUTH_TOKEN_FROM_ARGS;

    public static async Task<TravisJob> GetJob(string owner, string repository, string branch)
    {
      var encodedSlug = HttpUtility.UrlEncode($"{owner}/{repository}");

      var url = $"{OpenSourceAPI_Url}repo/{encodedSlug}/branch/{branch}";
      return await SerializationHelper.GetJsonModel<TravisJob>(url, default, (request) => ApplyAuthorizationHeader(request));
    }
    /// <summary>
    /// This method filters out only jobs that still exist on GitHub.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static async Task<IList<TravisJob>> GetJobs(string address)
    {
      var slug = GetSlug(address);
      if (string.IsNullOrWhiteSpace(slug))
      {
        return new List<TravisJob>();
      }

      var organization = GetOwnerName(slug);
      var repository = GetRepositoryName(slug);

      if (string.IsNullOrEmpty(organization) || string.IsNullOrEmpty(repository))
      {
        throw new ArgumentException($"Invalid Travis CI Url format. Make sure to include the owner and the project! Accepted format: {OpenSourceUrl}/OWNER/PROJECT");
      }

      return await GetJobs(organization, repository);
    }

    public static async Task<IList<TravisJob>> GetJobs(string owner, string repository)
    {
      var encodedSlug = HttpUtility.UrlEncode($"{owner}/{repository}");

      var url = $"{OpenSourceAPI_Url}repo/{encodedSlug}/branches?exists_on_github=true";
      var jobs = await SerializationHelper.GetJsonModel<TravisJobs>(url, default, ApplyAuthorizationHeader);
      if (jobs == null || jobs.Jobs == null)
      {
        return new List<TravisJob>();
      }

      return jobs.Jobs;
    }

    public static async Task<TravisBuild> GetBuild(int buildId)
    {
      var url = $"{OpenSourceAPI_Url}build/{buildId}";
      return await SerializationHelper.GetJsonModel<TravisBuild>(url, default, ApplyAuthorizationHeader);
    }

    public static async Task<IList<TravisBuild>> GetBuilds(string owner, string repository, string branch, int limit = Connector.MaxSnapshots)
    {
      var encodedSlug = HttpUtility.UrlEncode($"{owner}/{repository}");

      var url = $"{OpenSourceAPI_Url}repo/{encodedSlug}/builds?branch.name={branch}&sort_by=number:desc&event_type=push&limit={limit}";
      var builds = await SerializationHelper.GetJsonModel<TravisBuilds>(url, default, ApplyAuthorizationHeader);
      if (builds == null || builds.Builds == null)
      {
        return new List<TravisBuild>();
      }

      return builds.Builds;
    }

    public static async Task<IList<TravisRepository>> GetRepositories(string address)
    {
      var slug = GetSlug(address);
      if (string.IsNullOrWhiteSpace(slug))
      {
        return new List<TravisRepository>();
      }

      var organization = GetOwnerName(slug);
      if (string.IsNullOrEmpty(organization))
      {
        throw new ArgumentException($"Invalid Travis CI Url format. Make sure to include the owner! Accepted format: {OpenSourceUrl}/OWNER");
      }

      var url = $"{OpenSourceAPI_Url}owner/{organization}/repos?active=true";
      var repositories = await SerializationHelper.GetJsonModel<TravisRepositories>(url, default, ApplyAuthorizationHeader);
      if (repositories == null || repositories.Repositories == null)
      {
        return new List<TravisRepository>();
      }

      return repositories.Repositories;
    }

    public static string GetSlug(string address)
    {
      if (string.IsNullOrWhiteSpace(address) || !address.StartsWith(OpenSourceUrl))
      {
        return null;
      }

      var rawSlug = address.Substring(OpenSourceUrl.Length);
      return $"{GetOwnerName(rawSlug)}/{GetRepositoryName(rawSlug)}";
    }

    public static string GetOwnerName(string slug)
    {
      if (!slug.Contains("/"))
      {
        return slug;
      }

      return slug.Split('/')[0];
    }

    public static string GetRepositoryName(string slug)
    {
      if (!slug.Contains("/"))
      {
        return null;
      }

      return slug.Split('/')[1];
    }

    private static void ApplyAuthorizationHeader(WebRequest request)
    {
      request.Headers.Add(HttpRequestHeader.Authorization, $"token {FetchTravisToken()}");
    }

    /// <summary>
    /// In order to perform the API calls against the real Travis CI server, you need to have the environment variable TRAVIS_AUTH_TOKEN set for your user to a valid authentication token.
    /// For details on how the tokens work, please see https://developer.travis-ci.com/authentication.
    /// </summary>
    /// <returns></returns>
    private static string FetchTravisToken()
    {
      if (TRAVIS_AUTH_TOKEN_FROM_ARGS != null)
      {
        return TRAVIS_AUTH_TOKEN_FROM_ARGS;
      }

      var token = Environment.GetEnvironmentVariable(ENV_TRAVIS_AUTH_TOKEN, EnvironmentVariableTarget.User);
      if (string.IsNullOrWhiteSpace(token))
      {
        throw new ArgumentException($"Could not fetch the Travis CI token from the environment variables of the user. Make sure that the environment variable {ENV_TRAVIS_AUTH_TOKEN} is set to a valid Travis CI API token (see https://developer.travis-ci.com/authentication).");
      }

      return token;
    }
  }
}
