using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PostML
{
    public class KeepHook : ModItem
	{
		public override void SetStaticDefaults()
		{
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
            Item.CloneDefaults(ItemID.BatHook);
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<KeepHook_End>();
		}
	}
	public class KeepHook_End : ModProjectile
    {
        private static Asset<Texture2D> chainTexture;
        public override void Load()
        {
            chainTexture = ModContent.Request<Texture2D>("Redemption/Items/Accessories/PostML/KeepHook_Chain");
        }
        public override void Unload()
        {
            chainTexture = null;
        }
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Keep Hook");
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.SingleGrappleHook[Type] = true;
        }
		public override void SetDefaults()
        {
			Projectile.CloneDefaults(ProjectileID.BatHook);
		}
        public override void AI()
        {
			if (Projectile.velocity.Length() == 0)
                Projectile.frame = 1;
			else
                Projectile.frame = 0;
        }
        public override float GrappleRange() => 150f;

        public override void NumGrappleHooks(Player player, ref int numHooks) => numHooks = 1;

        public override void GrappleRetreatSpeed(Player player, ref float speed) => speed = 18f;

        public override void GrapplePullSpeed(Player player, ref float speed) => speed = 16;

        public override bool PreDrawExtras()
        {
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 center = Projectile.Center;
            Vector2 directionToPlayer = playerCenter - Projectile.Center;
            float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
            float distanceToPlayer = directionToPlayer.Length();

            while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer))
            {
                directionToPlayer /= distanceToPlayer;
                directionToPlayer *= chainTexture.Height();

                center += directionToPlayer;
                directionToPlayer = playerCenter - center;
                distanceToPlayer = directionToPlayer.Length();

                Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

                //Draw chain
                Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
                    chainTexture.Value.Bounds, drawColor, chainRotation,
                    chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
