function Test-Register {
    param (
        [string]$FirstName = "Henk",

        [string]$LastName = "Jansen",

        [string]$Email = "hjansen@gmail.com",

        [string]$Password = "P@ssw0rd"
    )
    
    $headers=@{}
    $headers.Add("Content-Type", "application/json")
    $response = Invoke-RestMethod -Uri 'https://localhost:7167/api/Auth/register' `
    -Method POST -Headers $headers -ContentType 'application/json' `
    -Body "{'email':{'address':'$Email'},'fullName':{'firstName':'$FirstName','lastName':'$LastName'},'password':'$Password'}"

    return $response
}