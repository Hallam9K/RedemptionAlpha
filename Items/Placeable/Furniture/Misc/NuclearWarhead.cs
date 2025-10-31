using Redemption.NPCs.Friendly.TownNPCs;
using Redemption.Tiles.Furniture.Misc;
using Redemption.UI.ChatUI;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class NuclearWarhead : ModItem
	{
		public override void SetStaticDefaults()
		{
            /* Tooltip.SetDefault("Right-click the placed warhead to view the side panel" +
                "\nDetonation will create a wasteland\n" +
                "Can only detonate within the outer thirds of the world on the surface, and while no unexplodable tiles are nearby"); */
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<NuclearWarheadTile>(), 0);
			Item.width = 26;
			Item.height = 34;
			Item.maxStack = 1;
			Item.rare = ItemRarityID.Lime;
            Item.value = Item.buyPrice(0, 40, 0, 0);
        }
        public override void OnCreated(ItemCreationContext context)
        {
            if (context is not BuyItemCreationContext)
                return;

            int adamID = NPC.FindFirstNPC(NPCType<TBot>());
            if (adamID >= 0 && Main.hardMode && !TBot.warheadKnown)
            {
                NPC adam = Main.npc[adamID];

                TBot.warheadKnown = true;
                DialogueChain chain = new();
                chain.Add(new(adam, Mod.GetLocalization("Cutscene.TBotIntro.Warhead1").Value, Color.LightGreen, Color.DarkGreen, null, .05f, 2, 0, false))
                     .Add(new(adam, Mod.GetLocalization("Cutscene.TBotIntro.Warhead2").Value, Color.LightGreen, Color.DarkGreen, null, .05f, 2, 0, false))
                     .Add(new(adam, Mod.GetLocalization("Cutscene.TBotIntro.Warhead3").Value, Color.LightGreen, Color.DarkGreen, null, .05f, 2, 0, false))
                     .Add(new(adam, Mod.GetLocalization("Cutscene.TBotIntro.Warhead4").Value, Color.LightGreen, Color.DarkGreen, null, .05f, 2, 0, false))
                     .Add(new(adam, Mod.GetLocalization("Cutscene.TBotIntro.Warhead5").Value, Color.LightGreen, Color.DarkGreen, null, .05f, 2, 0, false))
                     .Add(new(adam, Mod.GetLocalization("Cutscene.TBotIntro.Warhead6").Value, Color.LightGreen, Color.DarkGreen, null, .05f, 2, .5f, boxFade: true));
                ChatUI.Visible = true;
                ChatUI.Add(chain);

                adam.netUpdate = true;
            }
        }
    }
}