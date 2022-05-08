using Redemption.Buffs.Debuffs;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Accessories.HM
{
    public class HazmatSuit3 : ModItem
    {
		public override void Load()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}", EquipType.Head, null, null, new EquipTexture());
				EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}", EquipType.Body, null, null, new EquipTexture());
				EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, null, null, new EquipTexture());
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
			}
		}
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hazmat Suit (Classic)");
			Tooltip.SetDefault("Grants immunity to the Abandoned Lab and Wasteland water"
                + "\nGreatly extends underwater breathing"
                + "\nGrants protection against low-level radiation");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
			SetupDrawing();
		}

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 38;
            Item.value = Item.buyPrice(1, 0, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.accessory = true;
            Item.canBePlacedInVanityRegardlessOfConditions = true;
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			var p = player.GetModPlayer<HazmatSuit3Player>();
			p.HideVanity = hideVisual;
			p.VanityOn = true;

			player.buffImmune[ModContent.BuffType<HeavyRadiationDebuff>()] = true;
			player.accDivingHelm = true;
			player.RedemptionPlayerBuff().hazmatSuit = true;
			player.RedemptionPlayerBuff().WastelandWaterImmune = true;
		}
    }
	public class HazmatSuit3Player : ModPlayer
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
				if (item.type == ModContent.ItemType<HazmatSuit3>())
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
				var hazmatCostume = ModContent.GetInstance<HazmatSuit3>();
				Player.head = EquipLoader.GetEquipSlot(Mod, hazmatCostume.Name, EquipType.Head);
				Player.body = EquipLoader.GetEquipSlot(Mod, hazmatCostume.Name, EquipType.Body);
				Player.legs = EquipLoader.GetEquipSlot(Mod, hazmatCostume.Name, EquipType.Legs);
			}
		}
	}
}