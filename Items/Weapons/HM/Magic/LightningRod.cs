using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class LightningRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Self-Sufficient Lighting Rod");
            /* Tooltip.SetDefault("Hold left-click to charge up electricity, release to fire a shock of lightning at the cursor\n" +
                "Lightning arcs to more enemies and does more damage as it is charged\n" +
                "Overcharging causes it to do less damage but have a wider radius"); */
            Item.staff[Item.type] = true;
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.height = 60;
            Item.width = 60;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.knockBack = 6;
            Item.channel = true;
            Item.rare = ItemRarityID.Pink;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 2, 0, 0);
            Item.UseSound = SoundID.Item125;
            Item.shoot = ModContent.ProjectileType<LightningRod_Proj>();
        }
    }
}