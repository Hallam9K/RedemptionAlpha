using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Bars
{
    public class DragonLeadAlloyTile : ModTile
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

            ItemDrop = ModContent.ItemType<DragonLeadAlloy>();   
            DustType = DustID.Torch;
            AddMapEntry(new Color(160, 50, 40));
			MinPick = 0;
        }
    }
}