using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Materials.HM;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Redemption.Globals.Player;
using Terraria.DataStructures;

namespace Redemption.Items.Weapons.HM.Ammo
{
    public class EnergyPack : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Energy Pack");
            /* Tooltip.SetDefault("While in inventory, allows the user to use energy-based weaponry\n" +
                "Energy-based weaponry can pierce through Guard Points\n" +
                "Can be stacked up to 3 times, each giving +100 energy\n" +
                "Having more than 3 in your inventory will not increase energy further"); */
            Item.ResearchUnlockCount = 1;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
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
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

            spriteBatch.Draw(texture, position, frame, drawColor, 0, origin, scale, 0, 0f);
            spriteBatch.Draw(glow, position, frame, RedeColor.EnergyPulse, 0, origin, scale, 0, 0f);
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Rectangle frame;
            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();

            Vector2 origin = frame.Size() / 2f;

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, lightColor, rotation, origin, scale, 0, 0f);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, frame, RedeColor.EnergyPulse, rotation, origin, scale, 0, 0f);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<Xenomite>(), 10)
                .AddIngredient(ModContent.ItemType<Plating>(), 2)
                .AddIngredient(ModContent.ItemType<Capacitor>())
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
