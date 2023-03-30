using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Melee
{
    public class TidalWake : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tidal Wake");
            /* Tooltip.SetDefault("Turns into a whirlpool, pulling in weak enemies\n" +
                "Slain enemies affected by the whirlpool will heal the user"); */
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.damage = 63;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 20;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.knockBack = 5f;
            Item.value = Item.buyPrice(0, 7, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<TidalWake_Proj>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item1;
        }
    }
}