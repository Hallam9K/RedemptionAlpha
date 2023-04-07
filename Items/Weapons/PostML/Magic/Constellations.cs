using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Materials.PostML;
using Redemption.Projectiles.Magic;
using Redemption.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class Constellations : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("'We own the stars, we own the sky'");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 500;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 18;
            Item.width = 30;
            Item.height = 34;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 5;
            Item.value = Item.buyPrice(1, 0, 0, 0);
            Item.rare = ModContent.RarityType<CosmicRarity>();
            Item.shootSpeed = 0;
            Item.shoot = ModContent.ProjectileType<Constellations_Star>();
            Item.UseSound = CustomSounds.Teleport1 with { Pitch = -0.1f };
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld + RedeHelper.PolarVector(Main.rand.Next(100, 301), RedeHelper.RandomRotation());
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpellTome)
                .AddIngredient(ItemID.NebulaArcanum)
                .AddIngredient<LifeFragment>(6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}