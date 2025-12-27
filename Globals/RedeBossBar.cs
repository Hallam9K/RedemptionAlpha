using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.NPCs.Bosses.ADD;
using Redemption.NPCs.Bosses.Cleaver;
using Redemption.NPCs.Bosses.Erhan;
using Redemption.NPCs.Bosses.Gigapora;
using Redemption.NPCs.Bosses.KSIII;
using Redemption.NPCs.Bosses.Neb;
using Redemption.NPCs.Bosses.Neb.Clone;
using Redemption.NPCs.Bosses.Neb.Phase2;
using Redemption.NPCs.Bosses.Obliterator;
using Redemption.NPCs.Bosses.SeedOfInfection;
using Redemption.NPCs.Bosses.Thorn;
using Redemption.NPCs.Lab;
using Redemption.NPCs.Lab.Behemoth;
using Redemption.NPCs.Lab.Blisterface;
using Redemption.NPCs.Lab.Janitor;
using Redemption.NPCs.Lab.MACE;
using Redemption.NPCs.Lab.Volt;
using Redemption.NPCs.Minibosses.Calavia;
using Redemption.NPCs.Minibosses.EaglecrestGolem;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeBossBar : GlobalBossBar
    {
        Asset<Texture2D> akkaBarTex;
        Asset<Texture2D> ukkoBarTex;
        Asset<Texture2D> erhanBarTex;
        Asset<Texture2D> nebBarTex;
        Asset<Texture2D> labBarTex;
        Asset<Texture2D> eaglecrestGolemBarTex;
        public override bool PreDraw(SpriteBatch spriteBatch, Terraria.NPC npc, ref BossBarDrawParams drawParams)
        {
            if (npc.ModNPC == null)
                return base.PreDraw(spriteBatch, npc, ref drawParams);

            if (npc.ModNPC is EaglecrestGolem or EaglecrestGolem2)
            {
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/EaglecrestGolemBossBar").Value;
                eaglecrestGolemBarTex ??= ModContent.Request<Texture2D>("Redemption/Textures/BossBars/EaglecrestGolemBossBar_Bar");
                spriteBatch.Draw(eaglecrestGolemBarTex.Value, drawParams.BarCenter - new Vector2(6, 0), null, Color.White, 0, eaglecrestGolemBarTex.Size() / 2, 1, 0, 0);
            }
            else if (npc.ModNPC is SoI)
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/InfectionBossBar").Value;
            else if (npc.ModNPC is KS3 or KS3_Clone)
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/KS3BossBar").Value;
            else if (npc.ModNPC is JanitorBot or IrradiatedBehemoth or Blisterface or ProtectorVolt or MACEProject or Stage3Scientist)
            {
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/LabBossBar").Value;
                labBarTex ??= ModContent.Request<Texture2D>("Redemption/Textures/BossBars/LabBossBar_Bar");
                spriteBatch.Draw(labBarTex.Value, drawParams.BarCenter - new Vector2(8, 2), null, Color.White, 0, labBarTex.Size() / 2, 1, 0, 0);
            }
            else if (npc.ModNPC is Nebuleus or Nebuleus2 or Nebuleus_Clone or Nebuleus2_Clone)
            {
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/NebBossBar").Value;
                nebBarTex ??= ModContent.Request<Texture2D>("Redemption/Textures/BossBars/NebBossBar_Bar");
                spriteBatch.Draw(nebBarTex.Value, drawParams.BarCenter - new Vector2(8, 2), null, Color.White, 0, nebBarTex.Size() / 2, 1, 0, 0);
            }
            else if (npc.ModNPC is OmegaCleaver or Wielder or Gigapora or Gigapora_ShieldCore or OO)
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/OmegaBossBar").Value;
            else if (npc.ModNPC is Thorn)
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/ThornBossBar").Value;
            else if (npc.ModNPC is Akka)
            {
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/AkkaBossBar").Value;
                akkaBarTex ??= ModContent.Request<Texture2D>("Redemption/Textures/BossBars/AkkaBossBar_Bar");
                spriteBatch.Draw(akkaBarTex.Value, drawParams.BarCenter - new Vector2(14, 4), null, Color.White, 0, akkaBarTex.Size() / 2, 1, 0, 0);
            }
            else if (npc.ModNPC is Erhan)
            {
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/ErhanBossBar").Value;
                erhanBarTex ??= ModContent.Request<Texture2D>("Redemption/Textures/BossBars/ErhanBossBar_Bar");
                spriteBatch.Draw(erhanBarTex.Value, drawParams.BarCenter - new Vector2(4, 3), null, Color.White, 0, erhanBarTex.Size() / 2, 1, 0, 0);
            }
            else if (npc.ModNPC is Ukko)
            {
                drawParams.BarTexture = ModContent.Request<Texture2D>("Redemption/Textures/BossBars/UkkoBossBar").Value;
                ukkoBarTex ??= ModContent.Request<Texture2D>("Redemption/Textures/BossBars/UkkoBossBar_Bar");
                spriteBatch.Draw(ukkoBarTex.Value, drawParams.BarCenter - new Vector2(4, 2), null, Color.White, 0, ukkoBarTex.Size() / 2, 1, 0, 0);
            }
            else if (npc.ModNPC is Calavia)
                drawParams.BarTexture = Request<Texture2D>("Redemption/Textures/BossBars/GPBossBar").Value;
            return base.PreDraw(spriteBatch, npc, ref drawParams);
        }
    }
}