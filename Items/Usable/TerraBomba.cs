using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Misc;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public abstract class BaseTerraBombaItem : ModItem
    {
        protected abstract int ProjType { get; }
        protected abstract int SeedType { get; }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.value = Item.buyPrice(3);
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.shoot = ProjType;
            Item.shootSpeed = 0f;
            Item.rare = ItemRarityID.LightPurple;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<TerraBombaTail>())
                .AddIngredient(ItemType<TerraBombaCore>())
                .AddIngredient(ItemType<TerraBombaNose>())
                .AddIngredient(SeedType)
                .AddTile(TileType<XeniumRefineryTile>())
                .Register();
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = new Vector2(player.Center.X, player.Center.Y + -1300);
            damage = 500;
        }
    }
    public class TerraBombaCorruption : BaseTerraBombaItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Terraforma Bomba");
            //Tooltip.SetDefault("[c/b883d8:Corruption]"
            //    + "\n'Country roads, take me home...'"
            //    + "\nWARNING: Turns a colossal radius into Corruption");
        }
        protected override int ProjType => ProjectileType<TerraBombaCorruption_Proj>();
        protected override int SeedType => ItemID.CorruptSeeds;
    }
    public class TerraBombaCrimson : BaseTerraBombaItem
    {
        protected override int ProjType => ProjectileType<TerraBombaCrimson_Proj>();
        protected override int SeedType => ItemID.CrimsonSeeds;
    }
    public class TerraBombaGlowingMushroom : BaseTerraBombaItem
    {
        protected override int ProjType => ProjectileType<TerraBombaGlowingMushroom_Proj>();
        protected override int SeedType => ItemID.MushroomGrassSeeds;
    }
    public class TerraBombaHallow : BaseTerraBombaItem
    {
        protected override int ProjType => ProjectileType<TerraBombaHallow_Proj>();
        protected override int SeedType => ItemID.HallowedSeeds;
    }
    public class TerraBombaPurity : BaseTerraBombaItem
    {
        protected override int ProjType => ProjectileType<TerraBombaPurity_Proj>();
        protected override int SeedType => ItemID.GrassSeeds;
    }
    public class TerraBombaDirt : BaseTerraBombaItem
    {
        protected override int ProjType => ProjectileType<TerraBombaDirt_Proj>();
        protected override int SeedType => ItemID.DirtBlock;
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<TerraBombaTail>())
                .AddIngredient(ItemType<TerraBombaCore>())
                .AddIngredient(ItemType<TerraBombaNose>())
                .AddIngredient(SeedType, 50)
                .AddTile(TileType<XeniumRefineryTile>())
                .Register();
        }
    }
    public class TerraBombaSand : BaseTerraBombaItem
    {
        protected override int ProjType => ProjectileType<TerraBombaSand_Proj>();
        protected override int SeedType => ItemID.SandBlock;
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<TerraBombaTail>())
                .AddIngredient(ItemType<TerraBombaCore>())
                .AddIngredient(ItemType<TerraBombaNose>())
                .AddIngredient(SeedType, 50)
                .AddTile(TileType<XeniumRefineryTile>())
                .Register();
        }
    }
    public class TerraBombaSnow : BaseTerraBombaItem
    {
        protected override int ProjType => ProjectileType<TerraBombaSnow_Proj>();
        protected override int SeedType => ItemID.SnowBlock;
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemType<TerraBombaTail>())
                .AddIngredient(ItemType<TerraBombaCore>())
                .AddIngredient(ItemType<TerraBombaNose>())
                .AddIngredient(SeedType, 50)
                .AddTile(TileType<XeniumRefineryTile>())
                .Register();
        }
    }
}