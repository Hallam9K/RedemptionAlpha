using Redemption.Globals;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class HolyBible : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Tosses the book, stopping at cursor point\n" +
                "Shoots 4 short-ranged rays of light\n" +
                "'You dare question the words of the mighty Jimmy!?'");

            SacrificeTotal = 1;
        }

        public override void SetDefaults()
		{
            Item.damage = 14;
            Item.DamageType = DamageClass.Magic;
            Item.width = 30;
            Item.height = 36;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.value = Item.sellPrice(0, 0, 54, 0);
            Item.UseSound = SoundID.Item19;
            Item.mana = 15;
            Item.autoReuse = false;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<HolyBible_Proj>();
            Item.shootSpeed = 10;
            Item.ExtraItemShoot(ModContent.ProjectileType<HolyBible_Ray>());
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }
    }
}
