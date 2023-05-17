using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Shade
{
    public class ShadestoneCrateTile : ModTile
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
            LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Shadestone Crate");
			AddMapEntry(new Color(59, 61, 87), name);
			DustType = ModContent.DustType<ShadestoneDust>();
		}
	}
}