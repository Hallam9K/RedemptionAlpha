using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Items.Materials.HM;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class OversizedScrewdriver : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Holds out a spinning screwdriver\n" +
                "Holding down left-click and hitting an enemy will cause you to bounce on it\n" +
                "Each successful bounce increases damage up to 300%\n" +
                "Deals extra damage to robotic enemies\n" +
                "'May cause dizziness'"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.width = 40;
            Item.height = 40;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 8, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item23;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<OversizedScrewdriver_Proj>();
            Item.shootSpeed = 5f;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<OmegaPowerCell>())
                .AddIngredient(ModContent.ItemType<CorruptedXenomite>(), 6)
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 3)
                .AddIngredient(ModContent.ItemType<Plating>(), 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}