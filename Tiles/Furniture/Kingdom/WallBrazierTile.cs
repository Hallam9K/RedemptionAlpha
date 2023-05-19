using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Furniture.Kingdom;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Kingdom
{
    public class WallBrazierTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 3;
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<SlateDust>();
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            MinPick = 100;
            MineResist = 7f;
            LocalizedText name = CreateMapEntryName();
            //name.SetDefault("Wall Brazier");
            AddMapEntry(new Color(200, 150, 100), name);
            AnimationFrameHeight = 54;
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].TileFrameX % 54 == 0 && Main.tile[i, j].TileFrameY % 54 == 0)
            {
                if (Main.rand.NextBool(4))
                {
                    int d = Dust.NewDust(new Vector2(i * 16 + 14, j * 16 + 28), 20, 4, DustID.Torch, 0f, -1, 0, default, 3f);
                    Main.dust[d].velocity.Y -= 2f;
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
                if (frame > 3)
                    frame = 0;
            }
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = .6f;
            b = .1f;
        }
    }
}