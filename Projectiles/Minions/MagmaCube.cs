using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Minions;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Projectiles.Minions
{
    public class MagmaCube : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ElementID.ProjFire[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 26;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.alpha = 50;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
        }

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => true;

        public override void AI()
        {
            Target();
            Player projOwner = Main.player[Projectile.owner];

            if (!CheckActive(projOwner))
                return;

            Projectile.rotation = Projectile.velocity.X * 0.05f;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            int sparkle = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.FlameBurst, Scale: 1);
            Main.dust[sparkle].velocity *= 0.3f;
            Main.dust[sparkle].noGravity = true;

            if (Main.myPlayer == projOwner.whoAmI && Projectile.DistanceSQ(projOwner.Center) > 2000 * 2000)
            {
                Projectile.position = projOwner.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
            BaseAI.AIMinionSlime(Projectile, ref Projectile.ai, projOwner, false, 40, 1400, 2000, 3, 5, 10, getTarget: (proj, owner) => { return target == projOwner ? null : target; });
        }

        public int maxDistToAttack = 800;
        private Entity target;
        private NPC target2;
        public void Target()
        {
            Player projOwner = Main.player[Projectile.owner];
            if (RedeHelper.ClosestNPC(ref target2, 800, Projectile.Center, false, projOwner.MinionAttackTargetNPC))
                target = target2;
            else 
                target = projOwner;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 3;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            return false;
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<MagmaCubeBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<MagmaCubeBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.OnFire, 240);

            if (player.RedemptionPlayerBuff().dragonLeadBonus)
                target.AddBuff(ModContent.BuffType<DragonblazeDebuff>(), 300);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate == 0)
                Projectile.Kill();
            return false;
        }
    }
}