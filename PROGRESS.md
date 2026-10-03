# SHOOTER --- DEVELOPMENT PROGRESS

> Checklist phát triển game Shooter --- 2D Top-Down Arena Roguelite /
> Auto-Shooter.

## Quy ước

-   [ ] TODO
-   [x] DONE
-   🔄 IN PROGRESS
-   ⛔ BLOCKED

Các mục [x] dưới đây ghi nhận phần đã triển khai trong code/scene, chưa
xác nhận bằng Play Mode hoặc kiểm thử trên thiết bị mobile.

## Mục tiêu hiện tại

Ưu tiên tạo Vertical Slice:

``` text
MOVE → ENEMY CHASE → AUTO SHOOT → KILL → XP → LEVEL UP
→ WAVE → SHOP → NEXT WAVE → BOSS
```

Không tăng số lượng content trước khi core loop đủ vui.

## PHASE 0 --- Project Setup

-   [x] Tạo Unity 6 project 2D.
-   [x] Thiết lập Git + Unity `.gitignore`.
-   [ ] Landscape + Android/iOS target.
-   [ ] Tạo folder structure.
-   [ ] Tạo scene Boot, MainMenu, Game, Result.
-   [ ] Sorting Layers.
-   [ ] Collision Layers.

**Milestone:** Project sạch và chạy được Game scene.

## PHASE 1 --- Camera & Arena

-   [x] Orthographic Main Camera.
-   [ ] Camera cố định.
-   [ ] Camera không làm child Player.
-   [ ] Arena landscape.
-   [x] Ground/background.
-   [ ] Arena Boundary.
-   [ ] Canvas Screen Space - Overlay.
-   [ ] Test nhiều aspect ratio mobile.

**Milestone:** Arena hiển thị đúng trên mobile.

**Hiện có:** Scene `Battle_01` với nền Tilemap và `CameraFollow` bám Player
bằng `SmoothDamp`. Camera cố định theo thiết kế vẫn chưa triển khai.

## PHASE 2 --- Player Movement

-   [ ] Player prefab.
-   [x] Rigidbody2D + Collider2D.
-   [x] PlayerController.
-   [x] Keyboard input cho Editor.
-   [ ] Virtual Joystick.
-   [x] Normalize movement.
-   [x] Move Speed.
-   [ ] Arena boundary.
-   [x] Idle/Move animation.

**Milestone:** Player di chuyển mượt.

**Hiện có:** State machine `Idle/Run/Die`, animation theo 8 hướng và input
bàn phím qua `Input.GetAxisRaw`. Player đang nằm trong scene, chưa có prefab riêng.

## PHASE 3 --- Health & Damage

-   [x] IDamageable.
-   [x] PlayerHealth.
-   [x] EnemyHealth.
-   [ ] DamageSystem.
-   [x] Max HP.
-   [ ] Hit feedback.
-   [x] Death.
-   [ ] Player i-frame.
-   [x] HP UI.

**Hiện có:** `IHealth`, nhận sát thương, hồi máu cho Player và cập nhật thanh
HP qua event `HealthChanged`. Khi chết, Player dừng di chuyển, tắt collider
và gửi trigger animation; Enemy bị vô hiệu hóa. Animation chết cần kiểm tra
trong Play Mode; chưa có luồng Game Over.

## PHASE 4 --- Enemy Foundation

-   [ ] EnemyData ScriptableObject.
-   [ ] EnemyController.
-   [ ] Chase.
-   [ ] Separation.
-   [ ] Contact Damage.
-   [ ] Enemy Pooling.
-   [ ] Safe spawn distance.

### Chaser

-   [ ] Movement.
-   [ ] Contact damage.
-   [ ] Death.

### Dasher

-   [ ] Telegraph.
-   [ ] Lock direction.
-   [ ] Dash.
-   [ ] Recovery.

### Shooter

-   [ ] Maintain distance.
-   [ ] Aim.
-   [ ] Projectile.
-   [ ] Fire cooldown.

**Milestone:** 3 Enemy archetype hoạt động.

## PHASE 5 --- Auto Attack

-   [ ] WeaponData.
-   [ ] WeaponController.
-   [ ] TargetFinder.
-   [ ] Enemy-in-range query.
-   [ ] Independent target per Weapon.
-   [ ] Attack cooldown.
-   [ ] Projectile.
-   [ ] Projectile Pooling.
-   [ ] Damage on hit.
-   [ ] Crit.
-   [ ] Pierce.
-   [ ] Knockback.

