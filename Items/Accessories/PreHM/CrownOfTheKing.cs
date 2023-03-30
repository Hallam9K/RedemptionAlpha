using Redemption.BaseExtension;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    public class CrownOfTheKing : ModItem
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
                ArmorIDs.Body.Sets.HidesBottomSkin[equipSlotBody] = true;
                ArmorIDs.Body.Sets.HidesHands[equipSlotBody] = true;
                ArmorIDs.Body.Sets.HidesArms[equipSlotBody] = true;
                ArmorIDs.Legs.Sets.HidesTopSkin[equipSlotLegs] = true;
                ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlotLegs] = true;
            }
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Crown of the King");
            /* Tooltip.SetDefault("Become the Mighty King Chicken!\n" +
                "'The king's spirit lives on'"); */
            Item.ResearchUnlockCount = 1;
            SetupDrawing();
        }
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 24;
            Item.value = Item.buyPrice(0, 0, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            Item.vanity = true;
            Item.hasVanityEffects = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var p = player.GetModPlayer<ChickenPlayer>();
            p.HideVanity = hideVisual;
            p.VanityOn = true;

            player.RedemptionPlayerBuff().ChickenForm = true;
        }
        public override void UpdateVanity(Player player)
        {
            player.RedemptionPlayerBuff().ChickenForm = true;
        }
    }
    public class ChickenPlayer : ModPlayer
    {
        public bool HideVanity;
        public bool ForceVanity;
        public bool VanityOn;

        public override void ResetEffects()
        {
            //if ((VanityOn || ForceVanity) && !HideVanity)
            //{
                //Player.Hitbox = new Rectangle((int)Player.position.X, (int)Player.position.Y + 10, Player.width, Player.height - 10);
            //}
            VanityOn = HideVanity = ForceVanity = false;
        }
        public override void UpdateVisibleVanityAccessories()
        {
            for (int n = 13; n < 18 + Player.GetAmountOfExtraAccessorySlotsToShow(); n++)
            {
                Item item = Player.armor[n];
                if (item.type == ModContent.ItemType<CrownOfTheKing>())
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
                var chickenCostume = ModContent.GetInstance<CrownOfTheKing>();
                Player.head = EquipLoader.GetEquipSlot(Mod, chickenCostume.Name, EquipType.Head);
                Player.body = EquipLoader.GetEquipSlot(Mod, chickenCostume.Name, EquipType.Body);
                Player.legs = EquipLoader.GetEquipSlot(Mod, chickenCostume.Name, EquipType.Legs);
            }
        }
    }
}