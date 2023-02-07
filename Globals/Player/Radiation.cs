using Redemption.Buffs.Debuffs;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Redemption.BaseExtension;
using Terraria.Graphics.Effects;

namespace Redemption.Globals.Player
{
    public class Radiation : ModPlayer
    {
        public int irradiatedLevel;
        public int irradiatedTimer;
        public int irradiatedEffect;
        public float RadNoiseIntensity = 0f;
        public float RadNoiseIntensity2 = 0f;

        public override void PostUpdateMiscEffects()
        {
            UpdateFilterEffects();
            RadNoiseIntensity = 0;
            if (irradiatedLevel == 0)
                return;

            BuffPlayer suit = Player.RedemptionPlayerBuff();
            switch (irradiatedLevel)
            {
                case 1:
                    irradiatedTimer++;
                    if (irradiatedTimer == 39999 || irradiatedTimer == 47999)
                    {
                        if (suit.hazmatSuit || suit.HEVSuit)
                        {
                            irradiatedEffect = 0;
                            irradiatedLevel = 0;
                            irradiatedTimer = 0;
                        }
                        else if (Main.rand.NextBool(2))
                        {
                            irradiatedEffect = 0;
                            irradiatedLevel = 0;
                            irradiatedTimer = 0;
                        }
                    }
                    if (irradiatedTimer >= 38000 && irradiatedTimer < 40000)
                    {
                        if (Main.rand.NextBool(800))
                            Player.AddBuff(ModContent.BuffType<HeadacheDebuff>(), 120);
                    }
                    else if (irradiatedTimer >= 40000 && irradiatedTimer < 48000)
                    {
                        irradiatedEffect = 1;
                        if (Main.rand.NextBool(2000))
                            Player.AddBuff(ModContent.BuffType<FatigueDebuff>(), 800);
                        if (Main.rand.NextBool(12000))
                            Player.AddBuff(ModContent.BuffType<NauseaDebuff>(), 1200);
                    }
                    else if (irradiatedTimer >= 48000 && irradiatedTimer < 52000)
                    {
                        irradiatedEffect = 2;
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<FatigueDebuff>(), 800);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<NauseaDebuff>(), 1200);
                        if (Main.rand.NextBool(80000))
                            Player.AddBuff(ModContent.BuffType<HairLossDebuff>(), 60000);
                    }
                    else if (irradiatedTimer >= 52000 && irradiatedTimer < 58000)
                    {
                        irradiatedEffect = 3;
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<HairLossDebuff>(), 60000);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<SkinBurnDebuff>(), 60000);
                        if (Main.rand.NextBool(4000))
                            Player.AddBuff(ModContent.BuffType<FeverDebuff>(), 60000);
                    }
                    else if (irradiatedTimer >= 58000)
                    {
                        irradiatedEffect = 4;
                        Player.AddBuff(ModContent.BuffType<RadiationDebuff>(), 5);
                    }
                    break;
                case 2:
                    irradiatedTimer++;
                    if (irradiatedTimer == 37999 || irradiatedTimer == 45999)
                    {
                        if (suit.hazmatSuit || suit.HEVSuit)
                        {
                            irradiatedEffect = 0;
                            irradiatedLevel = 0;
                            irradiatedTimer = 0;
                        }
                        else if (Main.rand.NextBool(3))
                        {
                            irradiatedEffect = 0;
                            irradiatedLevel = 0;
                            irradiatedTimer = 0;
                        }
                    }
                    if (irradiatedTimer >= 36000 && irradiatedTimer < 38000)
                    {
                        if (Main.rand.NextBool(800))
                            Player.AddBuff(ModContent.BuffType<HeadacheDebuff>(), 120);
                    }
                    else if (irradiatedTimer >= 38000 && irradiatedTimer < 46000)
                    {
                        irradiatedEffect = 1;
                        if (Main.rand.NextBool(2000))
                            Player.AddBuff(ModContent.BuffType<FatigueDebuff>(), 800);
                        if (Main.rand.NextBool(10000))
                            Player.AddBuff(ModContent.BuffType<NauseaDebuff>(), 1200);
                    }
                    else if (irradiatedTimer >= 46000 && irradiatedTimer < 50000)
                    {
                        irradiatedEffect = 2;
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<FatigueDebuff>(), 800);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<NauseaDebuff>(), 1200);
                        if (Main.rand.NextBool(50000))
                            Player.AddBuff(ModContent.BuffType<HairLossDebuff>(), 60000);
                    }
                    else if (irradiatedTimer >= 50000 && irradiatedTimer < 56000)
                    {
                        irradiatedEffect = 3;
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<HairLossDebuff>(), 60000);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<SkinBurnDebuff>(), 60000);
                        if (Main.rand.NextBool(4000))
                            Player.AddBuff(ModContent.BuffType<FeverDebuff>(), 60000);
                    }
                    else if (irradiatedTimer >= 56000)
                    {
                        irradiatedEffect = 4;
                        Player.AddBuff(ModContent.BuffType<RadiationDebuff>(), 5);
                    }
                    break;
                case 3:
                    irradiatedTimer++;
                    if (irradiatedTimer == 33999 || irradiatedTimer == 41999)
                    {
                        if (suit.HEVSuit)
                        {
                            irradiatedEffect = 0;
                            irradiatedLevel = 0;
                            irradiatedTimer = 0;
                        }
                        else if (suit.hazmatSuit && Main.rand.NextBool(2))
                        {
                            irradiatedEffect = 0;
                            irradiatedLevel = 0;
                            irradiatedTimer = 0;
                        }
                        else if (Main.rand.NextBool(5))
                        {
                            irradiatedEffect = 0;
                            irradiatedLevel = 0;
                            irradiatedTimer = 0;
                        }
                    }
                    if (irradiatedTimer >= 32000 && irradiatedTimer < 34000)
                    {
                        if (Main.rand.NextBool(800))
                            Player.AddBuff(ModContent.BuffType<HeadacheDebuff>(), 120);
                    }
                    else if (irradiatedTimer >= 34000 && irradiatedTimer < 42000)
                    {
                        irradiatedEffect = 1;
                        if (Main.rand.NextBool(2000))
                            Player.AddBuff(ModContent.BuffType<FatigueDebuff>(), 800);
                        if (Main.rand.NextBool(5000))
                            Player.AddBuff(ModContent.BuffType<NauseaDebuff>(), 1200);
                    }
                    else if (irradiatedTimer >= 42000 && irradiatedTimer < 46000)
                    {
                        irradiatedEffect = 2;
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<FatigueDebuff>(), 800);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<NauseaDebuff>(), 1200);
                        if (Main.rand.NextBool(30000))
                            Player.AddBuff(ModContent.BuffType<HairLossDebuff>(), 60000);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<SkinBurnDebuff>(), 60000);
                    }
                    else if (irradiatedTimer >= 46000 && irradiatedTimer < 49000)
                    {
                        irradiatedEffect = 3;
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<HairLossDebuff>(), 60000);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<SkinBurnDebuff>(), 60000);
                        if (Main.rand.NextBool(4000))
                            Player.AddBuff(ModContent.BuffType<FeverDebuff>(), 60000);
                    }
                    else if (irradiatedTimer >= 49000)
                    {
                        irradiatedEffect = 4;
                        Player.AddBuff(ModContent.BuffType<RadiationDebuff>(), 5);
                    }
                    break;
                case 4:
                    irradiatedTimer++;
                    if (irradiatedTimer == 33999 || irradiatedTimer == 37999)
                    {
                        if (suit.HEVSuit && Main.rand.NextBool(5))
                        {
                            irradiatedEffect = 0;
                            irradiatedLevel = 0;
                            irradiatedTimer = 0;
                        }
                        else if (suit.hazmatSuit && Main.rand.NextBool(10))
                        {
                            irradiatedEffect = 0;
                            irradiatedLevel = 0;
                            irradiatedTimer = 0;
                        }
                        else if (Main.rand.NextBool(15))
                        {
                            irradiatedEffect = 0;
                            irradiatedLevel = 0;
                            irradiatedTimer = 0;
                        }
                    }
                    if (irradiatedTimer >= 30000 && irradiatedTimer < 34000)
                    {
                        if (Main.rand.NextBool(800))
                            Player.AddBuff(ModContent.BuffType<HeadacheDebuff>(), 120);
                    }
                    else if (irradiatedTimer >= 34000 && irradiatedTimer < 38000)
                    {
                        irradiatedEffect = 1;
                        if (Main.rand.NextBool(2000))
                            Player.AddBuff(ModContent.BuffType<FatigueDebuff>(), 800);
                        if (Main.rand.NextBool(2000))
                            Player.AddBuff(ModContent.BuffType<NauseaDebuff>(), 1200);
                    }
                    else if (irradiatedTimer >= 38000 && irradiatedTimer < 40000)
                    {
                        irradiatedEffect = 2;
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<FatigueDebuff>(), 800);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<NauseaDebuff>(), 1200);
                        if (Main.rand.NextBool(2000))
                            Player.AddBuff(ModContent.BuffType<HairLossDebuff>(), 60000);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<SkinBurnDebuff>(), 60000);
                    }
                    else if (irradiatedTimer >= 40000 && irradiatedTimer < 41000)
                    {
                        irradiatedEffect = 3;
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<HairLossDebuff>(), 60000);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<SkinBurnDebuff>(), 60000);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<FeverDebuff>(), 60000);
                    }
                    else if (irradiatedTimer >= 41000)
                    {
                        irradiatedEffect = 4;
                        Player.AddBuff(ModContent.BuffType<RadiationDebuff>(), 5);
                    }
                    break;
                case 5:
                    irradiatedTimer++;
                    if (irradiatedTimer == 27999 || irradiatedTimer == 29999)
                    {
                        if (suit.HEVSuit && Main.rand.NextBool(20))
                        {
                            irradiatedEffect = 0;
                            irradiatedLevel = 0;
                            irradiatedTimer = 0;
                        }
                        else if (suit.hazmatSuit && Main.rand.NextBool(30))
                        {
                            irradiatedEffect = 0;
                            irradiatedLevel = 0;
                            irradiatedTimer = 0;
                        }
                        else if (Main.rand.NextBool(40))
                        {
                            irradiatedEffect = 0;
                            irradiatedLevel = 0;
                            irradiatedTimer = 0;
                        }
                    }
                    if (irradiatedTimer >= 26000 && irradiatedTimer < 28000)
                    {
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<HeadacheDebuff>(), 120);
                    }
                    else if (irradiatedTimer >= 28000 && irradiatedTimer < 30000)
                    {
                        irradiatedEffect = 1;
                        if (Main.rand.NextBool(300))
                            Player.AddBuff(ModContent.BuffType<FatigueDebuff>(), 800);
                        if (Main.rand.NextBool(300))
                            Player.AddBuff(ModContent.BuffType<NauseaDebuff>(), 1200);
                    }
                    else if (irradiatedTimer >= 30000 && irradiatedTimer < 31000)
                    {
                        irradiatedEffect = 2;
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<FatigueDebuff>(), 800);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<NauseaDebuff>(), 1200);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<HairLossDebuff>(), 60000);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<SkinBurnDebuff>(), 60000);
                    }
                    else if (irradiatedTimer >= 31000 && irradiatedTimer < 32000)
                    {
                        irradiatedEffect = 3;
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<HairLossDebuff>(), 60000);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<SkinBurnDebuff>(), 60000);
                        if (Main.rand.NextBool(400))
                            Player.AddBuff(ModContent.BuffType<FeverDebuff>(), 60000);
                    }
                    else if (irradiatedTimer >= 32000)
                    {
                        irradiatedEffect = 4;
                        Player.AddBuff(ModContent.BuffType<RadiationDebuff>(), 5);
                    }
                    break;
            }
            if (irradiatedLevel > 5)
                irradiatedLevel = 5;
        }
        private void UpdateFilterEffects()
        {
            if (irradiatedLevel > 0)
            {
                if (!Filters.Scene["MoR:RadiationNoiseEffect"].IsActive())
                    Filters.Scene.Activate("MoR:RadiationNoiseEffect");

                float timerMax = 58000;
                float timerMin = 38000;
                switch (irradiatedLevel)
                {
                    case 2:
                        timerMax = 56000;
                        timerMin = 36000;
                        break;
                    case 3:
                        timerMax = 49000;
                        timerMin = 32000;
                        break;
                    case 4:
                        timerMax = 41000;
                        timerMin = 30000;
                        break;
                    case 5:
                        timerMax = 32000;
                        timerMin = 26000;
                        break;
                }
                if (irradiatedTimer > 0 && irradiatedTimer < 60)
                {
                    timerMax = 60;
                    timerMin = 600;
                }
                if (irradiatedTimer >= 60 && irradiatedTimer < 180)
                {
                    timerMax = 60;
                    timerMin = 780;
                }
                RadNoiseIntensity = 1f * Utils.GetLerpValue(timerMin - 600, timerMax, irradiatedTimer, true);
                Filters.Scene["MoR:RadiationNoiseEffect"].GetShader().UseIntensity(RadNoiseIntensity);
            }
            else
            {
                if (Filters.Scene["MoR:RadiationNoiseEffect"].IsActive())
                    Filters.Scene.Deactivate("MoR:RadiationNoiseEffect");
            }
        }
        public override void UpdateDead()
        {
            irradiatedLevel = 0;
            irradiatedEffect = 0;
            irradiatedTimer = 0;
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