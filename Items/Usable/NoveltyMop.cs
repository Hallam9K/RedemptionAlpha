using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class NoveltyMop : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Novelty Mop");
            // Tooltip.SetDefault("'Not as lethal as Janitor's'");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
            Item.width = 30;
            Item.height = 26;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<NoveltyMop_Proj>();
            Item.shootSpeed = 8f;
        }
    }
}
