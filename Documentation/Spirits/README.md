# Spirit tự gắn vào Player

Code đã chuyển sang data/loadout. Không sửa Battle_01 trong lần này vì SpiritPoints có thể chưa lưu.
Phải hoàn thành cấu hình bên dưới trước khi Play. Nếu thiếu, LevelLoader dừng trận và báo rõ reference thiếu.

## 1. Lưu Scene và tạo PF_LightningSpirit
1. Dừng Play, Ctrl+S để giữ các SpiritPoint.
2. Chọn LightingSouls đang có trong Scene, giữ SpriteRenderer, Animator và FirePoint.
3. Component SpiritLightningAttack cũ đã đổi thành SpiritProjectileAttack (giữ GUID).
   Nếu Inspector vẫn hiện tên cũ, chờ Unity biên dịch. Chỉ để một component tấn công.
4. Add Component: SpiritController lên root LightingSouls.
5. Trong SpiritProjectileAttack chỉ gán FirePoint là con của prefab. Không gán Player/pool/data/layer ở PF.
6. Đặt root local Position = 0,0,0 trước khi tạo prefab. Giữ Scale phù hợp hình ảnh hiện tại.
7. Có thể giữ SpiritOrbit với Radius khoảng 0.05-0.12 để chuyển động nhẹ quanh slot.
   Nếu muốn đứng đúng vị trí slot, bỏ SpiritOrbit hoặc đặt Radius = 0.
   Animator idle chỉ animate sprite, tránh animate Position khiến ghi đè chuyển động.
8. Kéo root vào Assets/Prefabs/Spirits, đặt tên PF_LightningSpirit.
9. Kiểm tra prefab đã có SpiritController và SpiritProjectileAttack bật.
10. Xóa instance LightingSouls đặt thủ công trong Scene sau khi lưu prefab.
    Chỉ giữ bốn SpiritPoint dưới Spirits, không xóa các điểm.

## 2. Cấu hình Player một lần
1. Chọn root Player có PlayerHealth, thêm PlayerSpiritManager.
2. Spirit Points: Size=4, gán lần lượt SpiritPoint_01,02,03,04.
   Hoặc menu component (ba chấm) > Collect SpiritPoints (sorted by name).
   Các điểm phải là con của Player, khác nhau và đang active.
3. Enemy Layer: chọn Enemy.
4. Nếu chấm màu là SpriteRenderer để đánh dấu, tắt renderer; không tắt GameObject SpiritPoint.
   Gizmo chỉ hiển thị trong Editor thì không cần tắt.

## 3. Gán data có sẵn
- Assets/GameData/Spirits/SD_Lightning:
  - Prefab: kéo PF_LightningSpirit vào (đây là ô còn trống cần bạn gán).
  - Projectile đã gán theo GD_Game: kiểm tra là PD_Lightning.
  - Range=6, Cooldown=0.7, Damage=20 có thể chỉnh tùy ý.
- Assets/GameData/Spirits/SL_Player:
  - Slot 0 = SD_Lightning; slot 1-3 đang trống.
- GD_Game/Spirit Loadout đã trỏ đến SL_Player.
- LevelLoader tự lấy PlayerSpiritManager từ Player đã gán. BattleManager không cần danh sách attack thủ công.

## 4. Thêm Spirit lửa
1. Tạo PF_FireSpirit: SpriteRenderer + Animator lửa + SpiritController + SpiritProjectileAttack.
2. Gán FirePoint con của PF; root local Position=0.
3. Tạo ProjectileData riêng nếu muốn đạn/VFX lửa. Prefab projectile lửa dùng SpriteProjectile; sét dùng LightningProjectile:
   cả hai dùng chung PooledProjectile để bay, gây damage và trả pool.
4. Create > Shooter > Data > Spirit: SD_Fire, Spirit Id=fire, gán prefab, projectile và chỉ số.
5. Gán SD_Fire vào Slot 1 của SL_Player. Play: lửa ở SpiritPoint_02.
6. Không cần sửa BattleManager, LevelLoader hay danh sách projectile của GD_Game.

