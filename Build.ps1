clear

$configuration = "Release" 
$specialRevision = "0"

&"$env:windir\Microsoft.NET\Framework\v4.0.30319\msbuild" Build\Build.proj /p:Configuration="$configuration" /p:Revision="$specialRevision"