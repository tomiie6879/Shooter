---
name: game-image-generator
description: Generate consistent 2D pixel-art characters, animations, sprite sheets, weapons, props, enemies, and ground tiles for the Shooter Unity project. Use when the user requests new or edited pixel-art game assets.
---
# SKILL.md — 2D Pixel Art Game Asset Generator for Unity

## Purpose

Use this skill to generate consistent, production-ready 2D pixel-art assets for a top-down action roguelite / survivor game in Unity.

Primary targets:
- Master character reference
- 4-direction character references
- Idle animation
- Run animation
- Attack animation
- Hit animation
- Die animation
- Weapons
- Props / decorations
- Enemies
- Ground tilesets
- Unity-ready sprite sheets

Highest priorities:
1. Character consistency
2. True pixel-art appearance
3. Stable alignment / pivot
4. Correct sprite-sheet layout
5. Gameplay readability
6. Unity compatibility

---

# 1. Global Pixel-Art Standard

All character assets use TRUE pixel art.

Visual detail target:
- approximately 48×48 to 64×64 meaningful pixel-art detail

Animation cell:
- 256×256 pixels per frame

Important distinction:
- 256×256 is the animation CELL size
- the character must still look like a native ~64×64 pixel-art sprite
- do NOT add 256×256 worth of tiny detail

Use:
- crisp hard pixel edges
- visible square pixel clusters
- deliberate blocky shapes
- limited palette
- controlled pixel shading
- simple highlights
- simple shadows
- consistent outline thickness
- consistent pixel density

Never use:
- anti-aliasing
- blurry edges
- smooth gradients
- painterly rendering
- vector-like smooth curves
- realistic 3D shading
- high-resolution illustration detail
- sub-pixel detail
- “pixel filter applied to HD art” appearance

---

# 2. Master Character Reference Rules

When a character reference image is provided, treat it as the EXACT MASTER CHARACTER REFERENCE.

Do not redesign the character unless explicitly requested.

Preserve exactly:
- head shape
- head size
- body proportions
- body size
- colors
- palette
- face
- eyes
- eye size
- mouth
- teeth
- spikes
- spike count
- spike shape
- tail
- tail thickness
- arms
- legs
- feet
- claws
- markings
- outline color
- outline thickness
- shading style
- pixel density

Only the requested pose / animation motion may change.

If a direction-specific reference is available, use that direction-specific image instead of a 4-direction sheet.

Preferred workflow:
MASTER SOUTH
→ NORTH / EAST / SOUTH / WEST neutral references
→ crop / extract each direction
→ animate each direction separately

---

# 3. Direction Rules

Standard directions:
- North = facing away from camera
- East = facing right
- South = facing toward camera
- West = facing left

The requested direction must remain unchanged throughout an animation.

Do not:
- rotate the character mid-animation
- change camera angle
- change perspective
- turn South into East/West/North
- introduce diagonal facing unless explicitly requested

Optimization:
- East may be reused as West using Flip X in Unity if the design is sufficiently symmetrical
- do NOT flip if weapon handedness, tail design, logos, scars, clothing, or asymmetric details make the result incorrect

---

# 4. Unity Character Cell Standard

Default frame/cell:
- 256×256 pixels

Target pivot:
- Bottom Center

Rules:
- same anchor across all frames
- same visual scale across all frames
- same pixel density across all frames
- same intended ground plane
- no accidental zoom
- no horizontal drift
- no vertical drift unless caused by intentional pose motion
- no frame-to-frame recentering

Transparent background for:
- characters
- enemies
- weapons
- props
- effects

No:
- checkerboard artwork
- floor
- shadow unless explicitly requested
- text
- UI
- frame labels
- borders

---

# 5. Animation Sheet Dimension Formula

For character animations:

frame size:
256×256

final width:
FRAME_COUNT × 256

final height:
256

Examples:
- 4 frames → 1024×256
- 6 frames → 1536×256
- 8 frames → 2048×256
- 10 frames → 2560×256

Layout:
FRAME_COUNT columns × 1 row

No:
- gaps between cells
- padding between cells
- separator lines
- borders

Each pose must remain completely inside its own 256×256 cell.

---

# 6. Idle Animation Standard

Default:
- 6 frames
- 6 columns × 1 row
- 1536×256

Animation style:
- very subtle breathing
- tiny head movement
- tiny torso movement
- optional blink
- optional tiny tail movement
- calm loop

Do not:
- walk
- step
- jump
- rotate
- bounce dramatically
- move across the cell

Loop:
- Frame 6 must transition naturally back to Frame 1
- no visible jump

Feet:
- ground contact remains locked
- breathing should happen mostly through torso / head / shoulders

Recommended pose flow:
1. neutral
2. slight inhale
3. near-max inhale
4. blink / transition
5. return
6. near-neutral

