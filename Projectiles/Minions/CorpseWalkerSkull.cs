using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Minions;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class CorpseWalkerSkull : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Corpse-Walker Skull");
            Main.projFrames[Projectile.type] = 5;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 38;
            Projectile.tileCollide = false;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
        }

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => false;

        private int attackPosition;
        private NPC target;
        Vector2 AttackPos;

        Vector2 projPos;
        private bool Flare;
        private float FlareTimer;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
                return;

            Projectile.rotation = Projectile.velocity.X * 0.05f;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
            if (Flare)
            {
                FlareTimer += 2;
                if (FlareTimer > 60)
                {
                    Flare = false;
                    FlareTimer = 0;
                }
            }

            OverlapCheck();

            if (RedeHelper.ClosestNPC(ref target, 700, Projectile.Center, false, owner.MinionAttackTargetNPC))
            {
                if (attackPosition == 0 || Projectile.ai[0] % 120 == 0)
                {
                    attackPosition = Main.rand.Next(target.width + 40, target.width + 120);
                    AttackPos = new(attackPosition * Projectile.RightOfDir(target), -attackPosition);
                }

                Projectile.Move(target.Center + AttackPos, 10, 30);
                Projectile.LookAtEntity(target);

                if (++Projectile.ai[0] % 50 == 0 && Main.myPlayer == owner.whoAmI)
                {
                    if (Projectile.ai[1] == 0)
                    {
                        projPos = new(Projectile.Center.X + (8 * Projectile.spriteDirection), Projectile.Center.Y + 4);
                        Projectile.ai[1] = 1;
                    }
                    else
                    {
                        projPos = new(Projectile.Center.X, Projectile.Center.Y + 4);
                        Projectile.ai[1] = 0;
                    }
                    Flare = true;
                    FlareTimer = 0;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), projPos, RedeHelper.PolarVector(10, (target.Center - Projectile.Center).ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f)), ModContent.ProjectileType<CorpseWalkerSkull_Proj>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                }
            }
            else
            {
                attackPosition = 0;
                Projectile.LookByVelocity();
                Projectile.Move(new Vector2(owner.Center.X + (20 + Projectile.minionPos * 40) * -owner.direction, owner.Center.Y - 42), 10, 0);
            }

            if (Main.myPlayer == owner.whoAmI && Projectile.DistanceSQ(owner.Center) > 2000 * 2000)
            {
                Projectile.position = owner.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Projectile.ModProjectile.Texture + "_Glow").Value;
            int height = texture.Height / 5;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.LightYellow) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }
        private float Opacity { get => FlareTimer; set => FlareTimer = value; }
        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Rectangle rect = new(0, 0, flare.Width, flare.Height);
            Vector2 origin = new(flare.Width / 2, flare.Height / 2);
            Vector2 position = projPos - Main.screenPosition;
            Color colour = Color.Lerp(Color.White, Color.Yellow, 1f / Opacity * 10f) * (1f / Opacity * 10f);
            if (Flare)
            {
                Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), colour, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(flare, position, new Rectangle?(rect), colour * 0.4f, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<CorpseSkullBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<CorpseSkullBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }

        private void OverlapCheck()
        {
            // If your minion is flying, you want to do this independently of any conditions
            float overlapVelocity = 0.04f;

            // Fix overlap with other minions
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.X += overlapVelocity;
                    }

                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.Y += overlapVelocity;
                    }
                }
            }
        }
    }
}