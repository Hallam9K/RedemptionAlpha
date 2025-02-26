using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class HammerOfProving : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.HolyS, ElementID.ArcaneS);
        public override void SetStaticDefaults()
        {
            ElementID.ItemHoly[Type] = true;
            ElementID.ItemArcane[Type] = true;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 80;
            Item.height = 80;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.sellPrice(gold: 5);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;

            // Weapon Properties
            Item.damage = 98;
            Item.knockBack = 9;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.channel = true;

            // Projectile Properties
            Item.shootSpeed = 5f;
            Item.shoot = ProjectileType<HammerOfProving_Proj>();
            Item.Redemption().TechnicallyHammer = true;
            Item.Redemption().HideElementTooltip[ElementID.Holy] = true;
            Item.Redemption().HideElementTooltip[ElementID.Arcane] = true;
        }
        public override bool MeleePrefix() => true;
        public override bool CanUseItem(Player player) => player.velocity.Y != 0;
        public int pogo;
        public bool onUse;
        public override void HoldItem(Player player)
        {
            Point tileBelow = player.Bottom.ToTileCoordinates();
            Tile tile = Framing.GetTileSafely(tileBelow.X, tileBelow.Y);
            if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileCut[tile.TileType] && !onUse)
                pogo = 0;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 15)
                .AddRecipeGroup(RedeRecipe.GoldRecipeGroup, 15)
                .AddIngredient(ItemID.Ruby, 2)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
