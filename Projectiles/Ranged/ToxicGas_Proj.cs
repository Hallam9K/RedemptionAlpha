using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;

namespace Redemption.Projectiles.Misc
{
    public class ToxicGas_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Textures/IceMist";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Toxic Gas");
            ElementID.ProjPoison[Type] = true;
            ElementID.ProjWind[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 10;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.scale = Main.rand.NextFloat(0.5f, 0.8f);
            Projectile.rotation = RedeHelper.RandomRotation();
        }
        public override void AI()
        {
            if (Projectile.ai[0] != 0)
                Projectile.usesLocalNPCImmunity = false;
            if (Projectile.localAI[0] == 0)
            {
                if (Projectile.ai[0] != 0)
                    Projectile.timeLeft = 180;
                Projectile.localAI[0] = Main.rand.Next(1, 3);
            }

            if (Projectile.localAI[0] == 1)
                Projectile.rotation -= 0.01f;
            else if (Projectile.localAI[0] == 2)
                Projectile.rotation += 0.01f;

            if (Projectile.timeLeft < 80)
            {
                Projectile.alpha += 20;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
            else
            {
                if (Projectile.alpha >= 0)
                    Projectile.alpha -= 5;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (!target.active || !target.CanBeChasedBy())
                        continue;

                    if (!Projectile.Hitbox.Intersects(target.Hitbox))
                        continue;

                    target.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), 420);
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(5))
                target.AddBuff(ModContent.BuffType<GlowingPustulesDebuff>(), 300);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.ForestGreen), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.ForestGreen), -Projectile.rotation / 2, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.ForestGreen), -Projectile.rotation, drawOrigin, Projectile.scale * 0.1f, effects, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.ForestGreen), Projectile.rotation / 2, drawOrigin, Projectile.scale * 1.1f, effects, 0);
            return false;
        }
    }
}