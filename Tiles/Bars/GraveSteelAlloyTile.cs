using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Bars
{
    public class GraveSteelAlloyTile : ModTile
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

            ItemDrop = ModContent.ItemType<GraveSteelAlloy>();   
            DustType = DustID.Iron;
            AddMapEntry(new Color(115, 98, 87));
			MinPick = 0;
        }
    }
}