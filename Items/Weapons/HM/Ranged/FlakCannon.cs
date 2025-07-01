using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Projectiles.Ranged;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class FlakCannon : ModItem
	{
		public override void SetStaticDefaults()
		{
            /* Tooltip.SetDefault("Uses grenades as ammo, sticky and bouncy grenades included\n" +
                "Fires flak grenades that penetrate through defense\n" +
                "\nHolding left-click will charge a stream of grenades with no additional ammo consumption\n" +
                "'Quite the unreal bang bang and boom boom'"); */
            ItemID.Sets.SkipsInitialUseSound[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 92;
            Item.height = 50;
            Item.crit = 4;
            Item.useTime = 33;
            Item.useAnimation = 33;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.knockBack = 9;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item61;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<FlakGrenade>();
            Item.shootSpeed = 10;
            Item.useAmmo = ItemID.Grenade;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ProjectileType<FlakCannon_Proj>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HellstoneBar, 10)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddIngredient(ItemID.ExplosivePowder, 15)
                .AddIngredient(ItemID.SoulofMight, 10)
                .AddIngredient(ItemID.IllegalGunParts)
                .AddTile(TileID.Hellforge)
                .Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-6, 0);
        }
    }
}
