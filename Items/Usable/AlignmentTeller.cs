using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Redemption.NPCs.Friendly;
using System.IO;
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
            ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;

            Item.ResearchUnlockCount = 1;
        }

        private float glowRot = 0;
        private Vector2 chaliceIntroSpawnPosition;
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
  
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<CursedGem>()
                .AddIngredient<ChaliceFragments>()
                .Register();
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.WriteVector2(chaliceIntroSpawnPosition);
        }

        public override void NetReceive(BinaryReader reader)
        {
            chaliceIntroSpawnPosition = reader.ReadVector2();
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Chalice_Intro>());
        }

        public override void UpdateInventory(Player player)
        {          
            if (player.whoAmI == Main.myPlayer)
            {
                chaliceIntroSpawnPosition = player.Center;
                Item.noGrabDelay = 100;
                player.QuickSpawnItem(Item.GetSource_Misc("Chalice_Intro"), Item);
                Item.TurnToAir();
            }          
        }

        public override void OnCreated(ItemCreationContext context)
        {
            if (context is InitializationItemCreationContext)
                return;

            chaliceIntroSpawnPosition = Main.LocalPlayer.Center;
            Item.noGrabDelay = 100;
            Main.LocalPlayer.QuickSpawnItem(Item.GetSource_Misc("Chalice_Intro"), Item);
            Item.TurnToAir();
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            chaliceIntroSpawnPosition = Item.Center; 

            if (!RedeWorld.alignmentGiven)
            {
                RedeWorld.alignmentGiven = true;
                RedeWorld.SyncData();
                RedeHelper.SpawnNPC(Item.GetSource_FromAI(), (int)chaliceIntroSpawnPosition.X, (int)chaliceIntroSpawnPosition.Y, ModContent.NPCType<Chalice_Intro>());
            }

            Item.active = false;
            Item.type = ItemID.None;
            Item.stack = 0;
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Item.whoAmI);
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
            spriteBatch.BeginAdditive();

            spriteBatch.Draw(glow, Item.Center - Main.screenPosition - new Vector2(0f, 18f), new Rectangle(0, 0, glow.Width, glow.Height), color, glowRot, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(glow, Item.Center - Main.screenPosition - new Vector2(0f, 18f), new Rectangle(0, 0, glow.Width, glow.Height), color, -glowRot, origin, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.BeginDefault();

            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Texture2D textureGlow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
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