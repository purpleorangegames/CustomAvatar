using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Avatar;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace CustomAvatar
{
	// Token: 0x02000003 RID: 3
	[HarmonyPatch(typeof(AvatarManager))]
	[HarmonyPatch("GetFloatMenu")]
	internal class GetFloatMenuPatch
	{
		// Token: 0x06000003 RID: 3
		private static void Postfix(AvatarManager __instance, ref FloatMenu __result)
		{
			__result.Close(true);
			string text = "0";
			string text2 = "0";
			Texture2D avatar = null;
			string fullname = __instance.pawn.NameFullColored;
			Action action = null;
			Action action2 = null;
			FieldInfo field = typeof(AvatarManager).GetField("avatar", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field != null)
			{
				avatar = (Texture2D)field.GetValue(__instance);
				if (avatar != null)
				{
					text = avatar.width.ToString();
					text2 = avatar.height.ToString();
				}
			}
			MethodInfo method = typeof(GetFloatMenuPatch).GetMethod("UpscaleSaveAsPngFullname", BindingFlags.Static | BindingFlags.Public);
			if (method != null)
			{
				action = delegate()
				{
					method.Invoke(null, new object[]
					{
						fullname,
						avatar
					});
				};
			}
			MethodInfo method2 = typeof(GetFloatMenuPatch).GetMethod("DeleteImage", BindingFlags.Static | BindingFlags.Public);
			if (method2 != null)
			{
				action2 = delegate()
				{
					method2.Invoke(null, new object[]
					{
						fullname
					});
				};
			}
			__result = new FloatMenu(new List<FloatMenuOption>
			{
				new FloatMenuOption("Remove Static Image", action2, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0),
				new FloatMenuOption("Save as Static Image", action, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0),
				new FloatMenuOption("Save upscaled (480x576)", new Action(__instance.UpscaleSaveAsPng), MenuOptionPriority.Default, null, null, 0f, null, null, true, 0),
				new FloatMenuOption(string.Concat(new string[]
				{
					"Save original (",
					text,
					"x",
					text2,
					")"
				}), new Action(__instance.SaveAsPng), MenuOptionPriority.Default, null, null, 0f, null, null, true, 0)
			});
		}

		// Token: 0x06000004 RID: 4
		private void SavePng(string filename, byte[] bytes)
		{
			string text = Application.persistentDataPath + "/avatar/";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			File.WriteAllBytes(text + filename, bytes);
		}

		// Token: 0x06000005 RID: 5
		public static void UpscaleSaveAsPngFullname(string fullName, Texture2D avatar)
		{
			fullName = fullName.Replace("'", "").Replace(" ", "_");
			Texture2D texture2D = TextureUtil.MakeReadableCopy(avatar, new int?(480), new int?(576));
			new GetFloatMenuPatch().SavePng(fullName + ".png", texture2D.EncodeToPNG());
			UnityEngine.Object.Destroy(texture2D);
		}

		// Token: 0x060000A2 RID: 162
		public static void DeleteImage(string fullName)
		{
			fullName = fullName.Replace("'", "").Replace(" ", "_");
			string path = Path.Combine(Application.persistentDataPath, "avatar", fullName + ".png");
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
	}
}
