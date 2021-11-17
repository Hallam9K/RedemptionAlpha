using Microsoft.Xna.Framework;
using Redemption.NPCs.Bosses.Erhan;
using Redemption.NPCs.Bosses.Keeper;
using Redemption.NPCs.Friendly;
using Redemption.NPCs.Lab;
using Redemption.Projectiles.Misc;
using Redemption.WorldGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Redemption.Globals
{
    public class LabArea : ModSystem
    {
        public static bool Active;
        public override void PreUpdateEntities()
        {
            Active = false;
        }
        public override void PreUpdateWorld()
        {
            if (!Active)
                return;

            Vector2 CraneOperatorPos = new(((RedeGen.LabVector.X + 107) * 16) + 8, (RedeGen.LabVector.Y + 157) * 16);
            if (RedeGen.LabVector.X != -1 && RedeGen.LabVector.Y != -1 && !Terraria.NPC.AnyNPCs(ModContent.NPCType<CraneOperator>()))
                Terraria.NPC.NewNPC((int)CraneOperatorPos.X, (int)CraneOperatorPos.Y, ModContent.NPCType<CraneOperator>());

            Vector2 ToasterPos = new(((RedeGen.LabVector.X + 84) * 16) + 10, (RedeGen.LabVector.Y + 42) * 16);
            if (RedeGen.LabVector.X != -1 && RedeGen.LabVector.Y != -1 && !Terraria.NPC.AnyNPCs(ModContent.NPCType<JustANormalToaster>()))
                Terraria.NPC.NewNPC((int)ToasterPos.X, (int)ToasterPos.Y, ModContent.NPCType<JustANormalToaster>());
        }
    }
}