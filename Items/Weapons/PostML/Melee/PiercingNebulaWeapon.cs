using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PostML;
using Redemption.NPCs.Bosses.Neb;
using Redemption.Projectiles.Melee;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class PiercingNebulaWeapon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Piercing Nebula");
            Tooltip.SetDefault("Deals less damage the further away the target\n" +
                "'Penetrates through even the fabric of space'");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 444;
            Item.DamageType = DamageClass.Melee;
            Item.width = 82;
            Item.height = 82;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8;
            Item.value = Item.buyPrice(1, 0, 0, 0);
            Item.UseSound = SoundID.Item125;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PNebula1_Friendly>();
            Item.shootSpeed = 9f;
            Item.rare = ModContent.RarityType<CosmicRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DayBreak)
                .AddIngredient(ModContent.ItemType<LifeFragment>(), 8)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return true;
        }
    }
}