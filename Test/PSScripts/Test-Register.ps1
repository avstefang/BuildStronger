function Test-Register {
    param (
        [string]$FirstName = "Henk",

        [string]$LastName = "Jansen",

        [string]$Email = "hjansen@gmail.com",

        [string]$Password = "P@ssw0rd"
    )

    [string]$body = @{
        FirstName = $FirstName
        LastName  = $LastName
        Email     = $Email
        Password  = $Password
    } | ConvertTo-Json
    
    $headers=@{}
    $headers.Add("Content-Type", "application/json")
    $response = Invoke-RestMethod -Uri 'https://localhost:7167/api/Auth/register' `
    -Method POST -Headers $headers -ContentType 'application/json' `
    -Body $body

    return $response
}