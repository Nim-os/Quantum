using System;
using System.Collections.Generic;
using System.Text;
using SteamKit2;

namespace steam_reminder_bot
{
	public class Bot
	{
		#region Variables

		#region Consts

		const string UnrecognisedMessage = "Unrecognised message, check that your command starts with ! and is typed correctly.\nType !help for a full list of commands!";

		#endregion

		private SteamClient steamClient = new SteamClient();
		private CallbackManager manager;
		private SteamUser steamUser;
		private SteamFriends steamFriends;

		private string username, password;

		private bool isRunning;

		private List<SteamID> admins;


		#endregion

		#region Setup and Teardown

		public void StartUp()
		{
			Console.Write("Username: ");

			username = Console.ReadLine();

			Console.Write("Password: ");

			password = GetHiddenConsoleInput();



			manager = new CallbackManager(steamClient);

			steamUser = steamClient.GetHandler<SteamUser>();

			steamFriends = steamClient.GetHandler<SteamFriends>();

			manager.Subscribe<SteamClient.ConnectedCallback>(OnConnected);
			manager.Subscribe<SteamClient.DisconnectedCallback>(OnDisconnected);

			manager.Subscribe<SteamUser.LoggedOnCallback>(OnLoggedOn);
			manager.Subscribe<SteamUser.LoggedOffCallback>(OnLoggedOff);
			manager.Subscribe<SteamUser.AccountInfoCallback>(OnAccountInfo);

			manager.Subscribe<SteamFriends.FriendsListCallback>(OnFriendsList);
			manager.Subscribe<SteamFriends.FriendMsgCallback>(OnFriendMsg);


			Console.WriteLine("Connecting to Steam...");

			steamClient.Connect();

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
		}

		void OnLoggedOff(SteamUser.LoggedOffCallback callback)
		{
			Console.WriteLine($"Successfully logged off of Steam. {callback.Result}");
		}

		#endregion

		void OnAccountInfo(SteamUser.AccountInfoCallback callback)
		{
			Console.WriteLine($"AccountInfo recieved. {callback.PersonaName} active.");

			steamFriends.SetPersonaState(EPersonaState.Online);
		}

		void OnFriendsList(SteamFriends.FriendsListCallback callback)
		{
			foreach (var friend in callback.FriendList)
			{
				Console.WriteLine($"Friend: {friend.SteamID}");

				if (friend.Relationship == EFriendRelationship.RequestRecipient)
				{
					steamFriends.AddFriend(friend.SteamID);
				}
			}
		}

		void OnFriendMsg(SteamFriends.FriendMsgCallback callback)
		{

			if(callback.EntryType == EChatEntryType.ChatMsg)
			{
				string msg = callback.Message;
				SteamID sender = callback.Sender;

				#region Message Handling

				if (msg.StartsWith("!"))
				{
					switch (msg)
					{
						case "!hello":
							SendChat(sender, "Hello!");
							break;
						case "!help":
							SendChat(sender, "Available Commands:\n" +
								"!ping, !reminder");
							break;
						case "!reminder":
							SendChat(sender, "Unfortunately that service is not set up yet:( Check back later!");
							break;
						case "!shutdown":
							SendChat(sender, "Goodnight...");
							steamUser.LogOff();
							break;
						case "!ping":
							SendChat(sender, "Pong!");
							break;
						default:
							SendChat(sender, UnrecognisedMessage);
							break;
					}
				}
				else if (msg.StartsWith("."))
				{
					if(false)
					{

					}
					else
					{
						SendChat(sender, "You do not have sufficient permissions to access this.");
					}
				}
				else
				{
					SendChat(sender, UnrecognisedMessage);
				}
				

				#endregion
			}

		}

		private void SendChat(SteamID sender, string message)
		{
			steamFriends.SendChatMessage(sender, EChatEntryType.ChatMsg, message);
		}

		public static string GetHiddenConsoleInput()
		{
			StringBuilder input = new StringBuilder();
			while (true)
			{
				var key = Console.ReadKey(true);
				if (key.Key == ConsoleKey.Enter)
				{
					break;
				}
				else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
				{
					input.Remove(input.Length - 1, 1);
				}
				else if (key.Key != ConsoleKey.Backspace)
				{
					input.Append(key.KeyChar);
				}
			}
			return input.ToString();
		}
	}
}
