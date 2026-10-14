# SHOOTER

> Game 2D top-down sinh tồn theo wave, thu thập Souls/linh vật và phát triển nhà chính.

**Cập nhật:** 13/10/2026 · **Unity:** 6000.4.5f1 · **Giai đoạn:** Prototype · **README:** 3.0

> Battle đã được chuyển sang thời gian tổng, lịch spawn và điều kiện Boss trong mã nguồn/Scene. Các hệ thống kho, trang bị, Home và chăm sóc vẫn là kế hoạch. Xem [hướng dẫn Battle mới](Documentation/BattleMigration/README.md); Play Mode chưa được xác nhận trong lần sửa này.

## Tổng quan

Người chơi chạy và né tránh trong các trận sinh tồn; Souls/linh vật đồng hành tự động tấn công. Chiến thắng mang về Gold. Các trận quan trọng sẽ tạo cơ hội thu thập thêm linh vật. Ngoài trận, người chơi trở về nhà chính để chăm sóc, cho ăn, nâng cấp và tiến hóa đồng hành, rồi chuẩn bị trận tiếp theo.

Nhà chính là một bản đồ có hoạt động: Souls và linh vật đã sở hữu đi tới đi lui tại đây. Trọng tâm mới là bộ sưu tập đồng hành và sự phát triển lâu dài, bên cạnh sinh tồn và lựa chọn trong từng trận.

Mục tiêu nền tảng: Android/iOS, màn hình ngang. Prototype hiện dùng bàn phím; joystick và hiệu năng trên thiết bị thật còn cần triển khai/kiểm thử.

## Vòng lặp chính

```text
NHÀ CHÍNH
→ Chăm sóc / cho ăn / nâng cấp / tiến hóa linh vật
→ Mặc trang bị, chọn trận và chuẩn bị Souls/Pet đủ năng lượng
→ Sinh tồn theo thời gian màn; quái xuất hiện liên tục theo lịch
→ Thu nhặt Gold, rương, vũ khí và vật phẩm
→ Hết giờ; nếu có Boss bắt buộc thì phải hạ Boss để Victory
→ Quyết toán chiến lợi phẩm và lưu kết quả một lần
→ Về Home: quản lý đồ, bán đồ, chăm sóc và phát triển đồng hành
```

Thua không nhận Gold tạm của trận; Gold đã có trong ví vẫn giữ nguyên. Luật nhận linh vật và xử lý linh vật trùng chưa chốt, chưa có hệ thống cấp thưởng linh vật.

## Battle theo thời gian — kế hoạch mới

- Mỗi màn có thời lượng cấu hình riêng, ví dụ màn đầu 120 giây; màn khó hơn có thể dài hơn.
- Thiết kế đề xuất dùng một thanh thời gian tổng cho mỗi màn, thay luồng nhiều wave và shop xen kẽ cũ. Wave Fill biểu diễn `elapsed / duration`; hết giờ mà Boss còn sống thì hiển thị yêu cầu hạ Boss.
- Lịch sinh quái quy định Enemy, giây bắt đầu/kết thúc, khoảng cách giữa các lượt sinh, số lượng mỗi lượt, giới hạn quái còn sống và nhóm điểm spawn. Các lịch có thể chồng lên nhau.
- Boss có lịch xuất hiện một lần gần cuối trận. Thời điểm, loại Boss và điểm spawn lấy từ data.
- Không có Boss bắt buộc: hết giờ và Player còn sống thì thắng. Có Boss bắt buộc: chỉ thắng khi vừa hết giờ vừa hạ Boss.
- Boss chết sớm thì vẫn phải sống hết thời gian. Boss còn sống sau khi hết giờ thì trận tiếp tục. Player chết trước khi hoàn thành điều kiện thì thua.
- Đề xuất dừng sinh quái thường khi hết giờ, giữ quái đang sống; kỹ năng Boss có thể tiếp tục triệu hồi. Quy tắc này cần chốt khi triển khai.
- Phải phân biệt Boss chưa xuất hiện, đang sống và đã bị hạ. Trả Boss về pool hoặc dọn scene không được tính là hạ Boss.

Ví dụ cấu hình minh họa cho màn 120 giây, chưa phải dữ liệu đã áp dụng:

