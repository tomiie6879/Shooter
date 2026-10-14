# SHOOTER — DEVELOPMENT PROGRESS

**Cập nhật:** 10/10/2026 · **Giai đoạn:** Prototype · **Phiên bản:** 2.0

## Quy ước và xác nhận

- [x] Tìm thấy triển khai trong code/tài nguyên được đối chiếu.
- [ ] Chưa triển khai hoặc chưa có bằng chứng xác nhận hoàn thành.

Có code không đồng nghĩa đã chạy đúng trong Play Mode hoặc đạt chất lượng phát hành. Lần này chỉ sửa tài liệu, không sửa code/scene/prefab; chưa chạy Play Mode, build hoặc benchmark. Giữ nguyên các thay đổi đang có của người dùng.

## Hướng sản phẩm mới

```text
Home → Chuẩn bị linh vật → Battle/Wave → Shop bằng Energy
→ Victory nhận Gold / Thua mất Gold trận
→ Home → Thu thập, cho ăn, nâng cấp, tiến hóa → Trận tiếp theo
```

Nhà chính và phát triển linh vật là kế hoạch mới, chưa triển khai. Chi tiết ở [README.md](README.md).

## 1. Player và chiến đấu

- [x] Scene Battle_01, arena Tilemap và map prefab MAP_Desert.
- [x] PlayerController, input bàn phím, state Idle/Run/Die và animation di chuyển.
- [x] PlayerHealth, EnemyHealth, IHealth/IDamageable và hồi máu.
- [x] EnemyChase, EnemyData và EnemyContactDamage; cooldown riêng mặc định 0,6 giây.
- [x] SpiritOrbit, SpiritLightningAttack, LightningProjectile và VFX trúng đòn.
- [x] CombatPoolManager/PoolItem cho Enemy, projectile, VFX, loot; reset Enemy khi tái sử dụng.
- [ ] Kiểm thử nhiều Enemy tiếp xúc cùng lúc, sát thương từng con độc lập.
- [ ] Kiểm thử animation chết và dừng sát thương trong shop.
- [ ] Virtual joystick, arena boundary và kiểm tra Android/iOS.
- [ ] Separation, Dasher, Shooter và Boss thực tế.

Nguồn: Assets/Scripts/Player, Enemy, Attack, Projectiles, Pooling, VFX. Có CameraFollow; camera cố định của thiết kế cũ chưa được coi là hoàn thành.

## 2. Level và wave theo data

- [x] GameData, LevelData, WaveSettings, EnemySpawnGroup và MapReferences.
- [x] LevelLoader/GameSession: Battle_01 chung, tải map và chuẩn bị pool theo Level.
- [x] WaveManager: thời lượng từng wave, shop giữa wave, Victory cuối Level.
- [x] Timer hiển thị giây, Wave Fill theo số wave hoàn thành.
- [x] Hàm Next Level và Replay trong LevelLoader.
- [x] LD_Level01 / LD_Level02 có mã level_01 / level_02.
- [ ] Kiểm thử lại OnClick Next Level/Replay và reference scene đã lưu.
- [ ] Menu chọn màn tự sinh và mở khóa màn được xác nhận hoạt động.

Data đọc lần này: Level01 có 3 wave × 10 giây, Level02 có 3 wave × 15 giây. Đây là cấu hình test, không phải 10 wave × 60 giây đã hoàn tất; hệ thống hỗ trợ tùy chỉnh theo LevelData.

## 3. Loot và tiền tệ

