using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class CalciteWand : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Drops stalagmites at cursor point" +
                "\nIncreased damage the higher they fall"); */
            Item.staff[Item.type] = true;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 5;
            Item.width = 34;
            Item.height = 36;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = 29500;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item42;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CalciteWand_Proj>();
            Item.shootSpeed = 0f;
        }
        public override bool CanUseItem(Player player)
        {
            Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
            if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileCut[tile.TileType])
                return false;

            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GraveSteelAlloy>(8)
                .AddRecipeGroup(RedeRecipe.GathicStoneRecipeGroup, 8)
                .AddIngredient(ItemID.Topaz, 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld + RedeHelper.Spread(20);
        }
    }
}