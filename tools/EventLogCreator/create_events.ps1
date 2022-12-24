for ($i = 0; $i -lt $args[0]; $i++) {
    $guid = New-Guid
    $message = "some bad error has occured! message: " + $guid
    Write-EventLog -LogName "Application" -Source "test-application" -EventID 3001 -EntryType Warning -Message $message -Category 1
}