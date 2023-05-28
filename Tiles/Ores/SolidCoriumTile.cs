using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Items.Accessories.HM;
using Redemption.Globals.Player;
using Terraria.Audio;
using Redemption.BaseExtension;

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
            MinPick = 1000;
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
            Radiation modPlayer = player.RedemptionRad();
            BuffPlayer suit = player.RedemptionPlayerBuff();
            var dist = (int)Vector2.Distance(player.Center / 16, new Vector2(i, j));
            if (dist <= 25 && dist > 20 && !suit.hazmatSuit && !suit.HEVSuit)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(100) && !Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Muller1, player.position);

                if (Main.rand.NextBool(80000) && modPlayer.irradiatedLevel < 5)
                    modPlayer.irradiatedLevel++;
            }
            else if (dist <= 20 && dist > 14 && !suit.hazmatSuit && !suit.HEVSuit)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(100) && !Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Muller2, player.position);

                if (Main.rand.NextBool(40000) && modPlayer.irradiatedLevel < 5)
                    modPlayer.irradiatedLevel++;
            }
            else if (dist <= 14 && dist > 8 && !suit.HEVSuit)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(100) && !Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Muller4, player.position);

                if (Main.rand.NextBool(2000) && modPlayer.irradiatedLevel < 5)
                    modPlayer.irradiatedLevel++;
            }
            else if (dist <= 8 && dist > 2)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(100) && !Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Muller5, player.position);

                if (Main.rand.NextBool(500) && modPlayer.irradiatedLevel < 5)
                    modPlayer.irradiatedLevel += 2;
            }
            else if (dist <= 2)
            {
                if (player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(100) && !Main.dedServ)
                    SoundEngine.PlaySound(CustomSounds.Muller5, player.position);

                if (Main.rand.NextBool(10) && modPlayer.irradiatedLevel < 5)
                    modPlayer.irradiatedLevel += 2;
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}