using Microsoft.Xna.Framework;
using Redemption.Buffs.Minions;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Minions
{
    public class LogStaff_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Log");
            ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 20;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.spriteDirection = (int)Projectile.ai[0];
            if (!CheckActive(owner))
                return;

            if (Projectile.localAI[0]++ % 80 == 0 && Projectile.localAI[0] >= 80 && Projectile.owner == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Item61 with { Volume = .5f }, Projectile.position);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - new Vector2(0, 8), new Vector2(Main.rand.Next(10, 13) * Projectile.spriteDirection, -Main.rand.Next(4, 7)), ModContent.ProjectileType<AcornBomb_Proj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            Projectile.velocity.Y += 1;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Y > 0)
                Projectile.velocity.Y = 0;
            return false;
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<LogStaffBuff>());
                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<LogStaffBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }
    }
}