---

# 7. Run Animation Standard

Default:
- 8 frames
- 8 columns × 1 row
- 2048×256

Run must be IN PLACE.

Allowed:
- alternating leg motion
- stronger leg extension
- arm swing
- subtle forward body lean
- rhythmic head movement
- subtle body bob
- subtle tail counter-motion

Do not:
- move forward across the sheet
- move world position
- change direction
- alter character scale

Ground rule:
- feet may lift during running
- whenever a foot contacts the ground, it must touch the same virtual baseline

Loop:
- Frame 8 must transition naturally to Frame 1

Suggested cycle:
1. first foot contact
2. compression
3. passing pose
4. transition / small airborne pose
5. opposite foot contact
6. compression
7. opposite passing pose
8. transition back to frame 1

---

# 8. Attack Animation Standard

Default:
- 6 frames
- 6 columns × 1 row
- 1536×256

For this project, default Attack is BODY-ONLY.

Do NOT render weapons inside the character sprite unless the user explicitly requests baked-in weapons.

Reason:
- weapons should normally be separate sprites / GameObjects in Unity
- allows weapon swapping
- allows independent rotation
- allows projectile logic
- reduces re-animation requirements

Attack progression:
1. ready
2. anticipation
3. attack / fire timing
4. recoil / follow-through
5. recovery
6. ready

Attack should:
- be fast
- readable
- work with external weapon attachment
- keep feet planted
- keep Bottom Center stable

Hands:
- remain visible
- do not disappear
- do not swap left/right randomly
- preserve arm length
- create a consistent weapon attachment area

Recommended Unity attack event:
- around Frame 3

No:
- muzzle flash
- bullets
- projectile
- weapon
- hit VFX
unless explicitly requested

---

# 9. Hit Animation Standard

Default:
- 4 frames
- 4 columns × 1 row
- 1024×256

Hit should be:
- quick
- readable
- compact
- non-gory

Suggested flow:
1. normal
2. impact
3. recoil
4. recovery

Allowed:
- small torso recoil
- subtle head reaction
- brief eye close / widen
- small tail reaction

Do not:
- step
- slide
- jump
- rotate
- move across cell
- add blood
- add gore
- add damage numbers
- add hit particles

Feet:
- remain planted
- same ground baseline

---

# 10. Die Animation Standard

Default:
- 8 frames
- 8 columns × 1 row
- 2048×256

Die animation is NOT looping.

Suggested flow:
1. normal
2. final hit reaction
3. lose balance
4. falling
5. near ground
6. landing
7. settling
8. final motionless dead pose

Important:
- keep the SAME 256×256 cell and SAME Bottom Center pivot across all frames
- do NOT recenter each frame around the changing body silhouette
- the body may move relative to the fixed anchor
- final body must remain fully inside the cell
- final pose rests on the same virtual ground plane

Frame 8:
- must be suitable to hold indefinitely
- do not transition back to Frame 1

No:
- blood
- gore
- dismemberment
- disturbing injury
- projectiles
- explosion
- damage text

---

# 11. Default Character Animation Set

Default project animation set:

Idle:
- 6 frames
- 1536×256

Run:
- 8 frames
- 2048×256

Attack:
- 6 frames
- 1536×256

Hit:
- 4 frames
- 1024×256

Die:
- 8 frames
- 2048×256

Walk:
- optional
- not required for a Brotato-style movement system unless explicitly requested

If Walk is requested:
- default 8 frames
- same rules as Run but slower, softer motion

---

# 12. Character Prompt Building Rules

When the user gives a short command such as:

"Idle South 6"

Expand it internally to:
- use provided South-facing reference as EXACT master reference
- 6-frame idle
- 6×1 row
- 256×256 each
- 1536×256 final logical sheet
- ~64×64 visual-detail level
- transparent background
- Bottom Center pivot
- same scale
- same baseline
- same pixel density
- seamless loop

When user says:

"Run North 8"

Expand to:
- North-facing exact reference
- 8-frame run-in-place
- seamless loop
- 2048×256
- stable ground plane
- Bottom Center pivot
- 64×64 visual detail
- transparent background

When user says:

"Attack East 6"

Expand to:
- East-facing exact reference
- 6-frame body-only attack
- no weapon
- weapon attachment area readable
- Frame 3 attack event timing
- 1536×256

When user says:

"Hit South 4"

Expand to:
- South-facing exact reference
- 4-frame hit
- feet locked
- no gore
- 1024×256

When user says:

"Die North 8"

Expand to:
- North-facing exact reference
- non-looping
- 8 frames
- fixed Bottom Center world anchor
- final dead pose stable
- 2048×256

---

# 13. Prompt Conflict Resolution

If the user gives contradictory values such as:
- "create a 6-frame animation"
- later "exactly 8 frames"

