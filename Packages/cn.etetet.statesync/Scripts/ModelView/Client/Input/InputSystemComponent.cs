namespace ET.Client
{
    public struct OnSpellTrigger
    {
        public EntityRef<Unit> Unit;
        public int SpellConfigId;
    }
    
    [ComponentOf(typeof(Unit))]
    public class InputSystemComponent: Entity, IAwake, IUpdate, IDestroy
    {
        public GameInput InputSystem;
        
        public long PressTime;

        public EntityRef<CinemachineComponent> CinemachineComponent;

    }
}

