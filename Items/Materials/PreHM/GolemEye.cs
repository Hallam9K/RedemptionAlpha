using Terraria.ID;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Tiles.Furniture.Misc;
using System.Collections.Generic;

namespace Redemption.Items.Materials.PreHM
{
    public class GolemEye : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Eye of the Eaglecrest Golem");

            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GolemEyeTile>());
            Item.width = 14;
            Item.height = 14;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }
        private float glowRot = 0;
        public override void PostUpdate()
        {
            glowRot += 0.03f;

            if (!Main.rand.NextBool(30))
                return;

            int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height / 2,
                DustID.TreasureSparkle, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string text = Language.GetTextValue("Mods.Redemption.Items.GolemEye.Tip1");
            TooltipLine line = new(Mod, "text", text) { OverrideColor = Color.White };
            if (NPC.downedMoonlord)
            {
                text = Language.GetTextValue("Mods.Redemption.Items.GolemEye.Tip2");
                line = new(Mod, "text", text) { OverrideColor = Color.LightGoldenrodYellow };
            }
            tooltips.Insert(2, line);
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (NPC.downedMoonlord)
            {
                glowRot += 0.03f;
                Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
                BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, scale, scale * 0.8f, scale);
                Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(241, 215, 108), new Color(255, 255, 255), new Color(241, 215, 108));
                Vector2 origin2 = new(glow.Width / 2, glow.Height / 2);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);

                spriteBatch.Draw(glow, position, new Rectangle(0, 0, glow.Width, glow.Height), color, glowRot, origin2, scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(glow, position, new Rectangle(0, 0, glow.Width, glow.Height), color, -glowRot, origin2, scale, SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            }
            return true;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(241, 215, 108), new Color(255, 255, 255), new Color(241, 215, 108));
            Vector2 origin = new(glow.Width / 2, glow.Height / 2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, new Rectangle(0, 0, glow.Width, glow.Height), color, glowRot, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, new Rectangle(0, 0, glow.Width, glow.Height), color, -glowRot, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
    }
}
