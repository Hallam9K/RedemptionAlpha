using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public interface IPhysChain
	{
		/// <summary>
		/// Load the Texture2D of the spritesheet.
		/// For a dangling rope, the sprite will be aligned where
		/// (right -> left is base -> tip)
		/// (up -> down is back -> forward)
		/// </summary>
		Texture2D GetTexture(Mod mod);
		Texture2D GetGlowmaskTexture(Mod mod);

        int NumberOfSegments { get; }
        int MaxFrames { get; }
        int FrameCounterMax { get; }
        bool Glow { get; }
        bool HasGlowmask { get; }
        int Shader { get; }
        int GlowmaskShader { get; }

        Rectangle GetSourceRect(Texture2D texture, int index);

		Color GetColor(PlayerDrawSet drawInfo, Color baseColour);

		/// <summary> Offset of this chain from the anchor of a player facing right. </summary>
		Vector2 AnchorOffset { get; }

		/// <summary>
		/// Each segment of the chain is attached by the anchor of the previous chain.
		/// This anchor is the position of the "tip" of a segment, as pixel offset from the centre of a sprite.
		/// The tip of a segment is the position on the left side of the sprite.
		/// </summary>
		Vector2 OriginOffset(int index);

		/// <summary>
		/// Each segment of the chain extends a fixed distance away from its anchor.
		/// This fixed distance is length from the sprite's origin/anchor, to its "base".
		/// The base of a segment is the position on the right side of the sprite.
		/// </summary>
		int Length(int index);

		Vector2 Force(Terraria.Player player, int index, int dir, float gravDir, float time, Terraria.NPC npc = null, Projectile proj = null);
	}
}