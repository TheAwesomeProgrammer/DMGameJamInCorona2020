using System.Collections.Generic;
using Common.Movement;
using Common.SpawnHanding;
using Common.UnitSystem;
using Common.UnitSystem.ExamplePlayer.Stats;
using Common.UnitSystem.Stats;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MovingUnit
{
    private PlayerMovement _movement;

    [SerializeField] 
    private UnitSetup _unitSetup;
        
    [SerializeField] 
    private UnitGraphicSetup _unitGraphicSetup;
        
    [SerializeField]
    private UnitMovementSetup _unitMovementSetup;

    [SerializeField] 
    private PlayerSetup _playerSetup;

    [SerializeField]
    private PlayerStatsManager _statsManager;

    private PlayerAttractedManager _playerAttractedManager;

    public override UnitType UnitType => UnitType.Player;
        
    protected override IUnitStatsManager StatsManager => _statsManager;

    protected override List<object> Setups => new List<object>() { _unitSetup, _unitGraphicSetup, _unitMovementSetup };
        
    protected override IArmor Armor { get; set; }
    protected override UnitSlowManager SlowManager { get; set; }

    protected  void Start()
    {
        if (Application.isPlaying)
        {
            _statsManager = Instantiate(_statsManager);
            SlowManager = new UnitSlowManager(GetStatsManager<PlayerStatsManager>().MovementStats);
            Armor = new UnitArmor(this, HealthFlag.Destructable | HealthFlag.Killable, _unitSetup, _statsManager.HealthStats);
            _movement = new PlayerMovement(_statsManager.GetStats<MovementStats>(), _unitMovementSetup, MovementType.Rigidbody);
        
            _playerAttractedManager = new PlayerAttractedManager(_statsManager.PlayerAttractedManagerData, 
                _playerSetup.AttractedFollowTriggerGo, _playerSetup.AttractedCheerTriggerGo);
            AddLifeCycleObjects(Armor, _movement);
        }
    }

    public void OnMove(InputValue inputValue)
    {
        _movement.OnPlayerMoved(inputValue.Get<Vector2>());
    }

    public void OnFireTnT(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            SpawnManager.Instance.Spawn(SpawnType.TNT, _unitMovementSetup.MovementTransform.position);
        }
    }

    protected override void EditorUpdate()
    {
        base.EditorUpdate();
        PlayerAttractedManager.UpdateTriggers(_statsManager.PlayerAttractedManagerData, 
            _playerSetup.AttractedFollowTriggerGo, _playerSetup.AttractedCheerTriggerGo);
    }
}