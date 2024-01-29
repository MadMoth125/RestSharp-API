using CustomConsole;
using GenericParse;
using GitHub_API_Access.Classes;
using System;

namespace GitHub_API_Access
{
	/* Program requirement the following the proper functionality:
	 * RestSharp NuGet package 
	 * A Github "Personal Access Token" 
	 * 
	 * (Go to GitHub account settings -> developer settings -> personal access tokens -> tokens)
	 * 
	 * Once you are at the tokens page, make sure to generate a token with these permissions as a minimum:
	 *		repo:status
	 *		repo_deployment
	 *		public_repo
	 *		read:user
	 * 
	 * Note: Those exact permissions may not be required to get the program working, but they are what the testing token(s) used and they ran into no issues.
	 */
	internal class Program
	{
		private static string[] _menu1 = new string[] { "See repositories from user", "Change Github personal access token"};
		private static string[] _menu2 = new string[] { "Clear console" };
		private static string[] _menu3 = new string[] { "Exit program" };

		private static string _authToken = "";
		private static GithubApiClient apiClient;
		private static bool _consoleCluttered;

		private static bool _loopMain = true;

		static void Main(string[] args)
		{
			SetAuthToken();

			// force apiClient to be initialized if it hasn't been already
			if (apiClient == null) apiClient = new GithubApiClient(_authToken);

			while (_loopMain)
			{
				PrintMenu();
				SelectMenuOption();
			}

			ConsoleHelper.HaltProgram();
		}

		/// <summary>
		/// Displays all menu options in the console.
		/// </summary>
		private static void PrintMenu()
		{
			Console.WriteLine("User Repo Viewer (RestSharp w/ GitHub)");
			ConsoleHelper.PrintBlank();

			if (_consoleCluttered)
			{
				ConsoleHelper.PrintStrings(_menu1, _menu2, _menu3);
			}
			else
			{
				ConsoleHelper.PrintStrings(_menu1, _menu3);
			}
		}

		/// <summary>
		/// Waits for user input and calls SwitchOnMenuSelection(), passing the user's input as a parameter.
		/// </summary>
		private static void SelectMenuOption()
		{
			// looping until a valid option is selected
			while (true)
			{
				ConsoleHelper.PrintBlank();
				Console.Write("Select Option: ");
				int tempSelect = GenericReadLine.TryReadLine<int>();

				if (!SwitchOnMenuSelection(tempSelect)) break;
			}
		}

		/// <summary>
		/// Uses a switch statement to call the appropriate method based on the user's menu selection.
		/// </summary>
		/// <param name="selection">The user's menu selection</param>
		/// <returns>The desired loop state</returns>
		private static bool SwitchOnMenuSelection(int selection)
		{
			bool tempReturnValue = true;

			// clearing console and printing menu again to prevent clutter
			Console.Clear();
			PrintMenu();
			ConsoleHelper.PrintBlank();

			if (_consoleCluttered)
			{
				switch (selection)
				{
					case 1: // see repositories from user
						LoadUserRepositories().Wait();
						break;
					
					case 2: // change github personal access token
						Console.Clear();
						SetAuthToken();
						break;
					
					case 3: // clear console
						_consoleCluttered = false;
						Console.Clear();
						PrintMenu();
						ConsoleHelper.PrintBlank();
						break;
					
					case 4: // exit
						tempReturnValue = false;
						_loopMain = false;
						break;
				}
			}
			else
			{
				switch (selection)
				{
					case 1: // see repositories from user
						LoadUserRepositories().Wait();
						break;
					
					case 2: // change github personal access token
						Console.Clear();
						SetAuthToken();
						break;

					case 3: // exit
						tempReturnValue = false;
						_loopMain = false;
						break;

				}
			}

			// _consoleCluttered = true;
			return tempReturnValue;
		}

		private async static Task LoadUserRepositories()
		{
			// getting username from user
			Console.Write("Enter GitHub username: ");
			var tempUser = Console.ReadLine();

			// checking if username is valid
			if (string.IsNullOrWhiteSpace(tempUser))
			{
				Console.WriteLine("Invalid entry, please enter a valid username.\n");
				return;
			}

			// getting repositories from api
			var repos = await apiClient.GetUserRepositories(tempUser);

			if (repos == null || ConsoleHelper.ListEmpty(repos))
			{
				Console.WriteLine("No repositories found for that user.\n");
				return;
			}

			// set to true to flag it for menu printing
			_consoleCluttered = true;

			// clearing console and printing menu again
			Console.Clear();
			PrintMenu();
			ConsoleHelper.PrintBlank();

			// printing repositories
			foreach (var repo in repos)
			{
				Console.WriteLine($"Repository Name: {repo.Name}");
				Console.WriteLine($"Description: {repo.Description}");
				Console.WriteLine($"Repository URL: {repo.Html_Url}");
				
				// only adding a blank line if it's not the last repo.
				// this prevents an extra blank line from being added
				// to the end of the list for when we select a new menu option.
				if (repo != repos[^1]) ConsoleHelper.PrintBlank();
			}
		}

		private static void SetAuthToken()
		{
			do
			{
				Console.Write("Enter GitHub personal access token: ");
				_authToken = Console.ReadLine();

				if (string.IsNullOrWhiteSpace(_authToken))
				{
					Console.Clear();
					Console.WriteLine("Invalid entry, please enter a valid token length.\n");
				}
				else
				{
					apiClient = new GithubApiClient(_authToken);
					break;
				}
			}
			while (true);

			Console.Clear();
			PrintMenu();
		}
	}
}