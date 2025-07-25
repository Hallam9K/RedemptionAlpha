using Redemption.BaseExtension;
using Redemption.Buffs.Debuffs;
using Redemption.Globals;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Misc
{
    public class BlueFly_Proj : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Critters/Fly";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 18000;
            Projectile.tileCollide = true;
            Projectile.scale = 1.2f;
        }

        private SlotId loop;
        private float loopVolume;
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            loopVolume = 0;

            if (Math.Abs(Projectile.velocity.X) > 0.2)
                Projectile.spriteDirection = -Projectile.direction;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

            if (!RedeConfigClient.Instance.NoFlyBuzz)
                loopVolume = .1f * Projectile.scale;

            if (Projectile.velocity.Length() < 4)
                Projectile.velocity = RedeHelper.PolarVector(10, RedeHelper.RandomRotation());

            Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f));

            if (Main.myPlayer == Projectile.owner && Projectile.ai[0]++ % 10 == 0)
                Projectile.velocity = Projectile.DirectionTo(Main.player[Projectile.owner].Center) * 10f;

            CustomSounds.UpdateLoopingSound(ref loop, CustomSounds.FlyBuzz with { MaxInstances = 3 }, loopVolume, 0, Projectile.position);

            if (Projectile.wet)
                Projectile.Kill();
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.DarkSlateBlue * Projectile.Opacity;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1.WithVolumeScale(.4f), Projectile.position);
            loopVolume = 0;
            CustomSounds.UpdateLoopingSound(ref loop, CustomSounds.FlyBuzz with { MaxInstances = 3 }, loopVolume, 0, Projectile.position);

            Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GreenBlood, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if ((target.RedemptionNPCBuff().devilScented || NPCLists.Undead.Contains(target.type) || NPCLists.SkeletonHumanoid.Contains(target.type)) && Main.rand.NextBool(3))
                target.AddBuff(BuffType<InfestedDebuff>(), Main.rand.Next(60, 180));
        }

        public override bool? CanHitNPC(NPC target) => target.CanBeChasedBy() ? null : false;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != Projectile.oldVelocity.X)
                Projectile.velocity.X = -Projectile.oldVelocity.X;

            if (Projectile.velocity.Y != Projectile.oldVelocity.Y)
                Projectile.velocity.Y = -Projectile.oldVelocity.Y;

            return false;
        }
    }
}