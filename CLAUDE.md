# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Tatica Defence is a Unity-based 2D tower defense game built with Unity 6 (6000.0) using C#. The project uses UniTask for async operations, Unity's new Input System, and URP for rendering.

**Key Technologies:**
- Unity 6 (6000.0)
- UniTask (async/await for Unity)
- Unity Input System
- Universal Render Pipeline (URP)

**Primary IDE:** JetBrains Rider

## Development Commands

### Unity Editor
This is a Unity project - open in Unity Editor to build, test, and run:
```
1. Open Unity Hub
2. Add project from: C:\Project\Tatica Defence
3. Open with Unity 6000.0+
```

### Git Workflow
- Main branch: `main`
- Feature branches: `TACD-{number}` (e.g., TACD-16)
- Always work on feature branches, not main

## Architecture Overview

### Feature-Based Organization

The project follows a feature-based architecture with clear separation:

```
Assets/
├── Common/                    # Shared resources across all scenes
│   ├── Data/                 # ScriptableObject definitions
│   ├── Scripts/              # Reusable utility scripts and frameworks
│   └── Sprites/              # Common sprite assets
└── Scenes/
    └── Battle/               # Main battle scene
        └── Feature/          # Feature-based modules
            ├── Unit/
            ├── Projectiles/
            ├── Market/
            ├── Round/
            └── [more features]
```

Each feature is self-contained with its own scripts, prefabs, and data.

### Core Architectural Patterns

#### 1. State Machine Pattern (StateBase Framework)

Location: `Assets/Common/Scripts/StateBase/`

The StateBase framework is the **primary pattern for managing behavioral states and game flow**. Do NOT use switch statements for state logic.

```csharp
// Generic type-safe state machine
public abstract class StateBaseController<T> where T : struct, Enum
{
    // States have lifecycle: Enter -> Run (Update) -> Exit
    // GlobalTransition applies to all states
}
```

**Key Implementations:**
- **ActionStateController** (Unit behaviors): Idle, Move, Attack, Downed, Freeze
  - Path: `Assets/Scenes/Battle/Feature/Unit/ActionState/`
- **RoundManager** (Game phases): Maintenance, Ready, Combat, GameOver
  - Path: `Assets/Scenes/Battle/Feature/Round/`

When adding new behavior or game flow, extend the StateBase framework rather than creating custom state logic.

#### 2. Component-Based Units

Units are composed of specialized components:
- `ActionStateController` - Behavioral state machine
- `Attacker` - Attack logic & targeting
- `Victim` - Receives damage
- `Mover` - Movement logic
- `HealthBar` - UI health display
- `SkillExecutor` - Skill execution
- `Draggable2D` - Drag & drop (defenders only)

Two unit types:
- **Defenders** (player units): Draggable, placeable on battlefield
- **Aggressors** (enemy units): Spawn from object pool, move automatically

#### 3. Data-Driven Design (ScriptableObjects)

All game data is defined as ScriptableObjects in `Assets/Common/Data/`:

```
UnitLoadOutData (complete unit configuration)
├── UnitDefinitionData        # Identity, name, icon, cost
├── SkillDefinitionData       # Skill info, cooldown
└── UnitStatsByLevelData      # Stats per level
```

When adding content, create ScriptableObject assets rather than hardcoding values.

#### 4. Object Pooling

Location: `Assets/Common/Scripts/ObjectPool/ObjectPooler.cs`

**Always use ObjectPooler for spawned objects** (projectiles, enemies, VFX). Do NOT use Instantiate/Destroy repeatedly.

```csharp
// Spawn from pool
ObjectPooler.Instance.Spawn(prefab, parent, position);

// Return to pool
ObjectPooler.Instance.DeSpawn(poolable);
```

#### 5. Event-Driven Communication

Two event systems:

**GlobalEventBus** (`Assets/Common/Scripts/GlobalEventBus/`)
- Use for cross-feature communication
- Type-safe publish/subscribe pattern
- Example: Defender drag events, market updates

**State Events** (built into StateBase)
- Enter, Run, Exit events for each state
- Use for state lifecycle hooks

Avoid direct references between features - use GlobalEventBus instead.

### Key System Interactions

#### Battle Flow
```
RoundManager (State Machine)
├─ MaintenancePhase: Increment gold, reroll market, player prepares
├─ ReadyPhase: Transition when player ready
├─ CombatPhase: Spawn enemies, combat, skills execute
└─ GameOverPhase
```

