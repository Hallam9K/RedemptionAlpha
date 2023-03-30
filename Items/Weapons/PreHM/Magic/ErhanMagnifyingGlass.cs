using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class ErhanMagnifyingGlass : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Holy Magnifying Glass");
            /* Tooltip.SetDefault("Hold left-click to charge a scorching ray" +
                "\n'Super effective on insects'"); */

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 38;
            Item.height = 44;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 0, 54, 0);
            Item.channel = true;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item1 with { Volume = 0 };
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<MagnifyingGlassRay>();
            Item.shootSpeed = 10f;
        }
        public override bool CanShoot(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }
}