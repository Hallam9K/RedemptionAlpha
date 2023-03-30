using Microsoft.Xna.Framework;
using Redemption.Projectiles.Magic;
using Redemption.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class Lament : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Conjures a massive mask");

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 735;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 40;
            Item.width = 30;
            Item.height = 34;
            Item.useTime = 66;
            Item.useAnimation = 66;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = 2500;
            Item.channel = true;
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GiantMask>();
            Item.shootSpeed = 0f;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = new Vector2(player.Center.X, player.Center.Y - 170);
        }

    }
}