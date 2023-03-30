using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class Electronade : ModItem
	{
		public override void SetStaticDefaults()
		{
            // Tooltip.SetDefault("Throw an energy-filled grenade");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 11));
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
            Item.width = 16;
            Item.height = 20;
            Item.damage = 190;
            Item.knockBack = 5;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 2, 54, 0);
            Item.rare = ItemRarityID.Purple;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.UseSound = SoundID.Item7;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<Electronade_Proj>();
        }
	}
}
