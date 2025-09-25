# Tower Defense System - Setup Guide

## Architecture Overview

The system was designed with a modular and scalable architecture, organized in the following folders:

### 📁 **Core/**
- `GameManager.cs` - Main game manager
- `GameInitializer.cs` - Automatic initial setup
- `ProjectilePool.cs` - Object pooling for projectiles

### 📁 **Data/**
- `GameConfig.cs` - General game configurations
- `CreepData.cs` - Enemy data
- `TurretData.cs` - Turret data
- `WaveData.cs` - Wave configuration

### 📁 **Entities/**
- `Creep.cs` - Enemy behavior
- `Turret.cs` - Turret behavior
- `Projectile.cs` - Projectile behavior

### 📁 **Systems/**
- `WaveManager.cs` - Manages enemy waves
- `EconomySystem.cs` - Coin system
- `BaseDefense.cs` - Base defense system
- `TurretPlacementSystem.cs` - Turret placement system

### 📁 **UI/**
- `GameUI.cs` - User interface

## 🚀 **Initial Setup**

### 1. Setup Scene
1. Add an empty object called "GameManager" to the scene
2. Add the `GameManager.cs` and `GameInitializer.cs` scripts to the GameManager
3. Make sure the Base object has the "Base" tag
4. Make sure the SpawnPoints have the "SpawnPoint" tag

### 2. Create ScriptableObjects

#### GameConfig
1. Right-click in Project → Create → Tower Defense → Game Config
2. Configure basic values (base health, starting coins, etc.)

#### CreepData (for each enemy type)
1. Create → Tower Defense → Creep Data
2. Configure: name, health, speed, reward, prefab
3. Create at least 2 types: "Small Creep" and "Big Creep"

#### TurretData (for each turret type)
1. Create → Tower Defense → Turret Data
2. Configure: name, cost, damage, range, fire rate
3. For freeze turret: set `hasSlowEffect = true`
4. Create 2 types: "Normal Turret" and "Freeze Turret"

#### WaveData (for each wave)
1. Create → Tower Defense → Wave Data
2. Configure which enemies to spawn and in what quantity
3. Create at least 3 waves with increasing difficulty

### 3. Setup Prefabs

#### Base Prefab
- Add Collider
- Tag: "Base"

#### Creep Prefabs
- Add Rigidbody (useGravity = false)
- Add Collider
- Add the `Creep.cs` script

#### Turret Prefabs
- Add the `Turret.cs` script
- Configure FirePoint (point where projectiles will spawn)

#### Projectile Prefabs
- Add Rigidbody (useGravity = false)
- Add Collider (isTrigger = true)
- Add the `Projectile.cs` script

### 4. Setup UI
1. Create a Canvas
2. Add UI elements as needed
3. Add the `GameUI.cs` script to the Canvas
4. Configure references in the inspector

## 🎮 **How to Play**

### Controls
- **Click turret buttons** or **keys 1-2**: Select turret type
- **Left click**: Place turret (in placement mode)
- **Right click or ESC**: Cancel placement
- **P**: Pause/Unpause
- **ESC**: Pause menu

### Mechanics
- **Economy**: Start with coins, earn more by killing enemies
- **Turrets**: Each turret costs coins, automatic attack
- **Waves**: Progressive enemy waves
- **Victory**: Survive all waves
- **Defeat**: Base loses all health

## 🔧 **Customization**

### Add New Enemy Types
1. Create new CreepData ScriptableObject
2. Configure unique stats
3. Add to desired waves

### Add New Turret Types
1. Create new TurretData ScriptableObject
2. Configure special abilities
3. Add to TurretPlacementSystem list

### Modify Difficulty
- Adjust values in GameConfig
- Modify CreepData stats
- Reconfigure WaveData

## 🏗️ **Architecture and Patterns**

### Used Patterns
- **Singleton**: GameManager, main systems
- **Observer**: Event system for communication
- **ScriptableObjects**: Data-driven configuration
- **Component-based**: Modular entities
- **Object Pooling**: Reusable projectiles

### Architecture Advantages
- **Scalability**: Easy to add new types
- **Maintainability**: Organized and modular code
- **Performance**: Object pooling, optimized systems
- **Flexibility**: Configuration via ScriptableObjects
- **Testability**: Independent systems

### System Communication
- Events for loose-coupled communication
- Singletons for global access when necessary
- Direct references minimized

## 🐛 **Troubleshooting**

### Common Issues
1. **Enemies don't spawn**: Check SpawnPoints and WaveData
2. **Turrets don't shoot**: Check TurretData and projectiles
3. **UI doesn't update**: Check events in GameUI
4. **Base doesn't take damage**: Check "Base" tag and BaseDefense

### Debug
- Use Debug.Log in scripts to track issues
- Gizmos show turret ranges in Scene View
- Console shows spawn and combat information

## 📈 **Next Steps**

To expand the system:
1. Add more turret types (explosive, laser, etc.)
2. Implement turret upgrades
3. Add temporary power-ups
4. Achievement system
5. Visual and sound effects
6. Multiplayer
7. In-game wave editor

---

**Developed as framework for Unity technical interviews**
