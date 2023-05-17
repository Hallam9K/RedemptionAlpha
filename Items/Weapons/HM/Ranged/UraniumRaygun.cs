using Microsoft.Xna.Framework;
using Redemption.Globals.Player;
using Redemption.Items.Materials.HM;
using Redemption.Projectiles.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.HM.Ranged
{
    public class UraniumRaygun : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Uranium Raygun");
            /* Tooltip.SetDefault("(2[i:" + ModContent.ItemType<EnergyPack>() + "]) Fires rings of uranium"
                + "\nCan pierce through tiles and enemies\n" +
                "Requires an Energy Pack to be in your inventory"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
            Item.damage = 86;
            Item.useTime = 5;
            Item.useAnimation = 20;
            Item.reuseDelay = 25;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<UraniumRaygun_Proj>();
            Item.shootSpeed = 11f;
            Item.UseSound = SoundID.Item92;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 48;
            Item.height = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.sellPrice(0, 6, 0, 0);
            Item.rare = ItemRarityID.Lime;
        }
        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<EnergyPlayer>().statEnergy >= 2;
        }
        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                player.GetModPlayer<EnergyPlayer>().statEnergy -= 2;
            return base.UseItem(player);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Uranium>(), 18)
                .AddIngredient(ModContent.ItemType<Plating>(), 3)
                .AddIngredient(ModContent.ItemType<Capacitor>())
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
