param([switch]$GroundOnly)
Add-Type -AssemblyName System.Drawing
$artRoot = Join-Path (Get-Location) 'Assets/Art/Home'
if ($GroundOnly) {
    $source = [System.Drawing.Bitmap]::new((Join-Path $artRoot 'Source/HomeGround.png'))
    $out = [System.Drawing.Bitmap]::new(256,256)
    for ($y=0;$y -lt 256;$y++) {
        for ($x=0;$x -lt 256;$x++) {
            $col=[math]::Floor($x/64); $row=[math]::Floor($y/64)
            $cw=$source.Width/4; $ch=$source.Height/4
            $sx=[int][math]::Floor($col*$cw+3+(($x%64)+0.5)/64*($cw-6))
            $sy=[int][math]::Floor($row*$ch+3+(($y%64)+0.5)/64*($ch-6))
            $out.SetPixel($x,$y,$source.GetPixel($sx,$sy))
        }
    }
    $out.Save((Join-Path $artRoot 'HomeGround64.png'),[System.Drawing.Imaging.ImageFormat]::Png)
    $out.Dispose();$source.Dispose()
    exit
}
$layout = Get-Content Documentation/HomeMap/layout.json -Raw | ConvertFrom-Json
$canvas = [System.Drawing.Bitmap]::new(1920,1080)
$g=[System.Drawing.Graphics]::FromImage($canvas)
$g.Clear([System.Drawing.Color]::CornflowerBlue)
$g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::NearestNeighbor
$g.PixelOffsetMode=[System.Drawing.Drawing2D.PixelOffsetMode]::Half
$ppu=1080/22
function Draw-World($bmp,$sourceRect,$left,$bottom,$width,$height) {
    $px=960+$left*$ppu
    $py=540-(($bottom+$height)-1.8)*$ppu
    $dest=[System.Drawing.RectangleF]::new([float]$px,[float]$py,[float]($width*$ppu),[float]($height*$ppu))
    $g.DrawImage($bmp,$dest,$sourceRect,[System.Drawing.GraphicsUnit]::Pixel)
}
$sky=[System.Drawing.Bitmap]::new((Join-Path $artRoot 'Source/HomeSky.png'))
Draw-World $sky ([System.Drawing.RectangleF]::new(0,0,$sky.Width,$sky.Height)) -21 -10 ($sky.Width/64*1.62) ($sky.Height/64*1.62)
$atlas=[System.Drawing.Bitmap]::new((Join-Path $artRoot 'Source/HomeProps.png'))
function Draw-Prop($p) {
    $s=$layout.sprites.($p.name)
    $w=[double]$p.width;$h=$w*$s.rect[3]/$s.rect[2]
    $src=[System.Drawing.RectangleF]::new($s.rect[0],($atlas.Height-$s.rect[1]-$s.rect[3]),$s.rect[2],$s.rect[3])
    Draw-World $atlas $src ($p.x-$w*0.5) ($p.y-$h*0.05) $w $h
}
foreach($p in ($layout.placements | Where-Object layer -eq 'distant')){Draw-Prop $p}
$terrain=[System.Drawing.Bitmap]::new((Join-Path $artRoot 'Source/HomeTerrain.png'))
Draw-World $terrain ([System.Drawing.RectangleF]::new(0,0,$terrain.Width,$terrain.Height)) -17 -5.5 ($terrain.Width/64) ($terrain.Height/64)
foreach($p in ($layout.placements | Where-Object layer -eq 'bridges')){Draw-Prop $p}
foreach($p in ($layout.placements | Where-Object layer -eq 'decor' | Sort-Object y -Descending)){Draw-Prop $p}
$canvas.Save((Join-Path (Get-Location) 'Documentation/HomeMap/HomeScene-layout-preview.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$g.Dispose();$canvas.Dispose();$atlas.Dispose();$terrain.Dispose();$sky.Dispose()
