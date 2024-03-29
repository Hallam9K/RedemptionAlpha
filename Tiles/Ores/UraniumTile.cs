using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Globals.Player;
using Redemption.Items.Accessories.HM;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Tiles.Ores
{
    public class UraniumTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.Ore[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 975;
            Main.tileOreFinderPriority[Type] = 440;
            DustType = DustID.Electric;
            MinPick = 210;
            MineResist = 7f;
            HitSound = SoundID.Tink;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Uranium");
            AddMapEntry(new Color(77, 240, 107), name);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0;
            g = 0.4f;
            b = 0;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            var dist = (int)Vector2.Distance(player.Center / 16, new Vector2(i, j));
            if (dist <= 15 && dist > 8)
                player.RedemptionRad().Irradiate(.001f, 0, 1.5f);
            else if (dist <= 8 && dist > 2)
                player.RedemptionRad().Irradiate(.002f, 1, 1.5f);
            else if (dist <= 2)
                player.RedemptionRad().Irradiate(.01f, 2, 2, 2);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }
}