using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Projectiles.Magic;
using Terraria.DataStructures;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Materials.HM;
using Redemption.BaseExtension;

namespace Redemption.Items.Weapons.PostML.Magic
{
    public class PlutoniumNukeRadio : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("P-2-Warhead Receiver");
            /* Tooltip.SetDefault("Calls plutonium nukes from the sky"
                + "\nDoesn't destroy tiles\n" +
                "'TACTICAL NUKE INCOMING!'"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 600;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 100;
            Item.width = 18;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 60;
            Item.reuseDelay = 60;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.knockBack = 11f;
            Item.rare = ItemRarityID.Cyan;
            Item.value = Item.sellPrice(0, 50, 0, 0);
            Item.UseSound = CustomSounds.AlarmItem;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<PlutoniumNuke_Proj>();
            Item.shootSpeed = 25f;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 spawn = new(Main.MouseWorld.X + Main.rand.Next(-300, 301), player.Center.Y - Main.rand.Next(800, 861));
            Projectile.NewProjectile(source, spawn, spawn.DirectionTo(Main.MouseWorld) * Item.shootSpeed, type, damage, knockback, Main.myPlayer);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Plutonium>(), 8)
                .AddIngredient(ModContent.ItemType<Plating>(), 6)
                .AddIngredient(ModContent.ItemType<Capacitor>())
                .AddIngredient(ModContent.ItemType<CarbonMyofibre>(), 4)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
