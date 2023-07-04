using Redemption.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Accessories.HM
{
    public class HazmatSuit : ModItem
    {
		public override void Load()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, Item.ModItem, null, new EquipTexture());
				EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, Item.ModItem, null, new EquipTexture());
				EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, Item.ModItem, null, new EquipTexture());
			}
		}

		private void SetupDrawing()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				int equipSlotHead = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);
				int equipSlotBody = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);
				int equipSlotLegs = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

				ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
				ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
				ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
				ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
                ArmorIDs.Legs.Sets.HidesTopSkin[equipSlotLegs] = true;
            }
        }
		public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Grants immunity to the Abandoned Lab and Wasteland water"
                + "\nGreatly extends underwater breathing"
                + "\nGrants protection against low-level radiation"); */
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<HazmatSuit2>();
            SetupDrawing();
		}

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 38;
            Item.value = Item.buyPrice(1, 0, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
            Item.hasVanityEffects = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
		{
			var p = player.GetModPlayer<HazmatSuitPlayer>();
			p.HideVanity = hideVisual;
			p.VanityOn = true;

			player.buffImmune[ModContent.BuffType<HeavyRadiationDebuff>()] = true;
			player.accDivingHelm = true;
			player.RedemptionPlayerBuff().hazmatSuit = true;
			player.RedemptionPlayerBuff().WastelandWaterImmune = true;
		}
    }
	public class HazmatSuitPlayer : ModPlayer
	{
		public bool HideVanity;
		public bool ForceVanity;
		public bool VanityOn;

		public override void ResetEffects()
		{
			VanityOn = HideVanity = ForceVanity = false;
		}
		public override void UpdateVisibleVanityAccessories()
		{
			for (int n = 13; n < 18 + Player.GetAmountOfExtraAccessorySlotsToShow(); n++)
			{
				Item item = Player.armor[n];
				if (item.type == ModContent.ItemType<HazmatSuit>())
				{
					HideVanity = false;
					ForceVanity = true;
				}
			}
		}
		public override void FrameEffects()
		{
			if ((VanityOn || ForceVanity) && !HideVanity)
			{
				var hazmatCostume = ModContent.GetInstance<HazmatSuit>();
				Player.head = EquipLoader.GetEquipSlot(Mod, hazmatCostume.Name, EquipType.Head);
				Player.body = EquipLoader.GetEquipSlot(Mod, hazmatCostume.Name, EquipType.Body);
				Player.legs = EquipLoader.GetEquipSlot(Mod, hazmatCostume.Name, EquipType.Legs);
			}
		}
	}
}