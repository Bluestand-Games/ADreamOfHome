using Epic.OnlineServices;
using Epic.OnlineServices.Presence;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ADreamOfHome;

public class ADreamOfHome : ModBehaviour
{
    private static INewHorizons newHorizons;


    private void Awake()
	{
		// You won't be able to access OWML's mod helper in Awake.
		// So you probably don't want to do anything here.
		// Use Start() instead.
	}

	private void Start()
	{
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
            OnTwistedClimateSideLoaded(body, name.Contains("Hot"));
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

    private void OnTwistedClimateSideLoaded(GameObject body, bool hot)
    {
        /*
        var sidereal = 2f * Mathf.PI / ((hot ? 1 : 0.2f) * 60f);
        var rotaxis = hot ? Vector3.forward : Vector3.back;
        GameObject.DestroyImmediate(body.GetComponent<AlignWithTargetBody>());
        var alignmentAxis = new Vector3(0, -1, 0);
        var alignment = body.AddComponent<AlignAndAllowRotate>();
        alignment.SetTargetBody(newHorizons.GetPlanet("Accommodation Beam").GetComponent<OWRigidbody>());
        alignment._localAlignmentAxis = alignmentAxis;
        alignment._owRigidbody = body.GetComponent<OWRigidbody>();
        var currentDirection = alignment.transform.TransformDirection(alignment._localAlignmentAxis);
        var targetDirection = alignment.GetAlignmentDirection();
        alignment.transform.rotation = Quaternion.FromToRotation(currentDirection, targetDirection) * alignment.transform.rotation;
        alignment._owRigidbody.SetAngularVelocity(Vector3.zero);
        alignment._usePhysicsToRotate = true;
        alignment.sidereal = hot ? sidereal : -sidereal;
        */
        body.GetComponentInChildren<GravityVolume>().GetComponent<SphereCollider>().radius = 1280;
        body.GetComponentInChildren<GravityVolume>()._lowerSurfaceRadius = 0;
        body.GetComponentInChildren<GravityVolume>()._upperSurfaceRadius = 320;
        var originalSector = newHorizons.GetPlanet("Twisted Climate").transform.Find("Sector");
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

