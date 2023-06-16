using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace Redemption.Items.Materials.PreHM
{
    public class MoonflareFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("'Shines in the moon's reflective light'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 6));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            Item.ResearchUnlockCount = 10;
        }
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 15;
            Item.rare = ItemRarityID.Blue;
        }
        public override void PostUpdate()
        {
            if (!Main.dayTime && Main.moonPhase != 4)
                Lighting.AddLight(Item.Center, Color.AntiqueWhite.ToVector3() * 0.55f * Main.essScale);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string text = Language.GetTextValue("Mods.Redemption.Items.MoonflareFragment.NoMoon");
            if (Main.dayTime || Main.moonPhase == 4)
            {
                TooltipLine line = new(Mod, "text", text)
                {
                    OverrideColor = Color.LightGray
                };
                tooltips.Insert(2, line);
            }
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D textureFaded = ModContent.Request<Texture2D>("Redemption/Items/Materials/PreHM/MoonflareFragment_Faded").Value;

            if (Main.dayTime || Main.moonPhase == 4)
            {
                spriteBatch.Draw(textureFaded, position, new Rectangle(0, 0, textureFaded.Width, textureFaded.Height), drawColor, 0, origin, scale, SpriteEffects.None, 0f);
                return false;
            }
            return true;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D textureFaded = ModContent.Request<Texture2D>(Texture + "_Faded").Value;
            Texture2D textureGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Vector2 originFaded = new(textureFaded.Width / 2, textureFaded.Height / 2);
            Rectangle frame;
            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();

            Vector2 origin = frame.Size() / 2f;

            if (Main.dayTime || Main.moonPhase == 4)
            {
                spriteBatch.Draw(textureFaded, Item.Center - Main.screenPosition, new Rectangle(0, 0, textureFaded.Width, textureFaded.Height), lightColor, rotation, originFaded, scale, SpriteEffects.None, 0f);
                return false;
            }

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(textureGlow, Item.Center - Main.screenPosition, frame, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
