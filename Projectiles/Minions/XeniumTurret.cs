using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Buffs.Minions;
using Redemption.Globals;
using Redemption.Items.Weapons.PostML.Summon;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class XeniumTurret : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Xenium Autoturret");
            Main.projPet[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 3;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 52;
            Projectile.tileCollide = false;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override bool? CanCutTiles() => false;

        private NPC target;
        Vector2 AttackPos;
        private int attackPosition;
        private int attackPositionY;
        public Vector2 MoveVector2;
        public Vector2 pos = new(0, -5);

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }

            if (!CheckActive(owner))
                return;
            OverlapCheck();

            if (RedeHelper.ClosestNPC(ref target, 2000, Projectile.Center, false, owner.MinionAttackTargetNPC))
            {
                pos *= 0.98f;

                if (Projectile.ai[0] % 120 == 0)
                {
                    attackPosition = Main.rand.Next(target.width + 140, target.width + 220);
                    attackPositionY = Main.rand.Next(target.width - 25, target.width);
                    AttackPos = new(attackPosition * Projectile.RightOfDir(target), -attackPositionY);
                }

                Projectile.Move(target.Center + AttackPos, 15, 10);
                Projectile.LookAtEntity(target);

                if (++Projectile.ai[0] % 18 == 0)
                {
                    if (owner.PickAmmo(new Item(ItemType<XeniumDrone>()), out int bulletID, out _, out _, out _, out _, !Main.rand.NextBool(5)))
                    {
                        float shootSpeed = 10f;

                        int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(shootSpeed, (target.Center - Projectile.Center).ToRotation()), bulletID, Projectile.damage, Projectile.knockBack, owner.whoAmI);
                        Main.projectile[p].DamageType = DamageClass.Summon;
                        Main.projectile[p].netUpdate = true;
                    }
                }
                MoveVector2 = Vector2.Zero;
            }
            else
            {
                if (Projectile.localAI[1] == 0)
                {
                    pos.Y += 0.3f;
                    if (pos.Y > 1.7f)
                        Projectile.localAI[1] = 1;
                }
                else if (Projectile.localAI[1] == 1)
                {
                    pos.Y -= 0.3f;
                    if (pos.Y < -1.7f)
                        Projectile.localAI[1] = 0;
                }
                Vector2 playerPos = new(owner.Center.X + (20 + Projectile.minionPos * 50) * -owner.direction, owner.Center.Y - 62 + MoveVector2.Y);
                if (Projectile.DistanceSQ(playerPos) < 100 * 100)
                    Projectile.spriteDirection = owner.direction;
                else
                    Projectile.LookByVelocity();
                Projectile.Move(playerPos, 10, 5);
                MoveVector2 = pos;
            }
            if (Main.myPlayer == owner.whoAmI && Projectile.DistanceSQ(owner.Center) > 2000 * 2000)
            {
                Projectile.position = owner.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(BuffType<XeniumTurretBuff>());

                return false;
            }

            if (owner.HasBuff(BuffType<XeniumTurretBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glow = Request<Texture2D>(Texture + "_Glow").Value;
            int height = texture.Height / 3;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), lightColor * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.White * Projectile.Opacity, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
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
