using System;
using System.Collections.Generic;
using System.Text;
using SteamKit2;

namespace steam_reminder_bot
{
	public class Bot
	{
		#region Variables

		private SteamClient steamClient = new SteamClient();
		private CallbackManager manager;
		private SteamUser steamUser;

		private string username, password;

		private bool isRunning;

		#endregion

		public void StartUp()
		{
			Console.Write("Username: ");

			username = Console.ReadLine();

			Console.Write("Password: ");

			password = Console.ReadLine();


			manager = new CallbackManager(steamClient);

			steamUser = steamClient.GetHandler<SteamUser>();

			manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
			manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

			manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
			manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);

			Console.WriteLine("Connecting to Steam...");

			isRunning = true;

			while(isRunning)
			{
				manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
			}

			Console.WriteLine("Bot ended successfully.");
		}

		void OnConnected(SteamClient.ConnectedCallback callback)
		{
			Console.WriteLine($"Successfully connected to Steam.\nLogging in {username}");

			steamUser.LogOn(new SteamUser.LogOnDetails
			{
				Username = username,
				Password = password
			});
		}

		void OnDisconnected(SteamClient.DisconnectedCallback callback)
		{
			Console.WriteLine($"Successfully disconnected from Steam.");

			isRunning = false;
		}

		void OnLoggedOn(SteamUser.LoggedOnCallback callback)
		{
			if(callback.Result != EResult.OK)
			{
				switch(callback.Result)
				{
					case EResult.AccountLogonDenied:
						Console.WriteLine("ERROR: Logon denied, SteamGuard required.");
						break;
					default:
						Console.WriteLine($"ERROR: Could not logon to Steam.\n{callback.Result}\n{callback.ExtendedResult}");
						break;
				}

				isRunning = false;
				return;
			}

			Console.WriteLine("Successfully logged on.");

			Console.Write("Continuing");

			for(int i = 0; i < 3; i++)
			{
				System.Threading.Thread.Sleep(650);
				Console.Write(".");
			}

			// Perform Actions

			steamUser.LogOff();
		}

		void OnLoggedOff(SteamUser.LoggedOffCallback callback)
		{
			Console.WriteLine($"Successfully logged off of Steam. {callback.Result}");
		}

	}
}
