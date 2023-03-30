using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Redemption.Base;

namespace Redemption.Items.Materials.PreHM
{
    public class GrimShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("'Infused with shadow magic'");

            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 0, 25);
            Item.rare = ItemRarityID.Blue;
        }
        public override void PostUpdate()
        {
            if (!Main.rand.NextBool(60))
                return;

            int sparkle = Dust.NewDust(new Vector2(Item.position.X, Item.position.Y), Item.width, Item.height,
                DustID.PurpleCrystalShard, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            BaseUtility.MultiLerp(Main.LocalPlayer.miscCounter % 100 / 100f, scale, scale * 0.8f, scale);
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(46, 32, 54), new Color(111, 83, 188), new Color(46, 32, 54));
            Vector2 origin = new(glow.Width / 2, glow.Height / 2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(glow, Item.Center - Main.screenPosition, new Rectangle(0, 0, glow.Width, glow.Height), color * 0.4f, rotation, origin, scale * 0.3f, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
    }
}
