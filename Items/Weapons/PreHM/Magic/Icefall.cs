using Microsoft.Xna.Framework;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class Icefall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Icefall");
            Tooltip.SetDefault("Lowers the air temperature at cursor point, forming damaging ice crystals that eventually fall to gravity");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.width = 30;
            Item.height = 34;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 3, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shootSpeed = 0;
            Item.shoot = ModContent.ProjectileType<Icefall_Mist>();
            if (Main.netMode != NetmodeID.Server)
                Item.UseSound = new("Redemption/Sounds/Custom/IceMist") { PitchVariance = .1f };
        }
        public override bool CanUseItem(Player player)
        {
            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileCut[tile.TileType])
                return false;

            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
            velocity = new Vector2(Main.rand.NextFloat(-2, 2), 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.WaterBolt)
            .AddIngredient(ModContent.ItemType<GathicCryoCrystal>(), 7)
            .AddTile(TileID.Bookcases)
            .Register();
        }
    }
}