taskkill /IM WireCloudTerminal.exe
taskkill /IM SDHManagement.exe
taskkill /IM NetworkNode.exe
taskkill /IM NetworkClientNode.exe /f
timeout /t 1
taskkill /IM cmd.exe