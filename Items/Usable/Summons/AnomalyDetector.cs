using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.NPCs.Bosses.SeedOfInfection;
using Terraria.DataStructures;
using Redemption.Globals;
using Redemption.WorldGeneration.Soulless;
using SubworldLibrary;

namespace Redemption.Items.Usable.Summons
{
    public class AnomalyDetector : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Summons a strange portal..."
                + "\nNot consumable"); */
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 4));

            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 38;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noUseGraphic = true;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !SubworldSystem.IsActive<SoullessSub>() && !NPC.AnyNPCs(ModContent.NPCType<SoI>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<SoI>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RedeRecipe.SilverRecipeGroup, 6)
                .AddIngredient(ItemID.MeteoriteBar, 12)
                .AddIngredient(ItemID.Obsidian, 2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
