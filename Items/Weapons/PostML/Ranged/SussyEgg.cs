using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class SussyEgg : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Suspicious Egg");
            // Tooltip.SetDefault("I wouldn't break it if I were you");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 7));
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
            Item.width = 16;
            Item.height = 20;
            Item.damage = 3;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 50;
            Item.rare = ItemRarityID.Cyan;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.UseSound = SoundID.Item1;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shootSpeed = 18f;
            Item.shoot = ModContent.ProjectileType<SussyEgg_Proj>();
        }
    }
}