Use the final explicit instruction if clearly intentional.

If ambiguity materially changes the output and cannot be resolved safely:
- ask one concise clarification before generation

Never silently generate a different frame count from the final explicit instruction.

---

# 14. Character Quality Validation

Before accepting any character sprite sheet, verify:

[ ] correct frame count
[ ] correct direction
[ ] correct layout
[ ] correct logical sheet dimensions
[ ] same character identity
[ ] same head size
[ ] same body proportions
[ ] same colors
[ ] same spikes
[ ] same tail
[ ] same markings
[ ] same pixel density
[ ] same outline thickness
[ ] ~64×64 visual-detail level
[ ] 256×256 cell per frame
[ ] same Bottom Center anchor
[ ] no accidental horizontal drift
[ ] no accidental vertical drift
[ ] no accidental scale changes
[ ] no cropped limbs / head / tail / spikes
[ ] transparent background
[ ] no text
[ ] no borders
[ ] suitable for Unity slicing

Animation-specific:
- Idle → loops cleanly
- Run → runs in place and loops cleanly
- Attack → clear attack timing and no weapon by default
- Hit → quick recovery and planted feet
- Die → does not loop and final pose is stable

---

# 15. Master Character Creation Standard

When creating a new main character from scratch:

Default:
- cute stylized baby dinosaur
- South-facing
- neutral standing pose
- full body
- centered
- simple silhouette
- animation-friendly anatomy
- no weapon
- transparent background
- no shadow
- no UI
- no text

Art detail:
- visually equivalent to a 48×48–64×64 native pixel sprite
- placed inside a 256×256 transparent cell
- do not use the larger cell to add excessive detail

Prefer:
- large readable eyes
- simple mouth
- simple spikes
- simple tail
- large color regions
- few markings
- minimal tiny decoration

Avoid:
- excessive skin texture
- tiny scales
- complex gradients
- too many spots
- complicated accessories

---

# 16. 4-Direction Reference Standard

When creating N/E/S/W references:

Create neutral standing views:
- North
- East
- South
- West

Keep:
- same height
- same scale
- same palette
- same style
- same outline
- same pixel density
- feet on same baseline
- equal spacing

Do not:
- create action poses
- create walking poses
- redesign the character

Preferred:
- separate each direction into its own reference image before generating animations

---

# 17. Weapon Asset Standard

Weapons should normally be separate assets.

Use:
- same pixel density as game
- transparent background
- readable silhouette
- limited palette
- no hand unless requested
- no environment
- no UI
- no shadow unless requested

Weapons may later be:
- positioned at a WeaponPivot
- rotated independently
- flipped if appropriate
- attached near a hand / firing origin

Do not bake weapon into every player animation unless explicitly requested.

---

# 18. Prop / Decoration Standard

Props use transparent background.

Examples:
- cactus
- rock
- bone
- fossil
- bush
- flower
- dry grass
- crate
- tree
- decoration

Props should:
- match environment palette
- match pixel density
- have readable silhouette
- not include a ground rectangle unless explicitly requested

---

# 19. Ground Tileset Standard

Base ground tiles are different from characters and props.

Default ground tile:
- native 64×64 pixels
- fully opaque
- perfect rectangle
- no transparent corners
- no transparent edges
- no rounded tile shape
- no external padding

Default tileset:
- 4 columns × 4 rows
- 16 tiles
- each tile 64×64
- final logical sheet 256×256

Ground is for:
- walkable terrain
- large repeated gameplay arenas

Ground must be:
- low contrast
- visually quiet
- readable
- consistent
- seamless
- non-distracting

Characters / enemies / projectiles must remain visually dominant.

---

# 20. Ground Tile Edge Rules

CRITICAL:

Every base ground tile must fill 100% of its 64×64 cell.

Ground pixels must reach:
- left edge
- right edge
- top edge
- bottom edge

Never create:
- transparent corners
- jagged outer silhouette
- rounded corners
- alpha gaps
- floating terrain islands
- padding
- visible border

Base ground tiles are 100% opaque.

Transparency is for separate props/decorations only.

---

# 21. Ground Seamless Rules

Base terrain tiles must connect naturally.

Avoid:
- obvious seams
- dark border lines
- light border lines
- strong edge transitions
- obvious repeating square pattern
- unique center object on every tile
- symmetrical repeating clusters

When repeated across a large map, the player should not easily see the 64×64 grid.

Recommended visual distribution:
- 70–85% simple ground
- 15–30% subtle variation

---

# 22. Ground Tileset Content Standard

Default 16-tile distribution:

1. clean base
2. base variation A
3. base variation B
4. base variation C
5. subtle darker patches
6. subtle lighter patches
7. tiny pebbles
8. sparse environmental variation
9. subtle dirt variation
10. sparse vegetation
11. subtle cracks
12. another low-contrast variation
13. rare variation A
14. rare variation B
15. rare variation C
16. rare variation D

