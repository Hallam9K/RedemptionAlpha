using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class RopeHook : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Affected by gravity");

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.AmethystHook);
            Item.shootSpeed = 16f;
            Item.shoot = ModContent.ProjectileType<RopeHook_Proj>();
            Item.value = Item.buyPrice(0, 1, 25, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Hook)
                .AddIngredient(ItemID.RopeCoil, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class RopeHook_Proj : ModProjectile
    {
        private static Asset<Texture2D> chainTexture;
        public override void Load()
        {
            chainTexture = ModContent.Request<Texture2D>("Redemption/Items/Accessories/PreHM/RopeHookChain");
        }

        public override void Unload()
        {
            chainTexture = null;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.SingleGrappleHook[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.GemHookAmethyst);
        }

        public override void PostAI()
        {
            Projectile.velocity.Y += 0.6f;
        }
        public override float GrappleRange() => 500f;

        public override void NumGrappleHooks(Player player, ref int numHooks) => numHooks = 1;

        public override void GrappleRetreatSpeed(Player player, ref float speed) => speed = 16f;

        public override void GrapplePullSpeed(Player player, ref float speed) => speed = 8;

        // Draws the grappling hook's chain.
        public override bool PreDrawExtras()
        {
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 center = Projectile.Center;
            Vector2 directionToPlayer = playerCenter - Projectile.Center;
            float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
            float distanceToPlayer = directionToPlayer.Length();

            while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer))
            {
                directionToPlayer /= distanceToPlayer; //get unit vector
                directionToPlayer *= chainTexture.Height(); //multiply by chain link length

                center += directionToPlayer; //update draw position
                directionToPlayer = playerCenter - center; //update distance
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
