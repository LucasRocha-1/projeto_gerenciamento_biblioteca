using Microsoft.Data.Sqlite;
using System;
using System.IO;
using SQLitePCL;

var dbPath = Path.Combine("..", "..", "Biblioteca", "API", "Biblioteca.db");
Console.WriteLine($"DB path: {Path.GetFullPath(dbPath)}");
if (!File.Exists(dbPath))
{
    Console.WriteLine("Database file not found.");
    return;
}

// Inicializa o provider nativo do SQLite
Batteries_V2.Init();
using var conn = new SqliteConnection($"Data Source={dbPath}");
conn.Open();

using (var cmd = conn.CreateCommand())
{
    cmd.CommandText = "CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\" (\"MigrationId\" TEXT NOT NULL CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY, \"ProductVersion\" TEXT NOT NULL);";
    cmd.ExecuteNonQuery();

    cmd.CommandText = "INSERT OR IGNORE INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ('20251119001616_AdicionandoCapas', '9.0.10');";
    var rows = cmd.ExecuteNonQuery();
    Console.WriteLine($"Inserted rows: {rows}");

    cmd.CommandText = "SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;";
    using var reader = cmd.ExecuteReader();
    Console.WriteLine("__EFMigrationsHistory:");
    while (reader.Read())
    {
        Console.WriteLine($" - {reader.GetString(0)} | {reader.GetString(1)}");
    }
}

conn.Close();
Console.WriteLine("Done.");
