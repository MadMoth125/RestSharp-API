using RestSharp;

namespace GitHub_API_Access.Classes
{
	public class GithubApiClient
	{
		private readonly string projectName = "GitHub-API-Access";
		private readonly string apiBaseUrl = "https://api.github.com";
		private readonly string authToken;

		public GithubApiClient(string authToken)
		{
			this.authToken = authToken;
		}

		/// <summary>
		/// Gets a list of public repositories for a specified GitHub user.
		/// Runs asynchronously for best user experience.
		/// </summary>
		/// <param name="username">The GitHub username.</param>
		/// <returns>A list of Repository objects representing public repositories.</returns>
		public async Task<List<Repository>?> GetUserRepositories(string username)
		{
			return await Task.Run(() => 
			{
				// Create a RestSharp client to interact with the GitHub API's user repositories endpoint.
				var client = new RestClient($"{apiBaseUrl}/users/{username}/repos");

				// Create a RestSharp request to send to the GitHub API.
				var request = new RestRequest();

				// Add headers, including authorization and user agent
				request.AddHeader("Authorization", $"Bearer {authToken}");
				request.AddHeader("User-Agent", projectName);

				// Execute the request and deserialize the JSON response into a list of Repository objects
				var response = client.Execute<List<Repository>>(request);

				if (response.IsSuccessful)
				{
					return response.Data;
				}
				else
				{
					// Handle errors
					Console.WriteLine($"Error: {response.ErrorMessage}");
					return null;
				}
			});
		}

		/// <summary>
		/// Gets information about a specific GitHub repository.
		/// Runs asynchronously for best user experience.
		/// </summary>
		/// <param name="owner">The owner (username or organization) of the repository.</param>
		/// <param name="repoName">The name of the repository.</param>
		/// <returns>A Repository object representing the specified repository, or null if an error occurs.</returns>
		[Obsolete("Lame version of \"GetUserRepositories(string username)\"")]
		public async Task<Repository?> GetRepositoryInfo(string owner, string repoName)
		{
			return await Task.Run(() =>
			{
				// Create a RestSharp client to interact with the GitHub API's repository endpoint.
				var client = new RestClient($"{apiBaseUrl}/repos/{owner}/{repoName}");

				// Create a RestSharp request to send to the GitHub API.
				var request = new RestRequest();

				// Add headers, including authorization and user agent
				request.AddHeader("Authorization", $"Bearer {authToken}");
				request.AddHeader("User-Agent", projectName);

				// Execute the request and deserialize the JSON response into a Repository object
				var response = client.Execute<Repository>(request);

				if (response.IsSuccessful)
				{
					return response.Data;
				}
				else
				{
					// Handle errors
					Console.WriteLine($"Error: {response.ErrorMessage}");
					return null;
				}
			});
		}
	}
}
