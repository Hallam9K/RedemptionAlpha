using Humanizer;
using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Furniture.Misc;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Misc
{
    public class WastelandCampfireTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileWaterDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.WaterDeath = true;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Wasteland Campfire");
            AddMapEntry(new Color(254, 121, 2), name);
            DustType = DustID.Torch;
            AnimationFrameHeight = 36;
            MinPick = 10;
            MineResist = 1;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            player.AddBuff(BuffID.Campfire, 10);
            if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0)
            {
                if (Main.rand.NextBool(2))
                {
                    int d = Dust.NewDust(new Vector2(i * 16 + 10, j * 16 - 8), 28, 24, DustID.Smoke, Alpha: 20, Scale: 1f);
                    Main.dust[d].velocity.Y = -2f;
                    Main.dust[d].velocity.X *= 0.5f;
                    Main.dust[d].fadeIn = 300;
                    Main.dust[d].noGravity = true;
                }
            }
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 4)
            {
                frameCounter = 0;
                frame++;
                if (frame > 7)
                    frame = 0;
            }
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.9f;
            g = 0.7f;
            b = 0.6f;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 32, ModContent.ItemType<WastelandCampfire>());
        }
    }
}