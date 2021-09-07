using System.Collections;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption.Globals
{
	public class RedeBossDowned : ModSystem
	{
		public static bool downedThorn = false;
		public static bool downedKeeper = false;
		//public static bool downedOtherBoss = false;

		public override void OnWorldLoad()
		{
			downedThorn = false;
			downedKeeper = false;
			//downedOtherBoss = false;
		}

		public override void OnWorldUnload()
		{
			downedThorn = false;
			downedKeeper = false;
			//downedOtherBoss = false;
		}

		public override TagCompound SaveWorldData()
		{
			var downed = new List<string>();

			if (downedThorn)
				downed.Add("downedThorn");
			if (downedKeeper)
				downed.Add("downedKeeper");

			return new TagCompound
			{
				["downed"] = downed,
			};
		}

		public override void LoadWorldData(TagCompound tag)
		{
			var downed = tag.GetList<string>("downed");

			downedThorn = downed.Contains("downedThorn");
			downedKeeper = downed.Contains("downedKeeper");
			//downedOtherBoss = downed.Contains("downedOtherBoss");
		}

		public override void NetSend(BinaryWriter writer)
		{
			//Order of operations is important and has to match that of NetReceive
			var flags = new BitsByte();
			flags[0] = downedThorn;
			flags[1] = downedKeeper;
			//flags[2] = downedOtherBoss;
			writer.Write(flags);
		}

		public override void NetReceive(BinaryReader reader)
		{
			//Order of operations is important and has to match that of NetSend
			BitsByte flags = reader.ReadByte();
			downedThorn = flags[0];
			downedKeeper = flags[1];
			//downedOtherBoss = flags[2];
		}
	}
}