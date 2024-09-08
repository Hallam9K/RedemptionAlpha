using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Redemption.Projectiles.Magic;
using Redemption.Items.Weapons.PostML.Melee;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class Petridish : ModItem
	{
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PZGauntlet>();
        }

        public override void SetDefaults()
		{
            Item.damage = 260;
            Item.width = 24;
            Item.height = 20;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 18;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(0, 7, 50, 0);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Petridish_Proj>();
            Item.shootSpeed = 14f;
        }
    }
}
