using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Dusts;
using Terraria.Audio;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class HolyPhalanx_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Phalanx");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.Redemption().Unparryable = true;
        }
        private float rot;
        private float speed = 1;
        private float dist;
        private float dist2 = 0.2f;
        private bool s;
        public override void AI()
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (!host.active || (host.type != ModContent.NPCType<Erhan>() && host.type != ModContent.NPCType<ErhanSpirit>()))
                Projectile.Kill();
            switch (Projectile.localAI[1])
            {
                case 0:
                    Projectile.rotation = -MathHelper.PiOver4;
                    Projectile.Center = host.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(Projectile.ai[1])) * 80;
                    if (!s)
                    {
                        DustHelper.DrawCircle(Projectile.Center, DustID.GoldFlame, 2, 2, 2, 1, 4, nogravity: true);
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), Vector2.Zero, Scale: 2);
                        dust.noGravity = true;
                        Color dustColor = new(255, 255, 120) { A = 0 };
                        dust.color = dustColor;
                        s = true;
                    }
                    if (Projectile.localAI[0]++ >= 50)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Slice2"), Projectile.position);
                        rot = host.Center.ToRotation();
                        Projectile.localAI[0] = 0;
                        Projectile.localAI[1] = 1;
                        Projectile.netUpdate = true;
                    }
                    break;
                case 1:
                    Projectile.Center = host.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(Projectile.ai[1] + rot)) * (80 + dist);
                    if (Projectile.localAI[0]++ >= 40)
                    {
                        Projectile.rotation = Projectile.DirectionTo(host.Center).ToRotation() - MathHelper.PiOver4;
                        rot += speed;
                        dist += dist2;
                        if (Projectile.localAI[0] >= 160)
                            speed *= 0.98f;
                        else
                            speed *= 1.02f;
                        speed = MathHelper.Min(speed, 3);
                        if (Projectile.localAI[0] >= 160)
                            dist2 *= 0.98f;
                        else
                            dist2 *= 1.03f;
                        dist2 = MathHelper.Min(dist2, 10);
                    }
                    else
                        Projectile.rotation.SlowRotation(Projectile.DirectionTo(host.Center).ToRotation() - MathHelper.PiOver4, MathHelper.Pi / 10);
                    if (Projectile.localAI[0] >= 240)
                    {
                        Projectile.velocity *= 0;
                        Projectile.localAI[0] = 0;
                        Projectile.localAI[1] = 2;
                        Projectile.netUpdate = true;
                    }
                    break;
                case 2:
                    Projectile.rotation.SlowRotation(Projectile.DirectionTo(host.Center).ToRotation() + MathHelper.PiOver4, MathHelper.Pi / 10);
                    if (Projectile.localAI[0]++ == 40 && !Main.dedServ)
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/Slice1"), host.Center);
                    if (Projectile.localAI[0] >= 40)
                        Projectile.velocity = Projectile.DirectionTo(host.Center) * 20;
                    if (Projectile.DistanceSQ(host.Center) <= 60 * 60)
                        Projectile.Kill();
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = new Color(255, 255, 120) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            Dust dust2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), Vector2.Zero, Scale: 2);
            dust2.noGravity = true;
            Color dustColor = new(255, 255, 120) { A = 0 };
            dust2.color = dustColor;
            for (int i = 0; i < 15; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GoldFlame, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 3);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
