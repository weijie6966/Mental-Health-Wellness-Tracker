; ModuleID = 'marshal_methods.armeabi-v7a.ll'
source_filename = "marshal_methods.armeabi-v7a.ll"
target datalayout = "e-m:e-p:32:32-Fi8-i64:64-v128:64:128-a:0:32-n32-S64"
target triple = "armv7-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [354 x ptr] zeroinitializer, align 4

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [702 x i32] [
	i32 2616222, ; 0: System.Net.NetworkInformation.dll => 0x27eb9e => 68
	i32 10166715, ; 1: System.Net.NameResolution.dll => 0x9b21bb => 67
	i32 15721112, ; 2: System.Runtime.Intrinsics.dll => 0xefe298 => 108
	i32 30277329, ; 3: System.Linq.AsyncEnumerable => 0x1cdfed1 => 223
	i32 32687329, ; 4: Xamarin.AndroidX.Lifecycle.Runtime => 0x1f2c4e1 => 266
	i32 34715100, ; 5: Xamarin.Google.Guava.ListenableFuture.dll => 0x211b5dc => 300
	i32 34839235, ; 6: System.IO.FileSystem.DriveInfo => 0x2139ac3 => 48
	i32 39109920, ; 7: Newtonsoft.Json.dll => 0x254c520 => 208
	i32 39485524, ; 8: System.Net.WebSockets.dll => 0x25a8054 => 80
	i32 42639949, ; 9: System.Threading.Thread => 0x28aa24d => 145
	i32 66541672, ; 10: System.Diagnostics.StackTrace => 0x3f75868 => 30
	i32 67008169, ; 11: zh-Hant\Microsoft.Maui.Controls.resources => 0x3fe76a9 => 345
	i32 68219467, ; 12: System.Security.Cryptography.Primitives => 0x410f24b => 124
	i32 72070932, ; 13: Microsoft.Maui.Graphics.dll => 0x44bb714 => 207
	i32 82292897, ; 14: System.Runtime.CompilerServices.VisualC.dll => 0x4e7b0a1 => 102
	i32 101534019, ; 15: Xamarin.AndroidX.SlidingPaneLayout => 0x60d4943 => 284
	i32 117431740, ; 16: System.Runtime.InteropServices => 0x6ffddbc => 107
	i32 120558881, ; 17: Xamarin.AndroidX.SlidingPaneLayout.dll => 0x72f9521 => 284
	i32 122350210, ; 18: System.Threading.Channels.dll => 0x74aea82 => 139
	i32 134690465, ; 19: Xamarin.Kotlin.StdLib.Jdk7.dll => 0x80736a1 => 308
	i32 142721839, ; 20: System.Net.WebHeaderCollection => 0x881c32f => 77
	i32 149972175, ; 21: System.Security.Cryptography.Primitives.dll => 0x8f064cf => 124
	i32 159306688, ; 22: System.ComponentModel.Annotations => 0x97ed3c0 => 13
	i32 165246403, ; 23: Xamarin.AndroidX.Collection.dll => 0x9d975c3 => 240
	i32 176265551, ; 24: System.ServiceProcess => 0xa81994f => 132
	i32 182336117, ; 25: Xamarin.AndroidX.SwipeRefreshLayout.dll => 0xade3a75 => 286
	i32 184328833, ; 26: System.ValueTuple.dll => 0xafca281 => 151
	i32 195452805, ; 27: vi/Microsoft.Maui.Controls.resources.dll => 0xba65f85 => 342
	i32 199333315, ; 28: zh-HK/Microsoft.Maui.Controls.resources.dll => 0xbe195c3 => 343
	i32 205061960, ; 29: System.ComponentModel => 0xc38ff48 => 18
	i32 209399409, ; 30: Xamarin.AndroidX.Browser.dll => 0xc7b2e71 => 238
	i32 220171995, ; 31: System.Diagnostics.Debug => 0xd1f8edb => 26
	i32 230216969, ; 32: Xamarin.AndroidX.Legacy.Support.Core.Utils.dll => 0xdb8d509 => 260
	i32 230752869, ; 33: Microsoft.CSharp.dll => 0xdc10265 => 1
	i32 231409092, ; 34: System.Linq.Parallel => 0xdcb05c4 => 59
	i32 231814094, ; 35: System.Globalization => 0xdd133ce => 42
	i32 246610117, ; 36: System.Reflection.Emit.Lightweight => 0xeb2f8c5 => 91
	i32 261689757, ; 37: Xamarin.AndroidX.ConstraintLayout.dll => 0xf99119d => 243
	i32 276479776, ; 38: System.Threading.Timer.dll => 0x107abf20 => 147
	i32 278686392, ; 39: Xamarin.AndroidX.Lifecycle.LiveData.dll => 0x109c6ab8 => 262
	i32 280482487, ; 40: Xamarin.AndroidX.Interpolator => 0x10b7d2b7 => 259
	i32 280992041, ; 41: cs/Microsoft.Maui.Controls.resources.dll => 0x10bf9929 => 314
	i32 291076382, ; 42: System.IO.Pipes.AccessControl.dll => 0x1159791e => 54
	i32 298918909, ; 43: System.Net.Ping.dll => 0x11d123fd => 69
	i32 317674968, ; 44: vi\Microsoft.Maui.Controls.resources => 0x12ef55d8 => 342
	i32 318968648, ; 45: Xamarin.AndroidX.Activity.dll => 0x13031348 => 229
	i32 321597661, ; 46: System.Numerics => 0x132b30dd => 83
	i32 336156722, ; 47: ja/Microsoft.Maui.Controls.resources.dll => 0x14095832 => 327
	i32 342366114, ; 48: Xamarin.AndroidX.Lifecycle.Common => 0x146817a2 => 261
	i32 347068432, ; 49: SQLitePCLRaw.lib.e_sqlite3.android.dll => 0x14afd810 => 220
	i32 356389973, ; 50: it/Microsoft.Maui.Controls.resources.dll => 0x153e1455 => 326
	i32 360082299, ; 51: System.ServiceModel.Web => 0x15766b7b => 131
	i32 364956269, ; 52: Grpc.Net.Common => 0x15c0ca6d => 191
	i32 367780167, ; 53: System.IO.Pipes => 0x15ebe147 => 55
	i32 371306672, ; 54: Grpc.Core.Api.dll => 0x1621b0b0 => 189
	i32 374914964, ; 55: System.Transactions.Local => 0x1658bf94 => 149
	i32 375677976, ; 56: System.Net.ServicePoint.dll => 0x16646418 => 74
	i32 379916513, ; 57: System.Threading.Thread.dll => 0x16a510e1 => 145
	i32 385762202, ; 58: System.Memory.dll => 0x16fe439a => 62
	i32 391886110, ; 59: Grpc.Net.Client.dll => 0x175bb51e => 190
	i32 392610295, ; 60: System.Threading.ThreadPool.dll => 0x1766c1f7 => 146
	i32 395744057, ; 61: _Microsoft.Android.Resource.Designer => 0x17969339 => 350
	i32 403441872, ; 62: WindowsBase => 0x180c08d0 => 165
	i32 435591531, ; 63: sv/Microsoft.Maui.Controls.resources.dll => 0x19f6996b => 338
	i32 441335492, ; 64: Xamarin.AndroidX.ConstraintLayout.Core => 0x1a4e3ec4 => 244
	i32 442565967, ; 65: System.Collections => 0x1a61054f => 12
	i32 450948140, ; 66: Xamarin.AndroidX.Fragment.dll => 0x1ae0ec2c => 257
	i32 451504562, ; 67: System.Security.Cryptography.X509Certificates => 0x1ae969b2 => 125
	i32 456227837, ; 68: System.Web.HttpUtility.dll => 0x1b317bfd => 152
	i32 459347974, ; 69: System.Runtime.Serialization.Primitives.dll => 0x1b611806 => 113
	i32 465846621, ; 70: mscorlib => 0x1bc4415d => 166
	i32 469710990, ; 71: System.dll => 0x1bff388e => 164
	i32 476646585, ; 72: Xamarin.AndroidX.Interpolator.dll => 0x1c690cb9 => 259
	i32 486930444, ; 73: Xamarin.AndroidX.LocalBroadcastManager.dll => 0x1d05f80c => 272
	i32 498788369, ; 74: System.ObjectModel => 0x1dbae811 => 84
	i32 500358224, ; 75: id/Microsoft.Maui.Controls.resources.dll => 0x1dd2dc50 => 325
	i32 501000162, ; 76: Prism.dll => 0x1ddca7e2 => 212
	i32 503918385, ; 77: fi/Microsoft.Maui.Controls.resources.dll => 0x1e092f31 => 319
	i32 504143952, ; 78: Plugin.LocalNotification.dll => 0x1e0ca050 => 209
	i32 513247710, ; 79: Microsoft.Extensions.Primitives.dll => 0x1e9789de => 201
	i32 526420162, ; 80: System.Transactions.dll => 0x1f6088c2 => 150
	i32 527452488, ; 81: Xamarin.Kotlin.StdLib.Jdk7 => 0x1f704948 => 308
	i32 530272170, ; 82: System.Linq.Queryable => 0x1f9b4faa => 60
	i32 539058512, ; 83: Microsoft.Extensions.Logging => 0x20216150 => 197
	i32 540030774, ; 84: System.IO.FileSystem.dll => 0x20303736 => 51
	i32 545304856, ; 85: System.Runtime.Extensions => 0x2080b118 => 103
	i32 546455878, ; 86: System.Runtime.Serialization.Xml => 0x20924146 => 114
	i32 548916678, ; 87: Microsoft.Bcl.AsyncInterfaces => 0x20b7cdc6 => 192
	i32 549171840, ; 88: System.Globalization.Calendars => 0x20bbb280 => 40
	i32 557405415, ; 89: Jsr305Binding => 0x213954e7 => 297
	i32 569601784, ; 90: Xamarin.AndroidX.Window.Extensions.Core.Core => 0x21f36ef8 => 295
	i32 577335427, ; 91: System.Security.Cryptography.Cng => 0x22697083 => 120
	i32 592146354, ; 92: pt-BR/Microsoft.Maui.Controls.resources.dll => 0x234b6fb2 => 333
	i32 601371474, ; 93: System.IO.IsolatedStorage.dll => 0x23d83352 => 52
	i32 605376203, ; 94: System.IO.Compression.FileSystem => 0x24154ecb => 44
	i32 613668793, ; 95: System.Security.Cryptography.Algorithms => 0x2493d7b9 => 119
	i32 627609679, ; 96: Xamarin.AndroidX.CustomView => 0x2568904f => 249
	i32 627931235, ; 97: nl\Microsoft.Maui.Controls.resources => 0x256d7863 => 331
	i32 639843206, ; 98: Xamarin.AndroidX.Emoji2.ViewsHelper.dll => 0x26233b86 => 255
	i32 643868501, ; 99: System.Net => 0x2660a755 => 81
	i32 646990296, ; 100: Google.Cloud.Firestore.V1.dll => 0x269049d8 => 183
	i32 662205335, ; 101: System.Text.Encodings.Web.dll => 0x27787397 => 136
	i32 663517072, ; 102: Xamarin.AndroidX.VersionedParcelable => 0x278c7790 => 291
	i32 666292255, ; 103: Xamarin.AndroidX.Arch.Core.Common.dll => 0x27b6d01f => 236
	i32 672442732, ; 104: System.Collections.Concurrent => 0x2814a96c => 8
	i32 683518922, ; 105: System.Net.Security => 0x28bdabca => 73
	i32 688181140, ; 106: ca/Microsoft.Maui.Controls.resources.dll => 0x2904cf94 => 313
	i32 690569205, ; 107: System.Xml.Linq.dll => 0x29293ff5 => 155
	i32 691348768, ; 108: Xamarin.KotlinX.Coroutines.Android.dll => 0x29352520 => 310
	i32 693804605, ; 109: System.Windows => 0x295a9e3d => 154
	i32 699345723, ; 110: System.Reflection.Emit => 0x29af2b3b => 92
	i32 700284507, ; 111: Xamarin.Jetbrains.Annotations => 0x29bd7e5b => 305
	i32 700358131, ; 112: System.IO.Compression.ZipFile => 0x29be9df3 => 45
	i32 706645707, ; 113: ko/Microsoft.Maui.Controls.resources.dll => 0x2a1e8ecb => 328
	i32 709557578, ; 114: de/Microsoft.Maui.Controls.resources.dll => 0x2a4afd4a => 316
	i32 720511267, ; 115: Xamarin.Kotlin.StdLib.Jdk8 => 0x2af22123 => 309
	i32 722857257, ; 116: System.Runtime.Loader.dll => 0x2b15ed29 => 109
	i32 735137430, ; 117: System.Security.SecureString.dll => 0x2bd14e96 => 129
	i32 748832960, ; 118: SQLitePCLRaw.batteries_v2 => 0x2ca248c0 => 218
	i32 752232764, ; 119: System.Diagnostics.Contracts.dll => 0x2cd6293c => 25
	i32 755313932, ; 120: Xamarin.Android.Glide.Annotations.dll => 0x2d052d0c => 226
	i32 759454413, ; 121: System.Net.Requests => 0x2d445acd => 72
	i32 762598435, ; 122: System.IO.Pipes.dll => 0x2d745423 => 55
	i32 775507847, ; 123: System.IO.Compression => 0x2e394f87 => 46
	i32 777317022, ; 124: sk\Microsoft.Maui.Controls.resources => 0x2e54ea9e => 337
	i32 789151979, ; 125: Microsoft.Extensions.Options => 0x2f0980eb => 200
	i32 790371945, ; 126: Xamarin.AndroidX.CustomView.PoolingContainer.dll => 0x2f1c1e69 => 250
	i32 804715423, ; 127: System.Data.Common => 0x2ff6fb9f => 22
	i32 807930345, ; 128: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx.dll => 0x302809e9 => 264
	i32 823281589, ; 129: System.Private.Uri.dll => 0x311247b5 => 86
	i32 830298997, ; 130: System.IO.Compression.Brotli => 0x317d5b75 => 43
	i32 832635846, ; 131: System.Xml.XPath.dll => 0x31a103c6 => 160
	i32 834051424, ; 132: System.Net.Quic => 0x31b69d60 => 71
	i32 843511501, ; 133: Xamarin.AndroidX.Print => 0x3246f6cd => 277
	i32 873119928, ; 134: Microsoft.VisualBasic => 0x340ac0b8 => 3
	i32 877678880, ; 135: System.Globalization.dll => 0x34505120 => 42
	i32 878954865, ; 136: System.Net.Http.Json => 0x3463c971 => 63
	i32 904024072, ; 137: System.ComponentModel.Primitives.dll => 0x35e25008 => 16
	i32 911108515, ; 138: System.IO.MemoryMappedFiles.dll => 0x364e69a3 => 53
	i32 926902833, ; 139: tr/Microsoft.Maui.Controls.resources.dll => 0x373f6a31 => 340
	i32 928116545, ; 140: Xamarin.Google.Guava.ListenableFuture => 0x3751ef41 => 300
	i32 952186615, ; 141: System.Runtime.InteropServices.JavaScript.dll => 0x38c136f7 => 105
	i32 955402788, ; 142: Newtonsoft.Json => 0x38f24a24 => 208
	i32 956575887, ; 143: Xamarin.Kotlin.StdLib.Jdk8.dll => 0x3904308f => 309
	i32 966729478, ; 144: Xamarin.Google.Crypto.Tink.Android => 0x399f1f06 => 298
	i32 967690846, ; 145: Xamarin.AndroidX.Lifecycle.Common.dll => 0x39adca5e => 261
	i32 975236339, ; 146: System.Diagnostics.Tracing => 0x3a20ecf3 => 34
	i32 975874589, ; 147: System.Xml.XDocument => 0x3a2aaa1d => 158
	i32 986514023, ; 148: System.Private.DataContractSerialization.dll => 0x3acd0267 => 85
	i32 987214855, ; 149: System.Diagnostics.Tools => 0x3ad7b407 => 32
	i32 992768348, ; 150: System.Collections.dll => 0x3b2c715c => 12
	i32 994442037, ; 151: System.IO.FileSystem => 0x3b45fb35 => 51
	i32 1001831731, ; 152: System.IO.UnmanagedMemoryStream.dll => 0x3bb6bd33 => 56
	i32 1012816738, ; 153: Xamarin.AndroidX.SavedState.dll => 0x3c5e5b62 => 281
	i32 1016205835, ; 154: Mental Health Wellness Tracker => 0x3c92120b => 0
	i32 1019214401, ; 155: System.Drawing => 0x3cbffa41 => 36
	i32 1028951442, ; 156: Microsoft.Extensions.DependencyInjection.Abstractions => 0x3d548d92 => 196
	i32 1029334545, ; 157: da/Microsoft.Maui.Controls.resources.dll => 0x3d5a6611 => 315
	i32 1031528504, ; 158: Xamarin.Google.ErrorProne.Annotations.dll => 0x3d7be038 => 299
	i32 1035644815, ; 159: Xamarin.AndroidX.AppCompat => 0x3dbaaf8f => 234
	i32 1036536393, ; 160: System.Drawing.Primitives.dll => 0x3dc84a49 => 35
	i32 1044663988, ; 161: System.Linq.Expressions.dll => 0x3e444eb4 => 58
	i32 1049751285, ; 162: Google.Api.CommonProtos.dll => 0x3e91eef5 => 176
	i32 1052210849, ; 163: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 0x3eb776a1 => 268
	i32 1067306892, ; 164: GoogleGson => 0x3f9dcf8c => 187
	i32 1082857460, ; 165: System.ComponentModel.TypeConverter => 0x408b17f4 => 17
	i32 1084122840, ; 166: Xamarin.Kotlin.StdLib => 0x409e66d8 => 306
	i32 1098259244, ; 167: System => 0x41761b2c => 164
	i32 1118262833, ; 168: ko\Microsoft.Maui.Controls.resources => 0x42a75631 => 328
	i32 1121599056, ; 169: Xamarin.AndroidX.Lifecycle.Runtime.Ktx.dll => 0x42da3e50 => 267
	i32 1127624469, ; 170: Microsoft.Extensions.Logging.Debug => 0x43362f15 => 199
	i32 1149092582, ; 171: Xamarin.AndroidX.Window => 0x447dc2e6 => 294
	i32 1164997248, ; 172: Prism.Maui => 0x45707280 => 215
	i32 1168523401, ; 173: pt\Microsoft.Maui.Controls.resources => 0x45a64089 => 334
	i32 1170634674, ; 174: System.Web.dll => 0x45c677b2 => 153
	i32 1175144683, ; 175: Xamarin.AndroidX.VectorDrawable.Animated => 0x460b48eb => 290
	i32 1178241025, ; 176: Xamarin.AndroidX.Navigation.Runtime.dll => 0x463a8801 => 275
	i32 1203173028, ; 177: Grpc.Net.Client => 0x47b6f6a4 => 190
	i32 1203215381, ; 178: pl/Microsoft.Maui.Controls.resources.dll => 0x47b79c15 => 332
	i32 1204270330, ; 179: Xamarin.AndroidX.Arch.Core.Common => 0x47c7b4fa => 236
	i32 1208641965, ; 180: System.Diagnostics.Process => 0x480a69ad => 29
	i32 1219128291, ; 181: System.IO.IsolatedStorage => 0x48aa6be3 => 52
	i32 1234928153, ; 182: nb/Microsoft.Maui.Controls.resources.dll => 0x499b8219 => 330
	i32 1243150071, ; 183: Xamarin.AndroidX.Window.Extensions.Core.Core.dll => 0x4a18f6f7 => 295
	i32 1253011324, ; 184: Microsoft.Win32.Registry => 0x4aaf6f7c => 5
	i32 1260983243, ; 185: cs\Microsoft.Maui.Controls.resources => 0x4b2913cb => 314
	i32 1264511973, ; 186: Xamarin.AndroidX.Startup.StartupRuntime.dll => 0x4b5eebe5 => 285
	i32 1267360935, ; 187: Xamarin.AndroidX.VectorDrawable => 0x4b8a64a7 => 289
	i32 1273260888, ; 188: Xamarin.AndroidX.Collection.Ktx => 0x4be46b58 => 241
	i32 1275534314, ; 189: Xamarin.KotlinX.Coroutines.Android => 0x4c071bea => 310
	i32 1278448581, ; 190: Xamarin.AndroidX.Annotation.Jvm => 0x4c3393c5 => 233
	i32 1292207520, ; 191: SQLitePCLRaw.core.dll => 0x4d0585a0 => 219
	i32 1293217323, ; 192: Xamarin.AndroidX.DrawerLayout.dll => 0x4d14ee2b => 252
	i32 1309188875, ; 193: System.Private.DataContractSerialization => 0x4e08a30b => 85
	i32 1322716291, ; 194: Xamarin.AndroidX.Window.dll => 0x4ed70c83 => 294
	i32 1324164729, ; 195: System.Linq => 0x4eed2679 => 61
	i32 1335329327, ; 196: System.Runtime.Serialization.Json.dll => 0x4f97822f => 112
	i32 1364015309, ; 197: System.IO => 0x514d38cd => 57
	i32 1373134921, ; 198: zh-Hans\Microsoft.Maui.Controls.resources => 0x51d86049 => 344
	i32 1376866003, ; 199: Xamarin.AndroidX.SavedState => 0x52114ed3 => 281
	i32 1379779777, ; 200: System.Resources.ResourceManager => 0x523dc4c1 => 99
	i32 1402170036, ; 201: System.Configuration.dll => 0x53936ab4 => 19
	i32 1406073936, ; 202: Xamarin.AndroidX.CoordinatorLayout => 0x53cefc50 => 245
	i32 1407947027, ; 203: Mental Health Wellness Tracker.dll => 0x53eb9113 => 0
	i32 1408315940, ; 204: Prism.Events => 0x53f13224 => 214
	i32 1408764838, ; 205: System.Runtime.Serialization.Formatters.dll => 0x53f80ba6 => 111
	i32 1411638395, ; 206: System.Runtime.CompilerServices.Unsafe => 0x5423e47b => 101
	i32 1422545099, ; 207: System.Runtime.CompilerServices.VisualC => 0x54ca50cb => 102
	i32 1430672901, ; 208: ar\Microsoft.Maui.Controls.resources => 0x55465605 => 312
	i32 1434145427, ; 209: System.Runtime.Handles => 0x557b5293 => 104
	i32 1435222561, ; 210: Xamarin.Google.Crypto.Tink.Android.dll => 0x558bc221 => 298
	i32 1437713837, ; 211: Grpc.Auth => 0x55b1c5ad => 188
	i32 1439761251, ; 212: System.Net.Quic.dll => 0x55d10363 => 71
	i32 1452070440, ; 213: System.Formats.Asn1.dll => 0x568cd628 => 38
	i32 1453312822, ; 214: System.Diagnostics.Tools.dll => 0x569fcb36 => 32
	i32 1457743152, ; 215: System.Runtime.Extensions.dll => 0x56e36530 => 103
	i32 1458022317, ; 216: System.Net.Security.dll => 0x56e7a7ad => 73
	i32 1461004990, ; 217: es\Microsoft.Maui.Controls.resources => 0x57152abe => 318
	i32 1461234159, ; 218: System.Collections.Immutable.dll => 0x5718a9ef => 9
	i32 1461719063, ; 219: System.Security.Cryptography.OpenSsl => 0x57201017 => 123
	i32 1462112819, ; 220: System.IO.Compression.dll => 0x57261233 => 46
	i32 1469204771, ; 221: Xamarin.AndroidX.AppCompat.AppCompatResources => 0x57924923 => 235
	i32 1470490898, ; 222: Microsoft.Extensions.Primitives => 0x57a5e912 => 201
	i32 1479771757, ; 223: System.Collections.Immutable => 0x5833866d => 9
	i32 1480492111, ; 224: System.IO.Compression.Brotli.dll => 0x583e844f => 43
	i32 1487239319, ; 225: Microsoft.Win32.Primitives => 0x58a57897 => 4
	i32 1490025113, ; 226: Xamarin.AndroidX.SavedState.SavedState.Ktx.dll => 0x58cffa99 => 282
	i32 1493001747, ; 227: hi/Microsoft.Maui.Controls.resources.dll => 0x58fd6613 => 322
	i32 1514721132, ; 228: el/Microsoft.Maui.Controls.resources.dll => 0x5a48cf6c => 317
	i32 1524747670, ; 229: Plugin.LocalNotification => 0x5ae1cd96 => 209
	i32 1536373174, ; 230: System.Diagnostics.TextWriterTraceListener => 0x5b9331b6 => 31
	i32 1543031311, ; 231: System.Text.RegularExpressions.dll => 0x5bf8ca0f => 138
	i32 1543355203, ; 232: System.Reflection.Emit.dll => 0x5bfdbb43 => 92
	i32 1550322496, ; 233: System.Reflection.Extensions.dll => 0x5c680b40 => 93
	i32 1551623176, ; 234: sk/Microsoft.Maui.Controls.resources.dll => 0x5c7be408 => 337
	i32 1565862583, ; 235: System.IO.FileSystem.Primitives => 0x5d552ab7 => 49
	i32 1566207040, ; 236: System.Threading.Tasks.Dataflow.dll => 0x5d5a6c40 => 141
	i32 1573704789, ; 237: System.Runtime.Serialization.Json => 0x5dccd455 => 112
	i32 1574306634, ; 238: Prism.Events.dll => 0x5dd6034a => 214
	i32 1580037396, ; 239: System.Threading.Overlapped => 0x5e2d7514 => 140
	i32 1582372066, ; 240: Xamarin.AndroidX.DocumentFile.dll => 0x5e5114e2 => 251
	i32 1592978981, ; 241: System.Runtime.Serialization.dll => 0x5ef2ee25 => 115
	i32 1597949149, ; 242: Xamarin.Google.ErrorProne.Annotations => 0x5f3ec4dd => 299
	i32 1601112923, ; 243: System.Xml.Serialization => 0x5f6f0b5b => 157
	i32 1603525486, ; 244: Microsoft.Maui.Controls.HotReload.Forms.dll => 0x5f93db6e => 346
	i32 1604827217, ; 245: System.Net.WebClient => 0x5fa7b851 => 76
	i32 1618516317, ; 246: System.Net.WebSockets.Client.dll => 0x6078995d => 79
	i32 1622152042, ; 247: Xamarin.AndroidX.Loader.dll => 0x60b0136a => 271
	i32 1622358360, ; 248: System.Dynamic.Runtime => 0x60b33958 => 37
	i32 1624863272, ; 249: Xamarin.AndroidX.ViewPager2 => 0x60d97228 => 293
	i32 1635184631, ; 250: Xamarin.AndroidX.Emoji2.ViewsHelper => 0x6176eff7 => 255
	i32 1636350590, ; 251: Xamarin.AndroidX.CursorAdapter => 0x6188ba7e => 248
	i32 1639515021, ; 252: System.Net.Http.dll => 0x61b9038d => 64
	i32 1639986890, ; 253: System.Text.RegularExpressions => 0x61c036ca => 138
	i32 1641389582, ; 254: System.ComponentModel.EventBasedAsync.dll => 0x61d59e0e => 15
	i32 1657153582, ; 255: System.Runtime => 0x62c6282e => 116
	i32 1658241508, ; 256: Xamarin.AndroidX.Tracing.Tracing.dll => 0x62d6c1e4 => 287
	i32 1658251792, ; 257: Xamarin.Google.Android.Material.dll => 0x62d6ea10 => 296
	i32 1663627514, ; 258: DryIoc => 0x6328f0fa => 173
	i32 1670060433, ; 259: Xamarin.AndroidX.ConstraintLayout => 0x638b1991 => 243
	i32 1675553242, ; 260: System.IO.FileSystem.DriveInfo.dll => 0x63dee9da => 48
	i32 1677501392, ; 261: System.Net.Primitives.dll => 0x63fca3d0 => 70
	i32 1678508291, ; 262: System.Net.WebSockets => 0x640c0103 => 80
	i32 1679769178, ; 263: System.Security.Cryptography => 0x641f3e5a => 126
	i32 1691477237, ; 264: System.Reflection.Metadata => 0x64d1e4f5 => 94
	i32 1696967625, ; 265: System.Security.Cryptography.Csp => 0x6525abc9 => 121
	i32 1698840827, ; 266: Xamarin.Kotlin.StdLib.Common => 0x654240fb => 307
	i32 1701541528, ; 267: System.Diagnostics.Debug.dll => 0x656b7698 => 26
	i32 1711441057, ; 268: SQLitePCLRaw.lib.e_sqlite3.android => 0x660284a1 => 220
	i32 1711749759, ; 269: Prism.Container.DryIoc => 0x66073a7f => 211
	i32 1720223769, ; 270: Xamarin.AndroidX.Lifecycle.LiveData.Core.Ktx => 0x66888819 => 264
	i32 1726116996, ; 271: System.Reflection.dll => 0x66e27484 => 97
	i32 1728033016, ; 272: System.Diagnostics.FileVersionInfo.dll => 0x66ffb0f8 => 28
	i32 1729485958, ; 273: Xamarin.AndroidX.CardView.dll => 0x6715dc86 => 239
	i32 1736233607, ; 274: ro/Microsoft.Maui.Controls.resources.dll => 0x677cd287 => 335
	i32 1743415430, ; 275: ca\Microsoft.Maui.Controls.resources => 0x67ea6886 => 313
	i32 1744735666, ; 276: System.Transactions.Local.dll => 0x67fe8db2 => 149
	i32 1746316138, ; 277: Mono.Android.Export => 0x6816ab6a => 169
	i32 1750313021, ; 278: Microsoft.Win32.Primitives.dll => 0x6853a83d => 4
	i32 1758240030, ; 279: System.Resources.Reader.dll => 0x68cc9d1e => 98
	i32 1763938596, ; 280: System.Diagnostics.TraceSource.dll => 0x69239124 => 33
	i32 1765942094, ; 281: System.Reflection.Extensions => 0x6942234e => 93
	i32 1766324549, ; 282: Xamarin.AndroidX.SwipeRefreshLayout => 0x6947f945 => 286
	i32 1770582343, ; 283: Microsoft.Extensions.Logging.dll => 0x6988f147 => 197
	i32 1776026572, ; 284: System.Core.dll => 0x69dc03cc => 21
	i32 1777075843, ; 285: System.Globalization.Extensions.dll => 0x69ec0683 => 41
	i32 1780572499, ; 286: Mono.Android.Runtime.dll => 0x6a216153 => 170
	i32 1782161461, ; 287: Grpc.Core.Api => 0x6a39a035 => 189
	i32 1782862114, ; 288: ms\Microsoft.Maui.Controls.resources => 0x6a445122 => 329
	i32 1788241197, ; 289: Xamarin.AndroidX.Fragment => 0x6a96652d => 257
	i32 1793755602, ; 290: he\Microsoft.Maui.Controls.resources => 0x6aea89d2 => 321
	i32 1796167890, ; 291: Microsoft.Bcl.AsyncInterfaces.dll => 0x6b0f58d2 => 192
	i32 1808609942, ; 292: Xamarin.AndroidX.Loader => 0x6bcd3296 => 271
	i32 1813058853, ; 293: Xamarin.Kotlin.StdLib.dll => 0x6c111525 => 306
	i32 1813201214, ; 294: Xamarin.Google.Android.Material => 0x6c13413e => 296
	i32 1818569960, ; 295: Xamarin.AndroidX.Navigation.UI.dll => 0x6c652ce8 => 276
	i32 1818787751, ; 296: Microsoft.VisualBasic.Core => 0x6c687fa7 => 2
	i32 1824175904, ; 297: System.Text.Encoding.Extensions => 0x6cbab720 => 134
	i32 1824722060, ; 298: System.Runtime.Serialization.Formatters => 0x6cc30c8c => 111
	i32 1827303595, ; 299: Microsoft.VisualStudio.DesignTools.TapContract => 0x6cea70ab => 348
	i32 1828688058, ; 300: Microsoft.Extensions.Logging.Abstractions.dll => 0x6cff90ba => 198
	i32 1842015223, ; 301: uk/Microsoft.Maui.Controls.resources.dll => 0x6dcaebf7 => 341
	i32 1847515442, ; 302: Xamarin.Android.Glide.Annotations => 0x6e1ed932 => 226
	i32 1853025655, ; 303: sv\Microsoft.Maui.Controls.resources => 0x6e72ed77 => 338
	i32 1858542181, ; 304: System.Linq.Expressions => 0x6ec71a65 => 58
	i32 1870277092, ; 305: System.Reflection.Primitives => 0x6f7a29e4 => 95
	i32 1875935024, ; 306: fr\Microsoft.Maui.Controls.resources => 0x6fd07f30 => 320
	i32 1879696579, ; 307: System.Formats.Tar.dll => 0x7009e4c3 => 39
	i32 1885316902, ; 308: Xamarin.AndroidX.Arch.Core.Runtime.dll => 0x705fa726 => 237
	i32 1885918049, ; 309: Microsoft.VisualStudio.DesignTools.TapContract.dll => 0x7068d361 => 348
	i32 1888955245, ; 310: System.Diagnostics.Contracts => 0x70972b6d => 25
	i32 1889954781, ; 311: System.Reflection.Metadata.dll => 0x70a66bdd => 94
	i32 1898237753, ; 312: System.Reflection.DispatchProxy => 0x7124cf39 => 89
	i32 1900519031, ; 313: Grpc.Auth.dll => 0x71479e77 => 188
	i32 1900610850, ; 314: System.Resources.ResourceManager.dll => 0x71490522 => 99
	i32 1908813208, ; 315: Xamarin.GooglePlayServices.Basement => 0x71c62d98 => 302
	i32 1910275211, ; 316: System.Collections.NonGeneric.dll => 0x71dc7c8b => 10
	i32 1927897671, ; 317: System.CodeDom.dll => 0x72e96247 => 222
	i32 1939592360, ; 318: System.Private.Xml.Linq => 0x739bd4a8 => 87
	i32 1956758971, ; 319: System.Resources.Writer => 0x74a1c5bb => 100
	i32 1961813231, ; 320: Xamarin.AndroidX.Security.SecurityCrypto.dll => 0x74eee4ef => 283
	i32 1968388702, ; 321: Microsoft.Extensions.Configuration.dll => 0x75533a5e => 193
	i32 1983156543, ; 322: Xamarin.Kotlin.StdLib.Common.dll => 0x7634913f => 307
	i32 1985761444, ; 323: Xamarin.Android.Glide.GifDecoder => 0x765c50a4 => 228
	i32 2003115576, ; 324: el\Microsoft.Maui.Controls.resources => 0x77651e38 => 317
	i32 2011961780, ; 325: System.Buffers.dll => 0x77ec19b4 => 7
	i32 2019424037, ; 326: Prism.Maui.dll => 0x785df725 => 215
	i32 2019465201, ; 327: Xamarin.AndroidX.Lifecycle.ViewModel => 0x785e97f1 => 268
	i32 2025202353, ; 328: ar/Microsoft.Maui.Controls.resources.dll => 0x78b622b1 => 312
	i32 2031763787, ; 329: Xamarin.Android.Glide => 0x791a414b => 225
	i32 2045470958, ; 330: System.Private.Xml => 0x79eb68ee => 88
	i32 2055257422, ; 331: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 0x7a80bd4e => 263
	i32 2060060697, ; 332: System.Windows.dll => 0x7aca0819 => 154
	i32 2066184531, ; 333: de\Microsoft.Maui.Controls.resources => 0x7b277953 => 316
	i32 2066202781, ; 334: Prism => 0x7b27c09d => 212
	i32 2067484054, ; 335: System.Linq.AsyncEnumerable.dll => 0x7b3b4d96 => 223
	i32 2070888862, ; 336: System.Diagnostics.TraceSource => 0x7b6f419e => 33
	i32 2079903147, ; 337: System.Runtime.dll => 0x7bf8cdab => 116
	i32 2090596640, ; 338: System.Numerics.Vectors => 0x7c9bf920 => 82
	i32 2103459038, ; 339: SQLitePCLRaw.provider.e_sqlite3.dll => 0x7d603cde => 221
	i32 2117912485, ; 340: Microsoft.VisualStudio.DesignTools.XamlTapContract.dll => 0x7e3cc7a5 => 349
	i32 2127167465, ; 341: System.Console => 0x7ec9ffe9 => 20
	i32 2129483829, ; 342: Xamarin.GooglePlayServices.Base.dll => 0x7eed5835 => 301
	i32 2142473426, ; 343: System.Collections.Specialized => 0x7fb38cd2 => 11
	i32 2143790110, ; 344: System.Xml.XmlSerializer.dll => 0x7fc7a41e => 162
	i32 2146852085, ; 345: Microsoft.VisualBasic.dll => 0x7ff65cf5 => 3
	i32 2159891885, ; 346: Microsoft.Maui => 0x80bd55ad => 205
	i32 2169148018, ; 347: hu\Microsoft.Maui.Controls.resources => 0x814a9272 => 324
	i32 2178612968, ; 348: System.CodeDom => 0x81dafee8 => 222
	i32 2181898931, ; 349: Microsoft.Extensions.Options.dll => 0x820d22b3 => 200
	i32 2192057212, ; 350: Microsoft.Extensions.Logging.Abstractions => 0x82a8237c => 198
	i32 2193016926, ; 351: System.ObjectModel.dll => 0x82b6c85e => 84
	i32 2201107256, ; 352: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 0x83323b38 => 311
	i32 2201231467, ; 353: System.Net.Http => 0x8334206b => 64
	i32 2207618523, ; 354: it\Microsoft.Maui.Controls.resources => 0x839595db => 326
	i32 2216717168, ; 355: Firebase.Auth.dll => 0x84206b70 => 174
	i32 2217644978, ; 356: Xamarin.AndroidX.VectorDrawable.Animated.dll => 0x842e93b2 => 290
	i32 2222056684, ; 357: System.Threading.Tasks.Parallel => 0x8471e4ec => 143
	i32 2244775296, ; 358: Xamarin.AndroidX.LocalBroadcastManager => 0x85cc8d80 => 272
	i32 2252106437, ; 359: System.Xml.Serialization.dll => 0x863c6ac5 => 157
	i32 2256313426, ; 360: System.Globalization.Extensions => 0x867c9c52 => 41
	i32 2265110946, ; 361: System.Security.AccessControl.dll => 0x8702d9a2 => 117
	i32 2266799131, ; 362: Microsoft.Extensions.Configuration.Abstractions => 0x871c9c1b => 194
	i32 2267999099, ; 363: Xamarin.Android.Glide.DiskLruCache.dll => 0x872eeb7b => 227
	i32 2270573516, ; 364: fr/Microsoft.Maui.Controls.resources.dll => 0x875633cc => 320
	i32 2279755925, ; 365: Xamarin.AndroidX.RecyclerView.dll => 0x87e25095 => 279
	i32 2293034957, ; 366: System.ServiceModel.Web.dll => 0x88acefcd => 131
	i32 2295906218, ; 367: System.Net.Sockets => 0x88d8bfaa => 75
	i32 2298471582, ; 368: System.Net.Mail => 0x88ffe49e => 66
	i32 2303942373, ; 369: nb\Microsoft.Maui.Controls.resources => 0x89535ee5 => 330
	i32 2305521784, ; 370: System.Private.CoreLib.dll => 0x896b7878 => 172
	i32 2315684594, ; 371: Xamarin.AndroidX.Annotation.dll => 0x8a068af2 => 231
	i32 2320631194, ; 372: System.Threading.Tasks.Parallel.dll => 0x8a52059a => 143
	i32 2340441535, ; 373: System.Runtime.InteropServices.RuntimeInformation.dll => 0x8b804dbf => 106
	i32 2344264397, ; 374: System.ValueTuple => 0x8bbaa2cd => 151
	i32 2353062107, ; 375: System.Net.Primitives => 0x8c40e0db => 70
	i32 2368005991, ; 376: System.Xml.ReaderWriter.dll => 0x8d24e767 => 156
	i32 2371007202, ; 377: Microsoft.Extensions.Configuration => 0x8d52b2e2 => 193
	i32 2378619854, ; 378: System.Security.Cryptography.Csp.dll => 0x8dc6dbce => 121
	i32 2383496789, ; 379: System.Security.Principal.Windows.dll => 0x8e114655 => 127
	i32 2395872292, ; 380: id\Microsoft.Maui.Controls.resources => 0x8ece1c24 => 325
	i32 2397347608, ; 381: Google.LongRunning.dll => 0x8ee49f18 => 185
	i32 2401565422, ; 382: System.Web.HttpUtility => 0x8f24faee => 152
	i32 2403452196, ; 383: Xamarin.AndroidX.Emoji2.dll => 0x8f41c524 => 254
	i32 2409983638, ; 384: Microsoft.VisualStudio.DesignTools.MobileTapContracts.dll => 0x8fa56e96 => 347
	i32 2421380589, ; 385: System.Threading.Tasks.Dataflow => 0x905355ed => 141
	i32 2423080555, ; 386: Xamarin.AndroidX.Collection.Ktx.dll => 0x906d466b => 241
	i32 2427813419, ; 387: hi\Microsoft.Maui.Controls.resources => 0x90b57e2b => 322
	i32 2435356389, ; 388: System.Console.dll => 0x912896e5 => 20
	i32 2435904999, ; 389: System.ComponentModel.DataAnnotations.dll => 0x9130f5e7 => 14
	i32 2441199521, ; 390: Google.Cloud.Firestore => 0x9181bfa1 => 182
	i32 2454642406, ; 391: System.Text.Encoding.dll => 0x924edee6 => 135
	i32 2458678730, ; 392: System.Net.Sockets.dll => 0x928c75ca => 75
	i32 2459001652, ; 393: System.Linq.Parallel.dll => 0x92916334 => 59
	i32 2465273461, ; 394: SQLitePCLRaw.batteries_v2.dll => 0x92f11675 => 218
	i32 2465532216, ; 395: Xamarin.AndroidX.ConstraintLayout.Core.dll => 0x92f50938 => 244
	i32 2471841756, ; 396: netstandard.dll => 0x93554fdc => 167
	i32 2475788418, ; 397: Java.Interop.dll => 0x93918882 => 168
	i32 2480646305, ; 398: Microsoft.Maui.Controls => 0x93dba8a1 => 203
	i32 2483903535, ; 399: System.ComponentModel.EventBasedAsync => 0x940d5c2f => 15
	i32 2484371297, ; 400: System.Net.ServicePoint => 0x94147f61 => 74
	i32 2486847491, ; 401: Google.Api.Gax => 0x943a4803 => 177
	i32 2490993605, ; 402: System.AppContext.dll => 0x94798bc5 => 6
	i32 2501346920, ; 403: System.Data.DataSetExtensions => 0x95178668 => 23
	i32 2505896520, ; 404: Xamarin.AndroidX.Lifecycle.Runtime.dll => 0x955cf248 => 266
	i32 2522472828, ; 405: Xamarin.Android.Glide.dll => 0x9659e17c => 225
	i32 2538310050, ; 406: System.Reflection.Emit.Lightweight.dll => 0x974b89a2 => 91
	i32 2550873716, ; 407: hr\Microsoft.Maui.Controls.resources => 0x980b3e74 => 323
	i32 2562349572, ; 408: Microsoft.CSharp => 0x98ba5a04 => 1
	i32 2570120770, ; 409: System.Text.Encodings.Web => 0x9930ee42 => 136
	i32 2581783588, ; 410: Xamarin.AndroidX.Lifecycle.Runtime.Ktx => 0x99e2e424 => 267
	i32 2581819634, ; 411: Xamarin.AndroidX.VectorDrawable.dll => 0x99e370f2 => 289
	i32 2585220780, ; 412: System.Text.Encoding.Extensions.dll => 0x9a1756ac => 134
	i32 2585805581, ; 413: System.Net.Ping => 0x9a20430d => 69
	i32 2589602615, ; 414: System.Threading.ThreadPool => 0x9a5a3337 => 146
	i32 2593496499, ; 415: pl\Microsoft.Maui.Controls.resources => 0x9a959db3 => 332
	i32 2605712449, ; 416: Xamarin.KotlinX.Coroutines.Core.Jvm => 0x9b500441 => 311
	i32 2615233544, ; 417: Xamarin.AndroidX.Fragment.Ktx => 0x9be14c08 => 258
	i32 2616218305, ; 418: Microsoft.Extensions.Logging.Debug.dll => 0x9bf052c1 => 199
	i32 2617129537, ; 419: System.Private.Xml.dll => 0x9bfe3a41 => 88
	i32 2618712057, ; 420: System.Reflection.TypeExtensions.dll => 0x9c165ff9 => 96
	i32 2620871830, ; 421: Xamarin.AndroidX.CursorAdapter.dll => 0x9c375496 => 248
	i32 2624644809, ; 422: Xamarin.AndroidX.DynamicAnimation => 0x9c70e6c9 => 253
	i32 2626831493, ; 423: ja\Microsoft.Maui.Controls.resources => 0x9c924485 => 327
	i32 2627185994, ; 424: System.Diagnostics.TextWriterTraceListener.dll => 0x9c97ad4a => 31
	i32 2629843544, ; 425: System.IO.Compression.ZipFile.dll => 0x9cc03a58 => 45
	i32 2633051222, ; 426: Xamarin.AndroidX.Lifecycle.LiveData => 0x9cf12c56 => 262
	i32 2635732976, ; 427: Google.Cloud.Firestore.dll => 0x9d1a17f0 => 182
	i32 2663391936, ; 428: Xamarin.Android.Glide.DiskLruCache => 0x9ec022c0 => 227
	i32 2663698177, ; 429: System.Runtime.Loader => 0x9ec4cf01 => 109
	i32 2664396074, ; 430: System.Xml.XDocument.dll => 0x9ecf752a => 158
	i32 2665622720, ; 431: System.Drawing.Primitives => 0x9ee22cc0 => 35
	i32 2676780864, ; 432: System.Data.Common.dll => 0x9f8c6f40 => 22
	i32 2686887180, ; 433: System.Runtime.Serialization.Xml.dll => 0xa026a50c => 114
	i32 2693849962, ; 434: System.IO.dll => 0xa090e36a => 57
	i32 2701096212, ; 435: Xamarin.AndroidX.Tracing.Tracing => 0xa0ff7514 => 287
	i32 2715334215, ; 436: System.Threading.Tasks.dll => 0xa1d8b647 => 144
	i32 2717744543, ; 437: System.Security.Claims => 0xa1fd7d9f => 118
	i32 2719963679, ; 438: System.Security.Cryptography.Cng.dll => 0xa21f5a1f => 120
	i32 2724373263, ; 439: System.Runtime.Numerics.dll => 0xa262a30f => 110
	i32 2732626843, ; 440: Xamarin.AndroidX.Activity => 0xa2e0939b => 229
	i32 2735172069, ; 441: System.Threading.Channels => 0xa30769e5 => 139
	i32 2737747696, ; 442: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 0xa32eb6f0 => 235
	i32 2740948882, ; 443: System.IO.Pipes.AccessControl => 0xa35f8f92 => 54
	i32 2744327253, ; 444: Google.Api.Gax.Grpc.dll => 0xa3931c55 => 178
	i32 2748088231, ; 445: System.Runtime.InteropServices.JavaScript => 0xa3cc7fa7 => 105
	i32 2752995522, ; 446: pt-BR\Microsoft.Maui.Controls.resources => 0xa41760c2 => 333
	i32 2757554483, ; 447: Google.Api.Gax.Grpc => 0xa45cf133 => 178
	i32 2758225723, ; 448: Microsoft.Maui.Controls.Xaml => 0xa4672f3b => 204
	i32 2764765095, ; 449: Microsoft.Maui.dll => 0xa4caf7a7 => 205
	i32 2765824710, ; 450: System.Text.Encoding.CodePages.dll => 0xa4db22c6 => 133
	i32 2768457651, ; 451: PropertyChanged => 0xa5034fb3 => 216
	i32 2770495804, ; 452: Xamarin.Jetbrains.Annotations.dll => 0xa522693c => 305
	i32 2778768386, ; 453: Xamarin.AndroidX.ViewPager.dll => 0xa5a0a402 => 292
	i32 2779977773, ; 454: Xamarin.AndroidX.ResourceInspection.Annotation.dll => 0xa5b3182d => 280
	i32 2785988530, ; 455: th\Microsoft.Maui.Controls.resources => 0xa60ecfb2 => 339
	i32 2788224221, ; 456: Xamarin.AndroidX.Fragment.Ktx.dll => 0xa630ecdd => 258
	i32 2801831435, ; 457: Microsoft.Maui.Graphics => 0xa7008e0b => 207
	i32 2803228030, ; 458: System.Xml.XPath.XDocument.dll => 0xa715dd7e => 159
	i32 2806116107, ; 459: es/Microsoft.Maui.Controls.resources.dll => 0xa741ef0b => 318
	i32 2810250172, ; 460: Xamarin.AndroidX.CoordinatorLayout.dll => 0xa78103bc => 245
	i32 2819470561, ; 461: System.Xml.dll => 0xa80db4e1 => 163
	i32 2821205001, ; 462: System.ServiceProcess.dll => 0xa8282c09 => 132
	i32 2821294376, ; 463: Xamarin.AndroidX.ResourceInspection.Annotation => 0xa8298928 => 280
	i32 2824502124, ; 464: System.Xml.XmlDocument => 0xa85a7b6c => 161
	i32 2831556043, ; 465: nl/Microsoft.Maui.Controls.resources.dll => 0xa8c61dcb => 331
	i32 2838993487, ; 466: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx.dll => 0xa9379a4f => 269
	i32 2839679515, ; 467: Google.LongRunning => 0xa942121b => 185
	i32 2847418871, ; 468: Xamarin.GooglePlayServices.Base => 0xa9b829f7 => 301
	i32 2849599387, ; 469: System.Threading.Overlapped.dll => 0xa9d96f9b => 140
	i32 2853208004, ; 470: Xamarin.AndroidX.ViewPager => 0xaa107fc4 => 292
	i32 2855708567, ; 471: Xamarin.AndroidX.Transition => 0xaa36a797 => 288
	i32 2861098320, ; 472: Mono.Android.Export.dll => 0xaa88e550 => 169
	i32 2861189240, ; 473: Microsoft.Maui.Essentials => 0xaa8a4878 => 206
	i32 2870099610, ; 474: Xamarin.AndroidX.Activity.Ktx.dll => 0xab123e9a => 230
	i32 2875164099, ; 475: Jsr305Binding.dll => 0xab5f85c3 => 297
	i32 2875220617, ; 476: System.Globalization.Calendars.dll => 0xab606289 => 40
	i32 2884993177, ; 477: Xamarin.AndroidX.ExifInterface => 0xabf58099 => 256
	i32 2887636118, ; 478: System.Net.dll => 0xac1dd496 => 81
	i32 2893550578, ; 479: Google.Apis.Core => 0xac7813f2 => 181
	i32 2898407901, ; 480: System.Management => 0xacc231dd => 224
	i32 2899753641, ; 481: System.IO.UnmanagedMemoryStream => 0xacd6baa9 => 56
	i32 2900621748, ; 482: System.Dynamic.Runtime.dll => 0xace3f9b4 => 37
	i32 2901442782, ; 483: System.Reflection => 0xacf080de => 97
	i32 2905242038, ; 484: mscorlib.dll => 0xad2a79b6 => 166
	i32 2909740682, ; 485: System.Private.CoreLib => 0xad6f1e8a => 172
	i32 2912646636, ; 486: Google.Api.CommonProtos => 0xad9b75ec => 176
	i32 2916838712, ; 487: Xamarin.AndroidX.ViewPager2.dll => 0xaddb6d38 => 293
	i32 2919462931, ; 488: System.Numerics.Vectors.dll => 0xae037813 => 82
	i32 2921128767, ; 489: Xamarin.AndroidX.Annotation.Experimental.dll => 0xae1ce33f => 232
	i32 2936416060, ; 490: System.Resources.Reader => 0xaf06273c => 98
	i32 2940926066, ; 491: System.Diagnostics.StackTrace.dll => 0xaf4af872 => 30
	i32 2942453041, ; 492: System.Xml.XPath.XDocument => 0xaf624531 => 159
	i32 2959614098, ; 493: System.ComponentModel.dll => 0xb0682092 => 18
	i32 2968338931, ; 494: System.Security.Principal.Windows => 0xb0ed41f3 => 127
	i32 2972252294, ; 495: System.Security.Cryptography.Algorithms.dll => 0xb128f886 => 119
	i32 2978675010, ; 496: Xamarin.AndroidX.DrawerLayout => 0xb18af942 => 252
	i32 2987532451, ; 497: Xamarin.AndroidX.Security.SecurityCrypto => 0xb21220a3 => 283
	i32 2990604888, ; 498: Google.Apis => 0xb2410258 => 179
	i32 2996846495, ; 499: Xamarin.AndroidX.Lifecycle.Process.dll => 0xb2a03f9f => 265
	i32 3016983068, ; 500: Xamarin.AndroidX.Startup.StartupRuntime => 0xb3d3821c => 285
	i32 3023353419, ; 501: WindowsBase.dll => 0xb434b64b => 165
	i32 3024354802, ; 502: Xamarin.AndroidX.Legacy.Support.Core.Utils => 0xb443fdf2 => 260
	i32 3038032645, ; 503: _Microsoft.Android.Resource.Designer.dll => 0xb514b305 => 350
	i32 3056245963, ; 504: Xamarin.AndroidX.SavedState.SavedState.Ktx => 0xb62a9ccb => 282
	i32 3057625584, ; 505: Xamarin.AndroidX.Navigation.Common => 0xb63fa9f0 => 273
	i32 3058099980, ; 506: Xamarin.GooglePlayServices.Tasks => 0xb646e70c => 304
	i32 3059408633, ; 507: Mono.Android.Runtime => 0xb65adef9 => 170
	i32 3059793426, ; 508: System.ComponentModel.Primitives => 0xb660be12 => 16
	i32 3075834255, ; 509: System.Threading.Tasks => 0xb755818f => 144
	i32 3077302341, ; 510: hu/Microsoft.Maui.Controls.resources.dll => 0xb76be845 => 324
	i32 3090735792, ; 511: System.Security.Cryptography.X509Certificates.dll => 0xb838e2b0 => 125
	i32 3099732863, ; 512: System.Security.Claims.dll => 0xb8c22b7f => 118
	i32 3103600923, ; 513: System.Formats.Asn1 => 0xb8fd311b => 38
	i32 3105931788, ; 514: Prism.Container.DryIoc.dll => 0xb920c20c => 211
	i32 3106263381, ; 515: Grpc.Net.Common.dll => 0xb925d155 => 191
	i32 3111772706, ; 516: System.Runtime.Serialization => 0xb979e222 => 115
	i32 3116360747, ; 517: Prism.DryIoc.Maui => 0xb9bfe42b => 213
	i32 3121463068, ; 518: System.IO.FileSystem.AccessControl.dll => 0xba0dbf1c => 47
	i32 3124832203, ; 519: System.Threading.Tasks.Extensions => 0xba4127cb => 142
	i32 3132293585, ; 520: System.Security.AccessControl => 0xbab301d1 => 117
	i32 3147165239, ; 521: System.Diagnostics.Tracing.dll => 0xbb95ee37 => 34
	i32 3148237826, ; 522: GoogleGson.dll => 0xbba64c02 => 187
	i32 3159123045, ; 523: System.Reflection.Primitives.dll => 0xbc4c6465 => 95
	i32 3160747431, ; 524: System.IO.MemoryMappedFiles => 0xbc652da7 => 53
	i32 3178803400, ; 525: Xamarin.AndroidX.Navigation.Fragment.dll => 0xbd78b0c8 => 274
	i32 3192346100, ; 526: System.Security.SecureString => 0xbe4755f4 => 129
	i32 3193515020, ; 527: System.Web => 0xbe592c0c => 153
	i32 3203277885, ; 528: Google.Api.Gax.dll => 0xbeee243d => 177
	i32 3204380047, ; 529: System.Data.dll => 0xbefef58f => 24
	i32 3209718065, ; 530: System.Xml.XmlDocument.dll => 0xbf506931 => 161
	i32 3211777861, ; 531: Xamarin.AndroidX.DocumentFile => 0xbf6fd745 => 251
	i32 3216641880, ; 532: Prism.Container.Abstractions.dll => 0xbfba0f58 => 210
	i32 3217618498, ; 533: Microsoft.VisualStudio.DesignTools.XamlTapContract => 0xbfc8f642 => 349
	i32 3220365878, ; 534: System.Threading => 0xbff2e236 => 148
	i32 3226221578, ; 535: System.Runtime.Handles.dll => 0xc04c3c0a => 104
	i32 3230466174, ; 536: Xamarin.GooglePlayServices.Basement.dll => 0xc08d007e => 302
	i32 3251039220, ; 537: System.Reflection.DispatchProxy.dll => 0xc1c6ebf4 => 89
	i32 3258312781, ; 538: Xamarin.AndroidX.CardView => 0xc235e84d => 239
	i32 3265493905, ; 539: System.Linq.Queryable.dll => 0xc2a37b91 => 60
	i32 3265893370, ; 540: System.Threading.Tasks.Extensions.dll => 0xc2a993fa => 142
	i32 3277815716, ; 541: System.Resources.Writer.dll => 0xc35f7fa4 => 100
	i32 3279906254, ; 542: Microsoft.Win32.Registry.dll => 0xc37f65ce => 5
	i32 3280506390, ; 543: System.ComponentModel.Annotations.dll => 0xc3888e16 => 13
	i32 3286872994, ; 544: SQLite-net.dll => 0xc3e9b3a2 => 217
	i32 3290767353, ; 545: System.Security.Cryptography.Encoding => 0xc4251ff9 => 122
	i32 3299363146, ; 546: System.Text.Encoding => 0xc4a8494a => 135
	i32 3303498502, ; 547: System.Diagnostics.FileVersionInfo => 0xc4e76306 => 28
	i32 3305363605, ; 548: fi\Microsoft.Maui.Controls.resources => 0xc503d895 => 319
	i32 3316684772, ; 549: System.Net.Requests.dll => 0xc5b097e4 => 72
	i32 3317135071, ; 550: Xamarin.AndroidX.CustomView.dll => 0xc5b776df => 249
	i32 3317144872, ; 551: System.Data => 0xc5b79d28 => 24
	i32 3340431453, ; 552: Xamarin.AndroidX.Arch.Core.Runtime => 0xc71af05d => 237
	i32 3345895724, ; 553: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller.dll => 0xc76e512c => 278
	i32 3346324047, ; 554: Xamarin.AndroidX.Navigation.Runtime => 0xc774da4f => 275
	i32 3357674450, ; 555: ru\Microsoft.Maui.Controls.resources => 0xc8220bd2 => 336
	i32 3358260929, ; 556: System.Text.Json => 0xc82afec1 => 137
	i32 3360279109, ; 557: SQLitePCLRaw.core => 0xc849ca45 => 219
	i32 3362336904, ; 558: Xamarin.AndroidX.Activity.Ktx => 0xc8693088 => 230
	i32 3362522851, ; 559: Xamarin.AndroidX.Core => 0xc86c06e3 => 246
	i32 3366347497, ; 560: Java.Interop => 0xc8a662e9 => 168
	i32 3374999561, ; 561: Xamarin.AndroidX.RecyclerView => 0xc92a6809 => 279
	i32 3381016424, ; 562: da\Microsoft.Maui.Controls.resources => 0xc9863768 => 315
	i32 3395150330, ; 563: System.Runtime.CompilerServices.Unsafe.dll => 0xca5de1fa => 101
	i32 3403906625, ; 564: System.Security.Cryptography.OpenSsl.dll => 0xcae37e41 => 123
	i32 3405233483, ; 565: Xamarin.AndroidX.CustomView.PoolingContainer => 0xcaf7bd4b => 250
	i32 3428513518, ; 566: Microsoft.Extensions.DependencyInjection.dll => 0xcc5af6ee => 195
	i32 3429136800, ; 567: System.Xml => 0xcc6479a0 => 163
	i32 3430777524, ; 568: netstandard => 0xcc7d82b4 => 167
	i32 3441283291, ; 569: Xamarin.AndroidX.DynamicAnimation.dll => 0xcd1dd0db => 253
	i32 3445260447, ; 570: System.Formats.Tar => 0xcd5a809f => 39
	i32 3452344032, ; 571: Microsoft.Maui.Controls.Compatibility.dll => 0xcdc696e0 => 202
	i32 3453592554, ; 572: Google.Apis.Core.dll => 0xcdd9a3ea => 181
	i32 3463511458, ; 573: hr/Microsoft.Maui.Controls.resources.dll => 0xce70fda2 => 323
	i32 3471940407, ; 574: System.ComponentModel.TypeConverter.dll => 0xcef19b37 => 17
	i32 3476120550, ; 575: Mono.Android => 0xcf3163e6 => 171
	i32 3479583265, ; 576: ru/Microsoft.Maui.Controls.resources.dll => 0xcf663a21 => 336
	i32 3484440000, ; 577: ro\Microsoft.Maui.Controls.resources => 0xcfb055c0 => 335
	i32 3485117614, ; 578: System.Text.Json.dll => 0xcfbaacae => 137
	i32 3486566296, ; 579: System.Transactions => 0xcfd0c798 => 150
	i32 3493954962, ; 580: Xamarin.AndroidX.Concurrent.Futures.dll => 0xd0418592 => 242
	i32 3494395880, ; 581: Xamarin.GooglePlayServices.Location.dll => 0xd0483fe8 => 303
	i32 3499097210, ; 582: Google.Protobuf.dll => 0xd08ffc7a => 186
	i32 3509114376, ; 583: System.Xml.Linq => 0xd128d608 => 155
	i32 3515174580, ; 584: System.Security.dll => 0xd1854eb4 => 130
	i32 3530912306, ; 585: System.Configuration => 0xd2757232 => 19
	i32 3539954161, ; 586: System.Net.HttpListener => 0xd2ff69f1 => 65
	i32 3560100363, ; 587: System.Threading.Timer => 0xd432d20b => 147
	i32 3570554715, ; 588: System.IO.FileSystem.AccessControl => 0xd4d2575b => 47
	i32 3580758918, ; 589: zh-HK\Microsoft.Maui.Controls.resources => 0xd56e0b86 => 343
	i32 3597029428, ; 590: Xamarin.Android.Glide.GifDecoder.dll => 0xd6665034 => 228
	i32 3598063517, ; 591: Google.Cloud.Firestore.V1 => 0xd676179d => 183
	i32 3598340787, ; 592: System.Net.WebSockets.Client => 0xd67a52b3 => 79
	i32 3608519521, ; 593: System.Linq.dll => 0xd715a361 => 61
	i32 3612435020, ; 594: System.Management.dll => 0xd751624c => 224
	i32 3624195450, ; 595: System.Runtime.InteropServices.RuntimeInformation => 0xd804d57a => 106
	i32 3627220390, ; 596: Xamarin.AndroidX.Print.dll => 0xd832fda6 => 277
	i32 3633644679, ; 597: Xamarin.AndroidX.Annotation.Experimental => 0xd8950487 => 232
	i32 3638274909, ; 598: System.IO.FileSystem.Primitives.dll => 0xd8dbab5d => 49
	i32 3641597786, ; 599: Xamarin.AndroidX.Lifecycle.LiveData.Core => 0xd90e5f5a => 263
	i32 3643446276, ; 600: tr\Microsoft.Maui.Controls.resources => 0xd92a9404 => 340
	i32 3643854240, ; 601: Xamarin.AndroidX.Navigation.Fragment => 0xd930cda0 => 274
	i32 3645089577, ; 602: System.ComponentModel.DataAnnotations => 0xd943a729 => 14
	i32 3645630983, ; 603: Google.Protobuf => 0xd94bea07 => 186
	i32 3655481159, ; 604: Firebase.Storage => 0xd9e23747 => 175
	i32 3657292374, ; 605: Microsoft.Extensions.Configuration.Abstractions.dll => 0xd9fdda56 => 194
	i32 3660523487, ; 606: System.Net.NetworkInformation => 0xda2f27df => 68
	i32 3672681054, ; 607: Mono.Android.dll => 0xdae8aa5e => 171
	i32 3676670898, ; 608: Microsoft.Maui.Controls.HotReload.Forms => 0xdb258bb2 => 346
	i32 3682565725, ; 609: Xamarin.AndroidX.Browser => 0xdb7f7e5d => 238
	i32 3684561358, ; 610: Xamarin.AndroidX.Concurrent.Futures => 0xdb9df1ce => 242
	i32 3697841164, ; 611: zh-Hant/Microsoft.Maui.Controls.resources.dll => 0xdc68940c => 345
	i32 3700866549, ; 612: System.Net.WebProxy.dll => 0xdc96bdf5 => 78
	i32 3706696989, ; 613: Xamarin.AndroidX.Core.Core.Ktx.dll => 0xdcefb51d => 247
	i32 3716563718, ; 614: System.Runtime.Intrinsics => 0xdd864306 => 108
	i32 3718780102, ; 615: Xamarin.AndroidX.Annotation => 0xdda814c6 => 231
	i32 3724971120, ; 616: Xamarin.AndroidX.Navigation.Common.dll => 0xde068c70 => 273
	i32 3732100267, ; 617: System.Net.NameResolution => 0xde7354ab => 67
	i32 3737834244, ; 618: System.Net.Http.Json.dll => 0xdecad304 => 63
	i32 3748608112, ; 619: System.Diagnostics.DiagnosticSource => 0xdf6f3870 => 27
	i32 3751444290, ; 620: System.Xml.XPath => 0xdf9a7f42 => 160
	i32 3754567612, ; 621: SQLitePCLRaw.provider.e_sqlite3 => 0xdfca27bc => 221
	i32 3757995660, ; 622: Google.Cloud.Location.dll => 0xdffe768c => 184
	i32 3780242343, ; 623: Prism.Container.Abstractions => 0xe151eba7 => 210
	i32 3786282454, ; 624: Xamarin.AndroidX.Collection => 0xe1ae15d6 => 240
	i32 3792276235, ; 625: System.Collections.NonGeneric => 0xe2098b0b => 10
	i32 3793997468, ; 626: Google.Apis.Auth.dll => 0xe223ce9c => 180
	i32 3800979733, ; 627: Microsoft.Maui.Controls.Compatibility => 0xe28e5915 => 202
	i32 3802395368, ; 628: System.Collections.Specialized.dll => 0xe2a3f2e8 => 11
	i32 3819260425, ; 629: System.Net.WebProxy => 0xe3a54a09 => 78
	i32 3823082795, ; 630: System.Security.Cryptography.dll => 0xe3df9d2b => 126
	i32 3829621856, ; 631: System.Numerics.dll => 0xe4436460 => 83
	i32 3841636137, ; 632: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 0xe4fab729 => 196
	i32 3844307129, ; 633: System.Net.Mail.dll => 0xe52378b9 => 66
	i32 3849253459, ; 634: System.Runtime.InteropServices.dll => 0xe56ef253 => 107
	i32 3870376305, ; 635: System.Net.HttpListener.dll => 0xe6b14171 => 65
	i32 3873536506, ; 636: System.Security.Principal => 0xe6e179fa => 128
	i32 3875112723, ; 637: System.Security.Cryptography.Encoding.dll => 0xe6f98713 => 122
	i32 3876362041, ; 638: SQLite-net => 0xe70c9739 => 217
	i32 3885497537, ; 639: System.Net.WebHeaderCollection.dll => 0xe797fcc1 => 77
	i32 3885922214, ; 640: Xamarin.AndroidX.Transition.dll => 0xe79e77a6 => 288
	i32 3888767677, ; 641: Xamarin.AndroidX.ProfileInstaller.ProfileInstaller => 0xe7c9e2bd => 278
	i32 3889960447, ; 642: zh-Hans/Microsoft.Maui.Controls.resources.dll => 0xe7dc15ff => 344
	i32 3896106733, ; 643: System.Collections.Concurrent.dll => 0xe839deed => 8
	i32 3896760992, ; 644: Xamarin.AndroidX.Core.dll => 0xe843daa0 => 246
	i32 3901907137, ; 645: Microsoft.VisualBasic.Core.dll => 0xe89260c1 => 2
	i32 3914643371, ; 646: Prism.DryIoc.Maui.dll => 0xe954b7ab => 213
	i32 3920810846, ; 647: System.IO.Compression.FileSystem.dll => 0xe9b2d35e => 44
	i32 3921031405, ; 648: Xamarin.AndroidX.VersionedParcelable.dll => 0xe9b630ed => 291
	i32 3928044579, ; 649: System.Xml.ReaderWriter => 0xea213423 => 156
	i32 3929187773, ; 650: Firebase.Storage.dll => 0xea32a5bd => 175
	i32 3930554604, ; 651: System.Security.Principal.dll => 0xea4780ec => 128
	i32 3931092270, ; 652: Xamarin.AndroidX.Navigation.UI => 0xea4fb52e => 276
	i32 3945713374, ; 653: System.Data.DataSetExtensions.dll => 0xeb2ecede => 23
	i32 3953953790, ; 654: System.Text.Encoding.CodePages => 0xebac8bfe => 133
	i32 3955647286, ; 655: Xamarin.AndroidX.AppCompat.dll => 0xebc66336 => 234
	i32 3959773229, ; 656: Xamarin.AndroidX.Lifecycle.Process => 0xec05582d => 265
	i32 3967165417, ; 657: Xamarin.GooglePlayServices.Location => 0xec7623e9 => 303
	i32 3970018735, ; 658: Xamarin.GooglePlayServices.Tasks.dll => 0xeca1adaf => 304
	i32 3980434154, ; 659: th/Microsoft.Maui.Controls.resources.dll => 0xed409aea => 339
	i32 3987592930, ; 660: he/Microsoft.Maui.Controls.resources.dll => 0xedadd6e2 => 321
	i32 4003436829, ; 661: System.Diagnostics.Process.dll => 0xee9f991d => 29
	i32 4015948917, ; 662: Xamarin.AndroidX.Annotation.Jvm.dll => 0xef5e8475 => 233
	i32 4024013275, ; 663: Firebase.Auth => 0xefd991db => 174
	i32 4025784931, ; 664: System.Memory => 0xeff49a63 => 62
	i32 4046471985, ; 665: Microsoft.Maui.Controls.Xaml.dll => 0xf1304331 => 204
	i32 4054681211, ; 666: System.Reflection.Emit.ILGeneration => 0xf1ad867b => 90
	i32 4056144661, ; 667: Google.Cloud.Location => 0xf1c3db15 => 184
	i32 4059682726, ; 668: Google.Apis.dll => 0xf1f9d7a6 => 179
	i32 4068434129, ; 669: System.Private.Xml.Linq.dll => 0xf27f60d1 => 87
	i32 4073602200, ; 670: System.Threading.dll => 0xf2ce3c98 => 148
	i32 4082882467, ; 671: Google.Apis.Auth => 0xf35bd7a3 => 180
	i32 4094352644, ; 672: Microsoft.Maui.Essentials.dll => 0xf40add04 => 206
	i32 4099507663, ; 673: System.Drawing.dll => 0xf45985cf => 36
	i32 4100113165, ; 674: System.Private.Uri => 0xf462c30d => 86
	i32 4101593132, ; 675: Xamarin.AndroidX.Emoji2 => 0xf479582c => 254
	i32 4102112229, ; 676: pt/Microsoft.Maui.Controls.resources.dll => 0xf48143e5 => 334
	i32 4125707920, ; 677: ms/Microsoft.Maui.Controls.resources.dll => 0xf5e94e90 => 329
	i32 4126470640, ; 678: Microsoft.Extensions.DependencyInjection => 0xf5f4f1f0 => 195
	i32 4127667938, ; 679: System.IO.FileSystem.Watcher => 0xf60736e2 => 50
	i32 4130442656, ; 680: System.AppContext => 0xf6318da0 => 6
	i32 4147896353, ; 681: System.Reflection.Emit.ILGeneration.dll => 0xf73be021 => 90
	i32 4150914736, ; 682: uk\Microsoft.Maui.Controls.resources => 0xf769eeb0 => 341
	i32 4151237749, ; 683: System.Core => 0xf76edc75 => 21
	i32 4159265925, ; 684: System.Xml.XmlSerializer => 0xf7e95c85 => 162
	i32 4161255271, ; 685: System.Reflection.TypeExtensions => 0xf807b767 => 96
	i32 4164802419, ; 686: System.IO.FileSystem.Watcher.dll => 0xf83dd773 => 50
	i32 4165582995, ; 687: DryIoc.dll => 0xf849c093 => 173
	i32 4181436372, ; 688: System.Runtime.Serialization.Primitives => 0xf93ba7d4 => 113
	i32 4182413190, ; 689: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 0xf94a8f86 => 270
	i32 4182880526, ; 690: Microsoft.VisualStudio.DesignTools.MobileTapContracts => 0xf951b10e => 347
	i32 4184000013, ; 691: PropertyChanged.dll => 0xf962c60d => 216
	i32 4185676441, ; 692: System.Security => 0xf97c5a99 => 130
	i32 4196529839, ; 693: System.Net.WebClient.dll => 0xfa21f6af => 76
	i32 4213026141, ; 694: System.Diagnostics.DiagnosticSource.dll => 0xfb1dad5d => 27
	i32 4256097574, ; 695: Xamarin.AndroidX.Core.Core.Ktx => 0xfdaee526 => 247
	i32 4258378803, ; 696: Xamarin.AndroidX.Lifecycle.ViewModel.Ktx => 0xfdd1b433 => 269
	i32 4260525087, ; 697: System.Buffers => 0xfdf2741f => 7
	i32 4271975918, ; 698: Microsoft.Maui.Controls.dll => 0xfea12dee => 203
	i32 4274976490, ; 699: System.Runtime.Numerics => 0xfecef6ea => 110
	i32 4292120959, ; 700: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 0xffd4917f => 270
	i32 4294763496 ; 701: Xamarin.AndroidX.ExifInterface.dll => 0xfffce3e8 => 256
], align 4

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [702 x i32] [
	i32 68, ; 0
	i32 67, ; 1
	i32 108, ; 2
	i32 223, ; 3
	i32 266, ; 4
	i32 300, ; 5
	i32 48, ; 6
	i32 208, ; 7
	i32 80, ; 8
	i32 145, ; 9
	i32 30, ; 10
	i32 345, ; 11
	i32 124, ; 12
	i32 207, ; 13
	i32 102, ; 14
	i32 284, ; 15
	i32 107, ; 16
	i32 284, ; 17
	i32 139, ; 18
	i32 308, ; 19
	i32 77, ; 20
	i32 124, ; 21
	i32 13, ; 22
	i32 240, ; 23
	i32 132, ; 24
	i32 286, ; 25
	i32 151, ; 26
	i32 342, ; 27
	i32 343, ; 28
	i32 18, ; 29
	i32 238, ; 30
	i32 26, ; 31
	i32 260, ; 32
	i32 1, ; 33
	i32 59, ; 34
	i32 42, ; 35
	i32 91, ; 36
	i32 243, ; 37
	i32 147, ; 38
	i32 262, ; 39
	i32 259, ; 40
	i32 314, ; 41
	i32 54, ; 42
	i32 69, ; 43
	i32 342, ; 44
	i32 229, ; 45
	i32 83, ; 46
	i32 327, ; 47
	i32 261, ; 48
	i32 220, ; 49
	i32 326, ; 50
	i32 131, ; 51
	i32 191, ; 52
	i32 55, ; 53
	i32 189, ; 54
	i32 149, ; 55
	i32 74, ; 56
	i32 145, ; 57
	i32 62, ; 58
	i32 190, ; 59
	i32 146, ; 60
	i32 350, ; 61
	i32 165, ; 62
	i32 338, ; 63
	i32 244, ; 64
	i32 12, ; 65
	i32 257, ; 66
	i32 125, ; 67
	i32 152, ; 68
	i32 113, ; 69
	i32 166, ; 70
	i32 164, ; 71
	i32 259, ; 72
	i32 272, ; 73
	i32 84, ; 74
	i32 325, ; 75
	i32 212, ; 76
	i32 319, ; 77
	i32 209, ; 78
	i32 201, ; 79
	i32 150, ; 80
	i32 308, ; 81
	i32 60, ; 82
	i32 197, ; 83
	i32 51, ; 84
	i32 103, ; 85
	i32 114, ; 86
	i32 192, ; 87
	i32 40, ; 88
	i32 297, ; 89
	i32 295, ; 90
	i32 120, ; 91
	i32 333, ; 92
	i32 52, ; 93
	i32 44, ; 94
	i32 119, ; 95
	i32 249, ; 96
	i32 331, ; 97
	i32 255, ; 98
	i32 81, ; 99
	i32 183, ; 100
	i32 136, ; 101
	i32 291, ; 102
	i32 236, ; 103
	i32 8, ; 104
	i32 73, ; 105
	i32 313, ; 106
	i32 155, ; 107
	i32 310, ; 108
	i32 154, ; 109
	i32 92, ; 110
	i32 305, ; 111
	i32 45, ; 112
	i32 328, ; 113
	i32 316, ; 114
	i32 309, ; 115
	i32 109, ; 116
	i32 129, ; 117
	i32 218, ; 118
	i32 25, ; 119
	i32 226, ; 120
	i32 72, ; 121
	i32 55, ; 122
	i32 46, ; 123
	i32 337, ; 124
	i32 200, ; 125
	i32 250, ; 126
	i32 22, ; 127
	i32 264, ; 128
	i32 86, ; 129
	i32 43, ; 130
	i32 160, ; 131
	i32 71, ; 132
	i32 277, ; 133
	i32 3, ; 134
	i32 42, ; 135
	i32 63, ; 136
	i32 16, ; 137
	i32 53, ; 138
	i32 340, ; 139
	i32 300, ; 140
	i32 105, ; 141
	i32 208, ; 142
	i32 309, ; 143
	i32 298, ; 144
	i32 261, ; 145
	i32 34, ; 146
	i32 158, ; 147
	i32 85, ; 148
	i32 32, ; 149
	i32 12, ; 150
	i32 51, ; 151
	i32 56, ; 152
	i32 281, ; 153
	i32 0, ; 154
	i32 36, ; 155
	i32 196, ; 156
	i32 315, ; 157
	i32 299, ; 158
	i32 234, ; 159
	i32 35, ; 160
	i32 58, ; 161
	i32 176, ; 162
	i32 268, ; 163
	i32 187, ; 164
	i32 17, ; 165
	i32 306, ; 166
	i32 164, ; 167
	i32 328, ; 168
	i32 267, ; 169
	i32 199, ; 170
	i32 294, ; 171
	i32 215, ; 172
	i32 334, ; 173
	i32 153, ; 174
	i32 290, ; 175
	i32 275, ; 176
	i32 190, ; 177
	i32 332, ; 178
	i32 236, ; 179
	i32 29, ; 180
	i32 52, ; 181
	i32 330, ; 182
	i32 295, ; 183
	i32 5, ; 184
	i32 314, ; 185
	i32 285, ; 186
	i32 289, ; 187
	i32 241, ; 188
	i32 310, ; 189
	i32 233, ; 190
	i32 219, ; 191
	i32 252, ; 192
	i32 85, ; 193
	i32 294, ; 194
	i32 61, ; 195
	i32 112, ; 196
	i32 57, ; 197
	i32 344, ; 198
	i32 281, ; 199
	i32 99, ; 200
	i32 19, ; 201
	i32 245, ; 202
	i32 0, ; 203
	i32 214, ; 204
	i32 111, ; 205
	i32 101, ; 206
	i32 102, ; 207
	i32 312, ; 208
	i32 104, ; 209
	i32 298, ; 210
	i32 188, ; 211
	i32 71, ; 212
	i32 38, ; 213
	i32 32, ; 214
	i32 103, ; 215
	i32 73, ; 216
	i32 318, ; 217
	i32 9, ; 218
	i32 123, ; 219
	i32 46, ; 220
	i32 235, ; 221
	i32 201, ; 222
	i32 9, ; 223
	i32 43, ; 224
	i32 4, ; 225
	i32 282, ; 226
	i32 322, ; 227
	i32 317, ; 228
	i32 209, ; 229
	i32 31, ; 230
	i32 138, ; 231
	i32 92, ; 232
	i32 93, ; 233
	i32 337, ; 234
	i32 49, ; 235
	i32 141, ; 236
	i32 112, ; 237
	i32 214, ; 238
	i32 140, ; 239
	i32 251, ; 240
	i32 115, ; 241
	i32 299, ; 242
	i32 157, ; 243
	i32 346, ; 244
	i32 76, ; 245
	i32 79, ; 246
	i32 271, ; 247
	i32 37, ; 248
	i32 293, ; 249
	i32 255, ; 250
	i32 248, ; 251
	i32 64, ; 252
	i32 138, ; 253
	i32 15, ; 254
	i32 116, ; 255
	i32 287, ; 256
	i32 296, ; 257
	i32 173, ; 258
	i32 243, ; 259
	i32 48, ; 260
	i32 70, ; 261
	i32 80, ; 262
	i32 126, ; 263
	i32 94, ; 264
	i32 121, ; 265
	i32 307, ; 266
	i32 26, ; 267
	i32 220, ; 268
	i32 211, ; 269
	i32 264, ; 270
	i32 97, ; 271
	i32 28, ; 272
	i32 239, ; 273
	i32 335, ; 274
	i32 313, ; 275
	i32 149, ; 276
	i32 169, ; 277
	i32 4, ; 278
	i32 98, ; 279
	i32 33, ; 280
	i32 93, ; 281
	i32 286, ; 282
	i32 197, ; 283
	i32 21, ; 284
	i32 41, ; 285
	i32 170, ; 286
	i32 189, ; 287
	i32 329, ; 288
	i32 257, ; 289
	i32 321, ; 290
	i32 192, ; 291
	i32 271, ; 292
	i32 306, ; 293
	i32 296, ; 294
	i32 276, ; 295
	i32 2, ; 296
	i32 134, ; 297
	i32 111, ; 298
	i32 348, ; 299
	i32 198, ; 300
	i32 341, ; 301
	i32 226, ; 302
	i32 338, ; 303
	i32 58, ; 304
	i32 95, ; 305
	i32 320, ; 306
	i32 39, ; 307
	i32 237, ; 308
	i32 348, ; 309
	i32 25, ; 310
	i32 94, ; 311
	i32 89, ; 312
	i32 188, ; 313
	i32 99, ; 314
	i32 302, ; 315
	i32 10, ; 316
	i32 222, ; 317
	i32 87, ; 318
	i32 100, ; 319
	i32 283, ; 320
	i32 193, ; 321
	i32 307, ; 322
	i32 228, ; 323
	i32 317, ; 324
	i32 7, ; 325
	i32 215, ; 326
	i32 268, ; 327
	i32 312, ; 328
	i32 225, ; 329
	i32 88, ; 330
	i32 263, ; 331
	i32 154, ; 332
	i32 316, ; 333
	i32 212, ; 334
	i32 223, ; 335
	i32 33, ; 336
	i32 116, ; 337
	i32 82, ; 338
	i32 221, ; 339
	i32 349, ; 340
	i32 20, ; 341
	i32 301, ; 342
	i32 11, ; 343
	i32 162, ; 344
	i32 3, ; 345
	i32 205, ; 346
	i32 324, ; 347
	i32 222, ; 348
	i32 200, ; 349
	i32 198, ; 350
	i32 84, ; 351
	i32 311, ; 352
	i32 64, ; 353
	i32 326, ; 354
	i32 174, ; 355
	i32 290, ; 356
	i32 143, ; 357
	i32 272, ; 358
	i32 157, ; 359
	i32 41, ; 360
	i32 117, ; 361
	i32 194, ; 362
	i32 227, ; 363
	i32 320, ; 364
	i32 279, ; 365
	i32 131, ; 366
	i32 75, ; 367
	i32 66, ; 368
	i32 330, ; 369
	i32 172, ; 370
	i32 231, ; 371
	i32 143, ; 372
	i32 106, ; 373
	i32 151, ; 374
	i32 70, ; 375
	i32 156, ; 376
	i32 193, ; 377
	i32 121, ; 378
	i32 127, ; 379
	i32 325, ; 380
	i32 185, ; 381
	i32 152, ; 382
	i32 254, ; 383
	i32 347, ; 384
	i32 141, ; 385
	i32 241, ; 386
	i32 322, ; 387
	i32 20, ; 388
	i32 14, ; 389
	i32 182, ; 390
	i32 135, ; 391
	i32 75, ; 392
	i32 59, ; 393
	i32 218, ; 394
	i32 244, ; 395
	i32 167, ; 396
	i32 168, ; 397
	i32 203, ; 398
	i32 15, ; 399
	i32 74, ; 400
	i32 177, ; 401
	i32 6, ; 402
	i32 23, ; 403
	i32 266, ; 404
	i32 225, ; 405
	i32 91, ; 406
	i32 323, ; 407
	i32 1, ; 408
	i32 136, ; 409
	i32 267, ; 410
	i32 289, ; 411
	i32 134, ; 412
	i32 69, ; 413
	i32 146, ; 414
	i32 332, ; 415
	i32 311, ; 416
	i32 258, ; 417
	i32 199, ; 418
	i32 88, ; 419
	i32 96, ; 420
	i32 248, ; 421
	i32 253, ; 422
	i32 327, ; 423
	i32 31, ; 424
	i32 45, ; 425
	i32 262, ; 426
	i32 182, ; 427
	i32 227, ; 428
	i32 109, ; 429
	i32 158, ; 430
	i32 35, ; 431
	i32 22, ; 432
	i32 114, ; 433
	i32 57, ; 434
	i32 287, ; 435
	i32 144, ; 436
	i32 118, ; 437
	i32 120, ; 438
	i32 110, ; 439
	i32 229, ; 440
	i32 139, ; 441
	i32 235, ; 442
	i32 54, ; 443
	i32 178, ; 444
	i32 105, ; 445
	i32 333, ; 446
	i32 178, ; 447
	i32 204, ; 448
	i32 205, ; 449
	i32 133, ; 450
	i32 216, ; 451
	i32 305, ; 452
	i32 292, ; 453
	i32 280, ; 454
	i32 339, ; 455
	i32 258, ; 456
	i32 207, ; 457
	i32 159, ; 458
	i32 318, ; 459
	i32 245, ; 460
	i32 163, ; 461
	i32 132, ; 462
	i32 280, ; 463
	i32 161, ; 464
	i32 331, ; 465
	i32 269, ; 466
	i32 185, ; 467
	i32 301, ; 468
	i32 140, ; 469
	i32 292, ; 470
	i32 288, ; 471
	i32 169, ; 472
	i32 206, ; 473
	i32 230, ; 474
	i32 297, ; 475
	i32 40, ; 476
	i32 256, ; 477
	i32 81, ; 478
	i32 181, ; 479
	i32 224, ; 480
	i32 56, ; 481
	i32 37, ; 482
	i32 97, ; 483
	i32 166, ; 484
	i32 172, ; 485
	i32 176, ; 486
	i32 293, ; 487
	i32 82, ; 488
	i32 232, ; 489
	i32 98, ; 490
	i32 30, ; 491
	i32 159, ; 492
	i32 18, ; 493
	i32 127, ; 494
	i32 119, ; 495
	i32 252, ; 496
	i32 283, ; 497
	i32 179, ; 498
	i32 265, ; 499
	i32 285, ; 500
	i32 165, ; 501
	i32 260, ; 502
	i32 350, ; 503
	i32 282, ; 504
	i32 273, ; 505
	i32 304, ; 506
	i32 170, ; 507
	i32 16, ; 508
	i32 144, ; 509
	i32 324, ; 510
	i32 125, ; 511
	i32 118, ; 512
	i32 38, ; 513
	i32 211, ; 514
	i32 191, ; 515
	i32 115, ; 516
	i32 213, ; 517
	i32 47, ; 518
	i32 142, ; 519
	i32 117, ; 520
	i32 34, ; 521
	i32 187, ; 522
	i32 95, ; 523
	i32 53, ; 524
	i32 274, ; 525
	i32 129, ; 526
	i32 153, ; 527
	i32 177, ; 528
	i32 24, ; 529
	i32 161, ; 530
	i32 251, ; 531
	i32 210, ; 532
	i32 349, ; 533
	i32 148, ; 534
	i32 104, ; 535
	i32 302, ; 536
	i32 89, ; 537
	i32 239, ; 538
	i32 60, ; 539
	i32 142, ; 540
	i32 100, ; 541
	i32 5, ; 542
	i32 13, ; 543
	i32 217, ; 544
	i32 122, ; 545
	i32 135, ; 546
	i32 28, ; 547
	i32 319, ; 548
	i32 72, ; 549
	i32 249, ; 550
	i32 24, ; 551
	i32 237, ; 552
	i32 278, ; 553
	i32 275, ; 554
	i32 336, ; 555
	i32 137, ; 556
	i32 219, ; 557
	i32 230, ; 558
	i32 246, ; 559
	i32 168, ; 560
	i32 279, ; 561
	i32 315, ; 562
	i32 101, ; 563
	i32 123, ; 564
	i32 250, ; 565
	i32 195, ; 566
	i32 163, ; 567
	i32 167, ; 568
	i32 253, ; 569
	i32 39, ; 570
	i32 202, ; 571
	i32 181, ; 572
	i32 323, ; 573
	i32 17, ; 574
	i32 171, ; 575
	i32 336, ; 576
	i32 335, ; 577
	i32 137, ; 578
	i32 150, ; 579
	i32 242, ; 580
	i32 303, ; 581
	i32 186, ; 582
	i32 155, ; 583
	i32 130, ; 584
	i32 19, ; 585
	i32 65, ; 586
	i32 147, ; 587
	i32 47, ; 588
	i32 343, ; 589
	i32 228, ; 590
	i32 183, ; 591
	i32 79, ; 592
	i32 61, ; 593
	i32 224, ; 594
	i32 106, ; 595
	i32 277, ; 596
	i32 232, ; 597
	i32 49, ; 598
	i32 263, ; 599
	i32 340, ; 600
	i32 274, ; 601
	i32 14, ; 602
	i32 186, ; 603
	i32 175, ; 604
	i32 194, ; 605
	i32 68, ; 606
	i32 171, ; 607
	i32 346, ; 608
	i32 238, ; 609
	i32 242, ; 610
	i32 345, ; 611
	i32 78, ; 612
	i32 247, ; 613
	i32 108, ; 614
	i32 231, ; 615
	i32 273, ; 616
	i32 67, ; 617
	i32 63, ; 618
	i32 27, ; 619
	i32 160, ; 620
	i32 221, ; 621
	i32 184, ; 622
	i32 210, ; 623
	i32 240, ; 624
	i32 10, ; 625
	i32 180, ; 626
	i32 202, ; 627
	i32 11, ; 628
	i32 78, ; 629
	i32 126, ; 630
	i32 83, ; 631
	i32 196, ; 632
	i32 66, ; 633
	i32 107, ; 634
	i32 65, ; 635
	i32 128, ; 636
	i32 122, ; 637
	i32 217, ; 638
	i32 77, ; 639
	i32 288, ; 640
	i32 278, ; 641
	i32 344, ; 642
	i32 8, ; 643
	i32 246, ; 644
	i32 2, ; 645
	i32 213, ; 646
	i32 44, ; 647
	i32 291, ; 648
	i32 156, ; 649
	i32 175, ; 650
	i32 128, ; 651
	i32 276, ; 652
	i32 23, ; 653
	i32 133, ; 654
	i32 234, ; 655
	i32 265, ; 656
	i32 303, ; 657
	i32 304, ; 658
	i32 339, ; 659
	i32 321, ; 660
	i32 29, ; 661
	i32 233, ; 662
	i32 174, ; 663
	i32 62, ; 664
	i32 204, ; 665
	i32 90, ; 666
	i32 184, ; 667
	i32 179, ; 668
	i32 87, ; 669
	i32 148, ; 670
	i32 180, ; 671
	i32 206, ; 672
	i32 36, ; 673
	i32 86, ; 674
	i32 254, ; 675
	i32 334, ; 676
	i32 329, ; 677
	i32 195, ; 678
	i32 50, ; 679
	i32 6, ; 680
	i32 90, ; 681
	i32 341, ; 682
	i32 21, ; 683
	i32 162, ; 684
	i32 96, ; 685
	i32 50, ; 686
	i32 173, ; 687
	i32 113, ; 688
	i32 270, ; 689
	i32 347, ; 690
	i32 216, ; 691
	i32 130, ; 692
	i32 76, ; 693
	i32 27, ; 694
	i32 247, ; 695
	i32 269, ; 696
	i32 7, ; 697
	i32 203, ; 698
	i32 110, ; 699
	i32 270, ; 700
	i32 256 ; 701
], align 4

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 4

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 4

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 0, ; id 0x0; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 4

