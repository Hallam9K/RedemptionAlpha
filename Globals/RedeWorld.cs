using Redemption.NPCs.Friendly;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Redemption
{
    public class RedeWorld : ModSystem
    {
        public static bool blobbleSwarm;
        public static int blobbleSwarmTimer;
        public static int blobbleSwarmCooldown;
        public static int alignment = 0;

        public override void PostUpdateWorld()
        {
            if (blobbleSwarm)
            {
                blobbleSwarmTimer++;
                if (blobbleSwarmTimer > 180)
                {
                    blobbleSwarm = false;
                    blobbleSwarmTimer = 0;
                    blobbleSwarmCooldown = 86400;
                }
            }
            if (blobbleSwarmCooldown > 0)
                blobbleSwarmCooldown--;
        }

		public override void OnWorldLoad()
		{
			alignment = 0;
		}

		public override void OnWorldUnload()
		{
			alignment = 0;
		}

		public override TagCompound SaveWorldData()
		{
			return new TagCompound
			{
				["alignment"] = alignment,
			};
		}

		public override void LoadWorldData(TagCompound tag)
		{
			alignment = tag.GetInt("alignment");
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(alignment);
		}

		public override void NetReceive(BinaryReader reader)
		{
			alignment = reader.ReadInt32();
		}
	}
}