| Thời gian | Lịch sinh |
|---|---|
| 0–120 giây | Slime xuất hiện đều đặn |
| 30–120 giây | Thêm quái chạy nhanh |
| 75–120 giây | Thêm quái mạnh và tăng áp lực |
| Giây 105 | Sinh Boss một lần |
| Giây 120 | Kiểm tra điều kiện thắng; tiếp tục nếu Boss còn sống |

### Battle hiện tại sau chuyển đổi

- BattleManager điều phối; BattleProgress giữ luật; EnemySpawner chạy lịch; BattleHUD hiển thị.
- Đã bỏ WaveSettings, vòng lặp shop giữa wave và các asset shop hồi máu cũ khỏi Battle.
- Level01: 120 giây; Level02: 180 giây. Cả hai chưa gán Boss vì chưa có Boss riêng.
- Thanh tiến độ theo thời gian; Enemy có cooldown tiếp xúc riêng, pooling và loot hiện có được tái sử dụng.

## Chiến lợi phẩm và trang bị — kế hoạch mới

- Enemy có bảng rơi đồ riêng: Gold, rương, vũ khí, trang bị và nguyên liệu. Tỷ lệ có rơi và trọng số chọn món là hai cấu hình khác nhau.
- Đồ xuất hiện trên sàn; Player đến gần thì hút về và cộng vào túi chiến lợi phẩm của trận. Tiếp tục dùng pool cho vật phẩm dưới đất.
- Tách túi trong trận khỏi kho lâu dài tại Home. Quyết toán kết quả đúng một lần để tránh nhận thưởng lặp khi đổi scene hoặc nhấn lại nút.
- Gold nhặt trong trận chỉ được nhận khi thắng; thua không nhận Gold tạm. Gold đã sở hữu vẫn giữ nguyên.
- Đề xuất rương được mang về mở tại Home. Việc thua có mất rương, vũ khí và vật phẩm mới nhặt hay không chưa chốt; không mặc định áp dụng luật mất đồ.
- Home có tiệm thu mua để bán vật phẩm đổi Gold. Cần kiểm tra đồ đang mặc/đồ khóa trước khi bán.
- Player có các ô nón, giáp, quần, bao tay, giày để tăng sức mạnh và giáp. Ô vũ khí và cách Player sử dụng vũ khí nhặt được cần chốt riêng.
- Tách định nghĩa loại đồ khỏi từng món sở hữu: mỗi món có mã riêng, loại đồ, cấp, độ hiếm và chỉ số bổ sung. Tính lại chỉ số từ chỉ số gốc + trang bị + nâng cấp + hiệu ứng, tránh cộng lặp khi đổi scene.

## Gold và EnergyShard — prototype hiện tại

| Tài nguyên | Nguồn | Công dụng và vòng đời |
|---|---|---|
| Gold | Enemy chết có tỷ lệ rơi theo EnemyData | Nhặt vào Gold tạm. Chỉ cộng vào ví lâu dài một lần khi thắng cả Level; thua mất Gold tạm. |
| EnergyShard | Enemy thường rơi 1; Boss có thể cấu hình nhiều hơn | Mua đồ giữa wave, giữ trong cùng Level; reset về 0 khi bắt đầu Level mới. Không chuyển thành Gold. |

Đồ rơi xuống sàn, Player vào phạm vi hút thì đồ tăng tốc bay về; chỉ cộng khi đến Player. Loot dùng pool; khi đầy, code gộp giá trị vào vật phẩm cùng loại. Đồ chưa nhặt khi kết thúc trận không tự tính thành phần thưởng.

Shop hồi máu giữa wave đã gỡ. EnergyShard vẫn được giữ trong hệ thống loot/currency; công dụng mới chưa chốt.

Gold lâu dài dự kiến phục vụ phát triển linh vật tại nhà chính. Chi phí nâng cấp, nguồn thức ăn, tài nguyên cho ăn và điều kiện tiến hóa cần thiết kế riêng. EnergyShard không trở thành tiền lâu dài tại nhà chính.

## HomeScene, Souls và Pet — kế hoạch gameplay chưa triển khai

