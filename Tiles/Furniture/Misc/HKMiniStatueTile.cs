using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Redemption.Dusts.Tiles;
using Terraria.ID;
using Redemption.Items.Placeable.Furniture.Misc;

namespace Redemption.Tiles.Furniture.Misc
{
    public class HKMiniStatueTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 18, 16, 16, 18 };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(0, 3);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide | AnchorType.Table, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            MinPick = 50;
            MineResist = 7f;
            HitSound = CustomSounds.StoneHit;
            RegisterItemDrop(ModContent.ItemType<HKMiniStatue>());
            AddMapEntry(new Color(104, 91, 83));
            DustType = ModContent.DustType<SlateDust>();
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.tile[i, j].TileFrameY == 0)
            {
                if (Main.tile[i, j].TileFrameX == 0)
                {
                    if (Main.rand.NextBool(10))
                    {
                        int d = Dust.NewDust(new Vector2(i * 16 + 11, j * 16 + 8), 4, 8, DustID.TreasureSparkle, Alpha: 20);
                        Main.dust[d].velocity *= 0;
                        Main.dust[d].noGravity = true;
                    }
                    if (Main.rand.NextBool(10))
                    {
                        int d = Dust.NewDust(new Vector2(i * 16 + 8, j * 16 + 22), 8, 8, DustID.TreasureSparkle, Alpha: 20);
                        Main.dust[d].velocity *= 0;
                        Main.dust[d].noGravity = true;
                    }
                }
                if (Main.tile[i, j].TileFrameX == 36)
                {
                    if (Main.rand.NextBool(10))
                    {
                        int d = Dust.NewDust(new Vector2(i * 16 + 13, j * 16 + 8), 4, 8, DustID.TreasureSparkle, Alpha: 20);
                        Main.dust[d].velocity *= 0;
                        Main.dust[d].noGravity = true;
                    }
                    if (Main.rand.NextBool(10))
                    {
                        int d = Dust.NewDust(new Vector2(i * 16 + 12, j * 16 + 22), 8, 8, DustID.TreasureSparkle, Alpha: 20);
                        Main.dust[d].velocity *= 0;
                        Main.dust[d].noGravity = true;
                    }
                }
            }
        }
        public override bool CanExplode(int i, int j) => false;
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}