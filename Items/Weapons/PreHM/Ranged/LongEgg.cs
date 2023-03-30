using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class LongEgg : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("L o n g  Chicken Egg");
            // Tooltip.SetDefault("'It takes an awfully  l o n g  c h i c k e n  to make a long egg'");

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 20;
            Item.damage = 6;
            Item.knockBack = 3;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 500;
            Item.rare = ItemRarityID.White;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.UseSound = SoundID.Item1;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shootSpeed = 18f;
            Item.shoot = ModContent.ProjectileType<LongEgg_Proj>();
        }
    }
}
