using Microsoft.Build.Execution;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Projectiles;
using Redemption.Textures;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Thorn
{
    public class Thorn_SplinterBall : ModProjectile
    {
        private bool spawnDust;

        public int TargetPlayer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 42;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            foreach (Projectile other in Main.ActiveProjectiles)
            {
                if (other.whoAmI != Projectile.whoAmI && other.type == Projectile.type && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                        Projectile.velocity.X -= .04f;
                    else
                        Projectile.velocity.X += .04f;

                    if (Projectile.position.Y < other.position.Y)
                        Projectile.velocity.Y -= .04f;
                    else
                        Projectile.velocity.Y += .04f;
                }
            }
            Projectile.localAI[1] = (float)Math.Sin(Projectile.localAI[0]++ / 15) / 10;
            Projectile.localAI[0] *= 1.02f;

            Projectile.velocity *= 0.92f;
            if (!spawnDust)
            {
                magicOpacity = 1;
                for (int i = 0; i < 10; i++)
                {
                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                    {
                        PositionInWorld = RedeHelper.RandAreaInEntity(Projectile),
                    });
                }
                spawnDust = true;
            }
            if (magicOpacity > 0)
                magicOpacity -= .01f;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 22; i++)
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.JungleGrass);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 8; i++)
                {
                    SoundEngine.PlaySound(SoundID.Item17, Projectile.position);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, RedeHelper.PolarVector(12, MathHelper.ToRadians(45) * i), ModContent.ProjectileType<Thorn_Splinter>(), Projectile.damage, 3, Main.myPlayer, ai2: Projectile.ai[2]);
                }
            }
        }

        float magicOpacity;
        float drawTimer;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Main.EntitySpriteDraw(CommonTextures.WhiteGlow.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.Brown with { A = 0 }) * (-magicOpacity + 1), Projectile.rotation, CommonTextures.WhiteGlow.Size() / 2, Projectile.scale * .2f + Projectile.localAI[1], 0, 0);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2, Projectile.scale + Projectile.localAI[1], 0, 0);

            if (magicOpacity > 0)
            {
                Color color = Color.SkyBlue;
                RedeDraw.DrawTreasureBagEffect(Main.spriteBatch, texture, ref drawTimer, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, texture.Size() / 2, Projectile.scale + Projectile.localAI[1], 0, magicOpacity);

                Main.EntitySpriteDraw(CommonTextures.WhiteFlare.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(color with { A = 0 }) * magicOpacity, Projectile.rotation, CommonTextures.WhiteFlare.Size() / 2, Projectile.scale * magicOpacity, 0, 0);
                Main.EntitySpriteDraw(CommonTextures.WhiteFlare.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White with { A = 0 }) * magicOpacity, -Projectile.rotation, CommonTextures.WhiteFlare.Size() / 2, Projectile.scale * .6f * magicOpacity, 0, 0);
            }
            return false;
        }
    }
}