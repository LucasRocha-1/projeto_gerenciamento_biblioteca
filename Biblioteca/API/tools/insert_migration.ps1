Add-Type -Path "$env:USERPROFILE\.nuget\packages\microsoft.data.sqlite.core\9.0.10\lib\net6.0\Microsoft.Data.Sqlite.dll"
$c = [Microsoft.Data.Sqlite.SqliteConnection]::new('Data Source=Biblioteca.db')
$c.Open()
$cmd = $c.CreateCommand()
$cmd.CommandText = 'INSERT OR IGNORE INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") VALUES (''20251119001616_AdicionandoCapas'', ''9.0.10'');'
$cmd.ExecuteNonQuery() | Out-Null
$c.Close()
Write-Output 'Inserted migration record (if not present)'
