using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Redemption.Projectiles.Ranged;
using Redemption.BaseExtension;
using Redemption.Items.Materials.PostML;
using Redemption.Items.Materials.HM;
using Terraria.DataStructures;
using Redemption.Globals.Player;
using Redemption.Globals;
using Terraria.Audio;

namespace Redemption.Items.Weapons.PostML.Ranged
{
    public class PlutoniumRailgun : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("(9[i:" + ModContent.ItemType<EnergyPack>() + "]) Shoots three piercing beams of plutonium, each consuming 3 Energy\n" +
                "Requires an Energy Pack to be in your inventory"); */
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 410;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 92;
            Item.height = 40;
            Item.useTime = 15;
            Item.useAnimation = 45;
            Item.reuseDelay = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<PlutoniumBeam>();
            Item.knockBack = 0;
            Item.value = Item.buyPrice(0, 35, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item75;
            Item.autoReuse = true;
            Item.shootSpeed = 3;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.GetModPlayer<EnergyPlayer>().statEnergy < 6)
                return false;

            SoundEngine.PlaySound(CustomSounds.Zap2 with { Pitch = 0.2f, Volume = 0.6f }, player.position);
            player.RedemptionScreen().ScreenShakeIntensity += 2;
            player.GetModPlayer<EnergyPlayer>().statEnergy -= 3;
            player.velocity -= RedeHelper.PolarVector(2, (Main.MouseWorld - player.Center).ToRotation());
            return true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Plutonium>(), 30)
                .AddIngredient(ModContent.ItemType<Plating>(), 5)
                .AddIngredient(ModContent.ItemType<Capacitor>())
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
