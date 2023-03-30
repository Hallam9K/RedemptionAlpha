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
            Item.shoot = ModContent.ProjectileType<FlakGrenade>();
            Item.shootSpeed = 10;
        }
        public override bool CanUseItem(Player player)
        {
            return player.FindItem(ItemID.Grenade) >= 0 || player.FindItem(ItemID.StickyGrenade) >= 0 || player.FindItem(ItemID.BouncyGrenade) >= 0;
        }
        public override bool? UseItem(Player player)
        {
            int grenade = player.FindItem(ItemID.Grenade);
            int grenadeStick = player.FindItem(ItemID.StickyGrenade);
            int grenadeBounce = player.FindItem(ItemID.BouncyGrenade);
            if (grenadeBounce >= 0)
            {
                player.inventory[grenadeBounce].stack--;
                if (player.inventory[grenadeBounce].stack <= 0)
                    player.inventory[grenadeBounce] = new Item();
            }
            else if (grenadeStick >= 0)
            {
                player.inventory[grenadeStick].stack--;
                if (player.inventory[grenadeStick].stack <= 0)
                    player.inventory[grenadeStick] = new Item();
            }
            else if (grenade >= 0)
            {
                player.inventory[grenade].stack--;
                if (player.inventory[grenade].stack <= 0)
                    player.inventory[grenade] = new Item();
            }
            return null;
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!Main.playerInventory)
            {
                int ammo = BasePlayer.GetItemstackSum(Main.LocalPlayer, ItemID.Grenade) + BasePlayer.GetItemstackSum(Main.LocalPlayer, ItemID.StickyGrenade) + BasePlayer.GetItemstackSum(Main.LocalPlayer, ItemID.BouncyGrenade);
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, ammo.ToString(), position + new Vector2(-2, 8), Color.White, 0, Vector2.Zero, new Vector2(scale + 0.34f, scale + 0.34f));
            }
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<FlakCannon_Proj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.FindItem(ItemID.BouncyGrenade) >= 0)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1);
                return false;
            }
            else if (player.FindItem(ItemID.StickyGrenade) >= 0)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 2);
                return false;
            }

            return true;
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
