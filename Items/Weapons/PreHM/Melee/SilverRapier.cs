using Terraria.ModLoader;
using Terraria.ID;
using Terraria;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class SilverRapier : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Causes armor penetration");
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            ItemID.Sets.Spears[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 68;
            Item.height = 68;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 98);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 14;
            Item.useTime = 14;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.channel = true;

            // Weapon Properties
            Item.damage = 20;
            Item.knockBack = 5;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.ArmorPenetration = 6;

            // Projectile Properties
            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<SilverRapier_Proj>();
        }
        public override void PostUpdate()
        {
            if (!Main.rand.NextBool(20))
                return;

            int sparkle = Dust.NewDust(Item.position, Item.width, Item.height, DustID.SilverCoin, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<SilverRapier_Proj>()] < 1;
        }
    }
}