using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.ID;
using Redemption.Dusts.Tiles;

namespace Redemption.Tiles.Furniture.Shade
{
    public class ShadestoneSofaTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Shadestone Sofa");
            AddMapEntry(new Color(59, 61, 87), name);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            AdjTiles = new int[] { TileID.Benches };
            DustType = ModContent.DustType<ShadestoneDust>();
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
