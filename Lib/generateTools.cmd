@echo off
cd /d "D:\сно╥\YZKCardGame\Lib\proto"
"D:\сно╥\YZKCardGame\Tools\ProtoBuf 1.0.0.280\protogen.exe" -i:"NetMessage.proto" -o:"..\..\YZKCardGame Unity2022\Assets\Script\Net\ProtoBuf\NetMessage.cs" -p:datacontract -q
pause