// Deterministic Unity serialization fallback; never modifies combat assets.
import fs from 'node:fs';
import path from 'node:path';
import crypto from 'node:crypto';
const root=process.cwd(), R='Assets/Art/Home';
const read=p=>fs.readFileSync(p,'utf8').replace(/\r/g,'');
const write=(p,s)=>{fs.mkdirSync(path.dirname(p),{recursive:true});fs.writeFileSync(p,s);};
const guid=s=>crypto.createHash('md5').update('Shooter/HomeMap/v1/'+s).digest('hex');
function meta(p,type='DefaultImporter',extra='') {
 const old=fs.existsSync(p+'.meta')?read(p+'.meta').match(/^guid: (\w+)/m):null;
 const id=old?.[1]??guid(p);
 write(p+'.meta',`fileFormatVersion: 2\nguid: ${id}\n${type}:\n  externalObjects: {}\n${extra}  userData: \n  assetBundleName: \n  assetBundleVariant: \n`);return id;
}
const header='%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n';
const shared='  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n';
const vec=(v)=>`{x: ${v[0]}, y: ${v[1]}, z: ${v[2]??0}}`;
function matrix(s=1,x=0,y=0,indent=4){let out='';for(let r=0;r<4;r++)for(let c=0;c<4;c++)out+=' '.repeat(indent)+`e${r}${c}: ${r===c?(r<3?s:1):(c===3?(r===0?x:r===1?y:0):0)}\n`;return out;}
const matPath=R+'/Materials/HomeUnlit.mat';
write(matPath,read('Library/PackageCache/com.unity.render-pipelines.universal@4d008a28541a/Runtime/Materials/Sprite-Unlit-Default.mat').replace('m_Name: Sprite-Unlit-Default','m_Name: HomeUnlit'));
const matGuid=meta(matPath,'NativeFormatImporter','  mainObjectFileID: 2100000\n');
let textureTemplate=read('Assets/Sprites/Ground/Tiles.png.meta');
let tileTemplate=read('Assets/Sprites/Ground/TilePalette/Tiles_0.asset');
let paletteTemplate=read('Assets/Sprites/Ground/TilePalette/New Tile Palette.prefab');
let rendererTemplate=paletteTemplate.match(/--- !u!483693784[^]*?(?=--- !u!)/)[0];
let sprites={};
function dims(file){const b=fs.readFileSync(file);return [b.readUInt32BE(16),b.readUInt32BE(20)];}
function texture(file,rects){
 const old=fs.existsSync(file+'.meta')?read(file+'.meta').match(/^guid: (\w+)/m):null;
 const id=old?.[1]??guid(file);
 let m=textureTemplate.replace(/^guid: \w+/m,'guid: '+id)
 .replace(/  internalIDToNameTable:[^]*?  externalObjects:/,'  internalIDToNameTable:\n'+rects.map((s,i)=>`  - first:\n      213: ${21300000+i*2}\n    second: ${s.name}\n`).join('')+'  externalObjects:')
 .replace(/maxTextureSize: 2048/g,'maxTextureSize: 4096').replace(/filterMode: 1/g,'filterMode: 0').replace(/textureCompression: 1/g,'textureCompression: 0')
 .replace(/spritePixelsToUnits: 100/,'spritePixelsToUnits: 64').replace(/spriteGenerateFallbackPhysicsShape: 1/,'spriteGenerateFallbackPhysicsShape: 0');
 let sheet='  spriteSheet:\n    serializedVersion: 2\n    sprites:\n';
 rects.forEach((s,i)=>{
  s.id=21300000+i*2;s.texGuid=id;s.file=file;s.tileGuid=guid(R+'/Tiles/'+s.group+'/'+s.name+'.asset');
  const [x,y,w,h]=s.rect;
  sheet+=`    - serializedVersion: 2\n      name: ${s.name}\n      rect:\n        serializedVersion: 2\n        x: ${x}\n        y: ${y}\n        width: ${w}\n        height: ${h}\n      alignment: 9\n      pivot: {x: ${s.pivot[0]}, y: ${s.pivot[1]}}\n      border: {x: 0, y: 0, z: 0, w: 0}\n      customData: \n      outline: []\n      physicsShape: []\n      tessellationDetail: 0\n      bones: []\n      spriteID: ${guid(file+'/'+s.name)}\n      internalID: ${s.id}\n      vertices: []\n      indices: \n      edges: []\n      weights: []\n`;
  sprites[s.name]=s;
 });
 sheet+='    outline: []\n    customData: \n    physicsShape: []\n    bones: []\n    spriteID: '+guid(file+'/sheet')+'\n    internalID: 0\n    vertices: []\n    indices: \n    edges: []\n    weights: []\n    secondaryTextures: []\n    nameFileIdTable:\n'+rects.map(s=>`      ${s.name}: ${s.id}\n`).join('');
 m=m.replace(/  spriteSheet:[^]*?  mipmapLimitGroupName:/,sheet+'  mipmapLimitGroupName:');write(file+'.meta',m);
 for(const s of rects){
  const p=R+'/Tiles/'+s.group+'/'+s.name+'.asset';
  let t=tileTemplate.replace(/  m_Name: .*/,'  m_Name: '+s.name).replace(/  m_Sprite: .*/,`  m_Sprite: {fileID: ${s.id}, guid: ${s.texGuid}, type: 3}`).replace('m_ColliderType: 1','m_ColliderType: 0');
  write(p,t);s.tileGuid=meta(p,'NativeFormatImporter','  mainObjectFileID: 11400000\n');
 }
 return rects;
}
function grid(file,group,size){const [w,h]=dims(file),arr=[];for(let y=0;y<h;y+=size)for(let x=0;x<w;x+=size)arr.push({name:`${group}_${String(x/size).padStart(2,'0')}_${String(y/size).padStart(2,'0')}`,group,rect:[x,y,Math.min(size,w-x),Math.min(size,h-y)],pivot:[0,0],cell:[x/size*(size/64),y/size*(size/64)]});return texture(file,arr);}
const terrain=grid(R+'/Source/HomeTerrain.png','Terrain',64);
const sky=grid(R+'/Source/HomeSky.png','Sky',256);
const ground=grid(R+'/HomeGround64.png','Ground',64);
const names=['TreeLarge','TreeSmall','FlowerBush','MossRock','CareHouse','AncientPortal','RopeBridge','Well','MarketStall','Crate','Barrel','Fence','Sign','Lantern','Flowers','SmallIsland'];
const rectangles=[[0,0,340,374],[345,30,300,344],[647,85,315,289],[972,90,282,280],[0,370,342,343],[344,375,302,341],[648,465,345,244],[995,408,259,302],[0,710,340,276],[347,734,292,252],[646,737,290,249],[936,735,318,251],[40,991,283,263],[361,987,265,267],[655,1005,305,249],[980,990,274,264]];
const [pw,ph]=dims(R+'/Source/HomeProps.png');
const props=texture(R+'/Source/HomeProps.png',names.map((name,i)=>{const [x,y,w,h]=rectangles[i];return {name,group:'Props',rect:[Math.round(x*pw/1254),Math.round(ph-(y+h)*ph/1254),Math.round(w*pw/1254),Math.round(h*ph/1254)],pivot:[.5,.05],cell:[i%4*7,-Math.floor(i/4)*7]};}));
let nextId=700000;
class SceneWriter {
 constructor(){this.nodes=[];this.extra=[];}
 node(name,parent=null,position=[0,0,0],scale=[1,1,1]){const n={id:++nextId,tr:++nextId,name,parent,position,scale,children:[],components:[]};if(parent)parent.children.push(n);this.nodes.push(n);return n;}
 comp(n,type,name,body){const id=++nextId;n.components.push(id);this.extra.push(`--- !u!${type} &${id}\n${name}:\n${shared}  m_GameObject: {fileID: ${n.id}}\n${body}`);return id;}
 grid(n){this.comp(n,156049354,'Grid','  m_Enabled: 1\n  m_CellSize: {x: 1, y: 1, z: 0}\n  m_CellGap: {x: 0, y: 0, z: 0}\n  m_CellLayout: 0\n  m_CellSwizzle: 0\n');}
 map(name,parent,entries,order=0,position=[0,0,0],scale=[1,1,1]){
  const n=this.node(name,parent,position,scale),len=entries.length;
  let body='  m_Enabled: 1\n  m_Tiles:'+(len?'\n':' []\n');
  entries.forEach((e,i)=>{body+=`  - first: ${vec(e.cell)}\n    second:\n      serializedVersion: 2\n      m_TileIndex: ${i}\n      m_TileSpriteIndex: ${i}\n      m_TileMatrixIndex: ${i}\n      m_TileColorIndex: 0\n      m_TileObjectToInstantiateIndex: 65535\n      dummyAlignment: 0\n      m_AllTileFlags: 1073741824\n`;});
  body+='  m_AnimatedTiles: {}\n  m_TileAssetArray:'+(len?'\n':' []\n');
  entries.forEach(e=>body+=`  - serializedVersion: 2\n    m_RefCount: 1\n    m_Data: {fileID: 11400000, guid: ${e.s.tileGuid}, type: 2}\n`);
  body+='  m_TileSpriteArray:'+(len?'\n':' []\n');entries.forEach(e=>body+=`  - serializedVersion: 2\n    m_RefCount: 1\n    m_Data: {fileID: ${e.s.id}, guid: ${e.s.texGuid}, type: 3}\n`);
  body+='  m_TileMatrixArray:'+(len?'\n':' []\n');entries.forEach(e=>body+='  - serializedVersion: 2\n    m_RefCount: 1\n    m_Data:\n'+matrix(e.scale??1,e.offset?.[0]??0,e.offset?.[1]??0,6));
  body+=`  m_TileColorArray:\n  - serializedVersion: 2\n    m_RefCount: ${Math.max(1,len)}\n    m_Data: {r: 1, g: 1, b: 1, a: 1}\n  m_TileObjectToInstantiateArray: []\n  m_AnimationFrameRate: 1\n  m_Color: {r: 1, g: 1, b: 1, a: 1}\n`;
  const xs=entries.map(e=>e.cell[0]),ys=entries.map(e=>e.cell[1]);const x=len?Math.min(...xs):0,y=len?Math.min(...ys):0;
  body+=`  m_Origin: ${vec([x,y])}\n  m_Size: ${vec([len?Math.max(...xs)-x+1:0,len?Math.max(...ys)-y+1:0,1])}\n  m_TileAnchor: {x: 0, y: 0, z: 0}\n  m_TileOrientation: 0\n  m_TileOrientationMatrix:\n`+matrix();
  this.comp(n,1839735485,'Tilemap',body);
  const renderId=++nextId;n.components.push(renderId);
  this.extra.push(rendererTemplate.replace(/&5190690407467543566/,'&'+renderId).replace('m_GameObject: {fileID: 1050612888050381571}',`m_GameObject: {fileID: ${n.id}}`).replace('a97c105638bdf8b4a8650670310a4cd3',matGuid).replace('m_SortingOrder: 0','m_SortingOrder: '+order).replace('m_Mode: 0','m_Mode: 1'));
  return n;
 }
 serialize(){return this.nodes.map(n=>`--- !u!1 &${n.id}\nGameObject:\n${shared}  serializedVersion: 6\n  m_Component:\n  - component: {fileID: ${n.tr}}\n${n.components.map(i=>`  - component: {fileID: ${i}}\n`).join('')}  m_Layer: 0\n  m_Name: ${n.name}\n  m_TagString: Untagged\n  m_Icon: {fileID: 0}\n  m_NavMeshLayer: 0\n  m_StaticEditorFlags: 0\n  m_IsActive: 1\n--- !u!4 &${n.tr}\nTransform:\n${shared}  m_GameObject: {fileID: ${n.id}}\n  serializedVersion: 2\n  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}\n  m_LocalPosition: ${vec(n.position)}\n  m_LocalScale: ${vec(n.scale)}\n  m_ConstrainProportionsScale: 0\n  m_Children:${n.children.length?'\n'+n.children.map(c=>`  - {fileID: ${c.tr}}\n`).join(''):' []\n'}  m_Father: {fileID: ${n.parent?.tr??0}}\n  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}\n`).join('')+this.extra.join('');}
}
const entries=arr=>arr.map(s=>({s,cell:s.cell}));
for(const [name,arr] of [['Home_01_TerrainLayout',terrain],['Home_02_GroundPaths',ground],['Home_03_Props',props],['Home_04_SkyClouds',sky]]){
 const w=new SceneWriter(),r=w.node(name);w.grid(r);w.map('Layer1',r,entries(arr));
 const p=R+'/Palettes/'+name+'.prefab';
 const settings=paletteTemplate.match(/--- !u!114 &4468971476010060478[^]*/)[0].replace('cellSizing: 0','cellSizing: 1');
 write(p,header+w.serialize()+settings);meta(p,'PrefabImporter');
}
// Read exactly the authored placements from the Unity builder so both paths agree.
const source=read('Assets/Editor/Home/HomeMapBuilder.cs');
const placements=[];
for(const m of source.matchAll(/Place\((bridges|decor|distant), names\["(\w+)"\], ([^,]+), ([^,]+), ([^)]+)\)/g))placements.push({layer:m[1],name:m[2],x:parseFloat(m[3]),y:parseFloat(m[4]),width:parseFloat(m[5])});
function propEntries(layer){const occupied=new Set();return placements.filter(p=>p.layer===layer).map(p=>{let cx=Math.floor(p.x),cy=Math.floor(p.y);while(occupied.has(cx+','+cy))cx++;occupied.add(cx+','+cy);const s=sprites[p.name];return {s,cell:[cx,cy],scale:p.width/(s.rect[2]/64),offset:[p.x-cx,p.y-cy]};});}
const w=new SceneWriter(),r=w.node('HomeMap_Generated'),g=w.node('Grid - paint using Home palettes',r);w.grid(g);
w.map('00_SkyClouds',g,entries(sky),-100,[-21,-10,0],[1.62,1.62,1.62]);
w.map('05_DistantIslands',g,propEntries('distant'),-50);
w.map('10_IslandTerrain',g,entries(terrain),-20,[-17,-5.5,0]);
w.map('20_GroundEdits - optional grass and paths',g,[],-10);
w.map('30_Bridges',g,propEntries('bridges'),0);
// Stable per-placement ordering avoids relying on editor-specific tilemap sorting axes.
for(const [i,e] of propEntries('decor').sort((a,b)=>(b.cell[1]+b.offset[1])-(a.cell[1]+a.offset[1])).entries())w.map('40_Prop_'+String(i).padStart(2,'0')+'_'+e.s.name,g,[e],10+i);
const a=w.node('GameplayAnchors - positions only',r);
for(const [name,p] of [['PlayerSpawn',[-.5,.3]],['PortalInteraction',[-12.3,.15]],['CareHouseInteraction',[12.5,.5]],['MarketInteraction',[13.5,-.9]]])w.node(name,a,p);
const wa=w.node('SoulWanderPoints',a);for(let i=0;i<6;i++)w.node('Point_'+i,wa,[-4+i*1.4,i%2===0?.3:1.5]);
const boundary=w.node('WalkBoundary - edit points for gameplay',r);
const pts=[[-16,.4],[-15,2.4],[-13,3.5],[-10,3],[-8.4,1],[-6.8,1],[-5,2.1],[-3.4,3.8],[0,3],[3.3,1.7],[5,.8],[6.9,.8],[7.6,2.5],[10,3.5],[14.2,3.4],[16,1.2],[15.7,-1],[13,-2],[9,-1.5],[7.2,-.5],[5,-.3],[3,-1.7],[0,-2.8],[-3,-2],[-5.7,-1.1],[-6.8,.1],[-8.4,.1],[-10,-1.6],[-14,-1.3],[-16,.4]];
w.comp(boundary,68,'EdgeCollider2D',`  m_Enabled: 1\n  serializedVersion: 3\n  m_Density: 1\n  m_Material: {fileID: 0}\n  m_IsTrigger: 0\n  m_UsedByEffector: 0\n  m_Offset: {x: 0, y: 0}\n  m_EdgeRadius: 0.05\n  m_Points:\n${pts.map(p=>`  - {x: ${p[0]}, y: ${p[1]}}\n`).join('')}  m_UseAdjacentStartPoint: 0\n  m_UseAdjacentEndPoint: 0\n`);
const prefab='Assets/Prefabs/Home/MAP_HomeFloatingIslands.prefab';write(prefab,header+w.serialize());meta(prefab,'PrefabImporter');
let scene=read('Documentation/HomeMap/HomeScene.before-map.unity.txt');
scene=scene.replace(/--- !u!1660057539[^]*/,'').replace('orthographic size: 5','orthographic size: 11').replace('m_LocalPosition: {x: 0, y: 0, z: -10}','m_LocalPosition: {x: 0, y: 1.8, z: -30}');
write('Assets/Scenes/HomeScene.unity',scene+w.serialize()+`--- !u!1660057539 &9223372036854775807\nSceneRoots:\n  m_ObjectHideFlags: 0\n  m_Roots:\n  - {fileID: 1650220043}\n  - {fileID: ${r.tr}}\n`);
write('Documentation/HomeMap/layout.json',JSON.stringify({sprites,placements,terrainSize:dims(R+'/Source/HomeTerrain.png'),skySize:dims(R+'/Source/HomeSky.png'),camera:{x:0,y:1.8,size:11}},null,2));
// Create only missing metadata for our own directories/scripts.
function folder(p){if(fs.existsSync(p+'.meta'))return;write(p+'.meta',`fileFormatVersion: 2\nguid: ${guid(p)}\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n`);}
function walk(p){folder(p);for(const e of fs.readdirSync(p,{withFileTypes:true})){const f=p+'/'+e.name;if(e.isDirectory())walk(f);}}
walk(R);folder('Assets/Prefabs/Home');folder('Assets/Editor');folder('Assets/Editor/Home');
if(!fs.existsSync('Assets/Editor/Home/HomeMapBuilder.cs.meta'))meta('Assets/Editor/Home/HomeMapBuilder.cs','MonoImporter','  serializedVersion: 2\n  defaultReferences: []\n  executionOrder: 0\n  icon: {instanceID: 0}\n');
const count=terrain.length+sky.length+ground.length+props.length;
write('Documentation/HomeMap/validation.txt',`Generated ${count} Tile assets; 4 palettes; ${placements.length} prop placements.\nNative Unity YAML generated from project templates.\nUnity batch compilation blocked by Windows Application Control (Bee.DotNet.dll).\nUnity import / Play Mode remains unverified.\n`);
console.log({tiles:count,terrain:terrain.length,sky:sky.length,ground:ground.length,props:props.length,placements:placements.length});
