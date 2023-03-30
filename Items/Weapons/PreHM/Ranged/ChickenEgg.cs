using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class ChickenEgg : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("'Which came first...'");

            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 20;
            Item.damage = 3;
            Item.knockBack = 3;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 50;
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
            Item.shoot = ModContent.ProjectileType<ChickenEgg_Proj>();
        }
    }
}
