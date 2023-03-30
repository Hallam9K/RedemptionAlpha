using Microsoft.Xna.Framework;
using Redemption.Items.Weapons.HM.Magic;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class EmpyreanBlind : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Casts orbs of light which shine blinding light onto nearby targets");
            Item.staff[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 104;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 16;
            Item.width = 42;
            Item.height = 58;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item105;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LightOrb_Proj>();
            Item.shootSpeed = 18f;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numberProjectiles = Main.rand.Next(1, 3);
            for (int i = 0; i < numberProjectiles; i++)
            {
                velocity *= Main.rand.NextFloat(0.8f, 1.2f);
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(10));
                Projectile.NewProjectile(source, position, perturbedSpeed, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<SunshardGreatstaff>())
                .AddIngredient(ItemID.LunarBar, 7)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}