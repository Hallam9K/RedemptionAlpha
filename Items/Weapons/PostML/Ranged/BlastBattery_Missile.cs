using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria.Audio;
using Terraria.GameContent;
using System.Collections.Generic;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class BlastBattery_Missile : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Obliterator/OO_BarrageMissile";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Missile");
            Main.projFrames[Projectile.type] = 3;
            ElementID.ProjExplosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public int hitCounter;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 2)
                    Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, 1 * Projectile.Opacity, 0.4f * Projectile.Opacity, 0.4f * Projectile.Opacity);

            Projectile projAim = Main.projectile[(int)Projectile.ai[0]];
            if (!projAim.active)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BlastBattery_MissileBlast>(), Projectile.damage, 0, Main.myPlayer);
                Projectile.Kill();
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            if (Projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref Projectile.velocity);
                Projectile.localAI[0] = 1f;
            }
            else
                Projectile.localAI[0]++;

            if (Projectile.localAI[0] > 20)
            {
                Vector2 move = Vector2.Zero;
                float distance = 5000f;
                bool target = false;
                for (int k = 0; k < Main.maxProjectiles; k++)
                {
                    if (Main.projectile[k].active && Main.projectile[k].identity == projAim.identity)
                    {
                        Vector2 newMove = Main.projectile[k].Center - Projectile.Center;
                        float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                        if (distanceTo < distance)
                        {
                            move = newMove;
                            distance = distanceTo;
                            target = true;
                        }
                    }
                }
                if (target)
                {
                    AdjustMagnitude(ref move);
                    Projectile.velocity = (10 * Projectile.velocity + move) / 11f;
                    AdjustMagnitude(ref Projectile.velocity);
                }
            }
            var list = Main.projectile.Where(x => x.Hitbox.Intersects(Projectile.Hitbox));
            foreach (var proj in list)
            {
                if (Projectile != proj && proj.whoAmI == projAim.whoAmI)
                {
                    RedeDraw.SpawnExplosion(Projectile.Center, Color.IndianRed, DustID.LifeDrain);
                    if (Projectile.ai[1] == 1)
                    {
                        proj.localAI[0]++;
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BlastBattery_MissileBlast>(), Projectile.damage, 0, Main.myPlayer);
                        Projectile.Kill();
                    }
                    else
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BlastBattery_MissileBlast>(), Projectile.damage, 0, Main.myPlayer);
                        Projectile.Kill();
                        proj.Kill();
                    }
                }
            }
        }
        private static void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 20f)
            {
                vector *= 20f / magnitude;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BlastBattery_MissileBlast>(), Projectile.damage, 0, Main.myPlayer);
            Projectile.Kill();
        }
        Projectile clearCheck;
        public override void OnKill(int timeLeft)
        {
            Projectile projAim = Main.projectile[(int)Projectile.ai[0]];

            if (Projectile.ai[1] == 0)
            {
                for (int p = 0; p < Main.maxProjectiles; p++)
                {
                    clearCheck = Main.projectile[p];
                    if (clearCheck.whoAmI == projAim.whoAmI && clearCheck.type != Projectile.type && clearCheck.active)
                        clearCheck.Kill();
                }
            }

            SoundEngine.PlaySound(CustomSounds.MissileExplosion with { Volume = 0.7f }, Projectile.position);
            RedeDraw.SpawnExplosion(Projectile.Center, Color.IndianRed, DustID.LifeDrain);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            int height = texture.Height / 3;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(glow, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

    }
    public class BlastBattery_MissileBlast : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/Obliterator/OO_MissileBlast";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Explosion");
            Main.projFrames[Projectile.type] = 5;
            ElementID.ProjExplosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 144;
            Projectile.height = 144;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 5)
                    Projectile.Kill();
            }
            Projectile.scale += 0.02f;
            if (Projectile.frame > 2)
                Projectile.hostile = false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localNPCImmunity[target.whoAmI] = 60;
            target.immune[Projectile.owner] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 5;
            int y = height * Projectile.frame;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 origin = new(texture.Width / 2f, height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