#### Unit Combat System
```
Attacker Component
├─ Detects enemies (OnTriggerEnter2D)
├─ Acquires Victim
├─ Changes to AttackState
└─ Attack Loop (DynamicRepeater)
    ├─ Creates AttackContext (Melee/Ranged)
    ├─ RANGED: Spawns Projectile → hits Victim
    └─ MELEE: Direct Victim.Hit()
```

#### Skill System
```
SkillExecutor (Component on Unit)
├─ Initialize: Load skill from UnitLoadOutData
└─ Update: Check CanExecute(), Execute(), manage cooldown
```

Skills implement the `ISkill` interface:
```csharp
public interface ISkill
{
    void Initialize(InitializeContext context);
    bool CanExecute(CanExecuteContext context);
    void Execute(ExecuteContext context);
}
```

Register new skills in SkillFactory.

### Common Utilities

#### RxValue (Reactive Values)
Location: `Assets/Common/Scripts/Rx/RxValue.cs`

Use RxValue for observed properties instead of polling:
```csharp
public class RxValue<T>
{
    public T Value { get; set; }  // Triggers OnChange
    public event Action<T> OnChange;
}
```

Used for: Gold, Level, Health, and any value that needs observation.

#### DynamicRepeater
Location: `Assets/Common/Scripts/DynamicRepeater/`

For async repeating tasks with dynamic intervals (using UniTask):
```csharp
var repeater = new DynamicRepeater(
    intervalNow: () => TimeSpan.FromSeconds(1 / attackSpeed),
    job: async () => Attack()
);
repeater.Start();
```

Used for attack loops, recurring actions with variable timing.

#### Timer System
Location: `Assets/Common/Scripts/Timer/`

```csharp
Timer timer = TimerManager.Instance.Make(coolTime, OnTimeout);
timer.Start();
timer.Restart();
timer.Pause();
```

Used for skill cooldowns, delayed actions, timed events.

## Adding New Content

### Adding a New Unit

1. Create ScriptableObjects in this order:
   - `Common/Data/Units/UnitDefinitions/` → UnitDefinitionData
   - `Common/Data/Units/UnitStatsByLevel/` → UnitStatsByLevelData
   - `Common/Data/Skills/SkillDefinitions/` → SkillDefinitionData (if needed)
   - `Common/Data/Units/UnitLoadOuts/` → UnitLoadOutData (combines above)

2. Add to spawn list:
   - Defenders: `MarketManager.appearUnits`
   - Aggressors: `RoundInfoData.spawnEntries`

### Adding a New Skill

1. Create SkillDefinitionData ScriptableObject in `Common/Data/Skills/SkillDefinitions/`
2. Implement ISkill interface in `Assets/Scenes/Battle/Feature/Unit/Skills/Scripts/Skills/`
3. Register in SkillFactory
4. Assign to UnitLoadOutData

### Modifying Round Flow

1. Edit phase logic in `Assets/Scenes/Battle/Feature/Round/Phase/Phases/`
2. Or modify RoundManager state transitions
3. Configure round data in `Common/Data/Rounds/` ScriptableObjects

## Critical Coding Patterns

1. **Use StateBase for behavior** - Don't use switch statements for state logic
2. **Use ObjectPooler for spawned objects** - Don't Instantiate/Destroy repeatedly
3. **Use RxValue for observed properties** - Subscribe to OnChange instead of polling
4. **Use GlobalEventBus for cross-feature events** - Avoid direct references between features
5. **Always dispose resources** - UniTask cancellation tokens, event subscriptions, timers
6. **Data-driven over hardcoded** - Create ScriptableObjects instead of hardcoding values
7. **Async with UniTask** - Use UniTask instead of Coroutines for better async/await support

## Important Singletons

All inherit from `SceneSingleton<T>`:
- `RoundManager` - Round/phase management
- `MarketManager` - Shop system
- `ObjectPooler` - Object pooling
- `TimerManager` - Timer management
- `DefenderManager` - Defender lifecycle
- `WaitingAreaReferences` - Waiting area slots

## Scene Structure

Main scene: `Assets/Scenes/Battle/Battle.unity`

Key GameObjects:
- RoundManager - Game flow controller
- MarketManager - Shop/economy system
- DefenderManager - Player unit management
- WaitingArea - Unit staging area
- BattleField - Combat area
- ObjectPooler - Object pooling
- TimerManager - Global timer management

## Namespace Conventions

- Common utilities: `Common.{UtilityName}`
- Battle features: `Battle.Feature.{FeatureName}`
- Data: `Data.{DataType}`

## Current Development Notes

Based on recent commits (TACD-16):
- Skill system is in active development
- New interfaces: Castable, Caster, Executable
- FireArrow.cs is a template/example skill
- Attack context system may need refactoring for skill integration

Check TODO comments in code for known issues and planned improvements.
