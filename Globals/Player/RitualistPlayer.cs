using Redemption.Biomes;
using Redemption.Items.Donator.Lizzy;
using Redemption.Items.Donator.Uncon;
using Redemption.Projectiles.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Redemption.BaseExtension;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Furniture.Lab;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ID;
using ParticleLibrary;
using Redemption.Particles;
using Terraria.GameInput;
using Terraria.Audio;
using Redemption.Items.Usable;

namespace Redemption.Globals.Player
{
    public class RitualistPlayer : ModPlayer
    {
        public int SpiritLevel;
        public int SpiritLevelCap = 2;
        public float SpiritGauge;
        public float SpiritGaugeMax = 60;
        public float SpiritGaugeCD;
        public float SpiritGaugeCDMax = 600;
        public float SpiritGaugeCDCD;
        public override void ResetEffects()
        {
            SpiritLevelCap = 2;
            SpiritGaugeMax = 60;
            SpiritGaugeCDMax = 600;
        }
        public override void PostUpdate()
        {
            if (SpiritGaugeCD <= 0)
                SpiritGauge--;
            else
                SpiritGaugeCD--;

            if (SpiritGauge >= SpiritGaugeMax && SpiritLevel < SpiritLevelCap)
            {
                SpiritGaugeCD = SpiritGaugeCDMax;
                SpiritGauge = 5;
                SpiritLevel++;
            }
            else if (SpiritGauge <= 0)
            {
                if (SpiritLevel > 0)
                    SpiritGauge = SpiritGaugeMax - 5;
                SpiritLevel--;
            }
            SpiritLevel = (int)MathHelper.Clamp(SpiritLevel, 0, SpiritLevelCap);
            SpiritGauge = MathHelper.Clamp(SpiritGauge, 0, SpiritGaugeMax);
            SpiritGaugeCD = MathHelper.Clamp(SpiritGaugeCD, 0, SpiritGaugeCDMax);
        }
        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit, int cooldownCounter)
        {
            SpiritGauge -= 15;
        }
        public override void OnHitNPCWithProj(Projectile proj, Terraria.NPC target, int damage, float knockback, bool crit)
        {
            if (proj.Redemption().RitDagger)
            {
                SpiritGaugeCD = SpiritGaugeCDMax;
                if (Main.rand.NextBool(2))
                {
                    for (int i = 0; i < Main.rand.Next(1, 3); i++)
                        Item.NewItem(proj.GetSource_FromAI(), target.getRect(), ModContent.ItemType<RitSpirit>(), 1, false, 0, true);
                }
            }
        }
    }
}