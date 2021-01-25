using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Nini.Config;

namespace SceneNavi
{
	/* Based on http://stackoverflow.com/a/15869868 */

	static class Configuration
	{
		static readonly string ConfigName = "Main";

		static string configPath;
		static string configFilename;

		static IConfigSource source;

		public static string FullConfigFilename { get { return Path.Combine(configPath, configFilename); } }

		public static string UpdateServer
		{
			get { return source.Configs[ConfigName].GetString("UpdateServer", string.Format("http://magicstone.de/dzd/progupdates/{0}.txt", System.Windows.Forms.Application.ProductName)); }
			set { source.Configs[ConfigName].Set("UpdateServer", value); }
		}

		public static bool RenderRoomActors
		{
			get { return source.Configs[ConfigName].GetBoolean("RenderRoomActors", true); }
			set { source.Configs[ConfigName].Set("RenderRoomActors", value); }
		}

		public static bool RenderSpawnPoints
		{
			get { return source.Configs[ConfigName].GetBoolean("RenderSpawnPoints", true); }
			set { source.Configs[ConfigName].Set("RenderSpawnPoints", value); }
		}

		public static bool RenderTransitions
		{
			get { return source.Configs[ConfigName].GetBoolean("RenderTransitions", true); }
			set { source.Configs[ConfigName].Set("RenderTransitions", value); }
		}

		public static string LastROM
		{
			get { return source.Configs[ConfigName].GetString("LastROM", string.Empty); }
			set { source.Configs[ConfigName].Set("LastROM", value); }
		}

		public static bool RenderPathWaypoints
		{
			get { return source.Configs[ConfigName].GetBoolean("RenderPathWaypoints", false); }
			set { source.Configs[ConfigName].Set("RenderPathWaypoints", value); }
		}

		public static bool LinkAllWPinPath
		{
			get { return source.Configs[ConfigName].GetBoolean("LinkAllWPinPath", true); }
			set { source.Configs[ConfigName].Set("LinkAllWPinPath", value); }
		}

		public static bool RenderTextures
		{
			get { return source.Configs[ConfigName].GetBoolean("RenderTextures", true); }
			set { source.Configs[ConfigName].Set("RenderTextures", value); }
		}

		public static bool RenderCollision
		{
			get { return source.Configs[ConfigName].GetBoolean("RenderCollision", false); }
			set { source.Configs[ConfigName].Set("RenderCollision", value); }
		}

		public static bool RenderCollisionAsWhite
		{
			get { return source.Configs[ConfigName].GetBoolean("RenderCollisionAsWhite", false); }
			set { source.Configs[ConfigName].Set("RenderCollisionAsWhite", value); }
		}

		public static bool OGLVSync
		{
			get { return source.Configs[ConfigName].GetBoolean("OGLVSync", true); }
			set { source.Configs[ConfigName].Set("OGLVSync", value); }
		}

		public static ToolModes LastToolMode
		{
			get { return (ToolModes)Enum.Parse(typeof(ToolModes), source.Configs[ConfigName].GetString("LastToolMode", "Camera")); }
			set { source.Configs[ConfigName].Set("LastToolMode", value); }
		}

		public static string LastSceneFile
		{
			get { return source.Configs[ConfigName].GetString("LastSceneFile", string.Empty); }
			set { source.Configs[ConfigName].Set("LastSceneFile", value); }
		}

		public static string LastRoomFile
		{
			get { return source.Configs[ConfigName].GetString("LastRoomFile", string.Empty); }
			set { source.Configs[ConfigName].Set("LastRoomFile", value); }
		}

		public static CombinerTypes CombinerType
		{
			get { return (CombinerTypes)Enum.Parse(typeof(CombinerTypes), (source.Configs[ConfigName].GetString("CombinerType", "None"))); }
			set { source.Configs[ConfigName].Set("CombinerType", value); }
		}

		public static bool ShownExtensionWarning
		{
			get { return source.Configs[ConfigName].GetBoolean("ShownExtensionWarning", false); }
			set { source.Configs[ConfigName].Set("ShownExtensionWarning", value); }
		}

		public static bool ShownIntelWarning
		{
			get { return source.Configs[ConfigName].GetBoolean("ShownIntelWarning", false); }
			set { source.Configs[ConfigName].Set("ShownIntelWarning", value); }
		}

		public static bool RenderWaterboxes
		{
			get { return source.Configs[ConfigName].GetBoolean("RenderWaterboxes", true); }
			set { source.Configs[ConfigName].Set("RenderWaterboxes", value); }
		}

		public static bool ShowWaterboxesPerRoom
		{
			get { return source.Configs[ConfigName].GetBoolean("ShowWaterboxesPerRoom", true); }
			set { source.Configs[ConfigName].Set("ShowWaterboxesPerRoom", value); }
		}

		public static bool IsRestarting
		{
			get { return source.Configs[ConfigName].GetBoolean("IsRestarting", false); }
			set { source.Configs[ConfigName].Set("IsRestarting", value); }
		}

		public static int AntiAliasingSamples
		{
			get { return source.Configs[ConfigName].GetInt("AntiAliasingSamples", 0); }
			set { source.Configs[ConfigName].Set("AntiAliasingSamples", value); }
		}

		public static bool EnableAntiAliasing
		{
			get { return source.Configs[ConfigName].GetBoolean("EnableAntiAliasing", false); }
			set { source.Configs[ConfigName].Set("EnableAntiAliasing", value); }
		}

		public static bool LimitDrawDistance
		{
			get { return source.Configs[ConfigName].GetBoolean("LimitDrawDistance", false); }
			set { source.Configs[ConfigName].Set("LimitDrawDistance", value); }
		}

		static Configuration()
		{
			PrepareConfig();

			source = new XmlConfigSource(FullConfigFilename) { AutoSave = true };

			CreateConfigSections();
		}

		static void CreateConfigSections()
		{
			if (source.Configs[ConfigName] == null) source.AddConfig(ConfigName);
		}

		static void PrepareConfig()
		{
			configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), System.Windows.Forms.Application.ProductName);
			configFilename = string.Format("{0}.xml", ConfigName);
			Directory.CreateDirectory(configPath);

			if (!File.Exists(FullConfigFilename)) File.WriteAllText(FullConfigFilename, "<Nini>\n</Nini>\n");
		}
	}
}
