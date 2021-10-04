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
		public static bool downedThorn;
		public static bool downedKeeper;
		public static bool downedSkullDigger;
		public static bool downedSeed;
		public static bool keeperSaved;
		public static bool skullDiggerSaved;
		public static bool downedSkeletonInvasion;
		public static bool downedEaglecrestGolem;
		//public static bool downedOtherBoss = false;

		public override void OnWorldLoad()
		{
			downedThorn = false;
			downedKeeper = false;
			downedSkullDigger = false;
			downedSeed = false;
			keeperSaved = false;
			skullDiggerSaved = false;
			downedSkeletonInvasion = false;
			downedEaglecrestGolem = false;
			//downedOtherBoss = false;
		}

		public override void OnWorldUnload()
		{
			downedThorn = false;
			downedKeeper = false;
			downedSkullDigger = false;
			downedSeed = false;
			keeperSaved = false;
			skullDiggerSaved = false;
			downedSkeletonInvasion = false;
			//downedOtherBoss = false;
		}

        public override void SaveWorldData(TagCompound tag)
		{
			var downed = new List<string>();

			if (downedThorn)
				downed.Add("downedThorn");
			if (downedKeeper)
				downed.Add("downedKeeper");
			if (downedSkullDigger)
				downed.Add("downedSkullDigger");
			if (downedSeed)
				downed.Add("downedSeed");
			if (keeperSaved)
				downed.Add("keeperSaved");
			if (skullDiggerSaved)
				downed.Add("skullDiggerSaved");
			if (downedSkeletonInvasion)
				downed.Add("downedSkeletonInvasion");
			if (downedEaglecrestGolem)
				downed.Add("downedEaglecrestGolem");

			tag["downed"] = downed;
		}

		public override void LoadWorldData(TagCompound tag)
		{
			var downed = tag.GetList<string>("downed");

			downedThorn = downed.Contains("downedThorn");
			downedKeeper = downed.Contains("downedKeeper");
			downedSkullDigger = downed.Contains("downedSkullDigger");
			downedSeed = downed.Contains("downedSeed");
			keeperSaved = downed.Contains("keeperSaved");
			skullDiggerSaved = downed.Contains("skullDiggerSaved");
			downedSkeletonInvasion = downed.Contains("downedSkeletonInvasion");
			downedEaglecrestGolem = downed.Contains("downedEaglecrestGolem");
			//downedOtherBoss = downed.Contains("downedOtherBoss");
		}

		public override void NetSend(BinaryWriter writer)
		{
			var flags = new BitsByte();
			flags[0] = downedThorn;
			flags[1] = downedKeeper;
			flags[2] = downedSkullDigger;
			flags[3] = downedSeed;
			flags[4] = keeperSaved;
			flags[5] = skullDiggerSaved;
			flags[6] = downedSkeletonInvasion;
			flags[7] = downedEaglecrestGolem;
			writer.Write(flags);
		}

		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			downedThorn = flags[0];
			downedKeeper = flags[1];
			downedSkullDigger = flags[2];
			downedSeed = flags[3];
			keeperSaved = flags[4];
			skullDiggerSaved = flags[5];
			downedSkeletonInvasion = flags[6];
			downedEaglecrestGolem = flags[7];
		}
	}
}