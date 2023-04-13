using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Melee;
using Redemption.Globals;
using Terraria.Localization;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class SlayerFist : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.ExplosiveS);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Slayer's Rocket Fist");
            /* Tooltip.SetDefault("Punches enemies up-close\n" +
                "Holding down left-click and hitting an enemy will fire an " + ElementID.ExplosiveS + " rocket fist if you are airborne"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 170;
            Item.DamageType = DamageClass.Melee;
            Item.width = 46;
            Item.height = 24;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item74;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SlayerFist_Proj>();
            Item.ExtraItemShoot(ModContent.ProjectileType<KS3_FistF>());
            Item.shootSpeed = 5f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CyberPlating>(), 4)
                .AddIngredient(ModContent.ItemType<Capacitor>(), 2)
                .AddIngredient(ModContent.ItemType<AIChip>())
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}