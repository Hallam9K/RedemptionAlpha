using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Redemption.WorldGeneration.Soulless;
using Terraria.Utilities;
using Terraria.Audio;
using Terraria.GameContent;

namespace Redemption.NPCs.Friendly
{
    [AutoloadBossHead]
    public class SoullessPortal : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadesoul Gateway");
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new(0) { Hide = true };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 188;
            NPC.height = 188;
            NPC.dontTakeDamage = true;
            NPC.immortal = true;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.lifeMax = 999;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0;
            NPC.noTileCollide = true;
            NPC.alpha = 255;
            NPC.npcSlots = 0;
        }
        public override bool UsesPartyHat() { return false; }
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || player.dead || !player.active)
                NPC.TargetClosest(true);
            switch (NPC.ai[0])
            {
                case 0:
                    NPC.scale = 0.1f;
                    NPC.ai[0] = 1;
                    break;
                case 1:
                    if (NPC.scale < 1)
                        NPC.scale += 0.02f;
                    NPC.alpha -= 3;
                    NPC.velocity.Y = -0.3f;
                    if (NPC.alpha <= 0)
                    {
                        NPC.ai[0] = 2;
                        NPC.scale = 1;
                        NPC.alpha = 0;
                    }
                    break;
                case 2:
                    NPC.ai[1]++;
                    if (NPC.ai[1] > 18000)
                    {
                        NPC.ai[0] = 3;
                        NPC.ai[1] = 0;
                        Main.NewText("A Shadesoul Gateway has faded by itself...", Color.LightSlateGray);
                    }
                    break;
                case 3:
                    if (NPC.scale > 0)
                        NPC.scale -= 0.02f;
                    NPC.alpha += 5;
                    NPC.velocity.Y = 0.1f;
                    if (NPC.alpha >= 255 || NPC.scale <= 0)
                        NPC.active = false;
                    break;
            }
            NPC.velocity *= 0;
            NPC.wet = false;
            NPC.lavaWet = false;
            NPC.honeyWet = false;
            NPC.dontTakeDamage = true;
            NPC.rotation += .02f;
            NPC.immune[255] = 30;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.homeless = false;
                NPC.homeTileX = -1;
                NPC.homeTileY = -1;
                NPC.netUpdate = true;
            }
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Enter Gateway";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath52, NPC.position);
                if (!SubworldSystem.AnyActive<Redemption>())
                {
                    Main.rand = new UnifiedRandom();
                    SubworldSystem.Enter<SoullessSub>();
                }
                if (SubworldSystem.IsActive<SoullessSub>())
                    SubworldSystem.Exit();
            }
        }
        public override string GetChat()
        {
            if (SubworldSystem.IsActive<SoullessSub>())
            {
                string s = "You wish to escape this cursed place...";
                return s;
            }
            else
            {
                string s = "You hear an ominous hum from the portal...";
                return s;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor * NPC.Opacity, -NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.2f, effects, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, NPC.Center - screenPos, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}