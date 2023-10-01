using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Redemption.Globals;
using Redemption.Base;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.NPCs.Lab.MACE;

namespace Redemption.Projectiles.Magic
{
    public class PlutoniumNuke_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Plutonium Nuke");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjExplosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 280;
        }

        public override void AI()
        {
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] >= 30)
                Projectile.tileCollide = true;

            Lighting.AddLight(Projectile.Center, Projectile.Opacity * .3f, Projectile.Opacity * .3f, Projectile.Opacity * .5f);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Vector2 position = Projectile.Center + (Vector2.Normalize(Projectile.velocity) * 10f);
            Dust dust20 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy)];
            dust20.position = position;
            dust20.velocity = (Projectile.velocity.RotatedBy(MathHelper.PiOver2) * 0.33f) + (Projectile.velocity / 4f);
            dust20.position += Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            dust20.fadeIn = 0.5f;
            dust20.noGravity = true;
            dust20 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy)];
            dust20.position = position;
            dust20.velocity = (Projectile.velocity.RotatedBy(-MathHelper.PiOver2) * 0.33f) + (Projectile.velocity / 4f);
            dust20.position += Projectile.velocity.RotatedBy(-MathHelper.PiOver2);
            dust20.fadeIn = 0.5f;
            dust20.noGravity = true;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(CustomSounds.MissileExplosion, Projectile.position);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 10; i++)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, -8 + Main.rand.Next(0, 17), -3 + Main.rand.Next(-11, 0), ModContent.ProjectileType<MACE_Miniblast>(), Projectile.damage / 5, 3, Main.myPlayer, 1);
                    Main.projectile[proj].timeLeft = 300;
                    Main.projectile[proj].hostile = false;
                    Main.projectile[proj].friendly = true;
                    Main.projectile[proj].netUpdate = true;
                }
            }
            RedeDraw.SpawnExplosion(Projectile.Center, Color.LightCyan, scale: 2);
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || target.dontTakeDamage || target.friendly)
                    continue;

                if (target.immune[Projectile.whoAmI] > 0 || Projectile.DistanceSQ(target.Center) > 140 * 140)
                    continue;

                target.immune[Projectile.whoAmI] = 20;
                int hitDirection = target.RightOfDir(Projectile);
                BaseAI.DamageNPC(target, Projectile.damage, Projectile.knockBack, hitDirection, Projectile, crit: Projectile.HeldItemCrit());
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.LightCyan) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.whoAmI] = 20;
        }
    }
}
