using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Natural
{
    public class GrubNestTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 4;
            TileObjectData.newTile.AnchorValidTiles = new int[] { ModContent.TileType<IrradiatedSandstoneTile>() };
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<IrradiatedStoneDust>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Grub Nest");
            AddMapEntry(new Color(40, 60, 40), name);
        }

        // TODO: grub nest spawning
        /*public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            float dist = Vector2.Distance(player.Center / 16f, new Vector2(i + 0.5f, j + 0.5f));
            if (dist <= 12f && dist > 5f)
            {
                if (Main.rand.Next(100) == 0)
                {
                    int index1 = NPC.NewNPC((int)((i + 1f) * 16), (int)((j + 1f) * 16), ModContent.NPCType<InfectedGrub>());
                    if (index1 < Main.maxNPCs && Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: index1);
                    }
                }
            }
        }*/

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}