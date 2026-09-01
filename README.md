# SHOOTER

> 2D Top-Down Arena Roguelite / Auto-Shooter dành cho Mobile

## Tổng quan

Shooter là game 2D top-down arena roguelite có nhịp chơi lấy cảm hứng từ
Brotato. Người chơi tập trung vào di chuyển, né đòn và xây dựng build;
các vũ khí tự động tìm mục tiêu và tấn công.

-   Engine: Unity 6
-   Platform: Android / iOS
-   Camera: Orthographic cố định, top-down
-   Điều khiển: Virtual Joystick
-   Combat: Auto Attack
-   Session mục tiêu: 10--20 phút/run
-   Orientation: Landscape

## Core Gameplay Loop

``` text
START RUN
→ WAVE
→ MOVE + DODGE
→ AUTO ATTACK
→ KILL ENEMIES
→ COLLECT XP + MATERIAL
→ LEVEL UP → CHOOSE STAT
→ SHOP
→ BUY / SELL / MERGE / REROLL
→ NEXT WAVE
→ BOSS
→ RUN COMPLETE / GAME OVER
→ META REWARD
→ NEW RUN
```

## Camera

-   Main Camera dùng Orthographic.
-   Camera cố định và không làm child của Player.
-   Player di chuyển trong mặt phẳng XY.
-   Canvas dùng Screen Space - Overlay.
-   MVP chưa cần Cinemachine.

## Player

Người chơi chủ yếu di chuyển, né enemy/projectile, thu thập XP/Material
và đưa ra quyết định build. Player không cần bấm nút bắn liên tục.

## Auto Attack

Player có tối đa 6 Weapon Slots. Mỗi Weapon hoạt động độc lập với
Damage, Cooldown, Range, Target Rule, Projectile Speed, Crit và hiệu ứng
riêng.

Không dùng một CurrentTarget chung cho tất cả Weapon. Mỗi Weapon tự chọn
target để nhiều vũ khí có thể tấn công các enemy khác nhau.

## Weapon Types

-   Pistol --- cân bằng.
-   SMG --- Attack Speed cao.
-   Shotgun --- nhiều projectile, range ngắn.
-   Rifle/Sniper --- range xa, damage cao.
-   Rocket Launcher --- Explosion/AoE.
-   Laser --- Pierce.
-   Drone --- tự động hỗ trợ.
-   Mine --- Area Control.

## Weapon Level & Merge

``` text
Weapon I + Weapon I
        ↓
     Weapon II
        ↓
     Weapon III
        ↓
     Weapon IV
```

Upgrade có thể tăng Damage, Attack Speed, Projectile Count, Pierce,
Range, Explosion Size, Crit hoặc thay đổi hành vi Weapon.

## Weapon Evolution

Sau MVP, Weapon cấp cao có thể tiến hóa theo hai nhánh. Ví dụ SMG IV có
thể thành Storm SMG (Chain Lightning) hoặc Toxic SMG (Poison).

## Stats

### Survival

-   Max HP
-   HP Regeneration
-   Armor
-   Dodge
-   Move Speed

### Offensive

-   Damage
-   Ranged Damage
-   Elemental Damage
-   Attack Speed
-   Crit Chance
-   Crit Damage
-   Range

### Utility

-   Pickup Range
-   Luck

Pierce, Bounce, Explosion Size, Burn Spread... ưu tiên là thuộc tính
Weapon/Item thay vì tất cả đều thành stat chính.

## XP & Level Up

Enemy cho XP. Khi đủ XP, Player chọn một Stat Upgrade như Max HP, Attack
Speed, Crit Chance hoặc Damage.

``` text
LEVEL UP → CHARACTER STATS
SHOP     → WEAPON + ITEM BUILD
```

## Items

Ví dụ: - Combat Boots → Move Speed - Armor Plate → Armor - Scope →
Range - Critical Chip → Crit Chance - Magazine → Attack Speed - Magnet →
Pickup Range - Lucky Coin → Luck

## Synergy

