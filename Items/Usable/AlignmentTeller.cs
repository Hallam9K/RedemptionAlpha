using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Redemption.NPCs.Bosses.FowlEmperor;
using Redemption.NPCs.Friendly;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class AlignmentTeller : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Chalice of Alignment");
            /* Tooltip.SetDefault("Tells you your current alignment"
                + "\n[c/ffea9b:A sentient treasure, cursed to judge those who wield it]"); */
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;

            Item.ResearchUnlockCount = 1;
        }

        private float glowRot = 0;
        public override void SetDefaults()
        {
            Item.width = 82;
            Item.height = 64;
            Item.maxStack = 1;
            Item.value = 22000;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Orange;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }
        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Chalice_Intro>());
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CursedGem>())
                .AddIngredient(ModContent.ItemType<ChaliceFragments>())
                .Register();
        }
        public override void UpdateInventory(Player player)
        {
            int item = player.FindItem(Type);
            if (item >= 0)
            {
                player.inventory[item].stack = 0;
                if (player.inventory[item].stack <= 0)
                    player.inventory[item] = new Item();
            }
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Item.active = false;
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Item.whoAmI);
            }
        }
        public override bool? UseItem(Player player)
        {
            CombatText.NewText(player.Hitbox, Color.DarkGoldenrod, RedeWorld.alignment, true, false);

            if (!Main.dedServ)
            {
                if (RedeWorld.alignment == 0)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You are truly neutral...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment == -1)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You have done harm, but you are safe for now...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment == 1)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You have done good so far...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= 2 && RedeWorld.alignment <= 3)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You are choosing the right path. Please, continue.", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -3 && RedeWorld.alignment <= -2)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("Be wary, you are straying from the path of good...", 120, 30, 0, Color.DarkGoldenrod);
                else if (RedeWorld.alignment >= -5 && RedeWorld.alignment <= -4)
                    RedeSystem.Instance.ChaliceUIElement.DisplayDialogue("You are really pushing it aren't you... If you continue down this road, justice will await you.", 120, 30, 0, Color.DarkGoldenrod);
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
            Lighting.AddLight(Item.Center, Color.Lime.ToVector3() * 0.6f * Main.essScale);
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

            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D textureGlow = ModContent.Request<Texture2D>(Item.ModItem.Texture + "_Glow").Value;
            Rectangle frame;
            if (Main.itemAnimations[Item.type] != null)
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            else
                frame = texture.Frame();

            Vector2 origin2 = frame.Size() / 2f;

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, frame, lightColor, rotation, origin2, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(textureGlow, Item.Center - Main.screenPosition, frame, Color.White, rotation, origin2, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}