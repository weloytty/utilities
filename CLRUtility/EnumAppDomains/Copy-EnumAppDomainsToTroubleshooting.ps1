[CmdletBinding()]
param([string[]]$ComputerName)
begin{
    $scriptDir = Split-Path $MyInvocation.MyCommand.Source
    push-location
    set-location $scriptDir
}
process{
    foreach ($server in $ComputerName) {
        $serverPath = "\\$server\troubleshooting\"
        if (Test-Connection -ComputerName $server -Quiet -Count 1) {
            if (-not (Test-Path -Path $serverPath -PathType Container)) {
                Write-Output "Can't find $serverPath"
            } else {
                if (-not (Test-Path $(Join-Path $serverPath "utils") -PathType Container)) {
                    Write-Output "Creating utils on $serverPath"
                    New-Item -Path $(Join-Path $serverPath "utils")
                }
                & robocopy.exe . "$serverPath" *.*
            }
        }else{Write-Output "Can't connect to $server"}
    }
}

end{pop-location}