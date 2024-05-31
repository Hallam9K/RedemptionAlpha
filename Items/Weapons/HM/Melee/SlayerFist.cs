using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class SlayerFist : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.ExplosiveS);
        public override void SetStaticDefaults()
        {
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
            Item.UseSound = SoundID.Item1;
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
