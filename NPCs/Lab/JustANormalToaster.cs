using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Redemption.Globals;
using Terraria.GameContent;

namespace Redemption.NPCs.Lab
{
    public class JustANormalToaster : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;

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
        }

        public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }
        public override bool CanChat() => true;
        public override string GetChat()
        {
            return RedeBossDowned.downedOmega3 ? "I am the toaster." : "Just a normal toaster.";
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanHitNPC(NPC target) => false;
    }
}