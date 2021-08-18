using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.NPCs.Friendly;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class LostSoul : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lost Soul");
			Tooltip.SetDefault("'The incarnation of Willpower'");
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
			ItemID.Sets.ItemIconPulse[Item.type] = true;
			ItemID.Sets.ItemNoGravity[Item.type] = true;

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
		}
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 99;
			Item.value = 50;
			Item.rare = ItemRarityID.Blue;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noUseGraphic = true;
			Item.consumable = true;
		}
		public override bool? UseItem(Player player)
		{
			int index = NPC.NewNPC((int)player.Center.X, (int)player.Center.Y,
				ModContent.NPCType<LostSoulNPC>(), ai2: 60);
			Main.npc[index].velocity = RedeHelper.PolarVector(5, (Main.MouseWorld - player.Center).ToRotation());

			if (Main.netMode == NetmodeID.Server && index < Main.maxNPCs)
				NetMessage.SendData(MessageID.SyncNPC, number: index);

			return true;
		}
		public override void PostUpdate()
		{
			Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale);
		}
	}
}