- [x] EnemyHealth gọi DropEnemyLoot trước khi trả Enemy về pool.
- [x] LootData/LootPickup, prefab PF_GoldPickup và PF_EnergyShardPickup.
- [x] Tỷ lệ Gold, lượng Energy cấu hình theo EnemyData.
- [x] Thả đồ quanh vị trí chết, hút về Player, cộng khi đến gần.
- [x] Pool loot và gộp giá trị khi hết object rảnh.
- [x] RunCurrency giữ Gold/Energy trận, cập nhật HUD bằng event.
- [x] Settle chỉ trả Gold khi thắng; chặn trả thưởng lặp cùng trận.
- [x] Ví Gold lưu PlayerPrefs; Energy reset khi bắt đầu Level mới.
- [ ] Kiểm thử tỷ lệ 0%/100%, hút đồ, pool đầy, thắng/thua và tải lại game.
- [ ] Xác nhận root prefab loot có PoolItem/LootPickup; lỗi khởi tạo pool từng được báo, chưa xác nhận đã hết bằng Play Mode.
- [ ] Save có phiên bản/migration và kiểm thử tính nhất quán khi mở rộng bộ sưu tập.

Loot scripts hiện ở Assets/Sprites/Loot; RunCurrency ở Assets/Scripts/Economy. Không tự di chuyển file trong lần cập nhật tài liệu này.

## 4. Shop và kết thúc trận

- [x] ShopItemData, ShopItemButton và ShopUI.
- [x] Sinh nút từ shopItems, mua hồi máu bằng Energy.
- [x] Kiểm tra đủ tiền, thiếu máu và đúng trạng thái Shop.
- [x] Dừng thời gian/attack/thu thập trong Shop; ContinueAfterShop bắt đầu wave mới.
- [x] Giữ Enemy/projectile/loot qua Shop; thu hồi khi kết thúc trận.
- [x] Nhánh Victory/Defeat và text Gold thưởng trong code.
- [ ] Kiểm thử popup, OnClick, nhấn lặp và không trừ tiền khi mua không hợp lệ.
- [ ] Xác nhận HUD/RewardTMP và reference sau việc di chuyển script.
- [ ] Món ngoài hồi máu, bán đồ, reroll và hiệu ứng khác.

Nguồn: Assets/Sprites/Manager/WaveManager.cs, Assets/Scripts/Shop, Assets/Scripts/Economy/RunCurrency.cs.

## 5. Nhà chính — chưa triển khai

- [ ] Scene Home cho hoạt động ngoài trận, dùng chung cho mọi Level.
- [ ] Home → chọn trận → Battle → Result → Return Home.
- [ ] Hiển thị ví Gold lâu dài tại Home.
- [ ] Souls/linh vật sở hữu đi lại: idle, wander và giới hạn vùng.
- [ ] Tương tác xem thông tin/bộ sưu tập.
- [ ] Nhà chăm sóc/nâng cấp và giao diện.
- [ ] Vòng đời Home/Battle không nhân đôi Player, pool hoặc manager.

## 6. Thu thập và lưu linh vật — chưa triển khai

- [ ] Chốt quan hệ Souls/linh vật và số đồng hành mang vào trận.
- [ ] Data định nghĩa và save trạng thái sở hữu riêng.
- [ ] Danh sách sở hữu và hiển thị đúng tại Home.
- [ ] Reward data cho trận quan trọng.
- [ ] Chốt cách nhận, điều kiện thưởng và xử lý linh vật trùng.
- [ ] Cấp thưởng một lần, lưu/tải lại bộ sưu tập.
- [ ] Chọn đồng hành, đưa chỉ số/khả năng vào Battle.

## 7. Cho ăn, nâng cấp, tiến hóa — chưa triển khai

- [ ] Chốt nguồn thức ăn, chi phí Gold và điều kiện phát triển.
- [ ] Cho ăn, cập nhật trạng thái phát triển.
- [ ] Nâng cấp: kiểm tra/trừ tài nguyên và tăng sức mạnh.
- [ ] Tiến hóa: điều kiện, hình ảnh và khả năng tương ứng.
- [ ] Lưu cấp độ/tiến hóa xuyên Level và sau khi thoát game.
- [ ] Kiểm thử không trừ hai lần, không sửa asset cấu hình gốc.
- [ ] Cân bằng kiếm Gold → chăm sóc → đánh trận khó hơn.

