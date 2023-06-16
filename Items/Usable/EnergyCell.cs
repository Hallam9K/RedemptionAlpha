using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals.Player;
using Redemption.Items.Materials.HM;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class EnergyCell : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Right-click to recharge +100 Energy if an Energy Pack is in your inventory\n" +
                "Automatically recharges when your Energy is low"); */
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 10;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.LightRed;
        }
        public override bool CanRightClick() => Main.LocalPlayer.GetModPlayer<EnergyPlayer>().energyMax > 0 && Main.LocalPlayer.GetModPlayer<EnergyPlayer>().statEnergy < Main.LocalPlayer.GetModPlayer<EnergyPlayer>().energyMax;
        public override void RightClick(Player player)
        {
            SoundEngine.PlaySound(CustomSounds.Spark1 with { Pitch = 0.5f }, player.position);
            player.GetModPlayer<EnergyPlayer>().statEnergy += 100;
        }
        public override void AddRecipes()
        {
            CreateRecipe(3)
                .AddIngredient(ModContent.ItemType<OmegaPowerCell>())
                .AddTile(ModContent.TileType<EnergyStationTile>())
                .Register();
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            spriteBatch.Draw(texture, position, null, drawColor, 0, origin, scale, 0, 0f);
            spriteBatch.Draw(glow, position, null, RedeColor.EnergyPulse, 0, origin, scale, 0, 0f);
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() / 2f;
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, null, lightColor, rotation, origin, scale, 0, 0f);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, null, RedeColor.EnergyPulse, rotation, origin, scale, 0, 0f);
            return false;
        }
    }
}