**Milestone:** Player tự động bắn và kill enemy.

## PHASE 6 --- Multiple Weapons

-   [ ] WeaponInventory.
-   [ ] 6 Weapon Slots.
-   [ ] Equip/Unequip.
-   [ ] Independent cooldown.
-   [ ] Independent targeting.
-   [ ] Weapon UI.

### MVP Weapons

-   [ ] Pistol.
-   [ ] SMG.
-   [ ] Shotgun.
-   [ ] Rifle/Sniper.
-   [ ] Rocket Launcher.
-   [ ] Laser/Drone.

## PHASE 7 --- Wave System

-   [ ] WaveData.
-   [ ] WaveManager.
-   [ ] Wave timer.
-   [ ] Spawn count/rate.
-   [ ] Enemy composition.
-   [ ] Wave start/end.
-   [ ] Difficulty scaling.
-   [ ] 10 prototype Waves.

**Milestone:** Chơi liên tục Wave 1--10.

## PHASE 8 --- XP & Level Up

-   [ ] Enemy XP reward.
-   [ ] XP drop/pickup.
-   [ ] XP Bar.
-   [ ] PlayerLevel.
-   [ ] XP curve.
-   [ ] Level Up pause.
-   [ ] Generate upgrade choices.
-   [ ] Apply Stat.
-   [ ] Resume.

### MVP Stats

-   [ ] Max HP.
-   [ ] HP Regen.
-   [ ] Armor.
-   [ ] Dodge.
-   [ ] Move Speed.
-   [ ] Damage.
-   [ ] Ranged Damage.
-   [ ] Attack Speed.
-   [ ] Crit Chance.
-   [ ] Crit Damage.
-   [ ] Range.
-   [ ] Pickup Range.
-   [ ] Luck.

## PHASE 9 --- Run Currency

-   [ ] Material drop.
-   [ ] Pickup.
-   [ ] RunCurrencyManager.
-   [ ] Material HUD.
-   [ ] Reset per run.
-   [ ] Economy balance.

## PHASE 10 --- Shop

-   [ ] ShopManager.
-   [ ] Shop UI.
-   [ ] Generate offers.
-   [ ] Weapon offers.
-   [ ] Item offers.
-   [ ] Buy.
-   [ ] Sell.
-   [ ] Reroll + cost.
-   [ ] Lock.
-   [ ] Shop between Waves.

**Milestone:** `Wave → Shop → Wave` hoàn chỉnh.

## PHASE 11 --- Weapon Merge

-   [ ] Tier I--IV.
-   [ ] Duplicate detection.
-   [ ] Merge rules.
-   [ ] Merge feedback.
-   [ ] Tier stats.
-   [ ] Shop duplicate support.

## PHASE 12 --- Items

-   [ ] ItemData.
-   [ ] ItemManager.
-   [ ] StatModifier.
-   [ ] Stack rules.
-   [ ] Item Shop/UI.
-   [ ] 10--12 MVP Items.

## PHASE 13 --- Synergy

-   [ ] Weapon Tags.
-   [ ] SynergyManager.
-   [ ] 2-piece.
-   [ ] 4-piece.
-   [ ] 6-piece.
-   [ ] Synergy UI.
-   [ ] RAPID.
-   [ ] EXPLOSIVE.
-   [ ] ENERGY.
-   [ ] DRONE.

## PHASE 14 --- Boss

-   [ ] Boss architecture.
-   [ ] Boss HP bar.
-   [ ] Spawn.
-   [ ] Phase logic.
-   [ ] Telegraph.
-   [ ] Charge.
-   [ ] Projectile.
-   [ ] AoE.
-   [ ] Summon.
-   [ ] Death/reward.

## PHASE 15 --- Complete Run Flow

-   [ ] RunManager.
-   [ ] Start Run.
-   [ ] Wave/Shop/LevelUp/Pause states.
-   [ ] Victory.
-   [ ] Game Over.
-   [ ] Result screen.
-   [ ] Restart.
-   [ ] Return Home.

**Milestone:** Một run hoàn chỉnh từ Start → Result.

## PHASE 16 --- Meta Progression

