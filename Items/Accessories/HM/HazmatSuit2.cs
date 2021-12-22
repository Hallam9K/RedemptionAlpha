using Redemption.Buffs.Debuffs;
using Redemption.Globals.Player;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    public class HazmatSuit2 : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Mod.AddEquipTexture(new EquipTexture(), this, EquipType.Head, $"{Texture}_{EquipType.Head}");
                Mod.AddEquipTexture(new EquipTexture(), this, EquipType.Body, $"{Texture}_{EquipType.Body}");
                Mod.AddEquipTexture(new EquipTexture(), this, EquipType.Legs, $"{Texture}_{EquipType.Legs}");
            }
        }

        private void SetupDrawing()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                int equipSlotHead = Mod.GetEquipSlot(Name, EquipType.Head);
                int equipSlotBody = Mod.GetEquipSlot(Name, EquipType.Body);
                int equipSlotLegs = Mod.GetEquipSlot(Name, EquipType.Legs);

                ArmorIDs.Head.Sets.DrawHead[equipSlotHead] = false;
                ArmorIDs.Body.Sets.HidesTopSkin[equipSlotBody] = true;
                ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
                ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
            }
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hazmat Suit");
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
            var p = player.GetModPlayer<HazmatSuit2Player>();
            p.HideVanity = hideVisual;
            p.VanityOn = true;

            player.buffImmune[ModContent.BuffType<HeavyRadiationDebuff>()] = true;
            player.accDivingHelm = true;
            player.GetModPlayer<BuffPlayer>().hazmatSuit = true;
            player.GetModPlayer<BuffPlayer>().WastelandWaterImmune = true;
        }
    }
    public class HazmatSuit2Player : ModPlayer
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
                if (item.type == ModContent.ItemType<HazmatSuit2>())
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
                var hazmatCostume = ModContent.GetInstance<HazmatSuit2>();
                Player.head = Mod.GetEquipSlot(hazmatCostume.Name, EquipType.Head);
                Player.body = Mod.GetEquipSlot(hazmatCostume.Name, EquipType.Body);
                Player.legs = Mod.GetEquipSlot(hazmatCostume.Name, EquipType.Legs);
            }
        }
    }
}