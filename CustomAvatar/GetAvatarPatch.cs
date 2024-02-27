using System;
using System.Collections.Generic;
using System.IO;
using Avatar;
using HarmonyLib;
using UnityEngine;

namespace CustomAvatar
{
	// Token: 0x0200000D RID: 13
	[HarmonyPatch(typeof(AvatarManager))]
	[HarmonyPatch("GetAvatar")]
	internal class GetAvatarPatch
	{
		// Token: 0x06000084 RID: 132
		private static void Postfix(AvatarManager __instance, ref Texture2D __result)
		{
			if (__result != null)
			{
				string text = __instance.pawn.NameFullColored;
				text = text.Replace("'", "").Replace(" ", "_");
				Texture2D cachedTexture = GetAvatarPatch.GetTexture(text);
				if (cachedTexture != null)
				{
					__result = cachedTexture;
					return;
				}
				string path = Path.Combine(Application.persistentDataPath, "avatar", text + ".png");
				if (File.Exists(path))
				{
					byte[] imageData = File.ReadAllBytes(path);
					__result.LoadImage(imageData);
					GetAvatarPatch.AddTexture(text, path, __result);
				}
			}
		}

		// Token: 0x0600008E RID: 142
		public static Texture2D GetTexture(string pawnName)
		{
			if (GetAvatarPatch.cache.ContainsKey(pawnName))
			{
				GetAvatarPatch.CacheEntry entry = GetAvatarPatch.cache[pawnName];
				if (entry.Texture != null && entry.LastModified == GetAvatarPatch.GetLastModified(entry.Path))
				{
					return entry.Texture;
				}
			}
			return null;
		}

		// Token: 0x0600008F RID: 143
		public static void AddTexture(string pawnName, string path, Texture2D texture)
		{
			GetAvatarPatch.cache[pawnName] = new GetAvatarPatch.CacheEntry(texture, path, GetAvatarPatch.GetLastModified(path));
		}

		// Token: 0x06000090 RID: 144
		private static DateTime GetLastModified(string path)
		{
			return File.GetLastWriteTime(path);
		}

		// Token: 0x04000049 RID: 73
		private static Dictionary<string, GetAvatarPatch.CacheEntry> cache = new Dictionary<string, GetAvatarPatch.CacheEntry>();

		// Token: 0x02000014 RID: 20
		private class CacheEntry
		{
			// Token: 0x17000001 RID: 1
			// (get) Token: 0x06000087 RID: 135
			// (set) Token: 0x06000088 RID: 136
			public Texture2D Texture { get; private set; }

			// Token: 0x17000002 RID: 2
			// (get) Token: 0x06000089 RID: 137
			// (set) Token: 0x0600008A RID: 138
			public string Path { get; private set; }

			// Token: 0x17000003 RID: 3
			// (get) Token: 0x0600008B RID: 139
			// (set) Token: 0x0600008C RID: 140
			public DateTime LastModified { get; private set; }

			// Token: 0x0600008D RID: 141
			public CacheEntry(Texture2D texture, string path, DateTime lastModified)
			{
				this.Texture = texture;
				this.Path = path;
				this.LastModified = lastModified;
			}
		}
	}
}
