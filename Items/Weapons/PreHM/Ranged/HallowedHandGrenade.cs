using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Redemption.Items.Weapons.PreHM.Summon;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class HallowedHandGrenade : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Hallowed Hand Grenade of Anglon");
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<HolyBible>();
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 36;
            Item.damage = 50;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 0, 54, 0);
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 58;
            Item.useTime = 58;
            Item.UseSound = SoundID.Item1;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shootSpeed = 8f;
            Item.shoot = ModContent.ProjectileType<HallowedHandGrenade_Proj>();
        }
    }
}
