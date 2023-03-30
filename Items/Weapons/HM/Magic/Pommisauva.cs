using Microsoft.Xna.Framework;
using Redemption.Buffs.Cooldowns;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Magic
{
    public class Pommisauva : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Casts a bomb, only 3 can be cast in a row" +
                "\nBombs can destroy tiles\n" +
                "'A magic wand that summons bombs that destroy ground efficiently'"); */
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.reuseDelay = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 9;
            Item.value = Item.buyPrice(0, 9, 25, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Pommisauva_Bomb>();
            Item.shootSpeed = 6f;
        }

        public override bool CanUseItem(Player player)
        {
            return !player.HasBuff<NoitaBombCooldown>();
        }

        public int bombCount;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bombCount++;
            if (bombCount >= 3)
            {
                player.AddBuff(ModContent.BuffType<NoitaBombCooldown>(), 600);
                bombCount = 0;
            }
            return true;
        }
    }
}
