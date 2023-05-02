using Redemption.Globals;
using Redemption.Items.Weapons.PreHM.Ranged;
using Redemption.NPCs.Minibosses.FowlEmperor;
using Redemption.WorldGeneration.Soulless;
using SubworldLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Summons
{
    public class EggCrown : ModItem
	{
		public override void SetStaticDefaults()
		{
            /* Tooltip.SetDefault("Summons a regal rooster"
                + "\nOnly usable at day"); */
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }

        public override void SetDefaults()
		{
			Item.width = 20;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item44;
            Item.consumable = true;
		}
        public override bool CanUseItem(Player player)
        {
            return !SubworldSystem.IsActive<SoullessSub>() && Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<FowlEmperor>());
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Roar, player.position);

                int type = ModContent.NPCType<FowlEmperor>();

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
                .AddIngredient<ChickenEgg>(5)
                .AddRecipeGroup(RedeRecipe.GoldRecipeGroup)
                .AddTile(TileID.Anvils)
                .Register();
        }
	}
}