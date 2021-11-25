using Microsoft.Xna.Framework;
using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Bars
{
	public class CorruptedStarliteBarTile : ModTile
	{
		public override void SetDefaults()
		{
            SoundType = SoundID.Tink;

            Main.tileShine[Type] = 1100;
			Main.tileSolid[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.set_StyleHorizontal(true);
			TileObjectData.newTile.set_LavaDeath(false);
			TileObjectData.addTile(Type);

			ItemDrop = ModContent.ItemType<CorruptedStarliteBar>();
			DustType = DustID.Iron;
			AddMapEntry(new Color(200, 100, 20));
			MinPick = 0;
		}
	}
}
