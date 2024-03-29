using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Debuffs;
using Redemption.Items.Accessories.HM;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Redemption.Globals.Player
{
    public class Radiation : ModPlayer
    {
        public double radiationLevel;
        public int pillCureTimer;
        public byte protectionLevel;

        public void Irradiate(double radIncrease, byte mullerLevel, float maxLevel = 3, byte protectionRequired = 1, int tickChance = 100)
        {
            if (protectionLevel >= protectionRequired)
                return;
            if (Player.GetModPlayer<MullerEffect>().effect && Main.rand.NextBool(tickChance) && !Main.dedServ)
            {
                var muller = mullerLevel switch
                {
                    1 => CustomSounds.Muller2,
                    2 => CustomSounds.Muller3,
                    3 => CustomSounds.Muller4,
                    4 => CustomSounds.Muller5,
                    _ => CustomSounds.Muller1,
                };
                SoundEngine.PlaySound(muller, Player.position);
            }

            if (radiationLevel < maxLevel)
                radiationLevel += radIncrease / (60 * (1 + protectionLevel));
        }

        public override void PostUpdateMiscEffects()
        {
            UpdateFilterEffects();
            if (protectionLevel >= 1)
                Player.buffImmune[ModContent.BuffType<HeavyRadiationDebuff>()] = true;
            if (protectionLevel >= 2)
                Player.buffImmune[ModContent.BuffType<RadioactiveFalloutDebuff>()] = true;

            if (pillCureTimer > 0)
            {
                if (radiationLevel < 3)
                {
                    double speed = .05f;
                    if (radiationLevel >= 2f)
                        speed = .025f;
                    if (radiationLevel < 1f)
                        speed = .2f;
                    if (radiationLevel <= 0)
                        pillCureTimer = 0;
                    radiationLevel -= speed / 60;
                }
                pillCureTimer--;
            }

            if (radiationLevel == 0)
                return;

            if (radiationLevel >= 1f)
                radiationLevel += .005f / 60;

            radiationLevel = MathHelper.Max((float)radiationLevel, 0);

            if (radiationLevel is >= 1f and < 1.2f)
            {
                Player.AddBuff(ModContent.BuffType<HeadacheDebuff>(), 120);
                if (Main.rand.NextBool(2000))
                    Player.AddBuff(ModContent.BuffType<FatigueDebuff>(), 800);
                if (Main.rand.NextBool(12000))
                    Player.AddBuff(ModContent.BuffType<NauseaDebuff>(), 1200);
            }
            else if (radiationLevel is >= 1.5f and < 2f)
            {
                Player.AddBuff(ModContent.BuffType<HeadacheDebuff>(), 120);
                Player.AddBuff(ModContent.BuffType<FatigueDebuff>(), 120);
                Player.AddBuff(ModContent.BuffType<NauseaDebuff>(), 1200);
            }
            else if (radiationLevel is >= 1.5f and < 2f)
            {
                Player.AddBuff(ModContent.BuffType<FatigueDebuff>(), 120);
                Player.AddBuff(ModContent.BuffType<NauseaDebuff>(), 1200);
                if (Main.rand.NextBool(12000))
                    Player.AddBuff(ModContent.BuffType<HairLossDebuff>(), 1200);
            }
            else if (radiationLevel is >= 2f and < 2.5f)
            {
                if (Main.rand.NextBool(400))
                    Player.AddBuff(ModContent.BuffType<HairLossDebuff>(), 1200);
                if (Main.rand.NextBool(400))
                    Player.AddBuff(ModContent.BuffType<SkinBurnDebuff>(), 1200);
                if (Main.rand.NextBool(4000))
                    Player.AddBuff(ModContent.BuffType<FeverDebuff>(), 1200);
            }
            else if (radiationLevel is >= 2.5f and < 3f)
            {
                Player.AddBuff(ModContent.BuffType<HairLossDebuff>(), 1200);
                Player.AddBuff(ModContent.BuffType<SkinBurnDebuff>(), 1200);
                Player.AddBuff(ModContent.BuffType<FeverDebuff>(), 1200);
            }
            else if (radiationLevel is >= 3f)
                Player.AddBuff(ModContent.BuffType<RadiationDebuff>(), 5);
        }
        private void UpdateFilterEffects()
        {
            if (Main.dedServ)
                return;

            if (radiationLevel > 1)
            {
                if (!Filters.Scene["MoR:RadiationNoiseEffect"].IsActive())
                    Filters.Scene.Activate("MoR:RadiationNoiseEffect");

                Filters.Scene["MoR:RadiationNoiseEffect"].GetShader().UseIntensity((float)(1f * ((radiationLevel - 1) / 3)));
            }
            else
            {
                if (Filters.Scene["MoR:RadiationNoiseEffect"].IsActive())
                    Filters.Scene.Deactivate("MoR:RadiationNoiseEffect");
            }
        }
        public override void UpdateDead()
        {
            radiationLevel = 0;
            protectionLevel = 0;
        }
        public override void PostUpdateBuffs()
        {
            if (Player.HasBuff(ModContent.BuffType<RadiationDebuff>()))
            {
                Filters.Scene["MoR:FogOverlay"]?.GetShader().UseOpacity(0.5f).UseIntensity(1f)
                    .UseColor(Color.GreenYellow).UseImage(ModContent.Request<Texture2D>("Redemption/Effects/Perlin", AssetRequestMode.ImmediateLoad).Value);
                Player.ManageSpecialBiomeVisuals("MoR:FogOverlay", Player.HasBuff(ModContent.BuffType<RadiationDebuff>()));
            }
        }
    }
}