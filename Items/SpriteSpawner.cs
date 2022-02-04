using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items
{
    public class SpriteSpawner : ModItem
    {
        public int x;
        public int y;
        public int divisions;
        public Vector2 offset = new(0f, 0f);
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sprite Spawner");
            Tooltip.SetDefault("Spawns sprites at the cursor and continuously draws them at that location.\n" +
                               "Can be used to test shaders. You should use Edit and Continue to do this.");
        }
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Purple;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                x = 0;
                y = 0;
                Talk("Coordinates cleared.", new Color(218, 70, 70));
                return true;
            }
            x = (int)(Main.MouseWorld.X / 16);
            y = (int)(Main.MouseWorld.Y / 16);
            Dust.QuickBox(new Vector2(x, y) * 16, new Vector2(x + 1, y + 1) * 16, 2, new Color(218, 70, 70), null);
            Talk($"Drawing sprites at [{x}, {y}]. Right-click to discard.", new Color(218, 70, 70));
            return true;
        }
        public static void DrawRenderTarget2D()
        {
            //GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;

            ////RenderTargetBinding[] tempTarget = graphicsDevice.GetRenderTargets();
            ////graphicsDevice.SetRenderTarget(RedeDetours.renderTarget);
            ////graphicsDevice.Clear(Color.Transparent);

            //SpriteBatch spriteBatch = new(graphicsDevice);
            //// Sprite drawing code here.
            //Texture2D tex = ModContent.Request<Texture2D>("Redemption/Items/Weapons/HM/Melee/Midnight_SlashProj").Value;
            ////Texture2D background = ModContent.Request<Texture2D>("Redemption/Effects/ParallaxBackground2").Value;
            //Effect effect = ModContent.Request<Effect>("Redemption/Effects/Midnight").Value;
            //float c = 1f / 255f;
            //effect.Parameters["conversion"].SetValue(new Vector2(1f / tex.Width, 1f / tex.Height) * 2f);
            ////effect.Parameters["offset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.3f, Main.GlobalTimeWrappedHourly * 0.25f));
            //effect.Parameters["innerColor"].SetValue(new Vector4(24 * c, 18 * c, 52 * c, 1f));
            //effect.Parameters["borderColor"].SetValue(new Vector4(255 * c, 25 * c, 153 * c, 1f));
            //effect.Parameters["sampleTexture"].SetValue(tex);
            ////effect.Parameters["sampleTexture2"].SetValue(background);
            //effect.CurrentTechnique.Passes[0].Apply();

            //spriteBatch.Begin(default, default, default, default, default, null, Main.GameViewMatrix.ZoomMatrix);
            //spriteBatch.Draw(tex, Main.LocalPlayer.position - Main.screenPosition, new Rectangle(0, 158 * 5, 238, 158), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            ////graphicsDevice.SetRenderTargets(tempTarget);
            ////spriteBatch.Draw(RedeDetours.renderTarget, Main.LocalPlayer.position - Main.screenPosition - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);
            ////spriteBatch.Draw(tex, Main.LocalPlayer.position - Main.screenPosition - new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f) + new Vector2(100f, 100f), null, Color.White, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);
            //spriteBatch.End();
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            //RenderTargetBinding[] tempTarget = graphicsDevice.GetRenderTargets();
            //graphicsDevice.SetRenderTarget(RedeDetours.renderTarget);
            //graphicsDevice.Clear(Color.Transparent);

            SpriteBatch sb = new(graphicsDevice);
            // Sprite drawing code here.
            Texture2D tex = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/Gigapora/Gigapora").Value;
            Texture2D mask = ModContent.Request<Texture2D>("Redemption/Textures/Hexagons").Value;
            Effect effect = ModContent.Request<Effect>("Redemption/Effects/Shield").Value;
            float c = 1f / 255f;
            effect.Parameters["conversion"].SetValue(new Vector2(1f / tex.Width, 1f / tex.Height) * 2f);
            effect.Parameters["offset"].SetValue(new Vector2(Main.GlobalTimeWrappedHourly * 0.3f, Main.GlobalTimeWrappedHourly * 0.25f));
            effect.Parameters["innerColor"].SetValue(new Vector4(104 * c * 0.5f, 213 * c * 0.5f, 237 * c * 0.5f, 0.5f));
            effect.Parameters["borderColor"].SetValue(new Vector4(154 * c, 243 * c, 215 * c, 1f));
            effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 6);
            effect.Parameters["sinMult"].SetValue(30f);
            effect.Parameters["sampleTexture"].SetValue(tex);
            effect.Parameters["sampleTexture2"].SetValue(mask);

            //effect.CurrentTechnique.Passes[0].Apply();
            sb.Begin(default, default, SamplerState.PointClamp, default, default, effect, Main.GameViewMatrix.ZoomMatrix);
            sb.Draw(tex, Main.LocalPlayer.position - Main.screenPosition, new Rectangle(0, 0, 120, 100), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            //graphicsDevice.SetRenderTargets(tempTarget);
            //spriteBatch.Draw(RedeDetours.renderTarget, Main.LocalPlayer.position - Main.screenPosition - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);
            //spriteBatch.Draw(tex, Main.LocalPlayer.position - Main.screenPosition - new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f) + new Vector2(100f, 100f), null, Color.White, 0, new Vector2(0, 0), 2f, SpriteEffects.None, 0);
            sb.End();
        }
        public void Talk(string message, Color color) => Main.NewText(message, color.R, color.G, color.B);
    }
}
