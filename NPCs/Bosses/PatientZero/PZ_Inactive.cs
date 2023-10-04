using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.GameContent;
using Redemption.Biomes;
using ReLogic.Content;

namespace Redemption.NPCs.Bosses.PatientZero
{
    public class PZ_Inactive : ModNPC
    {
        private static Asset<Texture2D> BodyAni;
        private static Asset<Texture2D> KariAni;
        private static Asset<Texture2D> SlimeAni;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            BodyAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/PatientZero/PZ_Body");
            KariAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/PatientZero/PZ_Kari");
            SlimeAni = ModContent.Request<Texture2D>("Redemption/NPCs/Bosses/PatientZero/PZ_Slime");
        }
        public override void Unload()
        {
            BodyAni = null;
            KariAni = null;
            SlimeAni = null;
        }
        public override string Texture => "Redemption/NPCs/Bosses/PatientZero/PZ_Eyelid_Glooped";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("");
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 98;
            NPC.height = 80;
            NPC.lifeMax = 1;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = true;
        }
        public override void AI()
        {
            Player player = Main.LocalPlayer;
            if (player.DistanceSQ(NPC.Center) < 400 * 400 && Collision.CanHit(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height))
                NPC.dontTakeDamage = false;
            else
                NPC.dontTakeDamage = true;
        }
        public override bool CheckDead()
        {
            NPC.Transform(ModContent.NPCType<PZ>());
            NPC.frame.Y = 164;
            NPC.netUpdate = true;
            return false;
        }
        private int BodyFrame;
        private int KariFrame;
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter % 30 == 0)
            {
                BodyFrame++;
                if (BodyFrame > 7)
                    BodyFrame = 0;
            }
            if (NPC.frameCounter % 60 == 0)
            {
                KariFrame++;
                if (KariFrame > 3)
                    KariFrame = 0;
            }
        }
        public override bool CheckActive() => !Main.LocalPlayer.InModBiome<LabBiome>();
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;

            Vector2 drawCenterC = new(NPC.Center.X + 5, NPC.Center.Y + 7);
            spriteBatch.Draw(SlimeAni.Value, drawCenterC - screenPos, new Rectangle?(new Rectangle(0, 0, SlimeAni.Value.Width, SlimeAni.Value.Height)), drawColor, NPC.rotation, new Vector2(SlimeAni.Value.Width / 2f, SlimeAni.Value.Height / 2f), NPC.scale, SpriteEffects.None, 0f);

            Vector2 drawCenterB = new(NPC.Center.X - 2, NPC.Center.Y + 14);
            int widthB = BodyAni.Value.Height / 8;
            int yB = widthB * BodyFrame;
            spriteBatch.Draw(BodyAni.Value, drawCenterB - screenPos, new Rectangle?(new Rectangle(0, yB, BodyAni.Value.Width, widthB)), drawColor, NPC.rotation, new Vector2(BodyAni.Value.Width / 2f, widthB / 2f), NPC.scale * 2, SpriteEffects.None, 0f);

            Vector2 drawCenterD = new(NPC.Center.X + 1, NPC.Center.Y + 123);
            int widthD = KariAni.Value.Height / 4;
            int yD = widthD * KariFrame;
            spriteBatch.Draw(KariAni.Value, drawCenterD - screenPos, new Rectangle?(new Rectangle(0, yD, KariAni.Value.Width, widthD)), drawColor, NPC.rotation, new Vector2(KariAni.Value.Width / 2f, widthD / 2f), NPC.scale, SpriteEffects.None, 0f);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}