# Battle theo lịch — cấu hình và kiểm tra

## Thành phần
- BattleProgress: luật thời gian và điều kiện Boss, C# thuần, không phụ thuộc Scene/UI.
- BattleManager: điều phối trận, dừng chiến đấu, quyết toán RunCurrency đúng một lần.
- EnemySpawner: thực thi lịch theo đồng hồ BattleManager, giới hạn số sống theo từng lịch.
- BattleHUD: đồng hồ, thanh tiến độ và popup kết quả.
- EnemyHealth: phát Died khi mất hết máu; Despawned khi bị ẩn. Dọn pool không phải hạ Boss.
- CombatPoolManager: tái sử dụng Enemy/Projectile/VFX/Loot hiện có, chuẩn bị cả Boss.

Không thêm interface chỉ để bọc một implementation. Dùng sự kiện cho vòng đời Enemy/Boss,
component riêng cho UI/spawn và lớp luật thuần để kiểm thử/tái sử dụng.

## Cấu hình màn
Chọn Assets/GameData/Levels/LD_Level01 hoặc LD_Level02:
1. Duration: thời gian tổng; hiện lần lượt 120 và 180 giây.
2. Spawn Schedules: thêm lịch cho từng loại Enemy.
   - Start Time / End Time: khoảng [bắt đầu, kết thúc), trong Duration.
   - Interval: số giây giữa lượt sinh, ít nhất 0.1.
   - Count Per Spawn: số lượng mỗi lượt.
   - Max Alive: giới hạn còn sống của riêng lịch; pool vẫn giới hạn tổng loại Enemy.
   - Spawn Point Index: -1 luân phiên tất cả điểm; 0 trở lên chọn điểm trong MapReferences.
3. Boss / Enemy: để trống nếu không có Boss. Gán EnemyData riêng cho Boss khi đã có prefab.
   - Spawn Time: từ 0 đến Duration (bao gồm đúng thời điểm hết giờ).
   - Spawn Point Index: quy tắc như trên.
   - Boss được gán luôn phải chết để thắng. Chưa có Boss thực trong hai data hiện tại.
4. Đăng ký LevelData mới trong GD_Game. Map prefab vẫn dùng scene Battle chung.

Các data cũ đã được chuyển sang thời gian tổng; không còn trường waves.
Mỗi loại Enemy phải có PoolItem, EnemyHealth, EnemyChase và Rigidbody2D trên root prefab.
Giới hạn pool được cấu hình trong EnemyData.prewarmCount; tăng nếu nhiều lịch dùng chung loại.
Khi quái thường dùng cùng data với Boss, pool giữ một chỗ cho Boss trước thời điểm sinh.
Nếu pool chưa có chỗ, Boss thử lại ở lần cập nhật tiếp theo và không được coi là đã chết.

## Kết thúc
- Không Boss: sống hết giờ => thắng.
- Có Boss: hết giờ VÀ Boss đã chết => thắng.
- Boss chết sớm: tiếp tục đến hết giờ.
- Hết giờ Boss còn sống: dừng sinh quái thường, hiện HẠ BOSS!, tiếp tục chiến đấu.
- Player chết: thua, không quyết toán Gold thắng.
- Boss bị vô hiệu hóa bằng tác vụ ngoài chiến đấu không tính là chết; không tự cho thắng.
- Tạm dừng bằng timeScale=0 giữ nguyên đồng hồ và lịch spawn.

ShopPopup, ShopController, nút tiếp tục wave và script/data/prefab shop hồi máu cũ
đã gỡ khỏi Battle_01. EnergyShard/currency/loot giữ lại để tái sử dụng, chưa có công dụng
mới được quyết định. Chưa triển khai kho/rương/vũ khí, Home shop hoặc chăm sóc Pet trong thay đổi này.

BattleHUD giữ các reference UI cũ. Scene chưa gán Defeat Popup; khi thua hiện THẤT BẠI
trên timer nếu không có popup. Có thể gán popup riêng vào BattleHUD sau.
Không sửa scene phục hồi Assets/_Recovery: đây là bản cũ và không dùng để chạy Battle mới.
Bản sao Scene trước migration nằm ở Battle_01.before-migration.unity.txt.

## Kiểm tra
- Chạy Tests/Battle/Run.ps1 bằng PowerShell: kiểm thử luật thời gian và EnemySpawner thật
  với pool/Unity test doubles, không thay thế kiểm thử Unity vật lý/Scene.
- Kiểm tra Play Mode: màn không Boss, Boss chết sớm, Boss sống sau giờ, Player chết,
  pool đầy, nhiều lịch chồng nhau, pause/resume, Next Level/Replay và Gold chỉ nhận một lần.
