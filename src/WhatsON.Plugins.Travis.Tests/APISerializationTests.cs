namespace Tests
{
  using System;
  using Newtonsoft.Json;
  using NUnit.Framework;
  using WhatsON.Plugins.Travis.Model;

  /// <summary>
  /// Ensures that the serialization matches Travis API v3 example data from the allmighty LITIengine.
  /// This data has been extracted from the Travis API playground: https://developer.travis-ci.com/explore.
  /// </summary>
  [TestFixture]
  public class APISerializationTests
  {
    private const string USER_JSON = @"{
      ""@type"":             ""user"",
      ""@href"":             ""/user/971191"",
      ""@representation"":   ""minimal"",
      ""id"":                971191,
      ""login"":             ""nightm4re94""
    }";

    private const string REPOSITORY_JSON = @"{
      ""@type"":           ""repository"",
      ""@href"":           ""/repo/5953577"",
      ""@representation"": ""minimal"",
      ""id"":              5953577,
      ""name"":            ""litiengine"",
      ""slug"":            ""gurkenlabs/litiengine""
    }";

    private const string COMMIT_JSON = @"{
      ""@type"":             ""commit"",
      ""@representation"":   ""minimal"",
      ""id"":                233465565,
      ""sha"":               ""faa092a03c2fb404c42bdedaab1e98bbfd973210"",
      ""ref"":               ""refs/heads/master"",
      ""message"":           ""Merge pull request #289 from CalvinMT/fix-lstfield-col-length\n\nFix 2D ListField accessing non-existent values"",
      ""compare_url"":       ""https://github.com/gurkenlabs/litiengine/compare/d9889661c56f...faa092a03c2f"",
      ""committed_at"":      ""2019-08-20T08:45:46Z""
    }";

    private const string BRANCH_JSON = @"{
    ""@type"":            ""branch"",
    ""@href"":            ""/repo/5953577/branch/master"",
    ""@representation"":  ""standard"",
    ""name"":             ""master"",
    ""repository"":       {
      ""@type"":           ""repository"",
      ""@href"":           ""/repo/5953577"",
      ""@representation"": ""minimal"",
      ""id"":              5953577,
      ""name"":            ""litiengine"",
      ""slug"":            ""gurkenlabs/litiengine""
    },
    ""default_branch"":   true,
    ""exists_on_github"": true,
    ""last_build"":       {
      ""@type"":               ""build"",
      ""@href"":               ""/build/123851153"",
      ""@representation"":     ""minimal"",
      ""id"":                  123851153,
      ""number"":              ""1870"",
      ""state"":               ""passed"",
      ""duration"":            185,
      ""event_type"":          ""push"",
      ""previous_state"":      ""passed"",
      ""pull_request_title"":  null,
      ""pull_request_number"": null,
      ""started_at"":          ""2019-08-20T08:46:21Z"",
      ""finished_at"":         ""2019-08-20T08:49:26Z"",
      ""private"":             false
      }
    }";

    private const string BUILD_JSON = @"{
      ""@type"":               ""build"",
      ""@href"":               ""/build/123851153"",
      ""@representation"":     ""standard"",
      ""@permissions"":        {
        ""read"":              true,
        ""cancel"":            true,
        ""restart"":           true
      },
      ""id"":                  123851153,
      ""number"":              ""1870"",
      ""state"":               ""passed"",
      ""duration"":            185,
      ""event_type"":          ""push"",
      ""previous_state"":      ""passed"",
      ""pull_request_title"":  null,
      ""pull_request_number"": null,
      ""started_at"":          ""2019-08-20T08:46:21Z"",
      ""finished_at"":         ""2019-08-20T08:49:26Z"",
      ""private"":             false,
      ""repository"":          {
        ""@type"":             ""repository"",
        ""@href"":             ""/repo/5953577"",
        ""@representation"":   ""minimal"",
        ""id"":                5953577,
        ""name"":              ""litiengine"",
        ""slug"":              ""gurkenlabs/litiengine""
      },
      ""branch"":              {
        ""@type"":             ""branch"",
        ""@href"":             ""/repo/5953577/branch/master"",
        ""@representation"":   ""minimal"",
        ""name"":              ""master""
      },
      ""tag"":                 null,
      ""commit"":              {
        ""@type"":             ""commit"",
        ""@representation"":   ""minimal"",
        ""id"":                233465565,
        ""sha"":               ""faa092a03c2fb404c42bdedaab1e98bbfd973210"",
        ""ref"":               ""refs/heads/master"",
        ""message"":           ""Merge pull request #289 from CalvinMT/fix-lstfield-col-length\n\nFix 2D ListField accessing non-existent values"",
        ""compare_url"":       ""https://github.com/gurkenlabs/litiengine/compare/d9889661c56f...faa092a03c2f"",
        ""committed_at"":      ""2019-08-20T08:45:46Z""
      },
      ""jobs"":                [
        {
          ""@type"":           ""job"",
          ""@href"":           ""/job/226599026"",
          ""@representation"": ""minimal"",
          ""id"":              226599026
        }
      ],
      ""stages"":              [ ],
      ""created_by"":          {
        ""@type"":             ""user"",
        ""@href"":             ""/user/971191"",
        ""@representation"":   ""minimal"",
        ""id"":                971191,
        ""login"":             ""nightm4re94""
      },
      ""updated_at"":          ""2019-08-20T08:49:26.797Z""
    }";

    [Test]
    public void CulpritTest()
    {
      var culprit = JsonConvert.DeserializeObject<Culprit>(USER_JSON);

      Assert.IsNotNull(culprit);
      Assert.AreEqual(971191, culprit.Id);
      Assert.AreEqual("nightm4re94", culprit.Name);
      Assert.AreEqual("/user/971191", culprit.Url);
    }

    [Test]
    public void RepositoryTest()
    {
      var repository = JsonConvert.DeserializeObject<TravisRepository>(REPOSITORY_JSON);

      Assert.IsNotNull(repository);
      Assert.AreEqual(5953577, repository.Id);
      Assert.AreEqual("/repo/5953577", repository.Url);
      Assert.AreEqual("litiengine", repository.Name);
      Assert.AreEqual("gurkenlabs/litiengine", repository.Slug);
    }

    [Test]
    public void CommitTest()
    {
      var commit = JsonConvert.DeserializeObject<TravisCommit>(COMMIT_JSON);

      Assert.IsNotNull(commit);
      Assert.AreEqual(233465565, commit.Id);
      Assert.AreEqual("faa092a03c2fb404c42bdedaab1e98bbfd973210", commit.Sha);
      Assert.AreEqual("Merge pull request #289 from CalvinMT/fix-lstfield-col-length\n\nFix 2D ListField accessing non-existent values", commit.Message);
      Assert.AreEqual("https://github.com/gurkenlabs/litiengine/compare/d9889661c56f...faa092a03c2f", commit.DiffUrl);
      Assert.AreEqual(new DateTime(2019, 8, 20, 8, 45, 46), commit.CommittedAt); // 2019-08-20T08:45:46Z
    }

    [Test]
    public void BranchTest()
    {
      var branch = JsonConvert.DeserializeObject<TravisJob>(BRANCH_JSON);

      Assert.IsNotNull(branch);
      Assert.AreEqual("master", branch.Name);
      Assert.AreEqual("/repo/5953577/branch/master", branch.Url);
      Assert.IsTrue(branch.IsDefault);
      Assert.IsTrue(branch.Exists);

      Assert.IsNotNull(branch.Repository);
      Assert.AreEqual("gurkenlabs/litiengine", branch.Repository.Slug);

      Assert.IsNotNull(branch.LastBuild);
      Assert.AreEqual(123851153, branch.LastBuild.Id);
      Assert.AreEqual("/build/123851153", branch.LastBuild.Url);
    }

    [Test]
    public void BuildTest()
    {
      var build = JsonConvert.DeserializeObject<TravisBuild>(BUILD_JSON);

      Assert.IsNotNull(build);
      Assert.AreEqual(123851153, build.Id);
      Assert.AreEqual(1870, build.BuildNumber);
      Assert.AreEqual(185, build.Duration);
      Assert.AreEqual("passed", build.State);
      Assert.AreEqual("passed", build.PreviousState);
      Assert.AreEqual("/build/123851153", build.Url);
      Assert.AreEqual(new DateTime(2019, 8, 20, 8, 46, 21), build.Started); // 2019-08-20T08:46:21Z
      Assert.AreEqual(new DateTime(2019, 8, 20, 8, 49, 26), build.Finished); // 2019-08-20T08:49:26Z

      Assert.IsNotNull(build.Repository);
      Assert.AreEqual("litiengine", build.Repository.Name);

      Assert.IsNotNull(build.Job);
      Assert.AreEqual("master", build.Job.Name);

      Assert.IsNotNull(build.Culprit);
      Assert.AreEqual("nightm4re94", build.Culprit.Name);

      Assert.IsNotNull(build.Commit);
      Assert.AreEqual("faa092a03c2fb404c42bdedaab1e98bbfd973210", build.Commit.Sha);
    }
  }
}