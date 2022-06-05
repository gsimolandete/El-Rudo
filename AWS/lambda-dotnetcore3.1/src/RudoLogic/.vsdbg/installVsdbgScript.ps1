# Copyright (c) Microsoft. All rights reserved.

<#
.SYNOPSIS
Downloads the given $Version of vsdbg for the given $RuntimeID and installs it to the given $InstallPath

.DESCRIPTION
The following script will download vsdbg and install vsdbg, the .NET Core Debugger

.PARAMETER Version
Specifies the version of vsdbg to install. Can be 'latest', 'vs2022', 'vs2019', 'vs2017u5', 'vs2017u1', or a specific version string i.e. 15.0.25930.0

.PARAMETER RuntimeID
Specifies the .NET Runtime ID of the vsdbg that will be downloaded. Example: linux-x64. Defaults to win7-x64.

.Parameter InstallPath
Specifies the path where vsdbg will be installed. Defaults to the directory containing this script.

.INPUTS
None. You cannot pipe inputs to GetVsDbg.

.EXAMPLE
C:\PS> .\GetVsDbg.ps1 -Version latest -RuntimeID linux-x64 -InstallPath .\vsdbg

.LINK
For more information about using this script with Visual Studio Code see: https://github.com/OmniSharp/omnisharp-vscode/wiki/Attaching-to-remote-processes

For more information about using this script with Visual Studio see: https://github.com/Microsoft/MIEngine/wiki/Offroad-Debugging-of-.NET-Core-on-Linux---OSX-from-Visual-Studio

To report issues, see: https://github.com/omnisharp/omnisharp-vscode/issues
#>

Param (
    [Parameter(Mandatory=$true, ParameterSetName="ByName")]
    [string]
    [ValidateSet("latest", "vs2022", "vs2019", "vs2017u1", "vs2017u5")]
    $Version,

    [Parameter(Mandatory=$true, ParameterSetName="ByNumber")]
    [string]
    [ValidatePattern("\d+\.\d+\.\d+.*")]
    $VersionNumber,

    [Parameter(Mandatory=$false)]
    [string]
    $RuntimeID,

    [Parameter(Mandatory=$false)]
    [string]
    $InstallPath = (Split-Path -Path $MyInvocation.MyCommand.Definition)
)

$ErrorActionPreference="Stop"

# In a separate method to prevent locking zip files.
function DownloadAndExtract([string]$url, [string]$targetLocation) {
    Add-Type -assembly "System.IO.Compression.FileSystem"
    Add-Type -assembly "System.IO.Compression"

    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

    Try {
        $zipStream = (New-Object System.Net.WebClient).OpenRead($url)
    }
    Catch {
        Write-Host "Info: Opening stream failed, trying again with proxy settings."
        $proxy = [System.Net.WebRequest]::GetSystemWebProxy()
        $proxy.Credentials = [System.Net.CredentialCache]::DefaultNetworkCredentials
        $webClient = New-Object System.Net.WebClient
        $webClient.UseDefaultCredentials = $false
        $webClient.proxy = $proxy

        $zipStream = $webClient.OpenRead($url)
    }
    
    $zipArchive = New-Object System.IO.Compression.ZipArchive -ArgumentList $zipStream
    [System.IO.Compression.ZipFileExtensions]::ExtractToDirectory($zipArchive, $targetLocation)
    $zipArchive.Dispose()
    $zipStream.Dispose()
}

# Checks if the existing version is the latest version.
function IsLatest([string]$installationPath, [string]$runtimeId, [string]$version) {
    $SuccessRidFile = Join-Path -Path $installationPath -ChildPath "success_rid.txt"
    if (Test-Path $SuccessRidFile) {
        $LastRid = Get-Content -Path $SuccessRidFile
        if ($LastRid -ne $runtimeId) {
            return $false
        }
    } else {
        return $false
    }

    $SuccessVersionFile = Join-Path -Path $installationPath -ChildPath "success_version.txt"
    if (Test-Path $SuccessVersionFile) {
        $LastVersion = Get-Content -Path $SuccessVersionFile
        if ($LastVersion -ne $version) {
            return $false
        }
    } else {
        return $false
    }

    return $true
}

function WriteSuccessInfo([string]$installationPath, [string]$runtimeId, [string]$version) {
    $SuccessRidFile = Join-Path -Path $installationPath -ChildPath "success_rid.txt"
    $runtimeId | Out-File -Encoding ascii $SuccessRidFile

    $SuccessVersionFile = Join-Path -Path $installationPath -ChildPath "success_version.txt"
    $version | Out-File -Encoding ascii $SuccessVersionFile
}

