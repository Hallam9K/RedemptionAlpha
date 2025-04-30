using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Items.Materials.PreHM;
using Redemption.NPCs.Friendly;
using ReLogic.Content;
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
            ItemID.Sets.ItemsThatShouldNotBeInInventory[Type] = true;

            Item.ResearchUnlockCount = 1;
        }

        private Vector2 chaliceIntroSpawnPosition;
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
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
            return !NPC.AnyNPCs(NPCType<Chalice_Intro>());
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
                RedeHelper.SpawnNPC(Item.GetSource_FromAI(), (int)chaliceIntroSpawnPosition.X, (int)chaliceIntroSpawnPosition.Y, NPCType<Chalice_Intro>());
            }

            Item.active = false;
            Item.type = ItemID.None;
            Item.stack = 0;
            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Item.whoAmI);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.Lime.ToVector3() * 0.6f * Main.essScale);
        }
    }
}