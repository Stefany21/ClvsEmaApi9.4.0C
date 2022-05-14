$web = New-Object System.Net.WebClient
$str = $web.DownloadString("https://superltposapi.clavisco.com/api/Connections/ConnectCompany?MappId=1")
$str # script para consumir api get y mantener conexion con sap activa