$ExplitVersionNumberUsed = $false
if ($Version -eq "latest") {
    $VersionNumber = "17.2.10518.1"
} elseif ($Version -eq "vs2022") {
    $VersionNumber = "17.2.10518.1"
} elseif ($Version -eq "vs2019") {
    $VersionNumber = "17.2.10518.1"
} elseif ($Version -eq "vs2017u5") {
    $VersionNumber = "17.2.10518.1"
} elseif ($Version -eq "vs2017u1") {
    $VersionNumber = "15.1.10630.1"
} else {
    $ExplitVersionNumberUsed = $true
}
Write-Host "Info: Using vsdbg version '$VersionNumber'"

if (-not $RuntimeID) {
    $RuntimeID = "win7-x64"
} elseif (-not $ExplitVersionNumberUsed) {
    $legacyLinuxRuntimeIds = @{ 
        "debian.8-x64" = "";
        "rhel.7.2-x64" = "";
        "centos.7-x64" = "";
        "fedora.23-x64" = "";
        "opensuse.13.2-x64" = "";
        "ubuntu.14.04-x64" = "";
        "ubuntu.16.04-x64" = "";
        "ubuntu.16.10-x64" = "";
        "fedora.24-x64" = "";
        "opensuse.42.1-x64" = "";
    }

    # Remap the old distro-specific runtime ids unless the caller specified an exact build number.
    # We don't do this in the exact build number case so that old builds can be used.
    if ($legacyLinuxRuntimeIds.ContainsKey($RuntimeID.ToLowerInvariant())) {
        $RuntimeID = "linux-x64"
    }
}
Write-Host "Info: Using Runtime ID '$RuntimeID'"

# if we were given a relative path, assume its relative to the script directory and create an absolute path
if (-not([System.IO.Path]::IsPathRooted($InstallPath))) {
    $InstallPath = Join-Path -Path (Split-Path -Path $MyInvocation.MyCommand.Definition) -ChildPath $InstallPath
}

if (IsLatest $InstallPath $RuntimeID $VersionNumber) {
    Write-Host "Info: Latest version of VsDbg is present. Skipping downloads"
} else {
    if (Test-Path $InstallPath) {
        Write-Host "Info: $InstallPath exists, deleting."
        Remove-Item $InstallPath -Force -Recurse -ErrorAction Stop
    }
 
    $target = ("vsdbg-" + $VersionNumber).Replace('.','-') + "/vsdbg-" + $RuntimeID + ".zip"
    $url = "https://vsdebugger.azureedge.net/" + $target

    DownloadAndExtract $url $InstallPath

    WriteSuccessInfo $InstallPath $RuntimeID $VersionNumber
    Write-Host "Info: Successfully installed vsdbg at '$InstallPath'"
}

