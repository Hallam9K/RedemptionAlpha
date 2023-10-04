using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Redemption.WorldGeneration;

namespace Redemption.NPCs.Lab.Behemoth
{
    public class IrradiatedBehemoth_Inactive : ModNPC
    {
        public override string Texture => "Redemption/NPCs/Lab/Behemoth/IrradiatedBehemoth";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("");
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 412;
            NPC.height = 56;
            NPC.friendly = false;
            NPC.dontTakeDamage = true;
            NPC.damage = 0;
            NPC.lifeMax = 250;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0;
            NPC.npcSlots = 0;
            NPC.behindTiles = true;
            NPC.netAlways = true;
            NPC.ShowNameOnHover = false;
        }
        public override void AI()
        {
            Player player = Main.player[Main.myPlayer];
            Rectangle activeZone = new((int)(RedeGen.LabVector.X + 201) * 16, (int)(RedeGen.LabVector.Y + 106) * 16, 25 * 16, 8 * 16);
            if (player.Hitbox.Intersects(activeZone) && !player.dead && player.active)
            {
                NPC.SetDefaults(ModContent.NPCType<IrradiatedBehemoth>());
                NPC.netUpdate = true;
            }
        }
        private int AniFrameY;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20)
            {
                NPC.frameCounter = 0;
                AniFrameY++;
                if (AniFrameY > 5)
                    AniFrameY = 0;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D HeadAni = ModContent.Request<Texture2D>(Texture + "_Head").Value;
            Texture2D HandAni = ModContent.Request<Texture2D>(Texture + "_Hand").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(new Color(20, 20, 20, 255)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            int Height = HeadAni.Height / 6;
            int y = Height * AniFrameY;
            Rectangle rect = new(0, y, HeadAni.Width, Height);
            Vector2 origin = new(HeadAni.Width / 2f, Height / 2f);
            spriteBatch.Draw(HeadAni, NPC.Center - screenPos + new Vector2(0, 32), new Rectangle?(rect), NPC.GetAlpha(new Color(20, 20, 20, 255)), NPC.rotation, origin, NPC.scale, effects, 0);
            spriteBatch.Draw(HandAni, NPC.Center - screenPos + new Vector2(0, 32), new Rectangle?(rect), NPC.GetAlpha(new Color(20, 20, 20, 255)), NPC.rotation, origin, NPC.scale, effects, 0);
            return false;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
    }
}