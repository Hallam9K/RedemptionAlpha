using Microsoft.Xna.Framework;
using Redemption.Items.Materials.HM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Bars
{
    public class StarliteBarTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			SoundType = SoundID.Tink;
			Main.tileShine[Type] = 1100;
			Main.tileSolid[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);
			ItemDrop = ModContent.ItemType<StarliteBar>();
            DustType = DustID.GreenTorch;
            AddMapEntry(new Color(40, 200, 100));
			MinPick = 0;
		}
	}
}