Weapon/Item có thể có Tags: - RAPID - EXPLOSIVE - ENERGY - DRONE

Ví dụ đủ 2/4/6 ENERGY sẽ mở các bonus mạnh dần. Synergy khiến người chơi
xây build thay vì chỉ mua món có Damage cao nhất.

## Shop

Shop xuất hiện giữa các Wave và có thể bán Weapon, Item và Special
Upgrade.

Player có thể: - Buy - Sell - Merge - Reroll - Lock

## Economy

### Material trong Run

Kiếm trong trận và dùng để mua Weapon, Item, Reroll. Reset khi run kết
thúc.

### Meta Currency

Nhận sau run để Unlock Character, Weapon, Item, Arena, Difficulty và
Cosmetic.

Weapon được unlock ngoài trận chỉ được thêm vào pool có thể xuất hiện
trong run sau.

## Enemy AI

### Chaser

Đuổi Player.

### Dasher

Telegraph → Lock Direction → Dash.

### Shooter

Giữ khoảng cách và bắn projectile.

Enemy cơ bản ưu tiên:

``` text
CHASE + SEPARATION + LIMITED TURN SPEED
```

## Wave System

Một run đầy đủ có thể khoảng 20 Wave. Prototype bắt đầu với 10 Wave.

-   Wave 1--4: Normal
-   Wave 5: Elite
-   Wave 6--9: Harder
-   Wave 10: Boss

Difficulty tăng bằng số lượng, loại enemy, speed, attack/spawn pattern;
không chỉ tăng HP.

## Boss

Boss cần telegraph rõ. Pattern mẫu: - Charge - Projectile Spread - AoE
Warning - Summon - Phase Change

## Game Depth

Điều khiển đơn giản: MOVE + DODGE.

Chiều sâu đến từ Weapon choice, Stat choice, Item, Shop economy, Merge,
Reroll, Synergy, Evolution, Character và Enemy/Boss patterns.

Mục tiêu: dễ hiểu trong 1 phút nhưng chơi nhiều giờ vẫn còn build để
khám phá.

## MVP

-   1 Player Character
-   1 Arena
-   3 Enemy: Chaser, Dasher, Shooter
-   1 Boss
-   6 Weapons
-   10--12 Items
-   10 Waves
-   Player Movement
-   Fixed Orthographic Camera
-   Enemy AI + Spawner
-   Health/Damage
-   Auto Attack + Target Finder
-   Projectile
-   6 Weapon Slots
-   XP + Level Up + Stats
-   Material
-   Shop + Buy/Sell/Reroll
-   Weapon Merge
-   Game Over + Victory + Run Reward

## Unity Project Structure

``` text
Assets/
└── _Game/
    ├── Art/
    ├── Audio/
    ├── Animations/
    ├── Prefabs/
    ├── Scenes/
    ├── ScriptableObjects/
    │   ├── Characters/
    │   ├── Enemies/
    │   ├── Weapons/
    │   ├── Items/
    │   └── Waves/
    └── Scripts/
        ├── Core/
        ├── Player/
        ├── Combat/
        ├── Weapons/
        ├── Enemies/
        ├── Waves/
        ├── Stats/
        ├── Items/
        ├── Shop/
        ├── Economy/
        └── UI/
```

Weapon, Enemy, Item, Character và Wave ưu tiên data-driven bằng
ScriptableObject.

## Nguyên tắc

1.  Fun First.
2.  Simple Controls, Deep Builds.
3.  Readable Combat.
4.  Meaningful Choices.
5.  Replayability.
6.  Mobile Performance.
7.  Data-Driven.
8.  Vertical Slice trước, content sau.

## Core Formula

``` text
MOVE & DODGE
+ AUTO ATTACK
+ WAVE SURVIVAL
+ LEVEL-UP STATS
+ SHOP
+ WEAPON MERGE
+ ITEMS
+ SYNERGY
+ EVOLUTION
= SHOOTER
```

**Project Status:** Pre-production\
**README Version:** 1.0