## 8. Chất lượng và hiệu năng

- [ ] Kiểm tra reference sprite/animation Enemy sau thay đổi art hiện tại.
- [ ] Kiểm tra compile sau di chuyển script; không Missing Script trong scene/prefab.
- [ ] Profiling target query, loot/Enemy đông, CPU, bộ nhớ và FPS trên mobile.
- [ ] Safe Area, tỷ lệ màn hình, điều khiển cảm ứng.
- [ ] Âm thanh chiến đấu/nhặt đồ/shop và phản hồi UI/hit.
- [ ] Cân bằng thời gian wave, spawn, Gold/Energy và giá Shop.

Pooling đã có không đồng nghĩa đã xác nhận không lag; cần đo trước khi đánh dấu đạt hiệu năng.

## Milestone tiếp theo

1. Xác nhận trận hiện tại chạy trọn vòng; ưu tiên pool loot và liên kết UI.
2. Home tối thiểu có ví Gold và một linh vật sở hữu đi lại; vào trận rồi quay về không mất dữ liệu.
3. Một trận quan trọng thưởng một linh vật; nhận/lưu/hiển thị đúng một lần.
4. Một luồng cho ăn → nâng cấp → tiến hóa có tác dụng trong trận.

## Điều chỉnh roadmap cũ

Trạng thái không còn dừng ở Phase 3 Health & Damage: code đã có wave, shop và loot. Checklist 6 vũ khí, XP/stat selection, weapon merge và synergy được hoãn, không ghi là hoàn thành và không còn là điều kiện MVP bắt buộc. Ưu tiên mới: chiến đấu ổn định + nhà chính + bộ sưu tập + phát triển linh vật.

## 13/10/2026 — Chuyển Battle sang lịch spawn

- Thay WaveManager bằng BattleManager, giữ GUID script và cập nhật reference Scene/LevelLoader.
- Tách BattleProgress (luật thuần), EnemySpawner (lịch), BattleHUD (hiển thị).
- LevelData dùng duration/spawnSchedules/boss; hai màn chuyển sang 120/180 giây, chưa gán Boss thực.
- Hết giờ dừng quái thường; có Boss bắt buộc thì chờ sự kiện chết, không coi despawn là hạ Boss.
- Gỡ shop giữa wave: script, data, prefab và cây UI khỏi Battle_01.
- Tái sử dụng EnemyHealth, pool và RunCurrency; chưa triển khai rương/kho/vũ khí/Home/Pet.
- Biên dịch toàn bộ runtime C# bằng compiler/thư viện Unity 6000.4.12f1: qua.
- Tests/Battle/Run.ps1: luật thời gian và spawn lifecycle qua; dùng test doubles, chưa thay thế Play Mode.
- Kiểm tra Scene không có reference nội bộ hỏng mới hoặc trùng fileID; chưa kiểm thử chạy Unity thực tế.
- Hướng dẫn: Documentation/BattleMigration/README.md.

## 13/10/2026 — Spirit theo data/loadout

- Thêm SpiritData, SpiritLoadout, SpiritContext/ISpiritAttack, SpiritController và PlayerSpiritManager.
- SpiritProjectileAttack thay SpiritLightningAttack, dùng chung cho kiểu bắn đạn; tìm mục tiêu dùng buffer tái sử dụng.
- LevelLoader khởi tạo đội hình và pool đạn/VFX; BattleManager bật/tắt qua PlayerSpiritManager.
- Tạo SD_Lightning và SL_Player, gán loadout trong GD_Game; ô Prefab còn chờ người dùng tạo PF.
- Không sửa Scene/SpiritPoints. Cần thêm manager vào Player, gán điểm/layer và prefab trước khi Play.
- Runtime biên dịch với Unity 6000.4.12f1 thành công; chưa chạy Play Mode.
- Hướng dẫn: Documentation/Spirits/README.md.
