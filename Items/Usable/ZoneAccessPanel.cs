using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Globals;
using Redemption.Tiles.Tiles;
using Redemption.WorldGeneration;
using System.Collections.Generic;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class ZoneAccessPanel1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lab Access Panel - Alpha");
            // Tooltip.SetDefault("Opens up the alpha sector of the lab");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Lime;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.consumable = true;
        }

        public override bool? UseItem(Player player)
        {
            if (LabArea.labAccess[0])
            {
                Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.LabAccessPanelUsed1"), Color.Cyan);
                return true;
            }
            string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.LabAccessPanel1");
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Cyan);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), Color.Cyan);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Dictionary<Color, int> colorToTile = new()
                {
                    [new Color(220, 255, 255)] = ModContent.TileType<DeactivatedLaserTile>(),
                    [new Color(255, 0, 0)] = ModContent.TileType<DeactivatedLaser2Tile>(),
                    [Color.Black] = -1
                };

                TexGenData tex = TexGen.GetTextureForGen("Redemption/WorldGeneration/ALabAccess1");
                Point origin = RedeGen.LabVector.ToPoint();
                TexGen gen = TexGen.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, false, true);

                LabArea.labAccess[0] = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            return true;
        }
    }

    public class ZoneAccessPanel2 : ZoneAccessPanel1
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lab Access Panel - Gamma");
            // Tooltip.SetDefault("Opens up the gamma sector of the lab");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults() => base.SetDefaults();
        public override bool? UseItem(Player player)
        {
            if (LabArea.labAccess[1])
            {
                Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.LabAccessPanelUsed2"), Color.Cyan);
                return true;
            }
            string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.LabAccessPanel2");
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Cyan);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), Color.Cyan);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Dictionary<Color, int> colorToTile = new()
                {
                    [new Color(220, 255, 255)] = ModContent.TileType<DeactivatedLaserTile>(),
                    [new Color(255, 0, 0)] = ModContent.TileType<DeactivatedLaser2Tile>(),
                    [Color.Black] = -1
                };

                TexGenData tex = TexGen.GetTextureForGen("Redemption/WorldGeneration/ALabAccess2");
                Point origin = RedeGen.LabVector.ToPoint();
                TexGen gen = TexGen.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, false, true);

                LabArea.labAccess[1] = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            return true;
        }
    }

    public class ZoneAccessPanel3 : ZoneAccessPanel1
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lab Access Panel - Sigma");
            // Tooltip.SetDefault("Opens up the sigma sector of the lab");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults() => base.SetDefaults();
        public override bool? UseItem(Player player)
        {
            if (LabArea.labAccess[2])
            {
                Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.LabAccessPanelUsed3"), Color.Cyan);
                return true;
            }
            string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.LabAccessPanel3");
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Cyan);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), Color.Cyan);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Dictionary<Color, int> colorToTile = new()
                {
                    [new Color(220, 255, 255)] = ModContent.TileType<DeactivatedLaserTile>(),
                    [new Color(255, 0, 0)] = ModContent.TileType<DeactivatedLaser2Tile>(),
                    [new Color(150, 150, 150)] = -2,
                    [Color.Black] = -1
                };

                TexGenData tex = TexGen.GetTextureForGen("Redemption/WorldGeneration/ALabAccess3");
                Point origin = RedeGen.LabVector.ToPoint();
                TexGen gen = TexGen.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, false, true);

                LabArea.labAccess[2] = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            return true;
        }
    }
    public class ZoneAccessPanel4 : ZoneAccessPanel1
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lab Access Panel - Omicron");
            // Tooltip.SetDefault("Opens up the omicron sector of the lab");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults() => base.SetDefaults();
        public override bool? UseItem(Player player)
        {
            if (LabArea.labAccess[3])
            {
                Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.LabAccessPanelUsed4"), Color.Cyan);
                return true;
            }
            string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.LabAccessPanel4");
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Cyan);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), Color.Cyan);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Dictionary<Color, int> colorToTile = new()
                {
                    [new Color(220, 255, 255)] = ModContent.TileType<DeactivatedLaserTile>(),
                    [new Color(255, 0, 0)] = ModContent.TileType<DeactivatedLaser2Tile>(),
                    [Color.Black] = -1
                };

                TexGenData tex = TexGen.GetTextureForGen("Redemption/WorldGeneration/ALabAccess4");
                Point origin = RedeGen.LabVector.ToPoint();
                TexGen gen = TexGen.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, false, true);

                LabArea.labAccess[3] = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            return true;
        }
    }

    public class ZoneAccessPanel5 : ZoneAccessPanel1
    {
        public override string Texture { get { return "Redemption/Items/Usable/ZoneAccessPanel6"; } }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lab Access Panel - 0");
            // Tooltip.SetDefault("Opens up the vault sector of the lab");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults() => base.SetDefaults();
        public override bool? UseItem(Player player)
        {
            if (LabArea.labAccess[4])
            {
                Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.LabAccessPanelUsed5"), Color.Cyan);
                return true;
            }
            string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.LabAccessPanel5");
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Cyan);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), Color.Cyan);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Dictionary<Color, int> colorToTile = new()
                {
                    [new Color(220, 255, 255)] = ModContent.TileType<DeactivatedLaserTile>(),
                    [new Color(255, 0, 0)] = ModContent.TileType<DeactivatedLaser2Tile>(),
                    [Color.Black] = -1
                };

                TexGenData tex = TexGen.GetTextureForGen("Redemption/WorldGeneration/ALabAccess5");
                Point origin = RedeGen.LabVector.ToPoint();
                TexGen gen = TexGen.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, false, true);

                LabArea.labAccess[4] = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            return true;
        }
    }
    public class ZoneAccessPanel6 : ZoneAccessPanel1
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lab Access Panel - Master");
            // Tooltip.SetDefault("Disables all lasers in the lab");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults() => base.SetDefaults();
        public override bool? UseItem(Player player)
        {
            if (LabArea.labAccess[5])
            {
                Main.NewText(Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.LabAccessPanelUsed6"), Color.Cyan);
                return true;
            }
            string status = Language.GetTextValue("Mods.Redemption.StatusMessage.Progression.LabAccessPanel6");
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Cyan);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), Color.Cyan);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Dictionary<Color, int> colorToTile = new()
                {
                    [new Color(220, 255, 255)] = ModContent.TileType<DeactivatedLaserTile>(),
                    [new Color(255, 0, 0)] = ModContent.TileType<DeactivatedLaser2Tile>(),
                    [new Color(0, 0, 255)] = ModContent.TileType<HalogenLampTile>(),
                    [Color.Black] = -1
                };

                TexGenData tex = TexGen.GetTextureForGen("Redemption/WorldGeneration/ALabAccess6");
                Point origin = RedeGen.LabVector.ToPoint();
                TexGen gen = TexGen.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, false, true);

                RedeWorld.labSafe = true;
                LabArea.labAccess[5] = true;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.WorldData);
            }
            return true;
        }
    }
}