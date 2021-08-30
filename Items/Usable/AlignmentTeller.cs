using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Items.Materials.PreHM;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class AlignmentTeller : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chalice of Alignment");
            Tooltip.SetDefault("Tells you your current alignment"
                + "\n[c/ffea9b:A sentient treasure, cursed with visions of what is yet to come]");
        }

        private float glowRot = 0;

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.maxStack = 1;
            Item.value = 22000;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Orange;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                    .AddIngredient(ModContent.ItemType<CursedGem>())
                    .AddIngredient(ItemID.SteampunkCup)
                    .AddTile(TileID.DemonAltar)
                    .Register();
        }

        public override bool? UseItem(Player player)
        {



            if (RedeWorld.alignment == 0)
            {
                Main.NewText("<Chalice of Alignment> You are truely neutral...", Color.DarkGoldenrod);
            }
            return true;
        }

        public override void PostUpdate()
        {
            glowRot += 0.03f;

            if (!Main.rand.NextBool(30))
                return;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(211, 232, 169), new Color(247, 247, 169), new Color(211, 232, 169));
            Vector2 origin = new(glow.Width / 2, glow.Height / 2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(glow, Item.Center - Main.screenPosition - new Vector2(0f, 18f), new Rectangle(0, 0, glow.Width, glow.Height ), color, glowRot, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition - new Vector2(0f, 18f), new Rectangle(0, 0, glow.Width, glow.Height), color, -glowRot, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}