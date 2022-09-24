using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Materials.HM;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Redemption.Globals.Player;

namespace Redemption.Items.Weapons.HM.Ammo
{
    public class EnergyPack : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Pack");
            Tooltip.SetDefault("While in inventory, allows the user to use energy-based weaponry\n" +
                "Energy-based weaponry can pierce through Guard Points\n" +
                "Can be stacked up to 3 times, each giving +100 energy");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.width = 18;
            Item.height = 34;
            Item.maxStack = 3;
            Item.consumable = false;
            Item.value = 50000;
            Item.rare = ItemRarityID.LightRed;
            Item.ammo = Item.type;
        }
        public override void UpdateInventory(Player player)
        {
            player.GetModPlayer<EnergyPlayer>().energyMax += 100 * Item.stack;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
            spriteBatch.Draw(texture, position, null, drawColor, 0, origin, scale, 0, 0f);
            spriteBatch.Draw(glow, position, null, RedeColor.EnergyPulse, 0, origin, scale, 0, 0f);
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() / 2f;
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, null, lightColor, rotation, origin, scale, 0, 0f);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, null, RedeColor.EnergyPulse, rotation, origin, scale, 0, 0f);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<XenomiteItem>(), 10)
                .AddIngredient(ModContent.ItemType<Plating>(), 2)
                .AddIngredient(ModContent.ItemType<Capacitator>())
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
