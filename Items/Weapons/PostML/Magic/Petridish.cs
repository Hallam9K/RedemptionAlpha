using Redemption.Items.Weapons.PostML.Melee;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class Petridish : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mitosis");
            // Tooltip.SetDefault("Throw a Petridish filled with bacteria");
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<PZGauntlet>();
        }

        public override void SetDefaults()
        {
            Item.damage = 260;
            Item.width = 24;
            Item.height = 20;
            Item.useTime = 43;
            Item.useAnimation = 43;
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
            Item.shoot = ProjectileType<Petridish_Proj>();
            Item.shootSpeed = 14f;
        }
    }
}
