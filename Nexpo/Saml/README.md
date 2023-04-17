# SAML Documenation


## Terminology

### SustainSys 
The nuget package that is used to handle SAML authentication. 

### Providers

- **Identity Provider**: Often shortened IDP The service that authenticates users. (Lunds Universitets SSO service)

- **Service Provider**: Often shortened SP. The service that provides a SAML login. (us)

### The saml2 scheme
- **Scheme**: The protocol (process) used to authenticate users. The scheme can later be used for automatic authentication (handled by sustainsys)

### SAML Requests and Responses
- **SAML Request**: A request from the service provider to the identity provider. The request contains information about the service provider and the user that is trying to authenticate. The request is signed by the service provider. Automatically handled by the scheme.

- **SAML Response**: A response from the identity provider to the service provider. The response contains information about the user that is trying to authenticate. The response is signed by the identity provider. Automatically handled by the scheme.



## Metadata
The metadata is a xml file that contains information about the identity provider or service provider.
- We have sent our metadata to the identity provider, so they can use it to configure their service.
- We have downloaded the identity providers metadata, so we can use it to configure our service.

These configurations are done in the startup.cs file (more specificly in the services.addSam2 method)

Most notable is the following data:
- **EntityId**: The name of the provider. This is used to identify the provider in the metadata file. Not much more than a simple string.

- **SSO Service endpoint**:  This is the url that the user is redirected to when they click the login button. 

- **Assertions Consumer Service endpoint**: Often shortened ACS endpoint. This is the url that the identity provider redirects the user to after they have authenticated. This is where the SAML response is sent.

- **Signing certificate**: Used for security reasons. The certificate that is used to sign the Saml request and response. This is used to verify that the request is from the correct provider.

- **Name ID format**: A standardized way of identifying a user in a SAML transaction. It specifies how the user's identity is represented in the SAML response sent by the IdP to the SP. 




## Our endpoints
Note that we also have our own endpoints that are used to handle the authentication process. For their documentation see their corresponding files.




## The Signing certificate
- The certificate we use for authorication is the `certificate.crt` file in the root of the project.
- It has a corresponding password and private.key that are used to sign the requests and responses.
    - These are not included in the repository for security reasons.
    - They are instead stored in the head of IT bitwarden.




## Scheme Setup
In the `startup.cs` the samlservice is configured:

### services.addSam2
Here the scheme is configured. Worth noting is:
- The identity provider is configured by loading their metadata
- Here we add the endpoints we have created for the authentication process, so the scheme knows how to use them


### SamlAuthService
Essentially the backbone of the scheme. Se its documentation for more information.



## The authentication process
1. The user clicks the login button.
2. The user is redirected to the endpoint api/saml/initiateSSO
3. The scheme:
    a. creates a SAML request
    b. redirects the user to the IDP
    1. The SAML request is sent to the IDP through the user 
4. The IDP authenticates the user
5. The IDP redirects the user to our ACS endpoint (api/saml/ACS), where:
    1. The ACS endpoint receives the SAML response, extracts the user information
    2. The ACS endpoint logs the user in 



## The logout process
TODO






