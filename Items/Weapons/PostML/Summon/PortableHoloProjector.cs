using Microsoft.Xna.Framework;
using Redemption.Buffs.Minions;
using Redemption.Projectiles.Minions;
using Redemption.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Summon
{
    public class PortableHoloProjector : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Portable Hologram Projector");
            /* Tooltip.SetDefault("Summon a hologram-projected minion to fight for you\n" +
                "Has multiple attack modes it automatically swaps to"); */
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 160;
            Item.DamageType = DamageClass.Summon;
            Item.width = 38;
            Item.height = 22;
            Item.mana = 25;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 20, 0, 0);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = false;
            Item.buffType = ModContent.BuffType<HoloMinionBuff>();
            Item.shoot = ModContent.ProjectileType<HoloProjector>();
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
