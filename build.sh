cd ./Matilda

if [ "$1" = "linux" ]
then
    wine ./Coco.exe Matilda.cs.ATG -namespace Matilda
else
    mono ./Coco.exe Matilda.cs.ATG -namespace Matilda
fi

dotnet build matilda.csproj -c Release