## Kiến trúc và giới hạn
- SpiritLoadout là cấu hình đội hình dùng chung hiện tại, chưa phải save đội hình từng người chơi.
- SpiritData là định nghĩa; không ghi năng lượng/cấp sở hữu vào ScriptableObject.
- PlayerSpiritManager tạo một lần đầu trận; SpiritController nhận SpiritContext.
- ISpiritAttack cho phép thay component kiểu đánh; SpiritProjectileAttack dùng chung cho sét/lửa bắn đạn.
- SpiritData hiện yêu cầu ProjectileData; kiểu đánh không dùng projectile cần mở rộng data/dependency sau.
- CombatPoolManager tự prewarm projectile và cast/hit VFX của đội hình đang dùng, không tạo lại pool trùng data.
- GameData.projectiles/vfx vẫn là catalog; không còn prewarm toàn catalog.
- Spirit không có máu riêng. Battle kết thúc tắt tấn công cả đội.
- SpiritOrbit dùng tọa độ local quanh vị trí ban đầu, không dùng tọa độ Player làm tâm.
- Không instantiate/destroy Spirit mỗi phát bắn; projectile/VFX tiếp tục thuê/trả pool.
- Đã biên dịch runtime; chưa xác nhận Play Mode vì PF/SpiritPoints cần được gán trong Editor.

## Vòng bay chung trên đầu Player
PlayerSpiritManager tự tạo/cấu hình SpiritOrbit ở root PF; không cần gán orbit từng PF.
- Orbit Center: tùy chọn. Nếu trống, lấy trung bình các SpiritPoint rồi bám theo Player.
- Orbit Center Offset: dịch tâm, ví dụ Y=0.2 để nâng vòng bay.
- Orbit Radius=0.45: bán kính vòng trong, tính theo world units.
- Radius Step=0.12: chênh bán kính cho các Spirit quay ngược.
- Orbit Speed=45: độ/giây.
- Alternate Directions=true: xen kẽ hai chiều theo thứ tự Spirit thực sự được trang bị.
- Starting Angle: góc khởi đầu cả đội.
Góc khởi đầu chia đều theo số Spirit có data, bỏ qua slot trống.
Đổi tham số ngoài Play Mode rồi chạy lại để áp dụng.
Quay hai chiều chỉ chia đều lúc bắt đầu, không giữ khoảng cách góc cố định về sau.
Nếu muốn luôn cách đều: tắt Alternate Directions, các Spirit dùng chung bán kính/tốc độ.
Orbit chạy LateUpdate trước component tấn công để FirePoint bám đúng vị trí mới.
Animator không nên ghi Position của root. Giữ hình idle dưới Visual nếu cần animate transform.


## Projectile lửa
Tạo prefab mới độc lập, không Apply All trên instance đang trỏ PF_Lightning.
Root PF_PJ_Fire: SpriteRenderer + SpriteProjectile + PoolItem; Animator tùy chọn trên root.
Không gắn LightningProjectile hoặc LineRenderer. Không cần Rigidbody2D/Collider để hit:
PooledProjectile dùng CircleCast theo quãng đường di chuyển.
Artwork hướng phải: Angle Offset=0; hướng lên: -90; tắt Face Direction để giữ hướng hình.
Tạo PD_Fire, gán PF_PJ_Fire và thông số tốc độ/tầm/lifetime/hitRadius/VFX.
Gán PD_Fire vào SD_Fire.Projectile. Pool tự chuẩn bị theo loadout, không đặt đạn sẵn trong Scene.
LightningProjectile cũ giữ GUID và thông số visual nên PF sét không cần tạo lại.

## Chia mục tiêu trong đội Spirit (14/10/2026)
- Coordinator dùng chung tự được tạo bởi PlayerSpiritManager; không cần thêm component hay sửa PF.
- Các Spirit ưu tiên Enemy có ít người nhắm nhất trong tầm, rồi chọn gần nhất.
- Đếm theo EnemyHealth, không đếm từng Collider. Ít Enemy thì cho phép nhiều Spirit cùng đánh.
- Target Review Interval trên PlayerSpiritManager: mặc định 0.75 giây giữa các lần xét lại.
- Initial Shot Spacing: mặc định 0.15 giây, giãn phát đầu theo thứ tự các slot có Spirit.
- Mục tiêu chết, ra khỏi tầm, Collider tắt hoặc về pool sẽ được bỏ; khi hết Battle xóa đăng ký.
- Đổi các thông số ngoài Play rồi chạy lại. Không đảm bảo đạn đang bay trúng đúng Enemy được chọn:
  projectile hiện bay thẳng và có thể va vào Enemy khác trên đường.
- Kiểm thử Tests/SpiritTargets/Run.ps1 xác nhận vòng đời đăng ký bằng test doubles.
  Runtime C# biên dịch thành công; chưa xác nhận hành vi hình ảnh trong Play Mode.
