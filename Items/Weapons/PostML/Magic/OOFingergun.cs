using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class OOFingergun : ModItem
	{
		public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Omega Finger Gun");
            Tooltip.SetDefault("Gradually increases fire rate");
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 194;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 3;
            Item.width = 56;
            Item.height = 26;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(0, 11, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<OOFingergun_Laser>();
            Item.shootSpeed = 20;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<OOFingergun_Proj>();
        }
    }
}
