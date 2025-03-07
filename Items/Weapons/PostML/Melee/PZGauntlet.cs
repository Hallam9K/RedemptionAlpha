using Redemption.Items.Weapons.PostML.Ranged;
using Redemption.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Melee
{
    public class PZGauntlet : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Infectious Gauntlet");
            /* Tooltip.SetDefault("Punches enemies up-close\n" +
                "Holding down left-click and hitting an enemy will fire a flurry of fists if you are airborne"); */
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<SwarmerCannon>();
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 410;
            Item.DamageType = DamageClass.Melee;
            Item.width = 56;
            Item.height = 34;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 9;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = RarityType<TurquoiseRarity>();
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<PZGauntlet_Proj>();
            Item.shootSpeed = 5f;
        }
    }
}
