using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Redemption.Globals;
using Terraria.GameContent;
using Terraria.Localization;

namespace Redemption.NPCs.Lab
{
    public class JustANormalToaster : ModNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("");
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0)
            {
                Hide = true
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.width = 20;
            NPC.height = 18;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.aiStyle = -1;
            NPC.value = 0f;
            NPC.knockBackResist = 0.0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.dontTakeDamage = true;
            NPC.hide = true;
            NPC.ShowNameOnHover = true;
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }
        private int AniFrameY;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Rectangle rect = NPC.frame;
            Vector2 origin = NPC.frame.Size() / 2;
            Vector2 offset = Vector2.Zero;
            if (RedeBossDowned.downedOmega3)
            {
                texture = ModContent.Request<Texture2D>(Texture + "_OO").Value;
                int Height = texture.Height / 7;
                int y = Height * AniFrameY;
                rect = new(0, y, texture.Width, Height);
                origin = new(texture.Width / 2f, Height / 2f);
                offset.Y = 4;
            }
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, NPC.Center - offset - screenPos, new Rectangle?(rect), NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
            return false;
        }
        public override bool CanChat() => true;
        public override string GetChat()
        {
            return RedeBossDowned.downedOmega3 ? Language.GetTextValue("Mods.Redemption.Dialogue.Toaster.2") : Language.GetTextValue("Mods.Redemption.Dialogue.Toaster.1");
        }
        public override void FindFrame(int frameHeight)
        {
            if (!RedeBossDowned.downedOmega3)
                return;
            if (Main.LocalPlayer.talkNPC > -1 && Main.npc[Main.LocalPlayer.talkNPC].whoAmI == NPC.whoAmI)
            {
                if (AniFrameY < 1)
                    AniFrameY = 1;

                if (++NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;
                    AniFrameY++;
                    if (AniFrameY > 6)
                        AniFrameY = 1;
                }
                return;
            }
            NPC.frameCounter = 0;
            AniFrameY = 0;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool CanHitNPC(NPC target) => false;
    }
}