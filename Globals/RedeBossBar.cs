using Microsoft.Xna.Framework.Graphics;
using Redemption.NPCs.Bosses.ADD;
using Redemption.NPCs.Bosses.Cleaver;
using Redemption.NPCs.Bosses.Gigapora;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.NPCs.Bosses.Neb;
using Redemption.NPCs.Bosses.Neb.Clone;
using Redemption.NPCs.Bosses.Neb.Phase2;
using Redemption.NPCs.Bosses.Obliterator;
using Redemption.NPCs.Bosses.SeedOfInfection;
using Redemption.NPCs.Bosses.Thorn;
using Redemption.NPCs.Lab.Behemoth;
using Redemption.NPCs.Lab.Blisterface;
using Redemption.NPCs.Lab.Janitor;
using Redemption.NPCs.Lab.MACE;
using Redemption.NPCs.Lab.Volt;
using Redemption.NPCs.Minibosses.EaglecrestGolem;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeBossBar : GlobalBossBar
    {
        public override bool PreDraw(SpriteBatch spriteBatch, Terraria.NPC npc, ref BossBarDrawParams drawParams)
        {
            if (npc.type == ModContent.NPCType<EaglecrestGolem>() || npc.type == ModContent.NPCType<EaglecrestGolem2>() || npc.type == ModContent.NPCType<Ukko>())
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/EaglecrestGolemBossBar").Value;
            if (npc.type == ModContent.NPCType<SoI>())
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/InfectionBossBar").Value;
            if (npc.type == ModContent.NPCType<KS3>() || npc.type == ModContent.NPCType<KS3_Clone>())
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/KS3BossBar").Value;
            if (npc.type == ModContent.NPCType<JanitorBot>() || npc.type == ModContent.NPCType<IrradiatedBehemoth>() || npc.type == ModContent.NPCType<Blisterface>() || npc.type == ModContent.NPCType<ProtectorVolt>() || npc.type == ModContent.NPCType<MACEProject>())
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/LabBossBar").Value;
            if (npc.type == ModContent.NPCType<Nebuleus>() || npc.type == ModContent.NPCType<Nebuleus_Clone>())
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/NebBossBar").Value;
            if (npc.type == ModContent.NPCType<Nebuleus2>() || npc.type == ModContent.NPCType<Nebuleus2_Clone>())
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/NebBossBar2").Value;
            if (npc.type == ModContent.NPCType<OmegaCleaver>() || npc.type == ModContent.NPCType<Wielder>() || npc.type == ModContent.NPCType<Gigapora>() || npc.type == ModContent.NPCType<Gigapora_ShieldCore>() || npc.type == ModContent.NPCType<OO>())
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/OmegaBossBar").Value;
            if (npc.type == ModContent.NPCType<Thorn>())
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/ThornBossBar").Value;
            return base.PreDraw(spriteBatch, npc, ref drawParams);
        }
    }
}