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

			manager.Subscribe<SteamClient.LoggedOnCallback>(OnLoggedOn);
			manager.Subscribe<SteamClient.LoggedOffCallback>(OnLoggedOff);

			Console.WriteLine("Connecting to Steam...");

			isRunning = true;

			while(isRunning)
			{
				manager.RunWaitCallbacks(TimeSpan.FromSeconds(1));
			}
		}

		void OnConnected(SteamClient.ConnectedCallback callback)
		{

		}

		void OnDisonnected(SteamClient.DisconnectedCallback callback)
		{

		}

		void OnLoggedOn(SteamUser.LoggedOnCallback callback)
		{

		}

		void OnLoggedOff(SteamUser.LoggedOffCallback callback)
		{

		}

	}
}
