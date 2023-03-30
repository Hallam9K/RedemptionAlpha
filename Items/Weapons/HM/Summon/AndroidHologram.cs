using Microsoft.Xna.Framework;
using Redemption.Buffs.Minions;
using Redemption.Globals;
using Redemption.Projectiles.Minions;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Summon
{
    public class AndroidHologram : ModItem
	{
		public override void SetStaticDefaults()
		{
            // Tooltip.SetDefault("Summons a little Android to fight for you");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 20));
            Item.ResearchUnlockCount = 1;

            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
		{
            Item.damage = 75;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 15;
            Item.width = 46;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.noUseGraphic = true;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.UseSound = SoundID.Item44;
            Item.shoot = ModContent.ProjectileType<AndroidMinion_Proj>();
            Item.ExtraItemShoot(ModContent.ProjectileType<AndroidMinion_Fist>());
            Item.buffType = ModContent.BuffType<AndroidMinionBuff>();
		}
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);

            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            return false;
        }
    }
}
