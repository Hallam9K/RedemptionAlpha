using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Biomes;
using Redemption.Globals;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace Redemption.NPCs.Soulless
{
    public class StalkingEyes : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stalking Eyes");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.width = 130;
            NPC.height = 34;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.immortal = true;
            NPC.chaseable = false;
            NPC.npcSlots = 0;
            NPC.ShowNameOnHover = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SoullessBiome>().Type };
        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.rotation = RedeHelper.RandomRotation();
        }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest();

            if (NPC.DistanceSQ(player.Center) < 100 * 100 || SoullessArea.soullessInts[2] != 1)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath6 with { Pitch = -1, Volume = .5f }, NPC.position);
                player.AddBuff(BuffID.Slow, 60);
                NPC.ai[1] = 2;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            switch (NPC.ai[1])
            {
                case 0:
                    NPC.frame.Y = 0;
                    if (Main.rand.NextBool(60))
                        NPC.ai[1] = 1;
                    break;
                case 1:
                    if (++NPC.frameCounter >= 4)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                        {
                            NPC.frame.Y = 0;
                            NPC.ai[1] = 0;
                        }
                    }
                    break;
                case 2:
                    if (++NPC.frameCounter >= 4)
                    {
                        NPC.frameCounter = 0;
                        NPC.frame.Y += frameHeight;
                        if (NPC.frame.Y > 3 * frameHeight)
                        {
                            NPC.alpha = 255;
                            NPC.active = false;
                        }
                    }
                    break;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2 = ModContent.Request<Texture2D>(Texture + "2").Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture2, NPC.Center + RedeHelper.Spread(2) - screenPos, NPC.frame, NPC.GetAlpha(drawColor) * .5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center + RedeHelper.Spread(2) - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
    }
}