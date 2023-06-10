using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class EggBomb : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("'Takes egging to a whole new level'");
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 24;
            Item.damage = 28;
            Item.knockBack = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 0, 70);
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 35;
            Item.useTime = 35;
            Item.UseSound = SoundID.Item1;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shootSpeed = 7f;
            Item.shoot = ModContent.ProjectileType<EggBomb_Proj>();
            Item.ammo = Item.type;
        }
    }
}
