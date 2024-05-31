using Epic.OnlineServices;
using Epic.OnlineServices.Presence;
using HarmonyLib;
using OWML.Common;
using OWML.Logging;
using OWML.ModHelper;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ADreamOfHome;

public class ADreamOfHome : ModBehaviour
{
	public static IModConsole ModConsole;
	private static INewHorizons newHorizons;


	private void Awake()
	{
        new Harmony("Bluestand.ADreamOfHome").PatchAll();
	}

	private void Start()
	{
		ModConsole = ModHelper.Console;
		// Starting here, you'll have access to OWML's mod helper.
		ModHelper.Console.WriteLine($"My mod {nameof(ADreamOfHome)} is loaded!", MessageType.Success);

        // Get the New Horizons API and load configs
        newHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
		newHorizons.LoadConfigs(this);
		newHorizons.GetBodyLoadedEvent().AddListener(name => OnBodyLoaded(newHorizons.GetCurrentStarSystem(), name, newHorizons.GetPlanet(name)));

		// Example of accessing game code.
		LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
		{
			if (loadScene != OWScene.SolarSystem) return;
			ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
		};
	}

    private void OnBodyLoaded(string starSystem, string name, GameObject body)
    {
        if (name == "Twisting Climate Hot Side" || name == "Twisting Climate Cold Side")
        {
            OnTwistingClimateSideLoaded(body, name.Contains("Hot"));
        }
        if (name == "Shrouded Tempest")
        {
            OnShroudedTempestLoaded(body);
        }
        if (name == "Molten Cherry")
        {
            OnMoltenCherryLoaded(body);
        }
    }

    private void OnTwistingClimateSideLoaded(GameObject body, bool hot)
    {
        body.GetComponentInChildren<GravityVolume>().GetComponent<SphereCollider>().radius = 1280;
        body.GetComponentInChildren<GravityVolume>()._lowerSurfaceRadius = 0;
        body.GetComponentInChildren<GravityVolume>()._upperSurfaceRadius = 320;
        body.GetComponentInChildren<AlignWithTargetBody>().SetTargetBody(newHorizons.GetPlanet("Accommodation Beam").GetAttachedOWRigidbody());
        var originalSector = newHorizons.GetPlanet("Twisting Climate").transform.Find("Sector");
        var sector = body.transform.Find("Sector");
        sector.GetComponent<SphereShape>().radius = 1290;
        sector.GetComponent<Sector>().SetParentSector(originalSector.GetComponent<Sector>());
        sector.gameObject.name = hot ? "Sector_HotSide" : "Sector_ColdSide";
    }
	
    private void OnShroudedTempestLoaded(GameObject body)
    {
        body.transform.Find("Sector/Ring").GetComponent<MeshRenderer>().sharedMaterial.renderQueue = 4001;
    }

	private void OnMoltenCherryLoaded(GameObject body)
	{
        var meteorLauncher = body.GetComponentInChildren<MeteorLauncher>();
        meteorLauncher.gameObject.SetActive(false);
        var veryActiveLauncher = meteorLauncher.gameObject.AddComponent<VeryActiveMeteorLauncher>();
        veryActiveLauncher._meteorPrefab = meteorLauncher._meteorPrefab;
        veryActiveLauncher._meteorPrefab.GetComponentInChildren<DynamicForceDetector>()._activeInheritedDetector = body.GetComponentInChildren<ConstantForceDetector>();
        veryActiveLauncher._dynamicMeteorPrefab = meteorLauncher._dynamicMeteorPrefab;
        veryActiveLauncher._dynamicProbability = meteorLauncher._dynamicProbability;
        veryActiveLauncher._audioSector = meteorLauncher._audioSector;
        veryActiveLauncher._minLaunchSpeed = meteorLauncher._minLaunchSpeed;
        veryActiveLauncher._maxLaunchSpeed = meteorLauncher._maxLaunchSpeed;
        veryActiveLauncher._minInterval = meteorLauncher._minInterval;
        veryActiveLauncher._maxInterval = meteorLauncher._maxInterval;
        veryActiveLauncher._launchParticles = meteorLauncher._launchParticles;
        veryActiveLauncher._launchSource = meteorLauncher._launchSource;
        veryActiveLauncher._launchDirection = meteorLauncher._launchDirection;
        GameObject.DestroyImmediate(meteorLauncher);
        veryActiveLauncher.gameObject.SetActive(true);
    }
}

[HarmonyPatch(typeof(TornadoController))]
public static class TornadoPatch
{
	[HarmonyPrefix]
    [HarmonyPatch(nameof(TornadoController.AttemptAudioFadeIn))]
    public static void TornadoController_AttemptAudioFadeIn_Prefix(TornadoController __instance, float duration)
	{
		__instance._audioSource.Awake();
	}
}