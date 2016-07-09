/* The size of these files were reduced. The following function fixes all references. */
var $customMSCore = JSIL.GetAssembly("mscorlib");
var $customSys = JSIL.GetAssembly("System");
var $customSysConf = JSIL.GetAssembly("System.Configuration");
var $customSysCore = JSIL.GetAssembly("System.Core");
var $customSysNum = JSIL.GetAssembly("System.Numerics");
var $customSysXml = JSIL.GetAssembly("System.Xml");
var $customSysSec = JSIL.GetAssembly("System.Security");

if (typeof (contentManifest) !== "object") { contentManifest = {}; };
contentManifest["Fusee.FuFiCycles.Web.contentproj"] = [
    ["Script",	"Fusee.Base.Core.Ext.js",	{  "sizeBytes": 1273 }],
    ["Script",	"Fusee.Base.Imp.Web.Ext.js",	{  "sizeBytes": 9244 }],
    ["Script",	"opentype.js",	{  "sizeBytes": 166330 }],
    ["Script",	"Fusee.Xene.Ext.js",	{  "sizeBytes": 1441 }],
    ["Script",	"Fusee.Xirkit.Ext.js",	{  "sizeBytes": 44215 }],
    ["Script",	"Fusee.Engine.Imp.Graphics.Web.Ext.js",	{  "sizeBytes": 105980 }],
    ["Script",	"SystemExternals.js",	{  "sizeBytes": 11976 }],
    ["File",	"Assets/Cycle.fus",	{  "sizeBytes": 568839 }],
    ["File",	"Assets/Land.fus",	{  "sizeBytes": 1056 }],
    ["File",	"Assets/Land_Lines.fus",	{  "sizeBytes": 2196499 }],
    ["File",	"Assets/PixelShader2.frag",	{  "sizeBytes": 530 }],
    ["Image",	"Assets/Sphere.jpg",	{  "sizeBytes": 26951 }],
    ["Image",	"Assets/Styles/loading_rocket.png",	{  "sizeBytes": 10975 }],
    ["File",	"Assets/VertexShader2.vert",	{  "sizeBytes": 393 }],
    ["File",	"Assets/Wall.fus",	{  "sizeBytes": 965 }],
    ];