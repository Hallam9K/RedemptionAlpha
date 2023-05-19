using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Dusts.Tiles;
using Terraria.Localization;

namespace Redemption.Tiles.Ores
{
    public class AncientSlateDiamondTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 975;
            Main.tileOreFinderPriority[Type] = 320;
            Main.tileBrick[Type] = true;
            TileID.Sets.Ore[Type] = true;
            DustType = ModContent.DustType<SlateDust>();
            RegisterItemDrop(ItemID.Diamond);
            MinPick = 350;
            MineResist = 11f;
            HitSound = SoundID.Tink;
            AddMapEntry(Color.LightBlue, Language.GetText("ItemName.Diamond"));
        }
        public override bool CanDrop(int i, int j) => Main.rand.NextBool(2);
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}