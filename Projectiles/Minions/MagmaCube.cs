using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Buffs.Minions;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using Redemption.Globals.Player;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class MagmaCube : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CountsAsHoming[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
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
            BaseAI.AIMinionSlime(Projectile, ref Projectile.ai, projOwner, false, 40, 800, 2000, 2, 5, 10, getTarget: (proj, owner) => { return target == projOwner ? null : target; });
        }

        public int maxDistToAttack = 800;
        private Entity target;
        public void Target()
        {
            Vector2 startPos = Main.player[Projectile.owner].Center;
            if (target != null && target != Main.player[Projectile.owner] && !RedeHelper.CanTarget(Projectile, target, startPos))
                target = null;

            if (target == null || target == Main.player[Projectile.owner])
            {
                int[] npcs = BaseAI.GetNPCs(startPos, -1, default, maxDistToAttack);
                float prevDist = maxDistToAttack;
                foreach (int i in npcs)
                {
                    NPC npc = Main.npc[i];
                    float dist = Vector2.Distance(startPos, npc.Center);
                    if (RedeHelper.CanTarget(Projectile, npc, startPos) && dist < prevDist) { target = npc; prevDist = dist; }
                }
            }
            if (target == null) 
                target = Main.player[Projectile.owner];
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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];

            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.OnFire, 240);

            if (player.GetModPlayer<BuffPlayer>().dragonLeadBonus)
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