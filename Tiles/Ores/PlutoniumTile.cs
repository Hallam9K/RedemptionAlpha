using Microsoft.Xna.Framework;
using Redemption.BaseExtension;
using Redemption.Globals.Players;
using Redemption.Items.Accessories.HM;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Tiles.Ores
{
    public class PlutoniumTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 975;
            TileID.Sets.Ore[Type] = true;
            Main.tileOreFinderPriority[Type] = 680;
            DustType = DustID.Electric;
            MinPick = 220;
            MineResist = 6f;
            HitSound = SoundID.Tink;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Plutonium");
            AddMapEntry(new Color(133, 253, 255), name);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.2f;
            g = 0.2f;
            b = 0.4f;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            var dist = (int)Vector2.Distance(player.Center / 16, new Vector2(i, j));
            if (dist <= 30 && dist > 18)
                player.RedemptionRad().Irradiate(.001f, 0, 1.5f, 1);
            else if (dist <= 18 && dist > 10)
                player.RedemptionRad().Irradiate(.002f, 1, 2, 1);
            else if (dist <= 10 && dist > 4)
                player.RedemptionRad().Irradiate(.01f, 2, 2, 3);
            else if (dist <= 4)
                player.RedemptionRad().Irradiate(.05f, 2, 2.5f, 4);
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