- Bản đồ Home có khu sinh hoạt cho Souls/linh vật sở hữu tự đi lại trong vùng cho phép.
- Điểm tương tác xem thông tin, mở bộ sưu tập và chọn trận.
- Kho đồ/trang bị: quản lý chiến lợi phẩm, mặc đồ; mở rương tại Home là phương án đề xuất.
- Tiệm thu mua: bán vật phẩm đổi Gold.
- Nhà chăm sóc: cho Souls/Pet ăn đúng thời điểm để duy trì năng lượng và sức mạnh trước Battle.
- Nhà nâng cấp: tăng cấp và tiến hóa Souls/Pet khi đủ điều kiện, tài nguyên.
- Trận quan trọng cho cơ hội thu thập thêm linh vật.
- Bộ sưu tập và trạng thái phát triển được lưu xuyên Level và sau khi thoát game.
- Sức mạnh phát triển tại Home cần nối vào khả năng chiến đấu của đồng hành.

### Phân biệt Souls và Pet

| Nội dung | Soul | Pet |
|---|---|---|
| Trong Battle | Bay trên/quanh Player, tự động tấn công | Đồng hành chiến đấu trên bản đồ |
| Máu riêng | Không có | Chưa chốt |
| Năng lượng | Chăm sóc để duy trì sức mạnh; công thức chưa chốt | Cần đủ năng lượng để tham chiến; tiêu hao sau trận và cần chăm sóc |
| Phát triển | Nâng cấp, tiến hóa | Nâng cấp, tiến hóa |

- Trạng thái từng cá thể được lưu độc lập với GameObject đi lại trong Home: mã cá thể, loại, cấp, kinh nghiệm, giai đoạn tiến hóa, năng lượng và thời điểm cập nhật.
- Đề xuất tính chăm sóc theo thời gian đã lưu, kể cả khi đóng game; không phụ thuộc bộ đếm của HomeScene. Game offline dùng thời gian máy có thể bị chỉnh, cần chấp nhận hoặc bổ sung thời gian máy chủ sau này.
- Đề xuất năng lượng có giới hạn tối thiểu 0, không làm mất Pet vì người chơi nghỉ game; chốt sức mạnh đầu Battle để tránh thay đổi đột ngột giữa trận.
- Cần quy định ngưỡng đủ năng lượng (đầy 100% hay một ngưỡng), mức hao hụt và xử lý khi thắng/thua/thoát trận để tránh né chi phí.

Chưa chốt: số đồng hành mang vào trận; máu Pet; cách nhận/trùng linh vật; công thức cho ăn, cấp độ và tiến hóa; tác dụng cụ thể của năng lượng Soul. Không mặc định 6 slot vũ khí cũ là 6 slot linh vật.

## Kiến trúc màn chơi

### Đã có trong prototype

- Battle_01 là scene gameplay chung, không cần tạo scene mới cho từng Level.
- GameData đăng ký LevelData. LevelData chứa mã riêng, map prefab, duration, spawnSchedules và boss.
- MapReferences giữ điểm Player/Enemy spawn trong map prefab.
- LevelLoader tạo map, đặt Player, chuẩn bị pool rồi bắt đầu BattleManager.
- CombatPoolManager/PoolItem quản lý Enemy, projectile, VFX và loot.
- GameSession giữ lựa chọn Level; Next Level/Replay tải lại scene chung. Chưa có Return Home.

Thêm màn có cơ chế đã hỗ trợ: tạo LevelData → chọn map prefab → cấu hình lịch spawn/Boss → đăng ký GameData. Cơ chế mới vẫn cần code tương ứng.

### Đề xuất mở rộng

- LevelData mới chứa thời lượng màn, danh sách lịch spawn, lịch Boss, điều kiện hạ Boss và thưởng hoàn thành; tiếp tục dùng scene Battle chung và map prefab.
- Mỗi lịch spawn chứa EnemyData, StartTime, EndTime, SpawnInterval, CountPerSpawn, MaxAlive và SpawnPointGroup. Boss chỉ sinh một lần, không lặp theo lịch quái thường.
- Thêm định nghĩa LootTableData, ItemData/EquipmentData, loại Soul/Pet, thức ăn và công thức nâng cấp/tiến hóa. Tên dữ liệu là đề xuất, chưa khẳng định đã có class.
- Tách trạng thái trận (timer, spawn, Boss, túi chiến lợi phẩm) khỏi save lâu dài (Gold, kho đồ, trang bị, Souls/Pet).

