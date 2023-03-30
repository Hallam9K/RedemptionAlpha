using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Globals.Player;

namespace Redemption.Items.Weapons.PreHM.Ritualist
{
    public class BloodLetter_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Ritualist/BloodLetter";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Blood Letter");
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }
        public float newX = 20;
        public float newX2 = 1;
        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];
            if (!projOwner.channel || (projOwner.GetModPlayer<RitualistPlayer>().SpiritLevel <= 0 && projOwner.GetModPlayer<RitualistPlayer>().SpiritGauge <= 0))
                Projectile.Kill();

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] >= 20 && newX != 0 && Projectile.localAI[0] < 30)
            {
                newX += -2f + newX2;
                newX2 *= 1.02f;
            }
            Player.CompositeArmStretchAmount arm = Player.CompositeArmStretchAmount.Full;
            if (Projectile.localAI[0] > 20 && Projectile.localAI[0] <= 30)
                arm = Player.CompositeArmStretchAmount.ThreeQuarters;
            else if (Projectile.localAI[0] > 30)
            {
                for (int i = 0; i < 2; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.position + (projOwner.direction == 1 ? Vector2.Zero : new Vector2(Projectile.width - 2, 0)), 2, Projectile.height, DustID.Blood);
                    Main.dust[dustIndex].velocity.X = 3f * projOwner.direction;
                }
                projOwner.crimsonRegen = true;
                projOwner.GetModPlayer<RitualistPlayer>().SpiritGaugeCD++;
                if (Projectile.localAI[0] % 4 == 0)
                    projOwner.GetModPlayer<RitualistPlayer>().SpiritGauge--;
                if (Projectile.localAI[0] % 8 == 0 && projOwner.statLife < projOwner.statLifeMax2)
                    projOwner.statLife += 1;

                arm = Player.CompositeArmStretchAmount.Quarter;
            }
            if (Projectile.localAI[0] == 30)
            {
                for (int i = 0; i < 10; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                    Main.dust[dustIndex].velocity.X *= -6f * projOwner.direction;
                }
                SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.position);
            }

            projOwner.SetCompositeArmFront(true, arm, (projOwner.Center - Projectile.Center).ToRotation() + MathHelper.PiOver2);
            Projectile.rotation = MathHelper.ToRadians(225) * projOwner.direction;
            Projectile.direction = projOwner.direction;
            Projectile.spriteDirection = projOwner.direction;
            projOwner.heldProj = Projectile.whoAmI;
            projOwner.itemTime = 2;
            projOwner.itemAnimation = 2;
            Projectile.position.X = projOwner.Center.X - (Projectile.width / 2) + (newX * projOwner.direction);
            Projectile.position.Y = projOwner.Center.Y - (Projectile.height / 2);
        }
    }
}