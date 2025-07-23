using System;
using UnityEngine;

namespace UnityGCC.Capabilities
{
    public abstract class BaseCapabilities
    {
        public Instigator Instigator;
        
        public virtual void SetUp()
        {
            Instigator = new Instigator(this);
            CapabilitiesController.Instance.RegisterCapability(this);
        }

        public virtual bool ShouldActivated()
        {
            return true;
        }
        
        public virtual bool ShouldDeActivated()
        {
            return false;
        }
        
        public virtual void OnActivated()
        {
            bActive = true;
            ActiveDuration = 0;
            DeActiveDuration = 0;
        }
        
        public virtual void OnDeActivated()
        {
            bActive = false;
            ActiveDuration = 0;
            DeActiveDuration = 0;
        }
        
        public virtual void TickActive(float deltaTime)
        {
            if (bActive)
                ActiveDuration += deltaTime;
            else
                DeActiveDuration += deltaTime;
        }

        public void OnOwnerDestroyed()
        {
            CapabilitiesController.Instance.RemoveCapability(this);
        }

        public TagEnum[] Tags;

        public TickGroup TickGroup;
        
        public int TickGroupOrder;


        public GameObject Owner;

        public bool bActive = false;

        public float ActiveDuration;

        public float DeActiveDuration;
    }
}