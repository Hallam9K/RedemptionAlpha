using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
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

            SacrificeTotal = 1;
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
                .AddIngredient(ModContent.ItemType<ChaliceFragments>())
                .AddTile(TileID.DemonAltar)
                .Register();
        }

        public override bool? UseItem(Player player)
        {
            CombatText.NewText(player.Hitbox, Color.DarkGoldenrod, RedeWorld.alignment, true, false);

            if (!Main.dedServ)
            {
                if (RedeWorld.alignment == 0)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You are truly neutral...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -1 && RedeWorld.alignment <= 1)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You are safe for now...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 2 && RedeWorld.alignment <= 3)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You are choosing the right path. Please, continue.", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -3 && RedeWorld.alignment <= -2)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Be wary, you are straying from the path of good...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -5 && RedeWorld.alignment <= -4)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You are really pushing it aren't you... If you continue this road, he will come...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 4 && RedeWorld.alignment <= 5)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("I am proud of you for keeping the light within you bright...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -7 && RedeWorld.alignment <= -6)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("... Listen, you are following the wrong path here... Please, go back.", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 6 && RedeWorld.alignment <= 7)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Vanquishing the evil of the world... You really are something.", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -9 && RedeWorld.alignment <= -8)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("I am sorry, you can't go back now...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 8 && RedeWorld.alignment <= 9)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Light shines within you, but I am sure more dangerous foes lie ahead...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment <= -10)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You are past redemption...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 10)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You are the redemption this world needed...", 120, 30, 0, Color.DarkGoldenrod);
            }
            return true;
        }

        public override void PostUpdate()
        {
            glowRot += 0.03f;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Color color = BaseUtility.MultiLerpColor(Main.LocalPlayer.miscCounter % 100 / 100f, new Color(211, 232, 169), new Color(247, 247, 169), new Color(211, 232, 169));
            Vector2 origin = new(glow.Width / 2, glow.Height / 2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(glow, Item.Center - Main.screenPosition - new Vector2(0f, 18f), new Rectangle(0, 0, glow.Width, glow.Height), color, glowRot, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition - new Vector2(0f, 18f), new Rectangle(0, 0, glow.Width, glow.Height), color, -glowRot, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
    }
}