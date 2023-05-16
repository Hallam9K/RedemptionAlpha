using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Furniture.PetrifiedWood;
using Redemption.Items.Placeable.Furniture.Silverwood;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Silverwood
{
    public class SilverwoodCrateTile : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileTable[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Silverwood Crate");
            AddMapEntry(new Color(228, 213, 173), name);
            DustType = DustID.Pearlwood;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<SilverwoodCrate>());
        }
    }
}