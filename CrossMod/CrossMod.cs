using System.Collections.Generic;
using Terraria.ModLoader;

namespace Redemption.CrossMod;

/// <summary> Handles general cross mod compatibility. </summary>
internal static class CrossMod
{
	public readonly record struct ModEntry(string Name)
	{
		public readonly string name = Name;

		public readonly bool Enabled
		{
			get
			{
				if (LoadedMods.ContainsKey(name))
					return true;

				if (ModLoader.TryGetMod(name, out var mod))
				{
					LoadedMods.Add(name, mod);
					return true;
				}

				return false;
			}
		}

		/// <summary> The mod instance associated with this entry.
		/// <br/>Should not be used unless you know that this mod is enabled (<see cref="Enabled"/>). </summary>
		public readonly Mod Instance
		{
			get
			{
				if (LoadedMods.TryGetValue(name, out var mod))
					return mod;

				var getMod = ModLoader.GetMod(name);
				LoadedMods.Add(name, getMod);

				return getMod;
			}
		}

		/// <inheritdoc cref="Mod.TryFind{T}(string, out T)"/>
		public readonly bool TryFind<T>(string s, out T t) where T : ModType => ((Mod)this).TryFind(s, out t);
		public static explicit operator Mod(ModEntry e) => e.Instance;
	}

	public static readonly ModEntry Fables = new("CalamityFables");
	public static readonly ModEntry Thorium = new("ThoriumMod");
	public static readonly ModEntry NewBeginnings = new("NewBeginnings");
	public static readonly ModEntry MusicDisplay = new("MusicDisplay");
	public static readonly ModEntry Reforged = new("SpiritReforged");

	/// <summary> The names and instances of loaded crossmod mods per <see cref="ModEntry"/>. </summary>
	private static readonly Dictionary<string, Mod> LoadedMods = [];
}