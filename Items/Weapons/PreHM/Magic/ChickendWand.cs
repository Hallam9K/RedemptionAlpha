using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Magic
{
    public class ChickendWand : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Lobs an arching egg that hatches into a chick upon impact");
            Item.staff[Item.type] = true;

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.width = 46;
            Item.height = 42;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3;
            Item.value = 2000;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ChickendEgg_Proj>();
            Item.shootSpeed = 16f;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = RedeHelper.GetArcVel(position, Main.MouseWorld, 0.3f, 50, 300, maxXvel: 8);
        }
    }
}