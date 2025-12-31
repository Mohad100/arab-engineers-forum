# PowerShell script to clear database
$ErrorActionPreference = "Stop"

Write-Host "Clearing database..." -ForegroundColor Yellow

# Database connection details
$server = "localhost"
$port = "5433"
$database = "ForumDb"
$username = "postgres"
$password = "postgres"

# Create connection string
$connString = "Host=$server;Port=$port;Database=$database;Username=$username;Password=$password"

# Load Npgsql assembly (PostgreSQL driver for .NET)
Add-Type -Path ".\bin\Debug\net8.0\Npgsql.dll"

try {
    # Create connection
    $conn = New-Object Npgsql.NpgsqlConnection($connString)
    $conn.Open()
    Write-Host "Connected to database successfully" -ForegroundColor Green

    # Execute DELETE statements
    $commands = @(
        'DELETE FROM "Replies"',
        'DELETE FROM "PrivateMessages"',
        'DELETE FROM "Threads"',
        'DELETE FROM "Users"'
    )

    foreach ($cmdText in $commands) {
        $cmd = New-Object Npgsql.NpgsqlCommand($cmdText, $conn)
        $rowsAffected = $cmd.ExecuteNonQuery()
        Write-Host "$cmdText - $rowsAffected rows deleted" -ForegroundColor Cyan
    }

    # Verify tables are empty
    $verifyCmd = New-Object Npgsql.NpgsqlCommand('SELECT COUNT(*) FROM "Users"', $conn)
    $userCount = $verifyCmd.ExecuteScalar()
    
    Write-Host "`nDatabase cleared successfully!" -ForegroundColor Green
    Write-Host "Users: $userCount" -ForegroundColor White
    
    $conn.Close()
}
catch {
    Write-Host "Error: $_" -ForegroundColor Red
}
