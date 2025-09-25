# 🚀 SETUP RÁPIDO - Tower Defense Unity

## ⚡ **Método Automático (Recomendado)**

### 1. Use o Wizard Automático
1. **Menu Unity** → **Tower Defense** → **Scene Setup Wizard**
2. Marque todas as opções
3. Clique **"Setup Scene"**
4. Clique **"Create ScriptableObjects"**

### 2. Conectar Referências
1. Selecione **GameManager** na Hierarchy
2. No Inspector, arraste:
   - **Base Transform**: Objeto "Base"
   - **Win Popup**: Panel "WinPopup"  
   - **Lose Popup**: Panel "LosePopup"
   - **Coins Text**: Text "CoinsText"
   - **Wave Text**: Text "WaveText"
   - **Base Health Text**: Text "BaseHealthText"
   - **Restart Button**: Button "RestartButton"

### 3. Configurar ScriptableObjects
1. Na pasta **TowerDefenseData** criada:
2. **GameConfig**: Deixe valores padrão
3. **CreepSmall/CreepBig**: Arraste prefabs dos creeps
4. **TurretNormal/TurretFreeze**: Arraste prefabs das torres
5. No **GameManager**:
   - **Game Config**: Arraste o GameConfig
   - **Waves**: Arraste Wave1, Wave2, Wave3

## 📋 **Método Manual (Se preferir fazer manualmente)**

### 1. Criar GameManager
```
Hierarchy → Create Empty → "GameManager"
Adicionar scripts:
- GameManager.cs
- GameInitializer.cs  
- WaveManager.cs
- EconomySystem.cs
- BaseDefense.cs
- TurretPlacementSystem.cs
```

### 2. Configurar Base
```
Encontrar objeto "Base" ou criar um Cube
Tag: "Base"
Posição: (0, 0, 0)
```

### 3. Criar Spawn Points
```
4x Create Empty → "SpawnPoint_1", "SpawnPoint_2", etc.
Tag: "SpawnPoint"
Posições nos cantos do mapa
```

### 4. Configurar Camera
```
Main Camera:
Position: (0, 45, 0)
Rotation: (90, 0, 0)
```

### 5. Criar UI
```
Hierarchy → UI → Canvas
Adicionar script: GameUI.cs
Criar elementos:
- Text "CoinsText"
- Text "WaveText" 
- Text "BaseHealthText"
- Button "RestartButton"
- Panel "WinPopup" (desativado)
- Panel "LosePopup" (desativado)
```

## ✅ **Verificação Final**

Antes de dar Play, certifique-se que:

- [ ] **GameManager** existe com todos os 6 scripts
- [ ] **Base** tem tag "Base" 
- [ ] **SpawnPoints** têm tag "SpawnPoint"
- [ ] **Canvas** tem script GameUI
- [ ] **EventSystem** existe na cena
- [ ] **ScriptableObjects** foram criados
- [ ] **Referências** conectadas no GameManager
- [ ] **Camera** posicionada corretamente

## 🎮 **Teste**

1. **Play** na Unity
2. Deve aparecer:
   - "Coins: 20"
   - "Wave: 1" 
   - "Base Health: 10"
3. Após 3 segundos, creeps começam a spawnar
4. Console mostra: "Starting wave 1 with X creeps"

## 🐛 **Problemas Comuns**

**❌ "Base transform not found"**
→ Verifique tag "Base" no objeto base

**❌ "No spawn points found"**  
→ Verifique tags "SpawnPoint" nos spawn points

**❌ NullReferenceException**
→ Verifique referências no GameManager Inspector

**❌ UI não aparece**
→ Verifique se Canvas está ativo e tem EventSystem

**❌ Creeps não spawnam**
→ Verifique se ScriptableObjects estão atribuídos

---

## 🎯 **Resumo dos Scripts Principais**

**GameManager** = Cérebro do jogo
**GameInitializer** = Configuração automática  
**WaveManager** = Controla ondas de inimigos
**EconomySystem** = Sistema de moedas
**BaseDefense** = Vida da base
**TurretPlacementSystem** = Colocação de torres
**GameUI** = Interface do usuário

**🎉 Seguindo este checklist, seu Tower Defense funcionará perfeitamente!**
