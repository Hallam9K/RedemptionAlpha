using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class GoldChickenEgg : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("'Woah...'");

            Item.ResearchUnlockCount = 2;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 20;
            Item.damage = 10;
            Item.knockBack = 3;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.UseSound = SoundID.Item1;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<GoldChickenEgg_Proj>();
        }
        public override void PostUpdate()
        {
            if (!Main.rand.NextBool(20))
                return;

            int sparkle = Dust.NewDust(Item.position, Item.width, Item.height,
                DustID.GoldCoin, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }
    }
}