; Functions

; Function attributes: "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.str.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 4, !tbaa !3
	ret void
}

; Strings
@.str.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 1

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+armv7-a,+d32,+dsp,+fp64,+neon,+vfp2,+vfp2sp,+vfp3,+vfp3d16,+vfp3d16sp,+vfp3sp,-aes,-fp-armv8,-fp-armv8d16,-fp-armv8d16sp,-fp-armv8sp,-fp16,-fp16fml,-fullfp16,-sha2,-thumb-mode,-vfp4,-vfp4d16,-vfp4d16sp,-vfp4sp" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="generic" "target-features"="+armv7-a,+d32,+dsp,+fp64,+neon,+vfp2,+vfp2sp,+vfp3,+vfp3d16,+vfp3d16sp,+vfp3sp,-aes,-fp-armv8,-fp-armv8d16,-fp-armv8d16sp,-fp-armv8sp,-fp16,-fp16fml,-fullfp16,-sha2,-thumb-mode,-vfp4,-vfp4d16,-vfp4d16sp,-vfp4sp" }

; Metadata
!llvm.module.flags = !{!0, !1, !7}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!"Xamarin.Android remotes/origin/release/8.0.4xx @ 82d8938cf80f6d5fa6c28529ddfbdb753d805ab4"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"min_enum_size", i32 4}
