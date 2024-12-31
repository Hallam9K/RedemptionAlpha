using Redemption.Buffs.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Pets
{
    public class NebPet2_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 13;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(1, 9, 5).WithOffset(2, 0).WithSpriteDirection(-1);
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyDino);
            Projectile.width = 34;
            Projectile.height = 52;
            AIType = ProjectileID.BabyDino;
        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            player.dino = false;
            return true;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CheckActive(player);
            if (Main.myPlayer == player.whoAmI && Projectile.DistanceSQ(player.Center) > 2000 * 2000)
            {
                Projectile.position = player.Center;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }
        }
        private void CheckActive(Player player)
        {
            if (!player.dead && player.HasBuff(BuffType<NebPetBuff2>()))
                Projectile.timeLeft = 2;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.penetrate == 0)
                Projectile.Kill();
            return false;
        }
    }
}