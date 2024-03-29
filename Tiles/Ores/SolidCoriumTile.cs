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
    public class SolidCoriumTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.Ore[Type] = true;
            Main.tileOreFinderPriority[Type] = 900;
            DustType = DustID.FlameBurst;
            MinPick = 5000;
            MineResist = 10f;
            HitSound = SoundID.Tink;
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Solid Corium");
            AddMapEntry(new Color(208, 101, 70), name);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.7f;
            g = 0.4f;
            b = 0.0f;
        }
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            var dist = (int)Vector2.Distance(player.Center / 16, new Vector2(i, j));
            if (dist <= 25 && dist > 20)
                player.RedemptionRad().Irradiate(.001f, 0, 2.5f, 1);
            else if (dist <= 20 && dist > 14)
                player.RedemptionRad().Irradiate(.002f, 1, 2.5f, 2);
            else if (dist <= 14 && dist > 8)
                player.RedemptionRad().Irradiate(.01f, 3, 2.75f, 3);
            else if (dist <= 8 && dist > 2)
                player.RedemptionRad().Irradiate(.05f, 4, 2.75f, 4);
            else if (dist <= 2)
                player.RedemptionRad().Irradiate(.5f, 4, 2.9f, 6);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}