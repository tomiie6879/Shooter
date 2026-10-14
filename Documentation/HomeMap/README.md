# HomeScene — floating island map

## Mở và chỉnh map

1. Mở `Assets/Scenes/HomeScene.unity`.
2. Mở **Window > 2D > Tile Palette**.
3. Chọn một trong bốn palette `Home_01_TerrainLayout`, `Home_02_GroundPaths`, `Home_03_Props`, `Home_04_SkyClouds`.
4. Trong Hierarchy, chọn Tilemap nằm trong `HomeMap_Generated/Grid - paint using Home palettes` làm Active Tilemap.
5. Dùng Brush/Eraser/Select như Tilemap bình thường. Save scene sau khi chỉnh.

## Các lớp

| Tilemap | Nội dung |
|---|---|
| 00_SkyClouds | Nền trời và mây |
| 05_DistantIslands | Đảo nhỏ ở xa |
| 10_IslandTerrain | Ba đảo lớn, đất/cỏ và vách đá |
| 20_GroundEdits | Lớp trống để vẽ thêm cỏ/đường |
| 30_Bridges | Hai cầu nối đảo |
| 40_TreesBuildingsProps | Cây, cổng đá, nhà mái xanh, giếng, chợ và đồ trang trí |

`Home_01_TerrainLayout` là địa hình minh họa đã cắt thành tile 64 px: chọn/copy cả vùng để di chuyển nguyên một đảo. Các mảnh này không phải RuleTile tự nối khi vẽ tùy ý. `Home_02_GroundPaths` là 16 tile cỏ/đất/cạnh/góc 64 px để chỉnh nhỏ; không có autotiling. Props được slice riêng và cũng có Tile assets. Tile props vẽ mới có kích thước gốc; map dựng sẵn dùng transform riêng theo từng ô.

Ảnh source giữ nguyên tại `Assets/Art/Home/Source`. Ground được chuẩn hóa về 16 ô 64×64 trong `HomeGround64.png`. Texture dùng Point, không nén, không mipmaps; props và đảo có alpha. Đây là art mới dựng theo ảnh tham chiếu, không phải trích xuất pixel chính xác từ ảnh mẫu.

## Gameplay chuẩn bị sẵn

- `GameplayAnchors` chứa PlayerSpawn, PortalInteraction, CareHouseInteraction, MarketInteraction và SoulWanderPoints.
- `WalkBoundary` có EdgeCollider2D bao quanh tuyến đi lại giữa ba đảo; cần kiểm tra/tinh chỉnh với kích thước collider Player khi nối gameplay.
- Chưa có Player điều khiển, AI đi lại, mua đồ, nâng cấp hay chuyển trận trong scene này. Các điểm tương tác chỉ là vị trí để nối hệ thống sau.
- Nhà/cây hiện là tile trang trí, chưa có collider vật cản riêng.
- Không thay đổi Battle_01 hoặc thiết lập build/scene khởi động.

## Dựng lại

Menu **Shooter > Home > Build or refresh floating island map** dựng lại root `HomeMap_Generated`, các palette và prefab `Assets/Prefabs/Home/MAP_HomeFloatingIslands.prefab`. Chỉ dùng khi muốn quay về bố cục gốc: chỉnh sửa thủ công bên trong root sẽ bị thay thế. Công cụ sẽ hỏi trước và yêu cầu xử lý scene chưa lưu.

Scene trước khi dựng được sao lưu tại `Documentation/HomeMap/HomeScene.before-map.unity.txt`. Script dựng chạy trong Editor, không chạy khi chơi game. Preview và kết quả kiểm tra được lưu trong thư mục này.
