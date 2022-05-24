using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PostML;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Bars
{
    public class XeniumAlloyTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            HitSound = SoundID.Tink;

            Main.tileShine[Type] = 1100;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.IgnoredByNpcStepUp[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            ItemDrop = ModContent.ItemType<XeniumAlloy>();
            DustType = DustID.Electric;
            AddMapEntry(new Color(48, 63, 73));
			MinPick = 0;
        }
    }
}