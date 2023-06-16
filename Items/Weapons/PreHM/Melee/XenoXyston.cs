using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Microsoft.Xna.Framework;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class XenoXyston : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Hold left-click to increase acceleration of the spin");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 16;
            Item.DamageType = DamageClass.Melee;
            Item.width = 90;
            Item.height = 90;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.channel = true;
            Item.useStyle = 100;
            Item.knockBack = 6f;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<XenoXyston_Proj>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            if (!Main.dedServ)
                Item.RedemptionGlow().glowTexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            knockback = 1;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<XenomiteShard>(), 18)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override void UseItemFrame(Player player)
        {
            player.bodyFrame.Y = 3 * player.bodyFrame.Height;
        }
    }
}