# SIG # Begin signature block
# MIIlmwYJKoZIhvcNAQcCoIIljDCCJYgCAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCC0nqFuI1YN1eAP
# HSMFbLNAH2GAORXaowOOXLHTFurmRaCCC3IwggT6MIID4qADAgECAhMzAAAEOfYf
# emdtoACvAAAAAAQ5MA0GCSqGSIb3DQEBCwUAMH4xCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25p
# bmcgUENBIDIwMTAwHhcNMjEwOTAyMTgyNTU4WhcNMjIwOTAxMTgyNTU4WjB0MQsw
# CQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9u
# ZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMR4wHAYDVQQDExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIB
# AQDYxnPFVXLjCNZotpu2pA/klQnh61TVmOwkp46L2lhfjh3H1JisbpZfdR7PSIOy
# thfERueQRQM4cYwlCHxZs2PJgVAWT1A09MgvyOnUu8+TP3rMJux8XpgfjbT1QY9W
# NvAV+9T/3+JaRgW+L/IarOJQ+fQx6fwoO8U1UDJykFo5fQIbgCGXO/uz69B0z6LE
# VrJP+qibVhromVIQ0vaip2Rh+EMlHNN3jDpuYJOfcI9iClLffv30NDVa7LNdr5S8
# 5uFW7WD6aVLd5Y4vytrD477um9drb3Xe/gXmBKUZ2JLMv+xZG39Xw/UbA1lQTN/t
# bof2MgifNoRRRRELlcOForTtAgMBAAGjggF5MIIBdTAfBgNVHSUEGDAWBgorBgEE
# AYI3PQYBBggrBgEFBQcDAzAdBgNVHQ4EFgQUxfAmBmr7eiyHypaAy6/f8G8lQsUw
# UAYDVR0RBEkwR6RFMEMxKTAnBgNVBAsTIE1pY3Jvc29mdCBPcGVyYXRpb25zIFB1
# ZXJ0byBSaWNvMRYwFAYDVQQFEw0yMzA4NjUrNDY3Mzk4MB8GA1UdIwQYMBaAFOb8
# X3u7IgBY5HJOtfQhdCMy5u+sMFYGA1UdHwRPME0wS6BJoEeGRWh0dHA6Ly9jcmwu
# bWljcm9zb2Z0LmNvbS9wa2kvY3JsL3Byb2R1Y3RzL01pY0NvZFNpZ1BDQV8yMDEw
# LTA3LTA2LmNybDBaBggrBgEFBQcBAQROMEwwSgYIKwYBBQUHMAKGPmh0dHA6Ly93
# d3cubWljcm9zb2Z0LmNvbS9wa2kvY2VydHMvTWljQ29kU2lnUENBXzIwMTAtMDct
# MDYuY3J0MAwGA1UdEwEB/wQCMAAwDQYJKoZIhvcNAQELBQADggEBAGaOsNHOxecF
# hmUQiipJkW1uEeTTuKdpftxfnqFzxAqNngYLPDHQb3Ja8CnFNwCN5BFh21p4TM15
# Pv1aO+HCA3mYRAexP5LM9mTTBEoC5WFMNVG+6x138G/BnafTHRIj5UjgZHWR3t2s
# /uWoNBRtTYVUKTdwuvh+2bCeJrEebuWi4cOOkHd3eBwaD+Dh/iJinmdUoYoAA8cN
# AnZ+4jsirVYsvnfHeYtzEPVUPFtRVsHSRhs+zMpm+66oju2d8z2HHS3Q+OVgbCXq
# BAg1c+BTzV9+9oaMXuq7klKeRNj1quZae0jisxP+fxQx3iWB7I8YVx0EmGg67aQS
# pjH84cst2PswggZwMIIEWKADAgECAgphDFJMAAAAAAADMA0GCSqGSIb3DQEBCwUA
# MIGIMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMH
# UmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMTIwMAYDVQQD
# EylNaWNyb3NvZnQgUm9vdCBDZXJ0aWZpY2F0ZSBBdXRob3JpdHkgMjAxMDAeFw0x
# MDA3MDYyMDQwMTdaFw0yNTA3MDYyMDUwMTdaMH4xCzAJBgNVBAYTAlVTMRMwEQYD
# VQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNy
# b3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jvc29mdCBDb2RlIFNpZ25p
# bmcgUENBIDIwMTAwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDpDmRQ
# eWe1xOP9CQBMnpSs91Zo6kTYz8VYT6mldnxtRbrTOZK0pB75+WWC5BfSj/1EnAjo
# ZZPOLFWEv30I4y4rqEErGLeiS25JTGsVB97R0sKJHnGUzbV/S7SvCNjMiNZrF5Q6
# k84mP+zm/jSYV9UdXUn2siou1YW7WT/4kLQrg3TKK7M7RuPwRknBF2ZUyRy9HcRV
# Yldy+Ge5JSA03l2mpZVeqyiAzdWynuUDtWPTshTIwciKJgpZfwfs/w7tgBI1TBKm
# vlJb9aba4IsLSHfWhUfVELnG6Krui2otBVxgxrQqW5wjHF9F4xoUHm83yxkzgGqJ
# TaNqZmN4k9Uwz5UfAgMBAAGjggHjMIIB3zAQBgkrBgEEAYI3FQEEAwIBADAdBgNV
# HQ4EFgQU5vxfe7siAFjkck619CF0IzLm76wwGQYJKwYBBAGCNxQCBAweCgBTAHUA
# YgBDAEEwCwYDVR0PBAQDAgGGMA8GA1UdEwEB/wQFMAMBAf8wHwYDVR0jBBgwFoAU
# 1fZWy4/oolxiaNE9lJBb186aGMQwVgYDVR0fBE8wTTBLoEmgR4ZFaHR0cDovL2Ny
# bC5taWNyb3NvZnQuY29tL3BraS9jcmwvcHJvZHVjdHMvTWljUm9vQ2VyQXV0XzIw
# MTAtMDYtMjMuY3JsMFoGCCsGAQUFBwEBBE4wTDBKBggrBgEFBQcwAoY+aHR0cDov
# L3d3dy5taWNyb3NvZnQuY29tL3BraS9jZXJ0cy9NaWNSb29DZXJBdXRfMjAxMC0w
# Ni0yMy5jcnQwgZ0GA1UdIASBlTCBkjCBjwYJKwYBBAGCNy4DMIGBMD0GCCsGAQUF
# BwIBFjFodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20vUEtJL2RvY3MvQ1BTL2RlZmF1
# bHQuaHRtMEAGCCsGAQUFBwICMDQeMiAdAEwAZQBnAGEAbABfAFAAbwBsAGkAYwB5
# AF8AUwB0AGEAdABlAG0AZQBuAHQALiAdMA0GCSqGSIb3DQEBCwUAA4ICAQAadO9X
# Tyl7xBaFeLhQ0yL8CZ2sgpf4NP8qLJeVEuXkv8+/k8jjNKnbgbjcHgC+0jVvr+V/
# eZV35QLU8evYzU4eG2GiwlojGvCMqGJRRWcI4z88HpP4MIUXyDlAptcOsyEp5aWh
# aYwik8x0mOehR0PyU6zADzBpf/7SJSBtb2HT3wfV2XIALGmGdj1R26Y5SMk3YW0H
# 3VMZy6fWYcK/4oOrD+Brm5XWfShRsIlKUaSabMi3H0oaDmmp19zBftFJcKq2rbty
# R2MX+qbWoqaG7KgQRJtjtrJpiQbHRoZ6GD/oxR0h1Xv5AiMtxUHLvx1MyBbvsZx/
# /CJLSYpuFeOmf3Zb0VN5kYWd1dLbPXM18zyuVLJSR2rAqhOV0o4R2plnXjKM+zeF
# 0dx1hZyHxlpXhcK/3Q2PjJst67TuzyfTtV5p+qQWBAGnJGdzz01Ptt4FVpd69+lS
# TfR3BU+FxtgL8Y7tQgnRDXbjI1Z4IiY2vsqxjG6qHeSF2kczYo+kyZEzX3EeQK+Y
# Zcki6EIhJYocLWDZN4lBiSoWD9dhPJRoYFLv1keZoIBA7hWBdz6c4FMYGlAdOJWb
# HmYzEyc5F3iHNs5Ow1+y9T1HU7bg5dsLYT0q15IszjdaPkBCMaQfEAjCVpy/JF1R
# Ap1qedIX09rBlI4HeyVxRKsGaubUxt8jmpZ1xTGCGX8wghl7AgEBMIGVMH4xCzAJ
# BgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9uMRAwDgYDVQQHEwdSZWRtb25k
# MR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRpb24xKDAmBgNVBAMTH01pY3Jv
# c29mdCBDb2RlIFNpZ25pbmcgUENBIDIwMTACEzMAAAQ59h96Z22gAK8AAAAABDkw
# DQYJYIZIAWUDBAIBBQCgga4wGQYJKoZIhvcNAQkDMQwGCisGAQQBgjcCAQQwHAYK
# KwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARUwLwYJKoZIhvcNAQkEMSIEIC+sO8qE
# omOjF3NtDPFQyuAJp4u63u1KGzx8RcGDeFOfMEIGCisGAQQBgjcCAQwxNDAyoBSA
# EgBNAGkAYwByAG8AcwBvAGYAdKEagBhodHRwOi8vd3d3Lm1pY3Jvc29mdC5jb20w
# DQYJKoZIhvcNAQEBBQAEggEAoobMKn4Lh6Art/QKqVRaq9nbM14JltLaJmtipH+A
# el7Tj3xtGPBvFVWnnzmb+H1WxlU++3s+A08m/vYaS2DzOm3dPn3NjOjZcZFjTGRM
# b5ybnWlJMILI3SQsIxnqrHNwfdtMGhf3wp19AhLyzDbTD8cu5gFtvnT2/+4cGUri
# ljZFY7/uuj+go3ameFzFhQKm8p79i8C6XeVqRX/rUDFsGSL6RiKnXfoP7GS/u22j
# /zMUotlAwNLtZiDtTwgJcGu+N9SaWALtC1ubauRRB1WDKb3ZBFdx6oY2FZ1cNw8Z
# M3g94IU2Kj8D/YhEJUpcNTBXLuuVdzKysqjt2+lyC78WC6GCFwkwghcFBgorBgEE
# AYI3AwMBMYIW9TCCFvEGCSqGSIb3DQEHAqCCFuIwghbeAgEDMQ8wDQYJYIZIAWUD
# BAIBBQAwggFVBgsqhkiG9w0BCRABBKCCAUQEggFAMIIBPAIBAQYKKwYBBAGEWQoD
# ATAxMA0GCWCGSAFlAwQCAQUABCDrwMkxT1YNL13efa/WBvLXsujMjpCfipRY0WWg
# DPHm0AIGYoTJOHl3GBMyMDIyMDUxODIzMzUyMS43MjlaMASAAgH0oIHUpIHRMIHO
# MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVk
# bW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSkwJwYDVQQLEyBN
# aWNyb3NvZnQgT3BlcmF0aW9ucyBQdWVydG8gUmljbzEmMCQGA1UECxMdVGhhbGVz
# IFRTUyBFU046RjdBNi1FMjUxLTE1MEExJTAjBgNVBAMTHE1pY3Jvc29mdCBUaW1l
# LVN0YW1wIFNlcnZpY2WgghFcMIIHEDCCBPigAwIBAgITMwAAAaUA3gjEQAdxTgAB
# AAABpTANBgkqhkiG9w0BAQsFADB8MQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2Fz
# aGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENv
# cnBvcmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQgVGltZS1TdGFtcCBQQ0EgMjAx
# MDAeFw0yMjAzMDIxODUxMTlaFw0yMzA1MTExODUxMTlaMIHOMQswCQYDVQQGEwJV
# UzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UE
# ChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSkwJwYDVQQLEyBNaWNyb3NvZnQgT3Bl
# cmF0aW9ucyBQdWVydG8gUmljbzEmMCQGA1UECxMdVGhhbGVzIFRTUyBFU046RjdB
# Ni1FMjUxLTE1MEExJTAjBgNVBAMTHE1pY3Jvc29mdCBUaW1lLVN0YW1wIFNlcnZp
# Y2UwggIiMA0GCSqGSIb3DQEBAQUAA4ICDwAwggIKAoICAQC6sYboIGpIvMLqDjDH
# e67BEJ5gIbVfIlNWNIrbB6t9E3QlyQ5r2Y2mfMrzh2BVYU8g9W+SRibcGY1s9X4J
# QqrMeagcT9VsdQmZ7ENbYkbEVkHNdlZBE5pGPMeOjIB7BsgJoTz6bIEZ5JRmoux6
# kBQd9cf0I5Me62wJa+j25QeLTpmkdZysZeFSILLQ8H53imqBBMOIjf8U3c7WY8Mh
# omOYTaem3nrZHIs4CRTt/8kR2IdILZPm0RIa5iIG2q664G8+zLJwO7ZSrxnDvYh3
# OvtrMpqwFctws0OCDDTxXE08fME2fpKb+pRbNXhvMZX7LtjQ1irIazJSh9iaWM1g
# FtXwjg+Yq17BOCzr4sWUL253kBOvohnyEMGm4/n0XaLgFNgIhPomjbCA2qXSmm/F
# i8c+lT0WxC/jOjBZHLKIrihx6LIQqeyYZmfYjNMqxMdl3mzoWv10N+NirERrNodN
# oKV+sAcsk/Hg9zCVSMUkZuDCyIpb1nKXfTd66KGsGy1OoHZO4KClkuvfsNo7aLlw
# hGLeiD32avJXYtC/wsGG7b+5mx5iGfTnNCRCXOm/YHFQ36D4npjCnM9eQS3qcse5
# 6UNjIgyiLHDqioV7mSPj2XqzTh4Yv77MtvxY/ZQepCazGEn1dBdn67wUgVzAe8Y7
# /KYKl+UF1HvJ08W+FHydHAwLwQIDAQABo4IBNjCCATIwHQYDVR0OBBYEFF+mjwMA
# l66urXDu+9xZF0toqRrfMB8GA1UdIwQYMBaAFJ+nFV0AXmJdg/Tl0mWnG1M1Gely
# MF8GA1UdHwRYMFYwVKBSoFCGTmh0dHA6Ly93d3cubWljcm9zb2Z0LmNvbS9wa2lv
# cHMvY3JsL01pY3Jvc29mdCUyMFRpbWUtU3RhbXAlMjBQQ0ElMjAyMDEwKDEpLmNy
# bDBsBggrBgEFBQcBAQRgMF4wXAYIKwYBBQUHMAKGUGh0dHA6Ly93d3cubWljcm9z
# b2Z0LmNvbS9wa2lvcHMvY2VydHMvTWljcm9zb2Z0JTIwVGltZS1TdGFtcCUyMFBD
# QSUyMDIwMTAoMSkuY3J0MAwGA1UdEwEB/wQCMAAwEwYDVR0lBAwwCgYIKwYBBQUH
# AwgwDQYJKoZIhvcNAQELBQADggIBAJabCxflMDCihEdqdFiZ6OBuhhhp34N6ow3W
# h3Obr12LRuiph66gH/2Kh5JjaLUq+mRBJ5RgiWEe1t7ifuW6b49N8Bahnn70LCiE
# dvquk686M7z+DbKHVk0+UlafwukxAxriwvZjkCgOLci+NB01u7cW9HAHX4J8hxaC
# PwbGaPxWl3s0PITuMVI4Q6cjTXielmL1+TQvh7/Z5k8s46shIPy9nFwDpsRFr3zw
# ENZX8b67VMBu+YxnlGnsJIcLc2pwpz95emI8CRSgep+/017a34pNcWNZIHr9ScEO
# WlHT8cEnQ5hhOF0zdrOqTzovCDtffTn+gBL4eNXg8Uc/tdVVHKbhp+7SVHkk1Eh7
# L80PBAjo+cO+zL+efxfIVrtO3oJxvEq1o+fkxcTTwqcfwBTb88/qHU0U2XeC1rqJ
# nDB1JixYlBjgHXrRekqHxxuRHBZ9A0w9WqQWcwj/MbBkHGYMFaqO6L9t/7iCZTAi
# wMk2GVfSEwj9PXIlCWygVQkDaxhJ0P1yxTvZsrMsg0a7x4VObhj3V8+Cbdv2TeyU
# GEblTUrgqTcKCtCa9bOnIg7xxHi8onM8aCHvRh90sn2x8er/6YSPohNw1qNUwiu+
# RC+qbepOYt+v5J9rklV3Ux+OGVZId/4oVd7xMLO/Lhpb7IjHKygYKaNx3XIwx4h6
# FrFH+BiMMIIHcTCCBVmgAwIBAgITMwAAABXF52ueAptJmQAAAAAAFTANBgkqhkiG
# 9w0BAQsFADCBiDELMAkGA1UEBhMCVVMxEzARBgNVBAgTCldhc2hpbmd0b24xEDAO
# BgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1pY3Jvc29mdCBDb3Jwb3JhdGlvbjEy
# MDAGA1UEAxMpTWljcm9zb2Z0IFJvb3QgQ2VydGlmaWNhdGUgQXV0aG9yaXR5IDIw
# MTAwHhcNMjEwOTMwMTgyMjI1WhcNMzAwOTMwMTgzMjI1WjB8MQswCQYDVQQGEwJV
# UzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UE
# ChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQgVGlt
# ZS1TdGFtcCBQQ0EgMjAxMDCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIB
# AOThpkzntHIhC3miy9ckeb0O1YLT/e6cBwfSqWxOdcjKNVf2AX9sSuDivbk+F2Az
# /1xPx2b3lVNxWuJ+Slr+uDZnhUYjDLWNE893MsAQGOhgfWpSg0S3po5GawcU88V2
# 9YZQ3MFEyHFcUTE3oAo4bo3t1w/YJlN8OWECesSq/XJprx2rrPY2vjUmZNqYO7oa
# ezOtgFt+jBAcnVL+tuhiJdxqD89d9P6OU8/W7IVWTe/dvI2k45GPsjksUZzpcGkN
# yjYtcI4xyDUoveO0hyTD4MmPfrVUj9z6BVWYbWg7mka97aSueik3rMvrg0XnRm7K
# MtXAhjBcTyziYrLNueKNiOSWrAFKu75xqRdbZ2De+JKRHh09/SDPc31BmkZ1zcRf
# NN0Sidb9pSB9fvzZnkXftnIv231fgLrbqn427DZM9ituqBJR6L8FA6PRc6ZNN3SU
# HDSCD/AQ8rdHGO2n6Jl8P0zbr17C89XYcz1DTsEzOUyOArxCaC4Q6oRRRuLRvWoY
# WmEBc8pnol7XKHYC4jMYctenIPDC+hIK12NvDMk2ZItboKaDIV1fMHSRlJTYuVD5
# C4lh8zYGNRiER9vcG9H9stQcxWv2XFJRXRLbJbqvUAV6bMURHXLvjflSxIUXk8A8
# FdsaN8cIFRg/eKtFtvUeh17aj54WcmnGrnu3tz5q4i6tAgMBAAGjggHdMIIB2TAS
# BgkrBgEEAYI3FQEEBQIDAQABMCMGCSsGAQQBgjcVAgQWBBQqp1L+ZMSavoKRPEY1
# Kc8Q/y8E7jAdBgNVHQ4EFgQUn6cVXQBeYl2D9OXSZacbUzUZ6XIwXAYDVR0gBFUw
# UzBRBgwrBgEEAYI3TIN9AQEwQTA/BggrBgEFBQcCARYzaHR0cDovL3d3dy5taWNy
# b3NvZnQuY29tL3BraW9wcy9Eb2NzL1JlcG9zaXRvcnkuaHRtMBMGA1UdJQQMMAoG
# CCsGAQUFBwMIMBkGCSsGAQQBgjcUAgQMHgoAUwB1AGIAQwBBMAsGA1UdDwQEAwIB
# hjAPBgNVHRMBAf8EBTADAQH/MB8GA1UdIwQYMBaAFNX2VsuP6KJcYmjRPZSQW9fO
# mhjEMFYGA1UdHwRPME0wS6BJoEeGRWh0dHA6Ly9jcmwubWljcm9zb2Z0LmNvbS9w
# a2kvY3JsL3Byb2R1Y3RzL01pY1Jvb0NlckF1dF8yMDEwLTA2LTIzLmNybDBaBggr
# BgEFBQcBAQROMEwwSgYIKwYBBQUHMAKGPmh0dHA6Ly93d3cubWljcm9zb2Z0LmNv
# bS9wa2kvY2VydHMvTWljUm9vQ2VyQXV0XzIwMTAtMDYtMjMuY3J0MA0GCSqGSIb3
# DQEBCwUAA4ICAQCdVX38Kq3hLB9nATEkW+Geckv8qW/qXBS2Pk5HZHixBpOXPTEz
# tTnXwnE2P9pkbHzQdTltuw8x5MKP+2zRoZQYIu7pZmc6U03dmLq2HnjYNi6cqYJW
# AAOwBb6J6Gngugnue99qb74py27YP0h1AdkY3m2CDPVtI1TkeFN1JFe53Z/zjj3G
# 82jfZfakVqr3lbYoVSfQJL1AoL8ZthISEV09J+BAljis9/kpicO8F7BUhUKz/Aye
# ixmJ5/ALaoHCgRlCGVJ1ijbCHcNhcy4sa3tuPywJeBTpkbKpW99Jo3QMvOyRgNI9
# 5ko+ZjtPu4b6MhrZlvSP9pEB9s7GdP32THJvEKt1MMU0sHrYUP4KWN1APMdUbZ1j
# dEgssU5HLcEUBHG/ZPkkvnNtyo4JvbMBV0lUZNlz138eW0QBjloZkWsNn6Qo3GcZ
# KCS6OEuabvshVGtqRRFHqfG3rsjoiV5PndLQTHa1V1QJsWkBRH58oWFsc/4Ku+xB
# Zj1p/cvBQUl+fpO+y/g75LcVv7TOPqUxUYS8vwLBgqJ7Fx0ViY1w/ue10CgaiQuP
# Ntq6TPmb/wrpNPgkNWcr4A245oyZ1uEi6vAnQj0llOZ0dFtq0Z4+7X6gMTN9vMvp
# e784cETRkPHIqzqKOghif9lwY1NNje6CbaUFEMFxBmoQtB1VM1izoXBm8qGCAs8w
# ggI4AgEBMIH8oYHUpIHRMIHOMQswCQYDVQQGEwJVUzETMBEGA1UECBMKV2FzaGlu
# Z3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwGA1UEChMVTWljcm9zb2Z0IENvcnBv
# cmF0aW9uMSkwJwYDVQQLEyBNaWNyb3NvZnQgT3BlcmF0aW9ucyBQdWVydG8gUmlj
# bzEmMCQGA1UECxMdVGhhbGVzIFRTUyBFU046RjdBNi1FMjUxLTE1MEExJTAjBgNV
# BAMTHE1pY3Jvc29mdCBUaW1lLVN0YW1wIFNlcnZpY2WiIwoBATAHBgUrDgMCGgMV
# ALPJcNtFs5sQyojdS4Ye5mVl7rSooIGDMIGApH4wfDELMAkGA1UEBhMCVVMxEzAR
# BgNVBAgTCldhc2hpbmd0b24xEDAOBgNVBAcTB1JlZG1vbmQxHjAcBgNVBAoTFU1p
# Y3Jvc29mdCBDb3Jwb3JhdGlvbjEmMCQGA1UEAxMdTWljcm9zb2Z0IFRpbWUtU3Rh
# bXAgUENBIDIwMTAwDQYJKoZIhvcNAQEFBQACBQDmL/ByMCIYDzIwMjIwNTE5MDIy
# MzQ2WhgPMjAyMjA1MjAwMjIzNDZaMHQwOgYKKwYBBAGEWQoEATEsMCowCgIFAOYv
# 8HICAQAwBwIBAAICCNYwBwIBAAICEbwwCgIFAOYxQfICAQAwNgYKKwYBBAGEWQoE
# AjEoMCYwDAYKKwYBBAGEWQoDAqAKMAgCAQACAwehIKEKMAgCAQACAwGGoDANBgkq
# hkiG9w0BAQUFAAOBgQBmkak7crjVmPF1+uk/Kc7WlVceNemedNFjwyD7G/g7YBQL
# kzp3kIKprdo47uApQVG/YauTpe+nZV215kVIKr6WO2dMNJkF9wVOBUskYGtmQaE3
# ACQSuJmBTTt3YJFavY2lNtSFUQGTKJFwvwkIlhtgOtMLFuRbg4r60Bj+KkTxljGC
# BA0wggQJAgEBMIGTMHwxCzAJBgNVBAYTAlVTMRMwEQYDVQQIEwpXYXNoaW5ndG9u
# MRAwDgYDVQQHEwdSZWRtb25kMR4wHAYDVQQKExVNaWNyb3NvZnQgQ29ycG9yYXRp
# b24xJjAkBgNVBAMTHU1pY3Jvc29mdCBUaW1lLVN0YW1wIFBDQSAyMDEwAhMzAAAB
# pQDeCMRAB3FOAAEAAAGlMA0GCWCGSAFlAwQCAQUAoIIBSjAaBgkqhkiG9w0BCQMx
# DQYLKoZIhvcNAQkQAQQwLwYJKoZIhvcNAQkEMSIEIHQ5KB25iuwGkjQ8otorOtLr
# PGrZlzPa9UHMVLuvelNyMIH6BgsqhkiG9w0BCRACLzGB6jCB5zCB5DCBvQQguAo4
# cX5mBLGgrdgFPNyoYfuiR5cpNwe9L3zBzJQS3FwwgZgwgYCkfjB8MQswCQYDVQQG
# EwJVUzETMBEGA1UECBMKV2FzaGluZ3RvbjEQMA4GA1UEBxMHUmVkbW9uZDEeMBwG
# A1UEChMVTWljcm9zb2Z0IENvcnBvcmF0aW9uMSYwJAYDVQQDEx1NaWNyb3NvZnQg
# VGltZS1TdGFtcCBQQ0EgMjAxMAITMwAAAaUA3gjEQAdxTgABAAABpTAiBCCMb2yD
# VG98jwTJlJyR15ljwPZCnVHookJqOsvV8DaSAzANBgkqhkiG9w0BAQsFAASCAgBO
# YMMbyl3VkPQJ6FIJE3WcA8qbVMDy40cPa0INeCBPUx803Kl4Tc2nQ8yVLeszsPHD
# awfeAHtrxbhxe6cb2PjeFXW3Dj4kdDoEaZZJQ21MteH6whw08hb5bYzrh9xB+p4a
# BB8pMMWOUMYxiddXRT08Yg2Ay7vrnNl1c+obUo2gdNm09Enf70usJVV6nyQthEXl
# 6l/aFxEm4PaYVoPt98P4YzLvscVdrGnJaqeF0eEzcWa+drr4/xxE93IF6NT9U7l9
# K//Fqnk2AA0EGuHxqHkEB9H1DL8rHsz/mvtuEBBpYCv3DACLa07H22iJ2DogZK0w
# /y4wklz2HRZh1HQ55wn32tmAbDhxTRdGVVPm+dYNpv6jrovnUfty+rKctwLl23R9
# 0CTDmNduK8S8AzsIMNIldJKZvoQsJWJABJnuhW90Z3Fy3mTOTZJf5N+ExkQCH57g
# Ewt6FU1rMeol53Bjd1GiO0cZs6wp+U1OmTC1gAWjBeVNnABf8OfCDSRoZG5eRA1F
# ovD0qmUxvir3DhsccUoudVrQJnhbAHdK2VxW0Opq+2dA4ocSGqjVBjB5jaBiTS4l
# uuZYLmV077SFRYPPc9AyDOu2UndcEKXz8wjfcvuT3DUgikeBX89vxMhGdXOya1I0
# FVkT2ANTpPuG3Y44CNk1hBGmFYJEPQfrwMrPh1JFUQ==
# SIG # End signature block
