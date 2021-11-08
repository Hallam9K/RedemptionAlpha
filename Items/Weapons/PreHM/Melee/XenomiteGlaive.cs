using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Redemption.Projectiles.Melee;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Melee
{
    public class XenomiteGlaive : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Hold left-click to increase acceleration of the spin");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
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
            Item.shoot = ModContent.ProjectileType<XenomiteGlaive_Proj>();
            Item.noUseGraphic = true;
            Item.noMelee = true;
            if (!Main.dedServ)
                Item.GetGlobalItem<ItemUseGlow>().glowTexture = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
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