All 16 must belong to the SAME biome.

Keep identical:
- palette
- pixel density
- lighting
- texture scale
- base terrain color
- visual complexity

Do not create 16 unrelated illustrations.

---

# 23. Ground Prop Separation

Do NOT bake major objects into base ground tiles.

Keep separate as transparent props:
- cactus
- large rocks
- trees
- bushes
- bones
- fossils
- crates
- walls
- fences
- large flowers
- mushrooms
- traps

Base ground:
- sand
- grass
- dirt
- stone
- snow
- lava floor
- swamp floor
- dungeon floor

---

# 24. Ground Prompt Shortcuts

When user says:

"Prehistoric desert tileset"

Expand internally to:
- native 64×64 true pixel art
- 4×4 = 16 tiles
- final logical 256×256
- full rectangular opaque ground
- seamless edges
- warm sand
- tiny pebbles
- subtle dry cracks
- sparse dry grass
- low contrast
- no props baked in
- no transparency in base ground

When user says:

"Grass tileset"

Expand to:
- green grass biome
- 64×64 tiles
- seamless
- low visual noise
- subtle dirt / tiny grass variation
- fully opaque base ground

---

# 25. Unity Import Standard — Character

Recommended:
- Texture Type: Sprite (2D and UI)
- Sprite Mode: Multiple
- Pixels Per Unit: choose consistently project-wide
- Filter Mode: Point (no filter)
- Compression: None

Sprite Editor:
- Slice
- Grid By Cell Size
- 256×256

Pivot:
- Bottom Center

---

# 26. Unity Import Standard — Ground Tiles

Recommended:
- Texture Type: Sprite (2D and UI)
- Sprite Mode: Multiple
- Pixels Per Unit: 64
- Filter Mode: Point (no filter)
- Compression: None

Sprite Editor:
- Slice
- Grid By Cell Size
- 64×64

For base walkable ground:
- Generate Physics Shape: Off unless explicitly needed

---

# 27. Important Image-Generation Limitation

Image models may not perfectly obey exact output dimensions even when the prompt requests:
- 1536×256
- 2048×256
- 256×256
- exact 64×64 cells

Therefore:

Treat prompt dimensions as LAYOUT TARGETS.

For production:
1. generate the animation / tileset
2. verify frame count
3. normalize / crop / pack frames programmatically if needed
4. ensure exact 256×256 character cells or exact 64×64 ground tiles
5. then import into Unity

Never assume the generated file is exact solely because the prompt requested exact dimensions.

---

# 28. Response Behavior

When the user provides a reference image and asks for an image:
- use the image as the exact reference
- infer the requested animation from the command
- apply this skill automatically
- do not add unrelated elements
- do not redesign the character

When the user asks for a prompt:
- provide a production-ready prompt
- include exact frame count
- include logical sprite-sheet dimensions
- include 64×64 visual-detail rule
- include pivot / baseline rules
- include transparent background for character assets
- include animation-specific loop / non-loop rules

When the user asks for tiles:
- use 64×64 native tile standard
- base ground must be fully opaque
- props must be separate transparent assets

---

# 29. Default Quick Commands

Recognize these as valid shorthand:

- Master South
- 4 Direction
- Idle South 6
- Idle North 6
- Idle East 6
- Idle West 6
- Run South 8
- Run North 8
- Run East 8
- Run West 8
- Attack South 6
- Attack North 6
- Attack East 6
- Attack West 6
- Hit South 4
- Hit North 4
- Hit East 4
- Hit West 4
- Die South 8
- Die North 8
- Die East 8
- Die West 8
- Prehistoric desert tileset
- Prehistoric grassland tileset
- Volcanic tileset
- Ice biome tileset
- Swamp tileset
- Dungeon tileset

Expand shorthand using all relevant rules in this skill.

---

# 30. Final Production Checklist

CHARACTER:
[ ] exact reference preserved
[ ] correct direction
[ ] true pixel art
[ ] ~64×64 visual-detail level
[ ] 256×256 frame cell
[ ] stable Bottom Center
[ ] transparent background
[ ] no accidental redesign
[ ] no crop
[ ] correct frame count
[ ] correct animation behavior

GROUND:
[ ] true native 64×64 pixel art
[ ] perfect rectangular tile
[ ] fully opaque
[ ] no transparent corners
[ ] seamless
[ ] low contrast
[ ] consistent biome
[ ] suitable for 64×64 Unity slicing

PROPS:
[ ] transparent background
[ ] isolated sprite
[ ] matching pixel density
[ ] no baked ground unless requested

UNITY:
[ ] correct grid size
[ ] Point filtering
[ ] Compression None
[ ] consistent PPU
[ ] correct pivot
