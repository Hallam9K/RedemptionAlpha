using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Bars
{
    public class PureIronBarTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            SoundType = SoundID.Tink;

            Main.tileShine[Type] = 1100;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            ItemDrop = ModContent.ItemType<PureIronBar>();
            DustType = DustID.SilverCoin;
            AddMapEntry(new Color(125, 131, 150));
			MinPick = 0;
        }
    }
}