- Thêm scene Home cho hoạt động ngoài trận; đây là khu chức năng riêng, không phải scene mới cho từng màn chiến đấu.
- Tách ScriptableObject định nghĩa linh vật khỏi save trạng thái sở hữu/cấp độ/tiến hóa của Player.
- Dùng một nguồn dữ liệu người chơi cho ví Gold và bộ sưu tập; giữ RunCurrency riêng cho tiền trong trận.
- Bổ sung reward data cho trận quan trọng, cấp thưởng một lần và lưu trước khi về Home.
- Mở rộng save: hiện mới có ví Gold lưu bằng PlayerPrefs, chưa có save bộ sưu tập.
- Giữ Next Level nếu muốn chơi liên tục, bổ sung Return Home; không reset dữ liệu sở hữu khi đổi scene.

## Tình trạng hiện tại

Đối chiếu mã nguồn ngày 10/10/2026: Player/health, Enemy chase/contact damage, linh hồn sét, pooling, LevelData/wave cũ, shop hồi máu cũ, loot hút về Player và Gold chỉ lưu khi thắng đã có code. Có prefab map, Slime, projectile, VFX, Gold/Energy và nút shop.

Chưa xác nhận toàn bộ luồng bằng Play Mode trong lần cập nhật này. Đã có tài sản/map HomeScene đang xây dựng nhưng chưa xác nhận import và chạy thành công; điều này không đồng nghĩa gameplay Home đã hoàn thành. Bộ sưu tập, nhận linh vật, cho ăn, nâng cấp, tiến hóa, kho/trang bị chưa được xác nhận triển khai. Battle theo lịch mới đã thay thế cơ chế cũ; kiểm thử luật và biên dịch runtime đã qua, chưa chạy Play Mode. Xem [PROGRESS.md](PROGRESS.md).

## Lộ trình ưu tiên

1. Chốt quy tắc còn mở; chuyển Battle sang thời gian tổng, lịch spawn, Boss và điều kiện thắng/thua. Kiểm thử màn có/không Boss, Boss chết sớm/muộn và Player chết.
2. Bảng rơi đồ, pool và túi chiến lợi phẩm trong trận; ngăn rơi đồ/nhận thưởng lặp.
3. Lưu kho và quyết toán Gold/chiến lợi phẩm; hoàn thiện Home → Battle → Result → Home.
4. Kho Home, mở rương và tiệm bán đồ.
5. Các ô trang bị Player và tính chỉ số.
6. Bộ sưu tập Souls/Pet, chọn đội hình, năng lượng và cho ăn; xử lý cả thua/thoát trận.
7. Nâng cấp, tiến hóa và thưởng linh vật ở trận quan trọng.
8. Hoàn thiện hành vi đi lại tại Home, giao diện, cân bằng và hiệu năng.

### Quyết định còn mở trước khi triển khai

- Pet có máu riêng và có thể gục trong Battle hay không?
- Thua xử lý rương/vũ khí/trang bị mới nhặt thế nào? Gold đã chốt: không nhận khi thua.
- EnergyShard và shop giữa wave cũ sẽ bỏ hay chuyển công dụng? Luồng Battle mới đề xuất không mở shop xen giữa; chưa tự động xóa hệ thống cũ.
- Rương mở ở Home hay ngay trong trận; vũ khí nhặt về được trang bị/sử dụng ra sao?
- Ngưỡng năng lượng tham chiến, lịch cho ăn, hao hụt khi thua/thoát và công thức tiến hóa.

Các con số thử nghiệm trên là phạm vi đề xuất, chưa phải nội dung đã hoàn thành.

## Ý tưởng cũ chưa ưu tiên

6 vũ khí Pistol/SMG/Shotgun…, merge vũ khí, XP chọn stat, synergy, reroll và nhiều nhánh vũ khí không còn là checklist MVP bắt buộc. Chỉ xem xét lại nếu phục vụ hướng đồng hành/nhà chính; không coi các hệ thống đó đã tồn tại.

## Nguyên tắc

- Hoàn thiện một vòng từ nhà chính đến trận và trở về trước khi tăng content.
- Data-driven cho nội dung; tách cấu hình khỏi save của Player.
- Phân biệt có code, đã nối scene và đã kiểm thử.
- Pooling giảm tạo/hủy trong combat; vẫn cần đo CPU, bộ nhớ và FPS trên thiết bị thật.
- Chiến đấu dễ đọc, nâng cấp có tác dụng rõ, vẫn coi trọng việc né đòn.
