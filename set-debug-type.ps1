copy 'src\Invio.Extensions.DependencyInjection\project.json' 'src\Invio.Extensions.DependencyInjection\project.json.bak'
$project = Get-Content 'src\Invio.Extensions.DependencyInjection\project.json.bak' -raw | ConvertFrom-Json
$project.buildOptions.debugType = "full"
$project | ConvertTo-Json  | set-content 'src\Invio.Extensions.DependencyInjection\project.json'