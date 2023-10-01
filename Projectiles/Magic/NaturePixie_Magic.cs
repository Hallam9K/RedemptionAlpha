using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.NPCs.PreHM;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class NaturePixie_Magic : ModProjectile
    {
        public override string Texture => "Redemption/Projectiles/Minions/NaturePixie";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.tileCollide = false;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (NPCLists.Dark.Contains(target.type))
                modifiers.FinalDamage *= 1.5f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.DryadsWardDebuff, 300);
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.timeLeft <= 261 && target.type != ModContent.NPCType<ForestNymph>() ? null : false;
        NPC target;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.frame = 0;
            }
            Projectile.LookByVelocity();
            if (Projectile.ai[1] == 0)
                Projectile.rotation = Projectile.velocity.X * 0.05f;
            Lighting.AddLight(Projectile.Center, .1f * Projectile.Opacity, .4f * Projectile.Opacity, .1f * Projectile.Opacity);
            if (Main.rand.NextBool() && Projectile.velocity.Length() > 10)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.DryadsWard, Scale: 1.5f);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }

            if (Projectile.timeLeft > 260)
            {
                Projectile.velocity *= 0.98f;
                Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-.1f, .1f));
                return;
            }
            if (RedeHelper.ClosestNPC(ref target, 600, Projectile.Center, false) && target.type != ModContent.NPCType<ForestNymph>())
            {
                Projectile.timeLeft++;
                if (Projectile.localAI[0] == 0)
                    Projectile.localAI[0] = Main.rand.Next(20, 41);

                Projectile.ai[0]++;
                if (Projectile.DistanceSQ(target.Center) >= 180 * 180)
                    Projectile.Move(target.Center, 12, 30);
                else
                {
                    if (Projectile.ai[0] % Projectile.localAI[0] == 0)
                    {
                        Projectile.localAI[0] = 0;
                        Projectile.velocity = Projectile.Center.DirectionTo(target.Center) * 18;
                    }
                }
            }
            else
            {
                Projectile.timeLeft -= 2;
                if (Projectile.velocity.Length() < 8)
                    Projectile.velocity *= 1.02f;
                if (Projectile.DistanceSQ(player.Center) >= 300 * 300)
                    Projectile.Move(player.Center, 8, 20);
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 14; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.DryadsWard, Scale: 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 6;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(-12f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.White) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.White * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
    }
}