-   [ ] Meta Currency.
-   [ ] Save/Load.
-   [ ] Unlock Character.
-   [ ] Unlock Weapon.
-   [ ] Unlock Item.
-   [ ] Unlock Arena.
-   [ ] Unlock Difficulty.
-   [ ] Achievement/Challenge hooks.

## PHASE 17 --- Weapon Evolution

Chỉ làm sau khi Weapon + Shop + Merge đã vui.

-   [ ] Evolution requirements.
-   [ ] Branch A/B.
-   [ ] Evolution selection UI.
-   [ ] Evolution effects/VFX.
-   [ ] Data-driven definitions.

## PHASE 18 --- UI/UX

**Hiện có một phần:** Scene có thanh HP đã nối với `PlayerHealth`, cùng
giao diện wave/gold. Chưa có logic wave/currency để hoàn thiện HUD.

-   [ ] Main Menu.
-   [ ] HUD.
-   [ ] HP / XP / Wave / Timer / Material.
-   [ ] Weapon Slots.
-   [ ] Level Up Popup.
-   [ ] Shop.
-   [ ] Pause.
-   [ ] Boss HP.
-   [ ] Victory / Game Over / Result.
-   [ ] Mobile Safe Area.

## PHASE 19 --- Audio & Feedback

-   [ ] Weapon SFX.
-   [ ] Hit/Death SFX.
-   [ ] Pickup/Level Up/Shop SFX.
-   [ ] Boss music.
-   [ ] Hit/Crit/Explosion VFX.
-   [ ] Controlled screen shake.
-   [ ] Damage numbers nếu cần.

## PHASE 20 --- Mobile Optimization

-   [ ] Enemy Pooling.
-   [ ] Projectile Pooling.
-   [ ] Drop Pooling.
-   [ ] Không Instantiate/Destroy liên tục.
-   [ ] Tối ưu TargetFinder/spatial query.
-   [ ] Sprite Atlas.
-   [ ] CPU/GPU/Memory profiling.
-   [ ] Test 50 enemy.
-   [ ] Test 100 enemy.
-   [ ] Stress test projectile.
-   [ ] Test Android thật.
-   [ ] Test iOS thật.

# MVP RELEASE CHECKLIST

-   [ ] Player movement mượt.
-   [ ] Orthographic fixed camera.
-   [ ] 3 Enemy archetypes.
-   [ ] 6 Weapons.
-   [ ] Independent Weapon targeting.
-   [ ] 10 Waves.
-   [ ] XP + Level Up + Stats.
-   [ ] Material.
-   [ ] Shop + Reroll.
-   [ ] Weapon Merge.
-   [ ] Items.
-   [ ] 1 Boss.
-   [ ] Victory/Game Over.
-   [ ] Run hoàn chỉnh.
-   [ ] Mobile performance đạt mục tiêu.
-   [ ] Combat đủ vui trước khi tăng content.

# Sau MVP

-   [ ] 20+ Waves.
-   [ ] Multiple Characters.
-   [ ] Multiple Arenas.
-   [ ] More Bosses.
-   [ ] Deep Weapon Evolution.
-   [ ] Advanced Synergy.
-   [ ] Daily Challenge.
-   [ ] Endless Mode.
-   [ ] Difficulty Tiers.
-   [ ] Cosmetic.
-   [ ] Achievements.
-   [ ] Leaderboard.

# Current Status

**Project Stage:** Prototype\
**Current Phase:** Phase 3 --- Health & Damage (đã triển khai một phần)\
**Implemented:** Arena Tilemap, camera follow, player movement,
state machine Idle/Run/Die, directional animation, health/damage
và Player HP UI.\
**Next Milestone:** Enemy đuổi theo Player và gây sát thương khi tiếp xúc.

**Validation:** Đã đối chiếu code và scene; chưa xác nhận bằng Play Mode.
Các mục chưa hoàn thành ở Phase 0--2 vẫn được giữ trong checklist.

# Development Rule

> **Không tăng content trước khi core loop vui.**

``` text
MOVEMENT
→ ENEMY
→ AUTO ATTACK
→ WEAPON
→ WAVE
→ XP
→ LEVEL UP
→ SHOP
→ MERGE
→ BOSS
→ COMPLETE RUN
→ POLISH
→ MORE CONTENT
```

**Project:** Shooter\
**Progress Version:** 1.1
