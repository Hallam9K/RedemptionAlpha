using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Tools.PostML;

namespace Redemption.Tiles.Tiles
{
    public class BedrockTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;
            DustType = DustID.Wraith;
            MinPick = 2000;
            MineResist = 50f;
            HitSound = SoundID.Tink;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Bedrock");
            AddMapEntry(new Color(0, 0, 0), name);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
            if (Main.LocalPlayer.HeldItem.type == ModContent.ItemType<NanoAxe2>())
                return true;
            return false;
        }
    }
}