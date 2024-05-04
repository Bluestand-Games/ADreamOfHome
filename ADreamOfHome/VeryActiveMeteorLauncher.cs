using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ADreamOfHome
{
    public class VeryActiveMeteorLauncher : MeteorLauncher, ILateInitializer
    {
        public int meteorCount = 256;
        public int dynamicMeteorCount => meteorCount/2;

        private new void Awake()
        {
            _parentBody = gameObject.GetAttachedOWRigidbody();
            _initialized = false;
            LateInitializerManager.RegisterLateInitializer(this);
        }

        private new void OnDestroy()
        {
            if (!_initialized)
            {
                LateInitializerManager.UnregisterLateInitializer(this);
            }
        }

        public new void LateInitialize()
        {
            _initialized = true;
            if (_meteorPrefab != null)
            {
                _meteorPool = new List<MeteorController>(meteorCount);
                _launchedMeteors = new List<MeteorController>(meteorCount);
                for (int i = 0; i < meteorCount; i++)
                {
                    MeteorController requiredComponent = GameObject.Instantiate(_meteorPrefab).GetRequiredComponent<MeteorController>();
                    requiredComponent.Suspend(base.transform);
                    _meteorPool.Add(requiredComponent);
                }
            }
            if (_dynamicMeteorPrefab != null)
            {
                _dynamicMeteorPool = new List<MeteorController>(dynamicMeteorCount);
                _launchedDynamicMeteors = new List<MeteorController>(dynamicMeteorCount);
                for (int j = 0; j < dynamicMeteorCount; j++)
                {
                    MeteorController requiredComponent2 = GameObject.Instantiate(_dynamicMeteorPrefab).GetRequiredComponent<MeteorController>();
                    requiredComponent2.Suspend(base.transform);
                    _dynamicMeteorPool.Add(requiredComponent2);
                }
            }
        }
    }
}
