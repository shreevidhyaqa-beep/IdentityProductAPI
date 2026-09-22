# Identity
* Membership system, Gives UserMangement(authentication, login,Registration) features are provided

# OAUTH-- open standard protocol(authorization framework) ---handling Authorization
oAuth2.0
react app--->sign in with Google--->user Authenticate---Google sends Authorization--->application
OpenId Connect+OAuth

## Best Security Practices
* HTTPS ---For encryption
* Password Hash
* JWT 
* Validate the JWT
* Authorize[Roles=""]
* DTO
* ModelState()
* CORS
* CSRF---(cross site Request Forgery)considered for Cookie Authentication
    * Authentication cookie+ Antiforgery Token-->Valid Request
    * [ValidateAntiForgeryToken]
    * 
* XSS--> 
   * <div>
   * <h1{product.name}</h1>
   * <div>

SQL Injection
``` sql
string query="select * from Users"+ where userName='"+username"'"+"And password='" password"'"
```
userName=admin
password='' 'oR'1'=1'

# parameters
where userName=@userName

# LINQ methods--
find(u=>u.username==username)