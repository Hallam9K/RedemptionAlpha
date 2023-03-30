using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using System.Collections.Generic;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class SunInThePalm_Proj : TrueMeleeProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun-In-Palm");
        }
        public override bool ShouldUpdatePosition() => false;
        public override void SetSafeDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Length = 24;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        private Vector2 startVector;
        private Vector2 vector;
        public ref float Length => ref Projectile.localAI[0];
        public ref float Rot => ref Projectile.localAI[1];
        public float Timer;
        public bool rotRight;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.noItems || (!player.channel && player.ownedProjectileCounts[ModContent.ProjectileType<SunInThePalm_EnergyBall>()] == 0) || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.Center = player.MountedCenter + vector;

            if (Main.MouseWorld.X < player.Center.X)
                player.direction = -1;
            else
                player.direction = 1;
            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver2;
            else
                Projectile.rotation = (Projectile.Center - player.Center).ToRotation() + MathHelper.PiOver2;

            if (Main.myPlayer == Projectile.owner)
            {
                startVector = RedeHelper.PolarVector(1, (Main.MouseWorld - player.Center).ToRotation());
                vector = startVector * Length;
                Player.CompositeArmStretchAmount arm = Player.CompositeArmStretchAmount.Full;
                switch (Projectile.frame)
                {
                    case 0:
                        arm = Player.CompositeArmStretchAmount.None;
                        break;
                    case 1:
                        arm = Player.CompositeArmStretchAmount.Quarter;
                        break;
                    case 2:
                        arm = Player.CompositeArmStretchAmount.ThreeQuarters;
                        break;
                }
                player.SetCompositeArmFront(true, arm, (player.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
                switch (Projectile.ai[0])
                {
                    case 0:
                        if (Projectile.frameCounter++ % 5 == 0)
                        {
                            if (++Projectile.frame > 3)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SunInThePalm_EnergyBall>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Projectile.whoAmI);
                                Projectile.frame = 3;
                                Projectile.ai[0] = 1;
                            }
                        }
                        break;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}