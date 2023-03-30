using Terraria.ModLoader;
using Terraria.ID;
using Terraria;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class Halbirdhouse : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("After every 3rd hit, the following thrust will additionally fire a damaging egg");
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            ItemID.Sets.Spears[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 32;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 20);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.channel = true;

            // Weapon Properties
            Item.damage = 10;
            Item.knockBack = 6;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 3.7f;
            Item.shoot = ModContent.ProjectileType<Halbirdhouse_Proj>();
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<Halbirdhouse_Proj>()] < 1;
        }
    }
}