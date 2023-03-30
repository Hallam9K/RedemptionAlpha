using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Redemption.Projectiles.Magic;
using Terraria.DataStructures;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class GiantDandelion : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Casts down giant dandelion seeds from the sky");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.mana = 7;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<GiantDandelionSeed>();
            Item.shootSpeed = 10f;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 spawn = new(Main.MouseWorld.X + Main.rand.Next(-200, 201), player.Center.Y - Main.rand.Next(800, 861));
                Projectile.NewProjectile(source, spawn, new Vector2(Main.rand.Next(-6, 7), 2), type, damage, knockback, Main.myPlayer);
            }
            return false;
        }
    }
}
