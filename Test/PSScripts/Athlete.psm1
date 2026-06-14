[CmdletBinding()]
class Athlete
{
    [string]$EmailAddress
    [string]$Password

    Athlete ([string]$emailAddress, [string]$username, [string]$password)
    {
        $this.EmailAddress = $emailAddress
        $this.Password = $password
    }

    hidden [object] InvokeApi([string]$path, [string]$method) {
        return $this.InvokeApi($path, $method, @{})
    }

    hidden [object] InvokeApi([string]$path, [string]$method, [hashtable]$extraParams)
    {
        [string]$body = @{
            Email     = $this.EmailAddress
            Password  = $this.Password
        } + $extraParams | ConvertTo-Json
        
        $headers=@{}
        $headers.Add("Content-Type", "application/json")
        $response = Invoke-RestMethod -Uri "https://localhost:7167/api/$path" `
        -Method $method -Headers $headers -ContentType 'application/json' `
        -Body $body

        return $response
    }

    [object] Register([string]$firstName, [string]$lastName) {
        [hashtable]$extraParams = @{
            FirstName = $firstName
            LastName  = $lastName
        }
        return $this.InvokeApi("Auth/register", "POST", $extraParams)
    }

    [object] ChangePassword([string]$newPassword) {
        [hashtable]$extraParams = @{
            NewPassword = $newPassword
        }
        return $this.InvokeApi("Athlete/changepassword", "PUT", $extraParams)
    }

    [object] ChangeUsername([string]$newUsername) {
        [hashtable]$extraParams = @{
            NewUsername = $newUsername
        }
        return $this.InvokeApi("Athlete/changepassword", "PUT", $extraParams)
    }

    [object] Delete() {
        # Code to delete the athlete
        return $this.InvokeApi("Athlete/delete", "DELETE")
    }

    [object] Get() {
        return $this.InvokeApi("Athlete/get", "GET")
    }

    [object] GetSubscriptions() {
        # Code to get the athlete's subscriptions
        return $this.InvokeApi("Athlete/$($this.EmailAddress)/subscriptions", "GET")
    }
}