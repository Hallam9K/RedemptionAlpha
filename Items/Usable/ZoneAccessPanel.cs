using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Redemption.Tiles.Tiles;
using Redemption.Globals;
using Redemption.Base;
using Terraria.Chat;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Redemption.WorldGeneration;

namespace Redemption.Items.Usable
{
    public class ZoneAccessPanel1 : ModItem
    {
        public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Lab Access Panel - Alpha");
            Tooltip.SetDefault("Opens up the alpha sector of the lab");
            SacrificeTotal = 1;
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
        }

		public override bool? UseItem(Player player)
        {
            if (LabArea.labAccess[0])
            {
                Main.NewText("Alpha laser security already deactivated", Color.Cyan);
                return true;
            }
            LabArea.labAccess[0] = true;
            string status = "Alpha sector laser security has been deactivated";
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Cyan);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), Color.Cyan);

            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(220, 255, 255)] = ModContent.TileType<DeactivatedLaserTile>(),
                [new Color(255, 0, 0)] = ModContent.TileType<DeactivatedLaser2Tile>(),
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/ALabAccess1", AssetRequestMode.ImmediateLoad).Value;
            Point origin = RedeGen.LabVector.ToPoint();
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
            return true;
		}
	}

    public class ZoneAccessPanel2 : ZoneAccessPanel1
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lab Access Panel - Gamma");
            Tooltip.SetDefault("Opens up the gamma sector of the lab");
            SacrificeTotal = 1;
        }
        public override void SetDefaults() => base.SetDefaults();
        public override bool? UseItem(Player player)
        {
            if (LabArea.labAccess[1])
            {
                Main.NewText("Gamma laser security already deactivated", Color.Cyan);
                return true;
            }
            LabArea.labAccess[1] = true;
            string status = "Gamma sector laser security has been deactivated";
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Cyan);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), Color.Cyan);

            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(220, 255, 255)] = ModContent.TileType<DeactivatedLaserTile>(),
                [new Color(255, 0, 0)] = ModContent.TileType<DeactivatedLaser2Tile>(),
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/ALabAccess2", AssetRequestMode.ImmediateLoad).Value;
            Point origin = RedeGen.LabVector.ToPoint();
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
            return true;
        }
    }

    public class ZoneAccessPanel3 : ZoneAccessPanel1
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lab Access Panel - Sigma");
            Tooltip.SetDefault("Opens up the sigma sector of the lab");
            SacrificeTotal = 1;
        }
        public override void SetDefaults() => base.SetDefaults();
        public override bool? UseItem(Player player)
        {
            if (LabArea.labAccess[2])
            {
                Main.NewText("Sigma laser security already deactivated", Color.Cyan);
                return true;
            }
            LabArea.labAccess[2] = true;
            string status = "Sigma sector laser security has been deactivated";
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Cyan);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), Color.Cyan);

            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(220, 255, 255)] = ModContent.TileType<DeactivatedLaserTile>(),
                [new Color(255, 0, 0)] = ModContent.TileType<DeactivatedLaser2Tile>(),
                [new Color(150, 150, 150)] = -2,
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/ALabAccess3", AssetRequestMode.ImmediateLoad).Value;
            Point origin = RedeGen.LabVector.ToPoint();
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
            return true;
        }
    }
    public class ZoneAccessPanel4 : ZoneAccessPanel1
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lab Access Panel - Omicron");
            Tooltip.SetDefault("Opens up the omicron sector of the lab");
            SacrificeTotal = 1;
        }
        public override void SetDefaults() => base.SetDefaults();
        public override bool? UseItem(Player player)
        {
            if (LabArea.labAccess[3])
            {
                Main.NewText("Omicron laser security already deactivated", Color.Cyan);
                return true;
            }
            LabArea.labAccess[3] = true;
            string status = "Omicron sector laser security has been deactivated";
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Cyan);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), Color.Cyan);

            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(220, 255, 255)] = ModContent.TileType<DeactivatedLaserTile>(),
                [new Color(255, 0, 0)] = ModContent.TileType<DeactivatedLaser2Tile>(),
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/ALabAccess4", AssetRequestMode.ImmediateLoad).Value;
            Point origin = RedeGen.LabVector.ToPoint();
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
            return true;
        }
    }

    public class ZoneAccessPanel5 : ZoneAccessPanel1
    {
        public override string Texture { get { return "Redemption/Items/Usable/ZoneAccessPanel6"; } }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lab Access Panel - 0");
            Tooltip.SetDefault("Opens up the vault sector of the lab");
            SacrificeTotal = 1;
        }
        public override void SetDefaults() => base.SetDefaults();
        public override bool? UseItem(Player player)
        {
            if (LabArea.labAccess[4])
            {
                Main.NewText("Vault laser security already deactivated", Color.Cyan);
                return true;
            }
            LabArea.labAccess[4] = true;
            string status = "Vault sector laser security has been deactivated";
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Cyan);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), Color.Cyan);

            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(220, 255, 255)] = ModContent.TileType<DeactivatedLaserTile>(),
                [new Color(255, 0, 0)] = ModContent.TileType<DeactivatedLaser2Tile>(),
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/ALabAccess5", AssetRequestMode.ImmediateLoad).Value;
            Point origin = RedeGen.LabVector.ToPoint();
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData);
            return true;
        }
    }
    public class ZoneAccessPanel6 : ZoneAccessPanel1
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lab Access Panel - Master");
            Tooltip.SetDefault("Opens up all sectors of the lab");
            SacrificeTotal = 1;
        }
        public override void SetDefaults() => base.SetDefaults();
        public override bool? UseItem(Player player)
        {
            if (LabArea.labAccess[5])
            {
                Main.NewText("Laser security already deactivated", Color.Cyan);
                return true;
            }
            LabArea.labAccess[5] = true;
            string status = "All laser security systems have been deactivated";
            if (Main.netMode == NetmodeID.Server)
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(status), Color.Cyan);
            else if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(Language.GetTextValue(status), Color.Cyan);

            Dictionary<Color, int> colorToTile = new()
            {
                [new Color(220, 255, 255)] = ModContent.TileType<DeactivatedLaserTile>(),
                [new Color(255, 0, 0)] = ModContent.TileType<DeactivatedLaser2Tile>(),
                [Color.Black] = -1
            };

            Texture2D tex = ModContent.Request<Texture2D>("Redemption/WorldGeneration/ALabAccess6", AssetRequestMode.ImmediateLoad).Value;
            Point origin = RedeGen.LabVector.ToPoint();
            GenUtils.InvokeOnMainThread(() =>
            {
                TexGen gen = BaseWorldGenTex.GetTexGenerator(tex, colorToTile);
                gen.Generate(origin.X, origin.Y, true, true);
            });
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.WorldData); return true;
        }
    }
}