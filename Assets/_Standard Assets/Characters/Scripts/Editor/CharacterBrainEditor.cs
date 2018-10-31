using System.Collections.Generic;
using System.Linq;
using StandardAssets.Characters.Common;
using StandardAssets.Characters.Examples.SimpleMovementController;
using StandardAssets.Characters.FirstPerson;
using StandardAssets.Characters.Physics;
using StandardAssets.Characters.ThirdPerson;
using StandardAssets.Characters.ThirdPerson.Configs;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	[CustomEditor(typeof(CharacterBrain))]
	public abstract class CharacterBrainEditor : UnityEditor.Editor
	{
		protected const string k_CharacterControllerAdapterName = "characterControllerAdapter";
		protected const string k_OpenCharacterControllerAdapterName = "openCharacterControllerAdapter";

		public override void OnInspectorGUI()
		{
			CharacterBrain brain = (CharacterBrain) target;

			if (brain.GetComponent<OpenCharacterController>() != null)
			{
				DrawPropertiesExcluding(serializedObject, GetOpenCharacterControllerExclusions());
			}
			else if (brain.GetComponent<CharacterController>() != null)
			{
				DrawPropertiesExcluding(serializedObject, GetCharacterControllerExclusions());
			}
			else
			{
				Debug.LogError("Could not find OpenCharacterController or CharacterController");
			}
			
			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		protected virtual string[] GetOpenCharacterControllerExclusions()
		{
			return new string[] {k_CharacterControllerAdapterName};
		}

		protected virtual string[] GetCharacterControllerExclusions()
		{
			return new string[] {k_OpenCharacterControllerAdapterName};
		}
	}

	[CustomEditor(typeof(CapsuleBrain))]
	public class CapsuleBrainEditor : CharacterBrainEditor
	{
	}

	[CustomEditor(typeof(FirstPersonBrain))]
	public class FirstPersonBrainEditor : CharacterBrainEditor
	{
		protected const string k_WeaponName = "weapon";

		protected override string[] GetOpenCharacterControllerExclusions()
		{
			List<string> exclusionList = new List<string> {k_CharacterControllerAdapterName};
			FirstPersonBrain brain = target as FirstPersonBrain;
			if (!brain.hasWeaponAttached)
			{
				exclusionList.Add(k_WeaponName);
			}
			return exclusionList.ToArray();
		}
		
		protected override string[] GetCharacterControllerExclusions()
		{
			List<string> exclusionList = new List<string> {k_OpenCharacterControllerAdapterName};
			FirstPersonBrain brain = target as FirstPersonBrain;
			if (!brain.hasWeaponAttached)
			{
				exclusionList.Add(k_WeaponName);
			}
			return exclusionList.ToArray();
		}
	}

	[CustomEditor(typeof(ThirdPersonBrain))]
	public class ThirdPersonBrainEditor : CharacterBrainEditor
	{
		protected const string k_AnimationTurnaroundName = "animationTurnaroundBehaviour",
		                       k_BlendspaceTurnaroundName = "blendspaceTurnaroundBehaviour",
		                       k_AnimationConfigName = "configuration",
							   k_MotorName = "motor",
							   k_MotorConfigPath = "motor.configuration";

		protected const string k_Help =
			"Configurations are separate assets (ScriptableObjects). Click on the associated configuration to locate it in the Project View. Values can be edited here during runtime and not be lost. It also allows one to create different settings and swap between them. To create a new setting Right click -> Create -> Standard Assets -> Characters -> ...";

		private bool motorFoldOut;
		
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox(k_Help, MessageType.Info);
			base.OnInspectorGUI();
			motorFoldOut = EditorGUILayout.Foldout(motorFoldOut, "Motor");
			if (motorFoldOut)
			{
				EditorGUI.indentLevel++;
				serializedObject.DrawExtendedScriptableObject(k_MotorConfigPath);
				EditorGUI.indentLevel--;
			}
			serializedObject.DrawExtendedScriptableObject(k_AnimationConfigName);
		}

		protected override string[] GetOpenCharacterControllerExclusions()
		{
			List<string> exclusionList = new List<string> {k_CharacterControllerAdapterName};
			exclusionList.AddRange(GetTurnaroundExclusions());
			exclusionList.Add(k_AnimationConfigName);
			exclusionList.Add(k_MotorName);
			return exclusionList.ToArray();
		}

		protected override string[] GetCharacterControllerExclusions()
		{
			List<string> exclusionList = new List<string> {k_OpenCharacterControllerAdapterName};
			exclusionList.AddRange(GetTurnaroundExclusions());
			exclusionList.Add(k_AnimationConfigName);
			exclusionList.Add(k_MotorName);
			return exclusionList.ToArray();
		}

		private List<string> GetTurnaroundExclusions()
		{
			ThirdPersonBrain brain = target as ThirdPersonBrain;

			switch (brain.typeOfTurnaround)
			{
				case TurnaroundType.Animation:
					return new List<string>() {k_BlendspaceTurnaroundName};
				case TurnaroundType.Blendspace:
					return new List<string>() {k_AnimationTurnaroundName};
				default:
					return new List<string>() {k_AnimationTurnaroundName, k_BlendspaceTurnaroundName};
			}
		}
	}
}