
$daxApSource = "C:\Projects\CSG\Inbound\DAX\Archivesf\"
$csgDaxInbound = "C:\Projects\CSG\Inbound\DAX"

$ErrorActionPreference = "Stop"

try {
    $files = Get-ChildItem -Path $daxApSource "DAXAP_BPS*_ClmStmt_*.csv" -File

    if ($files.Length -eq 0) {
        Write-Host "No files found."
    }

    foreach($file in $files) {
        if ($file.LastWriteTime -gt (Get-Date).AddDays(-1).Date) {
            Write-Host "Found:" $file.Name " LastWriteTime:" $file.LastWriteTime 

            $LastLength = -1
            $NewLength = $file.length
            while ($NewLength -ne $LastLength) { 
                Write-Host "File size changed last length:" $LastLength " new length:" $NewLength

                $LastLength = $NewLength
                Write-Host "Sleeping to verify file complete..."
                Start-Sleep -Seconds 60

                $file = Get-Item $file.FullName
                $NewLength = $file.length
            }

            Write-Host "Copying:" $file.FullName " To:" $csgDaxInbound
            Copy-Item $file.FullName -Destination $csgDaxInbound

            Write-Host "Deleting:" $file.FullName 
            Remove-Item $file.FullName
        } else {
            Write-Host "Not processing:" $file.Name " LastWriteTime:" $file.LastWriteTime -ForegroundColor Red
        }
    }
}
catch {
    $errorInfo = $_
    Write-Host $errorInfo -ForegroundColor Red
}