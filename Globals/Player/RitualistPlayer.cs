using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Microsoft.Xna.Framework;
using Redemption.Items.Usable;
using Redemption.Items.Weapons.PreHM.Ritualist;
using Redemption.Projectiles.Ritualist;

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

        public bool bolineFlower;

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
                IncreaseLevelEffects();
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
        private void IncreaseLevelEffects()
        {
            if (Player.HasItem(ModContent.ItemType<BuddingBoline>()))
                Player.GetModPlayer<RitualistPlayer>().bolineFlower = true;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.type != ModContent.ProjectileType<HellfireCharge_Proj>())
                    continue;

                proj.ai[1] = 1;
            }
        }
        public override void PostHurt(Terraria.Player.HurtInfo info)
        {
            SpiritGauge -= 2;
        }
        public override void OnHitNPCWithProj(Projectile proj, Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone)
        {
            if (proj.Redemption().RitDagger)
            {
                SpiritGaugeCD = SpiritGaugeCDMax;
                if (Main.rand.NextBool(3))
                {
                    for (int i = 0; i < Main.rand.Next(1, 3); i++)
                        Item.NewItem(proj.GetSource_FromAI(), target.getRect(), ModContent.ItemType<RitSpirit>(), 1, false, 0, true);
                }
            }
        }
    }
}