using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Redemption.Projectiles.Ranged;
using Redemption.Rarities;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class Salamanisku : ModItem
	{
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("20% chance not to consume ammo"
                + "\nReplaces arrows with Ukonvasara-tipped arrows\n" +
                "Ukonvasara-tipped arrows impale into enemies and eventually cause thunder to strike them\n" +
                "Two strikes will occur during a thunderstorm"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 18;
            Item.height = 70;
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.value = Item.sellPrice(gold: 25);

            // Use Properties
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.UseSound = SoundID.Item89;
            Item.autoReuse = true;

            // Weapon Properties
            Item.damage = 495;
            Item.knockBack = 4;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;

            // Projectile Properties
            Item.shootSpeed = 25f;
            Item.shoot = ModContent.ProjectileType<UkonvasaraArrow>();
            Item.useAmmo = AmmoID.Arrow;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
			return Main.rand.NextFloat() >= .2f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<UkonvasaraArrow>();
        }
    }
}
