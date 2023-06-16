using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Rarities;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class LifeFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fragment of Virtue");
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 48;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 75, 0);
            Item.rare = ModContent.RarityType<CosmicRarity>();
        }
        private float drawTimer;
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            float opac = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0f, 1f, 0f);

            RedeDraw.DrawTreasureBagEffect(spriteBatch, glow, ref drawTimer, position, new Rectangle(0, 0, texture.Width, texture.Height), drawColor, 0, origin, scale);

            spriteBatch.Draw(texture, position, new Rectangle(0, 0, texture.Width, texture.Height), drawColor, 0, origin, scale, 0, 0f);
            spriteBatch.Draw(glow, position, new Rectangle(0, 0, texture.Width, texture.Height), new Color(255, 255, 255, 150) * opac, 0, origin, scale, 0, 0f);
            return false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            float opac = BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, 0f, 1f, 0f);
            Rectangle frame;
            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();
            Vector2 origin = frame.Size() / 2f;

            RedeDraw.DrawTreasureBagEffect(spriteBatch, glow, ref drawTimer, Item.Center - Main.screenPosition, frame, lightColor, rotation, origin, scale);

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, frame, new Color(255, 255, 255, 150) * opac, rotation, origin, scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}