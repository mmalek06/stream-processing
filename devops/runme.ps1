function Finished
{
    process { Write-Host $_ -ForegroundColor DarkGreen }
}

function Starting
{
    process { Write-Host $_ -ForegroundColor DarkYellow }
}

Write-Output "Setting up the infrastructure..." | Starting

for ($i = 1; $i -lt 4; $i++) {
    $DataFolderName = "c$($i)_data"
    $CfgFolderName = "c$($i)_cfg"
    $DataFolderExists = Test-Path -Path $DataFolderName
    $CfgFolderExists = Test-Path -Path $CfgFolderName

    if (-Not $DataFolderExists) {
        Write-Output "Creating data folder $i" | Finished
        New-Item -Path . -Name $DataFolderName -ItemType "directory" | Out-Null
    }
    if (-Not $CfgFolderExists) {
        Write-Output "Creating configuration folder $i" | Finished
        New-Item -Path . -Name $CfgFolderName -ItemType "directory" | Out-Null
        Write-Output "Copying Cassandra configuration..." | Finished
        Copy-Item -Path ".\cassandra_cfg\*" -Destination $CfgFolderName -Recurse
    }
}

Write-Output "Starting Cassandra - nonclustered..." | Starting
Write-Output "If you wish to run a clustered version of Cassandra, you will need some serious amount of RAM!" | Starting
docker-compose -f docker-compose.cassandra-nonclustered.yml up -d
Write-Output "Cassandra cluster started" | Finished

Write-Output "Starting ElasticSearch - nonclustered..." | Starting
Write-Output "If you wish to run a clustered version of ElasticSearch, you will need some serious amount of RAM!" | Starting
docker-compose -f docker-compose.elasticsearch-nonclustered.yml up -d
Write-Output "ElasticSearch node started" | Finished

Write-Output "Starting Kafka - nonclustered..." | Starting
Write-Output "If you wish to run a clustered version of Kafka, you will need some serious amount of RAM!" | Starting
docker-compose -f docker-compose.kafka.yml up -d
Write-Output "Kafka node started" | Finished

docker ps
# docker exec cass1 nodetool status
# docker exec -it cass1 cqlsh -e "describe keyspaces"