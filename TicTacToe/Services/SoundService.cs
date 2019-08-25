using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Plugin.SimpleAudioPlayer;

namespace TicTacToe.Services
{
	//Singleton
	class SoundService
	{
		private static readonly SoundService instance = new SoundService();
		Dictionary<string, ISimpleAudioPlayer> NamedAudioPlayers = new Dictionary<string, ISimpleAudioPlayer>();

		static SoundService()
		{
		}

		private SoundService()
		{
		}

		public static SoundService Instance
		{
			get
			{
				return instance;
			}
		}

		public void PlaySound(string filename)
		{
			ISimpleAudioPlayer player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
			player.Load(GetStreamFromFile(filename));
			player.Play();
		}

		private void Player_PlaybackEnded(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		// Creates a player for a sound that will be played multiple times and
		// eliminates the lag because we don't have to load the sound from the
		// Embedded Resources each time it's played.
		// Caller needs to call Play() on the player object to play the sound.
		// Player object should be disposed when no longer needed, perhaps in
		// the OnDisappearing override for the page.
		public ISimpleAudioPlayer CreateAudioPlayer(string filename)
		{
			var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
			player.Load(GetStreamFromFile(filename));
			return player;
		}

		// This creates a named audio player and keeps it in a Dictionary
		// The functions PlayNamedSound and DeleteNamedSound use that dictionary
		public void CreateNamedAudioPlayer(string name, string filename)
		{
			var player = CreateAudioPlayer(filename);
			NamedAudioPlayers.Add(name, player);
		}

		public void PlayNamedAudioPlayer(string name)
		{
			var player = NamedAudioPlayers[name];
			if (player != null)
			{
				player.Play();
			}
		}

		public void DeleteNamedAudioPlayer(string name)
		{
			NamedAudioPlayers.Remove(name);
		}

		private Stream GetStreamFromFile(string filename)
		{
			var assembly = typeof(App).GetTypeInfo().Assembly;
			//foreach (var res in assembly.GetManifestResourceNames())
			//{
			//   Debug.WriteLine($"Found: {res}");
			//}
			var stream = assembly.GetManifestResourceStream(Constants.resourcePrefix + filename);
			return stream;
		}
	}
}
