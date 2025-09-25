# 🎮 Tower Defense - Guia de Configuração da Cena Unity

## 📋 **Pré-requisitos**
- Cena Unity aberta (pode ser uma cena vazia)
- Todos os scripts já criados na pasta Scripts/
- Terreno ou ground plane para colocação de torres

## 🏗️ **Configuração Passo a Passo**

### 1. **GameManager - Objeto Principal**

#### Criar o GameManager:
1. **Hierarchy** → Clique direito → **Create Empty**
2. Renomeie para **"GameManager"**
3. Adicione os seguintes scripts ao GameManager:
   - `GameManager.cs`
   - `GameInitializer.cs`
   - `WaveManager.cs`
   - `EconomySystem.cs`
   - `BaseDefense.cs`
   - `TurretPlacementSystem.cs`

### 2. **Base (Centro do Campo de Batalha)**

#### Configurar a Base:
1. Encontre o objeto **"Base"** na sua cena (ou crie um)
2. **Certifique-se que tem:**
   - **Tag**: "Base"
   - **Collider** (qualquer tipo)
   - **Posição**: Centro do mapa (0, 0, 0)

#### Se não tiver Base:
1. **Hierarchy** → Clique direito → **3D Object** → **Cube**
2. Renomeie para **"Base"**
3. **Inspector** → **Tag** → **"Base"**
4. Escale para ficar visível (ex: 2, 2, 2)
5. Mude a cor/material para destacar

### 3. **Spawn Points (Pontos de Spawn)**

#### Configurar Spawn Points existentes:
1. Encontre objetos "SpawnPoint" na cena
2. **Para cada SpawnPoint:**
   - **Tag**: "SpawnPoint"
   - Posicione longe da base
   - Distribua ao redor do mapa

#### Se não tiver Spawn Points:
1. **Hierarchy** → Clique direito → **Create Empty**
2. Renomeie para **"SpawnPoint_1"**
3. **Tag**: "SpawnPoint"
4. Posicione longe da base
5. **Duplique** (Ctrl+D) para criar mais 3-4 spawn points
6. Distribua pelos cantos do mapa

### 4. **Camera Setup**

#### Configurar Camera para Tower Defense:
1. Selecione **Main Camera**
2. **Transform:**
   - **Position**: (0, 45, 0)
   - **Rotation**: (90, 0, 0)
3. **Camera Component:**
   - **Projection**: Orthographic (recomendado) ou Perspective
   - **Size**: 20-30 (se orthographic)

### 5. **UI Canvas**

#### Criar Canvas:
1. **Hierarchy** → Clique direito → **UI** → **Canvas**
2. Adicione o script `GameUI.cs` ao Canvas
3. **Canvas Scaler** → **UI Scale Mode**: "Scale With Screen Size"

#### Elementos de UI obrigatórios:
1. **Text para Moedas:**
   - Hierarchy → Clique direito no Canvas → UI → Text
   - Renomeie para "CoinsText"
   - Posicione no canto superior esquerdo

2. **Text para Onda:**
   - Crie outro Text
   - Renomeie para "WaveText"
   - Posicione no topo centro

3. **Text para Vida da Base:**
   - Crie outro Text
   - Renomeie para "BaseHealthText"
   - Posicione no canto superior direito

4. **Botões de Torres:**
   - Hierarchy → Clique direito no Canvas → UI → Button
   - Crie 2 botões para as torres
   - Posicione na parte inferior

5. **Botão Restart:**
   - Crie outro Button
   - Renomeie para "RestartButton"
   - Text do botão: "Restart Game"

6. **Popups (Win/Lose):**
   - Crie 2 Panels
   - Renomeie para "WinPopup" e "LosePopup"
   - Adicione Text em cada um
   - Desative inicialmente (checkbox desmarcado)

### 6. **Event System**
- Certifique-se que existe um **EventSystem** na cena
- Se não existir: **Hierarchy** → Clique direito → **UI** → **Event System**

## 🔗 **Conectar Referências no Inspector**

### No GameManager:
1. Selecione o **GameManager**
2. No **GameManager Script:**
   - **Base Transform**: Arraste o objeto Base
   - **Spawn Points**: Arraste todos os SpawnPoints
   - **Win Popup**: Arraste o WinPopup Panel
   - **Lose Popup**: Arraste o LosePopup Panel
   - **Coins Text**: Arraste o CoinsText
   - **Wave Text**: Arraste o WaveText
   - **Base Health Text**: Arraste o BaseHealthText
   - **Restart Button**: Arraste o RestartButton

### No GameUI (Canvas):
1. Selecione o **Canvas**
2. No **GameUI Script:**
   - **Coins Text**: Arraste o CoinsText
   - **Wave Text**: Arraste o WaveText
   - **Base Health Text**: Arraste o BaseHealthText
   - **Restart Button**: Arraste o RestartButton
   - **Win Popup**: Arraste o WinPopup
   - **Lose Popup**: Arraste o LosePopup

## 📦 **Criar ScriptableObjects**

### Usando o Editor Tool:
1. **Menu Unity** → **Tower Defense** → **Setup Game**
2. Clique **"Complete Setup"**
3. Isso criará todos os ScriptableObjects necessários

### Ou manualmente:
1. **Project** → Clique direito → **Create** → **Tower Defense** → **Game Config**
2. Repita para **Creep Data**, **Turret Data**, **Wave Data**

## 🎯 **Atribuir ScriptableObjects**

### No GameManager:
1. **Game Config**: Arraste o GameConfig criado
2. **Waves**: Arraste as WaveData criadas (Wave1, Wave2, Wave3)

### No TurretPlacementSystem:
1. **Available Turrets**: Arraste os TurretData criados

## 🎮 **Configuração Final**

### Layers (Opcional mas recomendado):
1. **Edit** → **Project Settings** → **Tags and Layers**
2. Crie layer "Ground" para o terreno
3. No **TurretPlacementSystem** → **Ground Layer**: Selecione "Ground"

### Tags necessárias:
- **"Base"** - Para o objeto base
- **"SpawnPoint"** - Para pontos de spawn
- **"MainCamera"** - Para a camera principal

## ✅ **Teste Rápido**

1. **Play** na Unity
2. Deve aparecer:
   - Coins: 20
   - Wave: 1
   - Base Health: 10
3. Após alguns segundos, inimigos devem começar a spawnar
4. Teste colocar torres clicando nos botões

## 🐛 **Resolução de Problemas**

### "Base transform not found":
- Certifique-se que o objeto Base tem tag "Base"
- Verifique se está atribuído no GameManager

### "No spawn points found":
- Verifique tags dos SpawnPoints
- Certifique-se que estão na lista do GameManager

### "NullReferenceException":
- Verifique todas as referências no Inspector
- Certifique-se que todos os ScriptableObjects estão atribuídos

### UI não aparece:
- Verifique se Canvas está ativo
- Verifique referências no GameUI script
- Certifique-se que EventSystem existe

## 📝 **Checklist Final**

- [ ] GameManager criado com todos os scripts
- [ ] Base configurada com tag "Base"
- [ ] SpawnPoints criados com tag "SpawnPoint"
- [ ] Camera posicionada corretamente
- [ ] Canvas com GameUI script
- [ ] Todos os elementos de UI criados
- [ ] Referências conectadas no Inspector
- [ ] ScriptableObjects criados e atribuídos
- [ ] EventSystem presente na cena
- [ ] Teste executado com sucesso

---

**🎉 Seguindo estes passos, sua cena Tower Defense estará completamente funcional!**
